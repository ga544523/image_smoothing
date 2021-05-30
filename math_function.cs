using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace image_smoothing
{
     class math_function
    {
        double lamda = 3;
        public static double converge = 0.001;

        commom_method method = new commom_method();
        public double[,] weight_function(double[,]previousu,double[,]previousv,int windowsize,int loci,int locj) {

            double[,] ans = new double[windowsize, windowsize];

            int half = -windowsize / 2;
            double sum = 0;

            for (int i = 0; i < windowsize; i++)
            {

                for (int j = 0; j < windowsize; j++)
                {

                    int n =method.check_boundary(i + half + loci, j + half + locj);
                    if (n > 0)
                    {
                        if (Math.Abs(previousu[loci, locj] - previousu[i + half + loci, j + half + locj])
                            <= lamda * Math.Sqrt(previousv[loci, locj] ))
                        {
                            ans[i, j] = 1;
                        }
                        else {
                            ans[i, j] = lamda * Math.Sqrt(previousv[loci, locj]) / 
                                Math.Abs(previousu[loci, locj] - previousu[i + half + loci, j + half + locj]);

                        }
                        sum += ans[i, j];
                    }

                }
            }

            for (int i = 0; i < windowsize; i++) {
                for (int j = 0; j < windowsize; j++) {
                    ans[i, j] /= sum;
                }
            }

            return ans ;
        }


    }
}
