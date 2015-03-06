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
        int height = 0;
        int width = 0;
        int vtiefe = 0;

        internal byte[] Draw(int width, int height, int vt)
        {
            vtiefe = vt;
            int[,] Punkt3 = new int[3, 2];
            Punkt3[0, 0] = 0;
            Punkt3[0, 1] = height / 2;
            Punkt3[1, 0] = (int)Math.Sqrt((height * height) * 3 / 4);
            Punkt3[1, 1] = 0;
            Punkt3[2, 0] = (int)Math.Sqrt((height * height) * 3 / 4);
            Punkt3[2, 1] = height;
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
            for (int i = 0; i < vtiefe - 1; i++)
            {
                int pkt = zufall.Next(3);
                //Koordinaten werden bestimmt (Durchschnitt aus altem Punkt und Eckpunkt)
                dat[0] = (dat[0] + Punkt3[pkt, 0]) / 2;
                dat[1] = (dat[1] + Punkt3[pkt, 1]) / 2;

                // Shade pixel based on probability it's in the set
                result[(dat[0] * width + dat[1]) * 4 + 1] = (byte)(result[(dat[0] * width + dat[1]) * 4+1] +20);// Green value of pixel
            }


            return result;
        }
        internal byte[] Draw5(int width, int height, int vt)
        {
            double a = (height / (Math.Sin(72 * Math.PI / 180) + Math.Sin(36 * Math.PI / 180)));
            vtiefe = vt*50;
            int[,] Punkt3 = new int[5, 2];
            Punkt3[0, 0] = 0;
            Punkt3[0, 1] = (int)(Math.Cos(72 * Math.PI / 180) * a);
            Punkt3[1, 0] = 0;
            Punkt3[1, 1] = (int)(Math.Cos(72 * Math.PI / 180) * a + a);
            Punkt3[2, 0] = (int)(Math.Sin(72 * Math.PI / 180) * a);
            Punkt3[2, 1] = 0;
            Punkt3[3, 0] = (int)(Math.Sin(72 * Math.PI / 180) * a);
            Punkt3[3, 1] = (int)(2 * Math.Cos(72 * Math.PI / 180) * a + a);
            Punkt3[4, 0] = (int)(Math.Sin(36 * Math.PI / 180) * a + Math.Sin(72 * Math.PI / 180) * a);
            Punkt3[4, 1] = (int)(2 * Math.Cos(72 * Math.PI / 180) * a + a) / 2;
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
            for (int i = 0; i < vtiefe - 1; i++)
            {
                int pkt = zufall.Next(5);
                //Koordinaten werden bestimmt (Durchschnitt aus altem Punkt und Eckpunkt)
                dat[0] = (dat[0] + Punkt3[pkt, 0]) / 2;
                dat[1] = (dat[1] + Punkt3[pkt, 1]) / 2;

                // Shade pixel based on probability it's in the set
                if (result[(dat[0] * width + dat[1]) * 4 + 3] < 251)
                result[(dat[0] * width + dat[1]) * 4 + 1] = (byte)(result[(dat[0] * width + dat[1]) * 4 +1]+5);// Green value of pixel
            }


            return result;
        }
    }
}
