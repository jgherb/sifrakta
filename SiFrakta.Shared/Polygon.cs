using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiFrakta_D
{
    class Polygon
    {
        public static int[,] GetKoordinaten(int ecken, int width, int height)
        {
            int modus = ecken;
            double[,] daten = new double[modus, 2];
            double seite = 10;
            double rad = seite / (2 * sin(180 / modus));
            double winkela = 360 / modus;//180-360/modus;
            double winkelc = 180 - 2 * winkela;
            //daten[0,1] = cos(winkela)*seite;
            for (int i = 0; i < modus; i++)
            {
                daten[i, 1] = cos(winkelc) * rad;
                daten[i, 0] = sin(winkelc) * rad;
                winkelc = winkelc + winkela;
            }
            double min = 0;
            for (int i = 0; i < modus; i++)
            {
                if (min > daten[i, 0])
                {
                    min = daten[i, 0];
                }
            }
            double max = 0;
            for (int i = 0; i < modus; i++)
            {
                if (max < daten[i, 1])
                {
                    max = daten[i, 1];
                }
            }
            for (int i = 0; i < modus; i++)
            {
                daten[i, 0] = daten[i, 0] - min;
                daten[i, 1] = (daten[i, 1] - max);
            }
            double max2 = 0;
            for (int i = 0; i < modus; i++)
            {
                if (max2 < daten[i, 0])
                {
                    max2 = daten[i, 0];
                }
            }
            for (int i = 0; i < modus; i++)
            {
                if (max < daten[i, 1])
                {
                    max = daten[i, 1];
                }
            }
            double scale1 = height / max;
            double scale2 = width / max2;
            double scale = scale1;
            if (scale2 < scale1)
            {
                scale = scale2;
            }
            int[,] ret = new int[ecken, 2];
            for (int i = 0; i < modus; i++)
            {
                ret[i, 0] = (int)(daten[i, 0] * scale);
                ret[i, 1] = (int)(daten[i, 1] * scale*(-1));
            }
            return ret;
        }
        static double sin(double d)
        {
            return Math.Sin(d * Math.PI / 180);
        }
        static double cos(double d)
        {
            return Math.Cos(d * Math.PI / 180);
        }
    }
}
