using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_JPEG
{
    class MotionCompesation
    {
        public static void MotionVector(Bitmap reference, Bitmap target)
        {
            int[,] RR = new int[reference.Width, reference.Height];
            int[,] RG = new int[reference.Width, reference.Height];
            int[,] RB = new int[reference.Width, reference.Height];
            int[,] TR = new int[target.Width, target.Height];
            int[,] TG = new int[target.Width, target.Height];
            int[,] TB = new int[target.Width, target.Height];

            for (int x = 0; x < reference.Width; x = x + 16)
            {
                for (int y = 0; y < reference.Height; y = y + 16)
                {
                    Color pixelR = reference.GetPixel(x, y);
                    Color pixelT = target.GetPixel(x, y);

                    RR[x, y] = pixelR.R;
                    RG[x, y] = pixelR.G;
                    RB[x, y] = pixelR.B;
                    TR[x, y] = pixelT.R;
                    TG[x, y] = pixelT.G;
                    TB[x, y] = pixelT.B;
                }
            }
        }

        public static double MAD(int N, int[,] refR, int[,] refG, int[,] refB, int[,] tarR, 
            int[,] tarG, int[,]tarB, int x, int y, int i, int j)
        {
            double difference = 0;
            double diffTemp = 0;

            for (int k = 0; k < N; k++)
            {
                for (int l = 0; l < N; l++)
                {
                    diffTemp = Math.Abs(refR[x + k, y + l] - tarR[i + k, j + l]);
                    diffTemp += Math.Abs(refG[x + k, y + l] - tarG[i + k, j + l]);
                    diffTemp += Math.Abs(refB[x + k, y + l] - tarB[i + k, j + l]);
                }
            }
            difference = (1 / Math.Pow(N, 2)) * diffTemp;

            return difference;
        }
    }
}
