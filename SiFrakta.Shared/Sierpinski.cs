using SiFrakta_D;
using System;
using System.Collections.Generic;
using System.Text;

namespace SiFrakta
{
    class Sierpinski
    {
        public Sierpinski()
        {

        }
        static int ecken = 3;
        static int width = 1000;
        static int height = 600;
        static int[,] Punkt = new int[ecken, 2];
        static int[,] Punkt3 = Polygon.GetKoordinaten(3, width, height);
        static int[,] Punkt5 = Polygon.GetKoordinaten(5, width, height);
        public Sierpinski(int h, int w, int t)
        {
            height = h;
            width = w;
            vtiefe = t;
            if ((int)Math.Sqrt((height * height) * 3 / 4) > w)
            {
                width = (int)Math.Sqrt((height * height));
            }
        }
        int vtiefe = 0;

        internal byte[] Draw(int width, int height, int vt, int fd)
        {
            vtiefe = vt;
            // 4 bytes (RGBA) required for each pixel
            byte[] result = new byte[width * height * 4];
            int resultIndex = 0;

            // Max iterations when testing whether a point is in the set
            int maxIterationCount = 50;

            // Initialisierungen / Deklarationen
            Random zufall = new Random();

            // Plot the Mandelbrot set on x-y plane
            //Zufallszahl für verwendeten Eckpunkt wird generiert
            int[] dat = new int[2];
            dat[1] = height / 2;
            for (int i = 0; i < vtiefe - 1; i++)
            {
                int pkt = zufall.Next(3);
                //Koordinaten werden bestimmt (Durchschnitt aus altem Punkt und Eckpunkt)
                dat[0] = (dat[0] + Punkt3[pkt, 0]) / 2;
                dat[1] = (dat[1] + Punkt3[pkt, 1]) / 2;
                // Shade pixel based on probability it's in the set
                if (result[(dat[1] * width + dat[0]) * 4 + 3] + fd < 256)
                {
                    result[(dat[1] * width + dat[0]) * 4 + 1] = (byte)(result[(dat[1] * width + dat[0]) * 4 + 1] + fd);// Green value of pixel
                }
            }


            return result;
        }
        internal byte[] Draw5(int width, int height, int vt, int fd)
        {
            vtiefe = vt*50;
            byte[] result = new byte[width * height * 4];
            Random zufall = new Random();
            int[] dat = new int[2];
            dat[1] = Punkt5[0, 1];
            dat[0] = Punkt5[0, 0];
            for (int i = 0; i < vtiefe - 1; i++)
            {
                int pkt = zufall.Next(5);
                dat[0] = (dat[0] + Punkt5[pkt, 0]) / 2;
                dat[1] = (dat[1] + Punkt5[pkt, 1]) / 2;
                if (result[(dat[1] * width + dat[0]) * 4 + 3] + fd < 256)
                {
                    result[(dat[1] * width + dat[0]) * 4 + 1] = (byte)(result[(dat[1] * width + dat[0]) * 4 + 1] + fd);
                }
            }
            return result;
        }
        internal byte[] DrawC(int ecken_, int width_, int height_, int vt, int fd)
        {
            if ((ecken_ != ecken)|(width_!=width)|(height_!=height))
            {
                Punkt = Polygon.GetKoordinaten(ecken_, width_, height_);
                ecken = ecken_;
                width = width_;
                height = height_;
            }
            double a = (height / (Math.Sin(72 * Math.PI / 180) + Math.Sin(36 * Math.PI / 180)));
            vtiefe = vt * 50;
            // 4 bytes (RGBA) required for each pixel
            byte[] result = new byte[width * height * 4];
            // Initialisierungen / Deklarationen
            Random zufall = new Random();
            //Zufallszahl für verwendeten Eckpunkt wird generiert
            int[] dat = new int[2];
            dat[0] = Punkt[0, 0];
            dat[1] = Punkt[0, 1];
            for (int i = 0; i < vtiefe - 1; i++)
            {
                int pkt = zufall.Next(ecken);
                //Koordinaten werden bestimmt (Durchschnitt aus altem Punkt und Eckpunkt)
                dat[0] = (dat[0] + Punkt[pkt, 0]) / 2;
                dat[1] = (dat[1] + Punkt[pkt, 1]) / 2;

                // Shade pixel based on probability it's in the set
                if (result[(dat[1] * width + dat[0]) * 4 + 3] + fd < 256)
                {
                    result[(dat[1] * width + dat[0]) * 4 + 1] = (byte)(result[(dat[1] * width + dat[0]) * 4 + 1] + fd);// Green value of pixel
                }
            }


            return result;
        }
    }
}
