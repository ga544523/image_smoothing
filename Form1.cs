using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Win32;
using System.Collections.Concurrent;
using Emgu.CV.CvEnum;
using Emgu.CV.BgSegm;
using Emgu.CV.Ocl;
using System.Globalization;



namespace image_smoothing
{

    public partial class Form1 : Form
    {
        result f;
        window_approach window_solver = new window_approach();
        math_function math_solver = new math_function();

        int row, col;
        double[,] previousu;
        double[,] previousv;
        double[,] variance_ans;
        public List<double>[,] totalU,totalV;
        double[,] imgdata;

        public Form1()
        {
            InitializeComponent();
            f = new result();

        }
        void show_result() {

            Image<Gray, byte> result_image = new Image<Gray, byte>(row, col);

            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++)
                {
                    int n = totalU[i, j].Count;
                    double minr = 100000;

               

                        result_image.Data[i, j, 0] = (byte)(totalU[i, j][n - 1]+0.5);
                    
                }
            }
            f.pictureBox2.Image = result_image.ToBitmap();
      
        }

        public void inituv(double[,] imgdata) {




            previousu = window_solver.first_caclu(imgdata);
            previousv = window_solver.first_caclv(imgdata, variance_ans);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    totalU[i, j].Add(previousu[i, j]);
                    totalV[i, j].Add(previousv[i, j]);
                }
            }


        }

        public void solve(double[,] imgdata, double[,] variance_ans) {

            double[,] currentU = new double[row, col];
            double[,] currentV = new double[row, col];
            double[,] ones = new double[row, col];
            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++) {
                    ones[i, j] = 1.0;
                }
            }

            for (int window = 1; window < window_approach.km; window++)
            {

                double[,] flag = new double[row, col];

                currentU = window_solver.solveU(imgdata, window*2+1,previousu,previousv);
                currentV = window_solver.solveV(ones, window * 2 + 1, previousu, previousv,variance_ans);

                window_solver.judge(totalU,totalV,currentU,flag);

                bool ok = window_solver.judge_global(totalU, currentU);


              
                if (ok == true)
                {
                    return;
                }

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {

                        if (flag[i, j] == 0)
                        {
                            totalU[i, j].Add(currentU[i, j]);
                            totalV[i, j].Add(currentV[i, j]);
                        }
                        else {
                            int n = totalU[i, j].Count;
                            previousu[i, j] = totalU[i, j][n-1];
                            previousv[i, j] = totalV[i, j][n-1];
                        }

                    }
                }

            }


        }
        void addnoise() {

            Random random=new Random();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    double x1 = 1 - random.NextDouble();
                    double x2 = 1 - random.NextDouble();

                    double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
                    y1 *= 10;
                    imgdata[i, j] += y1;
                    if (imgdata[i, j] > 255)
                    {
                        imgdata[i, j] = 255;
                    }
                    else if (imgdata[i, j] < 0) {
                        imgdata[i, j] = 0;
                    }
                }
            }

            Image<Gray, byte> result_image = new Image<Gray, byte>(row, col);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    int n = totalU[i, j].Count;
                    result_image.Data[i, j, 0] = (byte)imgdata[i, j];
                }
            }

            f.pictureBox1.Image= result_image.ToBitmap();


        }
        private void button3_Click(object sender, EventArgs e)
        {

            addnoise();



        }

        private void button4_Click(object sender, EventArgs e)
        {
            variance_ans = window_solver.caculate_total_variance(imgdata, row, col);

            inituv(imgdata);

            solve(imgdata, variance_ans);
            show_result();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.ShowDialog();
            int imagesize = 360;

            Image<Gray, byte> grayImage = new Image<Gray, byte>(Openfile.FileName);
            grayImage=grayImage.Resize(imagesize, imagesize, Emgu.CV.CvEnum.Inter.Linear);
            //grayImage.Data[v, u, 0];
            pictureBox1.Image = grayImage.ToBitmap();
            col = imagesize;
            row = imagesize;

             imgdata = new double[row, col];
            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++) {
                    imgdata[i, j] = grayImage.Data[i, j, 0];
                }
            }
            variance_ans = new double[row, col];
            previousu = new double[row, col];
            previousv = new double[row, col];
            totalU = new List<double>[row, col];
            totalV = new List<double>[row, col];
            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++) {
                    totalU[i, j] = new List<double>();
                    totalV[i, j] = new List<double>();
                }
            }
            window_approach.boundaryi = row;
            window_approach.boundaryj = col;
            commom_method.boundaryi = row;
            commom_method.boundaryj = col;


        }

        private void button2_Click(object sender, EventArgs e)
        {
       
            f.Visible = true;

        }
    }
}
