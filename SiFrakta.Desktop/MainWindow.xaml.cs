using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;
using SiFrakta;

namespace SiFrakta_D
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int imgw = 500;// (int)Img1.ActualHeight;
            int imgh = 400;// (int)Img1.ActualWidth;
            Sierpinski s1 = new Sierpinski();
            byte[] pixelData = s1.Draw(imgw,imgh,1000000,1);//Pixeldaten mit je 4 Byte/Pixel
            var wb = new WriteableBitmap(imgw, imgh, 96, 96, PixelFormats.Bgr32, null);
            wb.WritePixels(new Int32Rect(0, 0, imgw, imgh), pixelData, imgw * 4, 0);
            Img1.Source = wb;
        }
        private byte[] DrawMandelbrotGraph(int width, int height)
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
                    byte grayScaleValue = Convert.ToByte(255.0 * iteration / maxIterationCount);
                    result[resultIndex++] = grayScaleValue; // Green value of pixel
                    result[resultIndex++] = grayScaleValue; // Blue value of pixel
                    result[resultIndex++] = grayScaleValue; // Red value of pixel
                    result[resultIndex++] = 255; // Alpha value of pixel
                }
               
            }
            return result;
        }
    }
}
   
