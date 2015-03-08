using System;
using System.Numerics;
using System.Collections.Generic;
using System.Text;

namespace SiFrakta
{
    class Mandelbrot
    {
        Mandelbrot()
        {

        }
        internal static byte[] DrawMandelbrotGraph(int width, int height)
        {
            // 4 bytes required for each pixel
            byte[] result = new byte[width * height * 4];
            int resultIndex = 0;

            // Max iterations when testing whether a point is in the set
            int maxIterationCount = 50;

            // Choose intervals
            Complex minimum = new Complex(-2.5, -1.0);
            Complex maximum = new Complex(1.0, 1);

            // Normalize x and y values based on chosen interval and size of WriteableBitmap
            double xScaleFactor = (maximum.Real - minimum.Real) / width;
            double yScaleFactor = (maximum.Imaginary - minimum.Imaginary) / height;

            // Plot the Mandelbrot set on x-y plane
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Complex c = new Complex(minimum.Real + x * xScaleFactor, maximum.Imaginary - y * yScaleFactor);
                    Complex z = new Complex(c.Real, c.Imaginary);

                    // Iterate with simple escape-time algorithm
                    int iteration = 0;
                    while (z.Magnitude < 2 && iteration < maxIterationCount)
                    {
                        z = (z * z) + c;
                        iteration++;
                    }

                    // Shade pixel based on probability it's in the set
                    byte grayScaleValue = Convert.ToByte(255 - 255.0 * iteration / maxIterationCount);
                    result[resultIndex++] = 0; // Green value of pixel
                    result[resultIndex++] = (byte)(255 - grayScaleValue); // Blue value of pixel
                    result[resultIndex++] = 0; // Red value of pixel
                    result[resultIndex++] = 255;            // Alpha value of pixel
                }
            }

            return result;
        }
    }
}
