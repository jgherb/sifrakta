using System;
using System.Collections.Generic;
using System.Text;

namespace SiFrakta
{
    class Feigenbaum
    {
        public Feigenbaum()
        {

        }
        double start = 0;
        double faktor = 0;
        int tiefe = 0;
        internal byte[] Draw(double s, int t, int width, int height, double y1, double y2, double x1, double x2, int fd)
        {
            double seq = (x2 - x1) / width;
            start = s;
            tiefe = t;
            // 4 bytes (RGBA) required for each pixel
            byte[] result = new byte[width * height * 4];
            double ergebnis = start;
            int zähler = -1;
            for (double p = x1; p < x2; p = p + seq) {
                zähler = zähler + 1;
                ergebnis = start;
                for (int i = 0; i < tiefe; i++)
                {
                    //neues Folgeglied der logistischen Gleichung wird berechnet
                    ergebnis = p * ergebnis * (1 - ergebnis);
                    //Höhe im Ausgabebild wird berechnet
                    int h = height - (int)(ergebnis * (height - 1) / (y2 - y1) - (height - 1) / (y2 - y1) * y1);
                    if (zähler < width & h >= 0 & h < height)
                    {
                        if (result[(zähler + h * width) * 4 + 3] + fd < 256)
                        {
                        result[(zähler + h * width) * 4 + 1] = (byte)(result[(zähler + h * width) * 4 + 1] + fd);
                        }
                    }
                }
            }
            return result;
        }
    }
}
