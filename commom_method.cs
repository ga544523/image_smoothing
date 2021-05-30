using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace image_smoothing
{
    class commom_method
    {
        static public int boundaryi,boundaryj;
        public int check_boundary(int i, int j)
        {

            if (i < 0 || j < 0 || i >= boundaryi || j >= boundaryj)
            {
                return -1;
            }

            return 1;

        }


    }
}
