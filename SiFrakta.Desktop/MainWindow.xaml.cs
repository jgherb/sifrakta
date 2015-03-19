using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Threading;
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
using System.Diagnostics;

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
            tickTimer = new DispatcherTimer();
            tickTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            tickTimer.Tick += aktu;
            //InitialisierePerformanceCounter();
            aktuValue = SliderVT.Value;
            tickTimer.Start();
            timerinit = true;
            Refresh((int)GetTiefe());
        }
        static PerformanceCounter cpuCounter;
        int imgw = 1000;
        int imgh = 600;
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
        static void InitialisierePerformanceCounter() // Initialisieren
        {
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total"; // "_Total" entspricht der gesamten CPU Auslastung, Bei Computern mit mehr als 1 logischem Prozessor: "0" dem ersten Core, "1" dem zweiten...
        }
        void aktu(object sender, object e)
        {
            if (!rechnen)
            {
                Refresh((int)(GetTiefe()));
            }
        }
        void Refresh(int vts)
        {
            rechnen = true;
            int fd = 5;
            StatusBox.Text = "Rendern...";
            Stopwatch zeit = new Stopwatch();
            zeit.Start();
            byte[] pixelData = null;
            if (modus == 0)
            {
                Sierpinski s1 = new Sierpinski();
                pixelData = s1.Draw(imgw, imgh, vts, fd);//Pixeldaten mit je 4 Byte/Pixel   
            }
            if (modus == 1)
            {
                Sierpinski s1 = new Sierpinski();
                pixelData = s1.Draw5(imgw, imgh, vts, fd);
            }
            if (modus == 2)
            {
                Feigenbaum fb = new Feigenbaum();
                pixelData = fb.Draw(0.5, vts, imgw, imgh, 0, 1, 3, 4, fd);
            }
            if (modus == 3)
            {
                pixelData = Mandelbrot.DrawMandelbrotGraph(imgw, imgh);
            }
            if (modus == 4)
            {
                Zellulär z1 = new Zellulär();
                pixelData = z1.Draw(imgw, imgh);
            }
            var wb = new WriteableBitmap(imgw, imgh, 96, 96, PixelFormats.Bgr32, null);
            wb.WritePixels(new Int32Rect(0, 0, imgw, imgh), pixelData, imgw * 4, 0);
            Img1.Source = wb;
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
        private void FarbeDelta_ValueChanged(object sender)
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
            /*SpeicherWriteableBitmap = new WriteableBitmap(pixelWidth, pixelHeight);
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
            rechnen = false;*/
        }
        private async void SpeichernCick(object sender, RoutedEventArgs e)
        {
            /*fpsvalue = new double[5];
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
            StatusBox.Text = "";*/
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
        private void ToggleButton_Checked_2(object sender, RoutedEventArgs e)
        {
            ButtonS.IsChecked = false;
            ButtonM.IsChecked = false;
            ButtonL.IsChecked = true;
            SliderVT.Maximum = 100000000;
            SliderVT.Value = 1000000;
            Box_Tiefe.Text = "1000000";
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
        private void FarbeDelta_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            fpsvalue = new double[5];
            try
            {
                Box_Tiefe.Text = GetTiefe() + "";
            }
            catch (Exception e1) { }
        }
        private void SliderVT_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        private void ButtonS_Checked(object sender, RoutedEventArgs e)
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
        private void ButtonM_Checked(object sender, RoutedEventArgs e)
        {
            ButtonS.IsChecked = false;
            ButtonM.IsChecked = true;
            ButtonL.IsChecked = false;
            SliderVT.Maximum = 1000000;
            SliderVT.Value = 10000;
            Box_Tiefe.Text = "10000";
        }
    }
}
   
