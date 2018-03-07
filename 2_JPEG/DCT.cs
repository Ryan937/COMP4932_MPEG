using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_JPEG
{
    public class DCT
    {
        private int[,] qTable = {
            { 16, 11, 10, 16, 24, 40, 51, 61 },
            { 12, 12, 14, 19, 26, 58, 60, 55 },
            { 14, 13, 16, 24, 40, 57, 69, 56 },
            { 14, 17, 22, 29, 51, 87, 80, 62 },
            { 18, 22, 37, 56, 68, 109, 103, 77 },
            { 24, 35, 55, 64, 81, 104, 113, 92 },
            { 49, 64, 78, 87, 103, 121, 120, 101 },
            { 72, 92, 95, 98, 112, 100, 103, 99 }
        };

        public DCT()
        {

        }

        private double C(int x)
        {
            if (x == 0)
            {
                return (1 / (Math.Sqrt(2)));
            }
            else
            {
                return 1;
            }
        }

        public double[,] forwardDCT(byte[,] img)
        {
            double[,] data = new double[img.Length, img.Length];
            double sum = 0;

            for (int u = 0; u < 8; u++)
            {
                for (int v = 0; v < 8; v++)
                {
                    sum = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            sum += Math.Cos(((2 * i + 1) * u * Math.PI) / 16) 
                                * Math.Cos(((2 * j + 1) * v * Math.PI) / 16) * (img[i, j] - 128);
                        }
                    }
                    data[u, v] = sum * ((C(u) * C(v)) / 4);
                }
            }
            return data;
        }

        public byte[,] inverseDCT(double[,] img)
        {
            byte[,] data = new byte[img.Length, img.Length];
            double sum = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    sum = 0;
                    for (int u = 0; u < 8; u++)
                    {
                        for (int v = 0; v < 8; v++)
                        {
                            sum += ((C(u) * C(v)))
                                * Math.Cos(((2 * i + 1) * u * Math.PI) / 16)
                                * Math.Cos(((2 * j + 1) * v * Math.PI) / 16)
                                * img[u, v];
                        }
                    }
                    sum = sum / 4 + 128;
                    if (sum > 255)
                        sum = 255;
                    if (sum < 0)
                        sum = 0;
                    data[i, j] = (byte)sum;
                }
            }
            return data;
        }

        public byte[,] quantize(double[,] img)
        {
            byte[,] quantized = new byte[img.Length, img.Length];

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    quantized[x, y] = (byte)(img[x, y] / qTable[x, y]);
                }
            }
            return quantized;
        }

        public double[,] inverseQuantize(byte[,] imgQ)
        {
            double[,] invQData = new double[imgQ.Length, imgQ.Length];

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    invQData[x, y] = imgQ[x, y] * qTable[x, y];
                }
            }
            return invQData;
        }
    }
}
