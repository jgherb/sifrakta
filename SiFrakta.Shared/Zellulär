using System;
using System.Collections.Generic;
using System.Text;

namespace SiFrakta
{
    class Zellulär
    {
        int n = 6;
        int[] Ü = { 1, 2, 3};
        int[] G = { 2, 3};
        double start = 0;
        double faktor = 0;
        int tiefe = 0;
        internal byte[] Draw(int width, int height)
        {
            // 4 bytes (RGBA) required for each pixel
            byte[] result = new byte[width * height * 4];
            Random r1 = new Random();
            for (int w = 0; w < width; w++)
            {
                if (r1.Next(0, 2) == 1)
                {
                    result[(w + 0 * width) * 4 + 1] = 255;
                }
            }
            for (int h = 1; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int summe = 0;
                    for (int i = 1; i <= n / 2; i++)
                    {
                        if(w-i>-1) 
                        {
                            if (result[(w - i + (h - 1) * width) * 4 + 1] == 255)
                            {
                                summe = summe + 1;
                            }
                        }
                        if (w + i < width)
                        {
                            if (result[(w + i + (h - 1) * width) * 4 + 1] == 255)
                            {
                                summe = summe + 1;
                            }
                        }
                    }
                    if (result[(w + (h-1) * width) * 4 + 1]==255)
                    {
                        if (Array.IndexOf(Ü, summe) != -1)
                        {
                            result[(w + h * width) * 4 + 1] = 255;
                        }
                    }
                    else
                    {
                        if (Array.IndexOf(G, summe) != -1)
                        {
                            result[(w + h * width) * 4 + 1] = 255;
                        }
                    }
                }
            }
            return result;
        }
    }
}
