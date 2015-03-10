using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.Threading;
using Windows.Storage.Provider;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SiFrakta
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //static PerformanceCounter cpuCounter;
        int fpscounter = 0;
        double[] fpsvalue = new double[100];
        int tapcounter = 1;
        double pX = 0.5;
        double pY = 0.5;
        bool timerinit = false;
        double aktuValue = 0;
        int modus = 0;
        bool scalelog = false;
        bool rechnen = false;
        bool timeraktiv = false;
        DispatcherTimer dispatcherTimer;
        DispatcherTimer tickTimer;
        DateTimeOffset startTime;
        DateTimeOffset lastTime;
        int timesTicked = 500;
        private WriteableBitmap Scenario4WriteableBitmap;
        private WriteableBitmap SpeicherWriteableBitmap;
        public MainPage()
        {
            this.InitializeComponent();
            Scenario4WriteableBitmap = new WriteableBitmap((int)ImgContainer.Width, (int)ImgContainer.Height);
            Img1.Source = Scenario4WriteableBitmap;
            tickTimer = new DispatcherTimer();
            tickTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            tickTimer.Tick += aktu;
            //InitialisierePerformanceCounter();
            aktuValue = SliderVT.Value;
            tickTimer.Start();
            timerinit = true;
            Refresh((int)GetTiefe());
        }
        /*static void InitialisierePerformanceCounter() // Initialisieren
        {
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total"; // "_Total" entspricht der gesamten CPU Auslastung, Bei Computern mit mehr als 1 logischem Prozessor: "0" dem ersten Core, "1" dem zweiten...
        }*/
        void aktu(object sender, object e)
        {
            if (!rechnen)
            {
                Refresh((int)(GetTiefe()));
            }
        }
        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            startTime = DateTimeOffset.Now;
            lastTime = startTime;
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            DateTimeOffset time = DateTimeOffset.Now;
            TimeSpan span = time - lastTime;
            lastTime = time;
            fbDraw(timesTicked);
            timesTicked = timesTicked + 200;
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
                    byte grayScaleValue = Convert.ToByte(255 - 255.0 * iteration / maxIterationCount);
                    result[resultIndex++] = grayScaleValue; // Green value of pixel
                    result[resultIndex++] = grayScaleValue; // Blue value of pixel
                    result[resultIndex++] = grayScaleValue; // Red value of pixel
                    result[resultIndex++] = 255;            // Alpha value of pixel
                }
            }

            return result;
        }
        async void Refresh(int vts)
        {
            StatusBox.Text = "Rendern...";
            Stopwatch zeit = new Stopwatch();
            zeit.Start();
            rechnen = true;
            if (modus == 0)
            {
                Sierpinski si = new Sierpinski((int)ImgContainer.Width, (int)ImgContainer.Height, 10);
                int pixelWidth = Scenario4WriteableBitmap.PixelWidth;
                int pixelHeight = Scenario4WriteableBitmap.PixelHeight;
                int fdelta = (int)FarbeDelta.Value;
                // Asynchronously graph the Mandelbrot set on a background thread
                byte[] result = null;
                await ThreadPool.RunAsync(new WorkItemHandler(
                    (IAsyncAction action) =>
                    {
                        result = si.Draw(pixelWidth, pixelHeight, vts, fdelta);
                    }
                    ));

                // Open a stream to copy the graph to the WriteableBitmap's pixel buffer
                using (Stream stream = Scenario4WriteableBitmap.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(result, 0, result.Length);
                }

                // Redraw the WriteableBitmap
                Scenario4WriteableBitmap.Invalidate();
            }
            if (modus == 1)
            {
                Sierpinski si = new Sierpinski((int)ImgContainer.Width, (int)ImgContainer.Height, 10);
                int pixelWidth = Scenario4WriteableBitmap.PixelWidth;
                int pixelHeight = Scenario4WriteableBitmap.PixelHeight;
                int fdelta = (int)FarbeDelta.Value;
                // Asynchronously graph the Mandelbrot set on a background thread
                byte[] result = null;
                await ThreadPool.RunAsync(new WorkItemHandler(
                    (IAsyncAction action) =>
                    {
                        result = si.Draw5(pixelWidth, pixelHeight, vts, fdelta);
                    }
                    ));

                // Open a stream to copy the graph to the WriteableBitmap's pixel buffer
                using (Stream stream = Scenario4WriteableBitmap.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(result, 0, result.Length);
                }

                // Redraw the WriteableBitmap
                Scenario4WriteableBitmap.Invalidate();
            }
            if (modus == 2)
            {
                Feigenbaum fb = new Feigenbaum();
                int pixelWidth = Scenario4WriteableBitmap.PixelWidth;
                int pixelHeight = Scenario4WriteableBitmap.PixelHeight;
                int fdelta = (int)FarbeDelta.Value;
                //StatusBox.Text = pY - 1 / tapcounter + 3 + "";
                // Asynchronously graph the Mandelbrot set on a background thread
                byte[] result = null;
                await ThreadPool.RunAsync(new WorkItemHandler(
                    (IAsyncAction action) =>
                    {
                        result = fb.Draw(0.5, vts, pixelWidth, pixelHeight, 0, 1, 3, 4, fdelta);
                    }
                    ));

                // Open a stream to copy the graph to the WriteableBitmap's pixel buffer
                using (Stream stream = Scenario4WriteableBitmap.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(result, 0, result.Length);
                }

                // Redraw the WriteableBitmap
                Scenario4WriteableBitmap.Invalidate();
            }
            if (modus == 3)
            {
                int pixelWidth = Scenario4WriteableBitmap.PixelWidth;
                int pixelHeight = Scenario4WriteableBitmap.PixelHeight;

                // Asynchronously graph the Mandelbrot set on a background thread
                byte[] result = null;
                await ThreadPool.RunAsync(new WorkItemHandler(
                    (IAsyncAction action) =>
                    {
                        result = Mandelbrot.DrawMandelbrotGraph(pixelWidth, pixelHeight);
                    }
                    ));
                // Open a stream to copy the graph to the WriteableBitmap's pixel buffer
                using (Stream stream = Scenario4WriteableBitmap.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(result, 0, result.Length);
                }

                // Redraw the WriteableBitmap
                Scenario4WriteableBitmap.Invalidate();
            }
            if (modus == 4)
            {
                int pixelWidth = Scenario4WriteableBitmap.PixelWidth;
                int pixelHeight = Scenario4WriteableBitmap.PixelHeight;
                Zellulär z1 = new Zellulär();
                // Asynchronously graph the Mandelbrot set on a background thread
                byte[] result = null;
                await ThreadPool.RunAsync(new WorkItemHandler(
                    (IAsyncAction action) =>
                    {
                        result = z1.Draw(pixelWidth, pixelHeight);
                    }
                    ));
                // Open a stream to copy the graph to the WriteableBitmap's pixel buffer
                using (Stream stream = Scenario4WriteableBitmap.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(result, 0, result.Length);
                }

                // Redraw the WriteableBitmap
                Scenario4WriteableBitmap.Invalidate();
            }
            rechnen = false;
            zeit.Stop();
            fpsvalue[fpscounter] = 1000 / zeit.ElapsedMilliseconds;
            if (fpscounter == fpsvalue.Length - 1)
            {
                fpscounter = 0;
            }
            fpscounter = fpscounter + 1;
            double sum = 0;
            foreach (double v in fpsvalue)
            {
                sum = sum + v;
            }
            double fpsv = sum / fpsvalue.Length;
            if (fpsv >= 10)
            {
                FBSbox.Text = (int)(fpsv) + " fps";
            }
            else if (fpsv > 1)
            {
                FBSbox.Text = String.Format("{0:0.0}", fpsv) + " fps";
            }
            else
            {
                FBSbox.Text = String.Format("{0:0.00}", fpsv) + " fps";
            }
            StatusBox.Text = "";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fpsvalue = new double[5];
            modus = 0;
            FarbeDelta.Visibility = Visibility.Visible;
            SliderVT.Visibility = Visibility.Visible;
            Box_Farbe.Visibility = Visibility.Visible;
            Box_Tiefe.Visibility = Visibility.Visible;
            Refresh((int)(GetTiefe()));
            //SierpinskiButton.Background = new Brush();
        }
        int TTcreate()
        {
            return 0;
        }
        private void MandelbrotButton_Click(object sender, RoutedEventArgs e)
        {
            fpsvalue = new double[5];
            modus = 3;
            FarbeDelta.Visibility = Visibility.Collapsed;
            SliderVT.Visibility = Visibility.Collapsed;
            Box_Farbe.Visibility = Visibility.Collapsed;
            Box_Tiefe.Visibility = Visibility.Collapsed;
            Refresh((int)(GetTiefe()));
        }

        private void FeigenbaumButton_Click(object sender, RoutedEventArgs e)
        {
            fpsvalue = new double[5];
            modus = 2;
            FarbeDelta.Visibility = Visibility.Visible;
            SliderVT.Visibility = Visibility.Visible;
            Box_Farbe.Visibility = Visibility.Visible;
            Box_Tiefe.Visibility = Visibility.Visible;
            Refresh((int)(GetTiefe()));
        }
        private async void fbDraw(int t)
        {
            Feigenbaum fb = new Feigenbaum();
            int pixelWidth = Scenario4WriteableBitmap.PixelWidth;
            int pixelHeight = Scenario4WriteableBitmap.PixelHeight;
            double start = 0.5;
            int tiefe = t;
            double X1 = 2.8;
            double X2 = 4;
            double Y1 = 0;
            double Y2 = 1;
            int fdelta = (int)FarbeDelta.Value;
            // Asynchronously graph the Feigenbaum set on a background thread
            byte[] result = null;
            await ThreadPool.RunAsync(new WorkItemHandler(
                (IAsyncAction action) =>
                {
                    result = fb.Draw(start, tiefe, pixelWidth, pixelHeight, Y1, Y2, X1, X2, fdelta);
                }
                ));
            // Open a stream to copy the graph to the WriteableBitmap's pixel buffer
            using (Stream stream = Scenario4WriteableBitmap.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(result, 0, result.Length);
            }

            // Redraw the WriteableBitmap
            Scenario4WriteableBitmap.Invalidate();
        }

        private void TimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (timeraktiv)
            {
                dispatcherTimer.Stop();
                //TimerButton.Content = "Timer Starten";
                timeraktiv = false;
                tickTimer.Start();
            }
            else
            {
                //TimerButton.Content = "Timer Stoppen";
                DispatcherTimerSetup();
                timeraktiv = true;
                tickTimer.Stop();
            }
        }

        private void SierpinskiButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            fpsvalue = new double[5];
            modus = 1;
            FarbeDelta.Visibility = Visibility.Visible;
            SliderVT.Visibility = Visibility.Visible;
            Box_Farbe.Visibility = Visibility.Visible;
            Box_Tiefe.Visibility = Visibility.Visible;
            Refresh((int)(GetTiefe()));
        }

        private void SliderVT_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            fpsvalue = new double[5];
            try
            {
                Box_Tiefe.Text = GetTiefe() + "";
            }
            catch (Exception e1) { }
        }

        private void Box_Tiefe_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SliderVT.Value = GetScale();
            }
            catch (Exception e1) { }
        }

        private void RenderAuto_Checked(object sender, RoutedEventArgs e)
        {
            if (timerinit) { tickTimer.Start(); }
        }

        private void RenderAuto_Unchecked(object sender, RoutedEventArgs e)
        {
            tickTimer.Stop();
        }

        private void RenderClick(object sender, RoutedEventArgs e)
        {
            fpsvalue = new double[5];
            Refresh((int)(GetTiefe()));
        }

        private void Box_Farbe_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                FarbeDelta.Value = Double.Parse(Box_Farbe.Text);
            }
            catch (Exception e1) { }
        }

        private void FarbeDelta_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            fpsvalue = new double[5];
            try
            {
                Box_Farbe.Text = FarbeDelta.Value + "";
            }
            catch (Exception e1) { }
        }

        async void Saving(int vts, int pixelWidth, int pixelHeight)
        {
            SpeicherWriteableBitmap = new WriteableBitmap(pixelWidth, pixelHeight);
            rechnen = true;
            if (modus == 0)
            {
                Sierpinski si = new Sierpinski(Int32.Parse(SaveX.Text), Int32.Parse(SaveY.Text), 10);
                int fdelta = (int)FarbeDelta.Value;
                // Asynchronously graph the Mandelbrot set on a background thread
                byte[] result = null;
                await ThreadPool.RunAsync(new WorkItemHandler(
                    (IAsyncAction action) =>
                    {
                        result = si.Draw(pixelWidth, pixelHeight, vts, fdelta);
                    }
                    ));
                using (Stream stream = SpeicherWriteableBitmap.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(result, 0, result.Length);
                }
            }
            if (modus == 1)
            {
                Sierpinski si = new Sierpinski(Int32.Parse(SaveX.Text), Int32.Parse(SaveY.Text), 10);
                int fdelta = (int)FarbeDelta.Value;
                // Asynchronously graph the Mandelbrot set on a background thread
                byte[] result = null;
                await ThreadPool.RunAsync(new WorkItemHandler(
                    (IAsyncAction action) =>
                    {
                        result = si.Draw5(pixelWidth, pixelHeight, vts, fdelta);
                    }
                    ));
                using (Stream stream = SpeicherWriteableBitmap.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(result, 0, result.Length);
                }
            }
            if (modus == 2)
            {
                Feigenbaum fb = new Feigenbaum();
                int fdelta = (int)FarbeDelta.Value;
                // Asynchronously graph the Mandelbrot set on a background thread
                byte[] result = null;
                await ThreadPool.RunAsync(new WorkItemHandler(
                    (IAsyncAction action) =>
                    {
                        result = fb.Draw(0.5, vts, pixelWidth, pixelHeight, 0, 1, 3, 4, fdelta);
                    }
                    ));
                using (Stream stream = SpeicherWriteableBitmap.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(result, 0, result.Length);
                }
            }
            if (modus == 3)
            {
                // Asynchronously graph the Mandelbrot set on a background thread
                byte[] result = null;
                await ThreadPool.RunAsync(new WorkItemHandler(
                    (IAsyncAction action) =>
                    {
                        result = Mandelbrot.DrawMandelbrotGraph(pixelWidth, pixelHeight);
                    }
                    ));
                // Open a stream to copy the graph to the WriteableBitmap's pixel buffer
                using (Stream stream = SpeicherWriteableBitmap.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(result, 0, result.Length);
                }
            }
            rechnen = false;
        }
        private async void SpeichernCick(object sender, RoutedEventArgs e)
        {
            fpsvalue = new double[5];
            StatusBox.Text = "Saving Bitmap...";
            Saving((int)(GetTiefe()), Int32.Parse(SaveX.Text), Int32.Parse(SaveY.Text));
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("JPG File", new List<string>() { ".jpg" });
            StorageFile savefile = await picker.PickSaveFileAsync();
            if (savefile == null)
                return;
            IRandomAccessStream stream = await savefile.OpenAsync(FileAccessMode.ReadWrite);
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            // Get pixels of the WriteableBitmap object 
            Stream pixelStream = SpeicherWriteableBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);
            // Save the image file with jpg extension 
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)SpeicherWriteableBitmap.PixelWidth, (uint)SpeicherWriteableBitmap.PixelHeight, 96.0, 96.0, pixels);
            await encoder.FlushAsync();
            encoder.FlushAsync();
            StatusBox.Text = "";
        }

        private void ToggleButton_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                ButtonS.IsChecked = true;
                ButtonM.IsChecked = false;
                ButtonL.IsChecked = false;
                scalelog = false;
                SliderVT.Maximum = 10000;
                SliderVT.Value = 1;
                Box_Tiefe.Text = "1";
            }
            catch (Exception ee)
            {

            }
        }
        int GetTiefe()
        {
            int ret = 0;
            if (scalelog)
            {
                ret = (int)Math.Pow(SliderVT.Value, 2);
            }
            else
            {
                ret = (int)SliderVT.Value;
            }
            return ret;
        }
        int GetScale()
        {
            int ret = 0;
            if (scalelog)
            {
                ret = (int)Math.Sqrt(Double.Parse(Box_Tiefe.Text));
            }
            else
            {
                ret = (int)SliderVT.Value;
            }
            return ret;
        }

        private void ToggleButton_Checked_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ButtonS.IsChecked = false;
            ButtonM.IsChecked = true;
            ButtonL.IsChecked = false;
            SliderVT.Maximum = 1000000;
            SliderVT.Value = 10000;
            Box_Tiefe.Text = "10000";
        }

        private void ToggleButton_Checked_2(object sender, RoutedEventArgs e)
        {
            ButtonS.IsChecked = false;
            ButtonM.IsChecked = false;
            ButtonL.IsChecked = true;
            SliderVT.Maximum = 100000000;
            SliderVT.Value = 1000000;
            Box_Tiefe.Text = "1000000";
        }

        private void Image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            /*tapcounter = tapcounter + 1;
            pX = e.GetCurrentPoint(sender as UIElement).Position.X / ImgContainer.Width;
            pY = e.GetCurrentPoint(sender as UIElement).Position.Y / ImgContainer.Height;
            Refresh((int)(GetTiefe()));*/
        }

        private void ZellButton_Click(object sender, RoutedEventArgs e)
        {
            fpsvalue = new double[5];
            modus = 4;
            FarbeDelta.Visibility = Visibility.Visible;
            SliderVT.Visibility = Visibility.Visible;
            Box_Farbe.Visibility = Visibility.Visible;
            Box_Tiefe.Visibility = Visibility.Visible;
            Refresh((int)(GetTiefe()));
        }
    }
}
