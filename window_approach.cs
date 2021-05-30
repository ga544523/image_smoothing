using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace image_smoothing
{
    public  class window_approach
    {
        public static int km = 15;
        public static int Bi=(2*km+1)*(2*km+1);

        public static int boundaryi, boundaryj;
        public static double average_variance=0;
        math_function mathsolver = new math_function();
        commom_method method = new commom_method();

        public double[,] first_caclu(double[,]data) {


            double[,] ans = new double[boundaryi, boundaryj];

            for (int i = 0; i < boundaryi; i++) {
                for (int j = 0; j < boundaryj; j++) {
                    ans[i, j] = data[i, j];
                }
            }


            return ans;
        
        }

        public double[,] first_caclv(double[,] data,double[,] variance_ans)
        {

            double[,] ans = new double[boundaryi, boundaryj];

            for (int i = 0; i < boundaryi; i++)
            {
                for (int j = 0; j < boundaryj; j++)
                {
                    ans[i, j] = variance_ans[i, j];
                }
            }

            return ans;


        }

        public double convolve(double[,]data,int loci,int locj,double[,]weight,int windowsize,int square_weight,int square_data) {

            int half = -windowsize / 2;
            double sum = 0;

            for (int i = 0; i < windowsize; i++) {

                for (int j = 0; j < windowsize; j++) {

                    int n = method.check_boundary(i + half + loci, j + half + locj);
                    if (n > 0)
                    {
                        if (square_weight == 1)
                        {
                            sum += data[loci + i + half, locj + j + half] * weight[i, j] * weight[i, j];
                        }
                        else if (square_data == 1) {

                            sum += data[loci + i + half, locj + j + half]* data[loci + i + half, locj + j + half] * weight[i, j];
                        }
                        else
                        {
                            sum += data[loci + i + half, locj + j + half] * weight[i, j];
                        }
                    }
                }
            }

            return sum;

        }

        public double[,] solveU(double [,]data,int windowsize,double[,]previousu,double[,]previousv) {

            double[,] ans = new double[boundaryi, boundaryj];
            double[,] weight = new double[windowsize, windowsize];

            for (int i = 0; i < boundaryi; i++) {
                for (int j = 0; j < boundaryj; j++) {

                    weight = mathsolver.weight_function(previousu, previousv, windowsize, i, j);
                    double num = convolve(data, i, j, weight, windowsize, 0,0);
                    ans[i, j] = num;

                }
            }


            return ans;
        }

        public double[,] solveV(double [,]data,int windowsize,double[,] previousu, double[,] previousv,double [,]total_variance)
        {
            double[,] ans = new double[boundaryi, boundaryj];
            double[,] weight = new double[windowsize, windowsize];
            for (int i = 0; i < boundaryi; i++)
            {
                for (int j = 0; j < boundaryj; j++)
                {

                    weight = mathsolver.weight_function(previousu, previousv, windowsize, i, j);
                    double num = convolve(data, i, j, weight, windowsize, 1,0);
                    ans[i, j] = num*total_variance[i,j];

                }
            }

            return ans;
        }

        public double get_epislon(double[,]imgdata,int loci,int locj) {

            double sum = 4 * imgdata[loci, locj];
            int n= method.check_boundary(loci - 1, locj);
            if (n < 0)
            {
                n = 0;
            }
            else {
                sum -= imgdata[loci - 1, locj];
            }

             n = method.check_boundary(loci , locj-1);
            if (n < 0)
            {
                n = 0;
            }
            else
            {
                sum -= imgdata[loci , locj-1];
            }
            n = method.check_boundary(loci + 1, locj);
            if (n < 0)
            {
                n = 0;
            }
            else
            {
                sum -= imgdata[loci + 1, locj];
            }
            n = method.check_boundary(loci  , locj+1);

            if (n < 0)
            {
                n = 0;
            }
            else
            {
                sum -= imgdata[loci , locj+1];
            }
            return sum/Math.Sqrt(20);
        }
        public double get_average_variance(double[,]data) {

            double ans = 0;
            double val = 1.4826;

            List<double> mid = new List<double>();
            for (int i = 0; i < boundaryi; i++) {
                for (int j = 0; j < boundaryj; j++) {
                    mid.Add(Math.Abs( data[i, j]) );
                }
            }
            mid.Sort();
            int n = mid.Count;
            double mid_value = mid[n / 2];
            mid.Clear();
            for (int i = 0; i < boundaryi; i++)
            {
                for (int j = 0; j < boundaryj; j++)
                {
                    mid.Add(Math.Abs( Math.Abs(data[i, j])-mid_value) );
                }
            }

            mid.Sort();
            ans = mid[n / 2];
            return ans* val;
        }

        public double get_box_variance(double[,] data,int loci,int locj)
        {
            int half = -km ;
            int window = 2 * km + 1;
            double ans = 0;
            double val = 1.4826;

            List<double> mid = new List<double>();
            int m;

            for (int i = 0; i < window; i++)
            {
                for (int j = 0; j < window; j++)
                {
                     m = method.check_boundary(i + half + loci, j + half + locj);
                    if (m > 0)
                    {
                        mid.Add(Math.Abs(data[i + half + loci, j + half + locj]));
                    }
                  
                   
                }
            }

            mid.Sort();
           m = mid.Count;
            double mid_value = mid[m / 2];
            mid.Clear();
            for (int i = 0; i < window; i++)
            {
                for (int j = 0; j < window; j++)
                {
                    m = method.check_boundary(i + half + loci, j + half + locj);
                    if (m > 0)
                    {
                        mid.Add(Math.Abs(Math.Abs(data[i + half + loci, j + half + locj]) - mid_value));
                    }
                }
            }
            m = mid.Count;
            mid.Sort();
            ans = mid[m / 2];
            return ans * val;
        }

        public double[,] caculate_total_variance(double[,]imgdata,int row,int col) {
            double[,] tmp = new double[row, col];
            double[,] ans = new double[row, col];
            double[,] weight = new double[2*km+1, 2*km+1];
            for (int i = 0; i < 2*km+1; i++) {
                for (int j = 0; j < 2*km+1; j++) {
                    weight[i, j] = 1;
                }
            }


            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++) {

                    double epislon = get_epislon(imgdata, i, j);
                    tmp[i,j] = epislon;
                
                }
            }
            average_variance = get_average_variance(tmp);


            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++) {

                    double sum = convolve(tmp, i, j, weight, 2 * km + 1, 0, 1);

                    
                    double tmp_num= sum / (Bi);


                    ans[i, j] = Math.Max(average_variance, get_box_variance(tmp, i, j)  );
                    ans[i, j] *= ans[i, j];


                    ans[i, j] = Math.Max(tmp_num, ans[i, j]);



                }
            }
       
            return ans;
        
        }
        public void judge(List<double>[,] totalU,List<double>[,]totalV,double[,] currentU, double[,] flag) {


            for (int i = 0; i < boundaryi; i++) {
                for (int j = 0; j < boundaryj; j++) {
       
                    for (int k = 0; k < totalU[i, j].Count; k++) {
                        if ((totalU[i, j][k] - currentU[i, j] )*(totalU[i, j][k] - currentU[i, j]) > 8 * totalV[i, j][k]) {
                            flag[i, j] = 1;
                            break;
                        }
                       

                    }
                }
            }
        }

        public bool judge_global(List< double>[,] totalU,double[,] currentU) {

            double sum = 0;
         
            for (int i = 0; i < boundaryi; i++) {
                for (int j = 0; j < boundaryj; j++) {
                    int n = totalU[i, j].Count;
                    if (n >=1) {
                        
                        sum += totalU[i, j][n - 1]  * Math.Log(totalU[i, j][n - 1] / currentU[i, j] +0.000001) + currentU[i, j] - totalU[i, j][n - 1];
            
                    }

                }
            }

            if (sum <= math_function.converge) { 
            
                return true;
            
            }

            return false;
        }
    }


}
