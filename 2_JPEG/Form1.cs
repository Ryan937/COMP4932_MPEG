using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2_JPEG
{
    public struct YCrCb
    {
        public byte[,] Y;
        public byte[,] Cb;
        public byte[,] Cr;

        public YCrCb(byte[,] Y, byte[,] Cb, byte[,] Cr)
        {
            this.Y = Y;
            this.Cb = Cb;
            this.Cr = Cr;
        }
}


public partial class Form1 : Form
    {
        Bitmap image;
        Bitmap image2;
        Bitmap bmpCompressed;
        Bitmap bmpDecompressed;
        YCrCb rgbConverted;
        YCrCb compressed;
        YCrCb decompressed;
        DCT dctObject = new DCT();

        public Form1()
        {
            InitializeComponent();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = "NULL";
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    size = text.Length;
                }
                catch (IOException)
                {
                }
            }
            Console.WriteLine(file);

            image = new Bitmap(file);

            pictureBox1.Image = image;
        }

        private void mPEGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = "NULL";
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    size = text.Length;
                }
                catch (IOException)
                {
                }
            }
            Console.WriteLine(file);

            image2 = new Bitmap(file);

            pictureBox2.Image = image2;
        }

        private void compressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rgbConverted = rgbToYCbCr(image);
            compressed = compress(rgbConverted.Y, rgbConverted.Cb, rgbConverted.Cr);
            decompressed = decompress(compressed.Y, compressed.Cb, compressed.Cr);
            bmpDecompressed = YCbCrTorgb(decompressed.Y, decompressed.Cb, decompressed.Cr);
            pictureBox2.Image = bmpDecompressed;
        }

        YCrCb rgbToYCbCr(Bitmap img)
        {
            YCrCb result;
            Color color;
            byte[,] y = new byte[img.Width, img.Height];
            byte[,] cb = new byte[img.Width, img.Height];
            byte[,] cr = new byte[img.Width, img.Height];
            int R = 0;
            int G = 0;
            int B = 0;

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    color = image.GetPixel(i, j);
                    R = color.R;
                    G = color.G;
                    B = color.B;
                    y[i, j] = (byte)(0 + 0.299 * R + 0.587 * G + 0.114 * B);
                    cb[i, j] = (byte)(128 - 0.168736 * R - 0.331264 * G + 0.5 * B);
                    cr[i, j] = (byte)(128 + 0.5 * R - 0.418688 * G - 0.081312 * B);
                }
            }
            result = new YCrCb(y, cb, cr);
            return result;
        }

        Bitmap YCbCrTorgb(byte[,] y, byte[,] cb, byte[,] cr)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);
            byte[,] R = new byte[image.Width, image.Height];
            byte[,] G = new byte[image.Width, image.Height];
            byte[,] B = new byte[image.Width, image.Height];

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    R[i, j] = checkLimit(y[i, j] + 1.402 * (cr[i, j] - 128));
                    G[i, j] = checkLimit(y[i, j] - 0.344136 * (cb[i, j] -128) - 0.714136 * (cr[i, j] - 128));
                    B[i, j] = checkLimit(y[i, j] + 1.772 * (cb[i, j] - 128));
                    Color color = Color.FromArgb(R[i, j], G[i, j], B[i, j]);
                    result.SetPixel(i, j, color);
                }
            }
            return result;
        }

        byte checkLimit(double value)
        {
            if (value > 255)
                value = 255;
            if (value < 0)
                value = 0;
            
            return (byte)value;
        }

        YCrCb compress(byte[,] y, byte[,] cb, byte[,] cr)
        {
            YCrCb result;
            result.Y = new Byte[image.Width, image.Height];
            result.Cb = new Byte[image.Width / 2, image.Height / 2];
            result.Cr = new Byte[image.Width / 2, image.Height / 2];

            for (int m = 0; m < image.Width; m++)
            {
                for (int n = 0; n < image.Height; n++)
                {
                    result.Y[m, n] = y[m, n];
                }
            }

            for (int i = 0, k = 0; i < image.Width; i = i+2, k++)
            {
                for (int j = 0, l = 0; j < image.Height; j = j+2, l++)
                {
                    result.Cb[k, l] = cb[i, j];
                    result.Cr[k, l] = cr[i, j];
                }
            }
            return result;
        }

        YCrCb decompress(byte[,] y, byte[,] cb, byte[,] cr)
        {
            YCrCb result;
            result.Y = new Byte[image.Width, image.Height];
            result.Cb = new Byte[image.Width, image.Height];
            result.Cr = new Byte[image.Width, image.Height];

            for (int m = 0; m < image.Width; m++)
            {
                for (int n = 0; n < image.Height; n++)
                {
                    result.Y[m, n] = y[m, n];
                }
            }

            for (int i = 0, k = 0; i < image.Width / 2; i++, k = k + 2)
            {
                for (int j = 0, l = 0; j < image.Height / 2; j++, l = l + 2)
                {
                    result.Cb[k, l] = cb[i, j];
                    result.Cb[k, l + 1] = cb[i, j];
                    result.Cb[k + 1, l] = cb[i, j];
                    result.Cb[k + 1, l + 1] = cb[i, j];
                    result.Cr[k, l] = cr[i, j];
                    result.Cr[k, l + 1] = cr[i, j];
                    result.Cr[k + 1, l] = cr[i, j];
                    result.Cr[k + 1, l + 1] = cr[i, j];
                }
            }
            return result;
        }

        private void rgbToYCbCrToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        YCrCb DeeCeeTee()
        {
            YCrCb result;
            result.Y = new Byte[image.Width, image.Height];
            result.Cb = new Byte[image.Width, image.Height];
            result.Cr = new Byte[image.Width, image.Height];

            for (int i = 0; i < image.Width; i = i + 8)
            {
                for (int j = 0; j < image.Height; j = j + 8)
                {
                    
                }
            }

            return result;
        }

        double dct(int uu, int vv)
        {
            double u = uu;
            double v = vv;
            double f = 1;
            double sum = 0;
            double real = 0;

            if (u == 0)
            {
                u = (double)(1 / Math.Sqrt(2));
            }
            else
            {
                u = 1;
            }

            if (v == 0)
            {
                v = (double)(1 / Math.Sqrt(2));
            }
            else
            {
                v = 1;
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    sum += (f * (Math.Cos(((2 * i + 1) * u * Math.PI) / 16))
                        * (Math.Cos((2 * j + 1) * v * Math.PI) / 16));
                }
            }
            real = sum * (u * v) / 4;

            return real;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void dCTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] yDCT, CbDCT, CrDCT, yInvQed, CbInvQed, CrInvQed;
            byte[,] yQuantized, CbQuantized, CrQuantized, yInvDCT, CbInvDCT, CrInvDCT;
            Bitmap result, subsamplebmp;
            YCrCb subsampled, desubsampled;

            rgbConverted = rgbToYCbCr(image);
            subsampled = compress(rgbConverted.Y, rgbConverted.Cb, rgbConverted.Cr);
            //subsamplebmp = YCbCrTorgb(subsampled.Y, subsampled.Cb, subsampled.Cr);
            //decompressed = decompress(compressed.Y, compressed.Cb, compressed.Cr);
            //bmpDecompressed = YCbCrTorgb(decompressed.Y, decompressed.Cb, decompressed.Cr);

            yDCT = dctObject.forwardDCT(subsampled.Y);
            CbDCT = dctObject.forwardDCT(subsampled.Cb);
            CrDCT = dctObject.forwardDCT(subsampled.Cr);

            yQuantized = dctObject.quantize(yDCT);
            CbQuantized = dctObject.quantize(CbDCT);
            CrQuantized = dctObject.quantize(CrDCT);

            yInvQed = dctObject.inverseQuantize(yQuantized);
            CbInvQed = dctObject.inverseQuantize(CbQuantized);
            CrInvQed = dctObject.inverseQuantize(CrQuantized);
            
            yInvDCT = dctObject.inverseDCT(yInvQed);
            CbInvDCT = dctObject.inverseDCT(CbInvQed);
            CrInvDCT = dctObject.inverseDCT(CrInvQed);

            desubsampled = decompress(yInvDCT, CbInvDCT, CrInvDCT);
            result = YCbCrTorgb(desubsampled.Y, desubsampled.Cb, desubsampled.Cr);

            pictureBox2.Image = result;
        }
        
        /*public static byte[,] ImageToByte(Bitmap img)
        {
            byte[,] result = new byte[img.Width, img.Height];

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; i++)
                {
                    result = img.ToByteArray(ImageFormatConverter.);
                }
            }

            return result;
        }*/

        public static byte[,] ImageToByte(Image img)
        {
            byte[,] data = (byte[,])TypeDescriptor.GetConverter(img).ConvertTo(img, typeof(byte[,]));

            return data;
        }
    }
}
