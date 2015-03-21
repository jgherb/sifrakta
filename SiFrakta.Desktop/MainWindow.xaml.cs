using System;
using System.ComponentModel;
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
using System.IO;
using System.Windows;

namespace SiFrakta_D
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Konstruktor
        public MainWindow()
        {
            InitializeComponent();
            tickTimer = new DispatcherTimer();
            tickTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);
            tickTimer.Tick += aktu;
            //InitialisierePerformanceCounter();
            aktuValue = SliderVT.Value;
            tickTimer.Start();
            timerinit = true;
            Refresh((int)GetTiefe());
        }
        //Deklarationen
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
        DispatcherTimer tickTimer;
        int timesTicked = 500;
        void Refresh(int vts)
        {
            rechnen = true;
            int fd = (int)FarbeDelta.Value;
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
        int TTcreate()
        {
            return 0;
        }
        //Initialisieren
        static void InitialisierePerformanceCounter()
        {
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total"; // "_Total" entspricht der gesamten CPU Auslastung, Bei Computern mit mehr als 1 logischem Prozessor: "0" dem ersten Core, "1" dem zweiten...
        }
        //--Renderaufruf
        void aktu(object sender, object e)
        {
            if (!rechnen)
            {
                Refresh((int)(GetTiefe()));
            }
        }
        private void RenderClick(object sender, RoutedEventArgs e)
        {
            fpsvalue = new double[5];
            Refresh((int)(GetTiefe()));
        }
        //--Fraktalmodus wählen
        private void ZellButton_Click(object sender, RoutedEventArgs e)
        {
            SierpinskiButton.Background = new SolidColorBrush(Colors.White);
            MandelbrotButton.Background = new SolidColorBrush(Colors.White);
            FeigenbaumButton.Background = new SolidColorBrush(Colors.White);
            ZellButton.Background = new SolidColorBrush(Colors.Green);
            SierpinskiButton_Copy.Background = new SolidColorBrush(Colors.White);
            fpsvalue = new double[5];
            modus = 4;
            FarbeDelta.Visibility = Visibility.Visible;
            SliderVT.Visibility = Visibility.Visible;
            Box_Farbe.Visibility = Visibility.Visible;
            Box_Tiefe.Visibility = Visibility.Visible;
            Refresh((int)(GetTiefe()));
        }
        private void MandelbrotButton_Click(object sender, RoutedEventArgs e)
        {
            SierpinskiButton.Background = new SolidColorBrush(Colors.White);
            MandelbrotButton.Background = new SolidColorBrush(Colors.Green);
            FeigenbaumButton.Background = new SolidColorBrush(Colors.White);
            ZellButton.Background = new SolidColorBrush(Colors.White);
            SierpinskiButton_Copy.Background = new SolidColorBrush(Colors.White);
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
            SierpinskiButton.Background = new SolidColorBrush(Colors.White);
            MandelbrotButton.Background = new SolidColorBrush(Colors.White);
            FeigenbaumButton.Background = new SolidColorBrush(Colors.Green);
            ZellButton.Background = new SolidColorBrush(Colors.White);
            SierpinskiButton_Copy.Background = new SolidColorBrush(Colors.White);
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
            SierpinskiButton.Background = new SolidColorBrush(Colors.White);
            MandelbrotButton.Background = new SolidColorBrush(Colors.White);
            FeigenbaumButton.Background = new SolidColorBrush(Colors.White);
            ZellButton.Background = new SolidColorBrush(Colors.White);
            SierpinskiButton_Copy.Background = new SolidColorBrush(Colors.Green);
            fpsvalue = new double[5];
            modus = 1;
            FarbeDelta.Visibility = Visibility.Visible;
            SliderVT.Visibility = Visibility.Visible;
            Box_Farbe.Visibility = Visibility.Visible;
            Box_Tiefe.Visibility = Visibility.Visible;
            Refresh((int)(GetTiefe()));
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SierpinskiButton.Background = new SolidColorBrush(Colors.Green);
            MandelbrotButton.Background = new SolidColorBrush(Colors.White);
            FeigenbaumButton.Background = new SolidColorBrush(Colors.White);
            ZellButton.Background = new SolidColorBrush(Colors.White);
            SierpinskiButton_Copy.Background = new SolidColorBrush(Colors.White);
            fpsvalue = new double[5];
            modus = 0;
            FarbeDelta.Visibility = Visibility.Visible;
            SliderVT.Visibility = Visibility.Visible;
            Box_Farbe.Visibility = Visibility.Visible;
            Box_Tiefe.Visibility = Visibility.Visible;
            Refresh((int)(GetTiefe()));
        }
        //--Rendermodus wählen
        private void RenderAuto_Checked(object sender, RoutedEventArgs e)
        {
            if (timerinit) { tickTimer.Start(); }
        }
        private void RenderAuto_Unchecked(object sender, RoutedEventArgs e)
        {
            tickTimer.Stop();
        }
        //--GUI Bindings
        private void ToggleButton_Checked_2(object sender, RoutedEventArgs e)
        {
            ButtonS.IsChecked = false;
            ButtonM.IsChecked = false;
            ButtonL.IsChecked = true;
            SliderVT.Maximum = 100000000;
            SliderVT.Value = 1000000;
            Box_Tiefe.Text = "1000000";
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
        private void FarbeDelta_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            fpsvalue = new double[5];
            try
            {
                Box_Farbe.Text = (int)FarbeDelta.Value + "";
            }
            catch (Exception e1) { }
        }
        private void SliderVT_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            fpsvalue = new double[5];
            try
            {
                Box_Tiefe.Text = GetTiefe() +"";
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
        //--Speicheraufruf
        void Saving(int vts, int savew, int saveh, String file)
        {
            savew = 1000;
            saveh = 1000;
            rechnen = true;
            int fd = 5;
            StatusBox.Text = "Saving...";
            Stopwatch zeit = new Stopwatch();
            zeit.Start();
            byte[] pixelData = null;
            if (modus == 0)
            {
                Sierpinski s1 = new Sierpinski();
                pixelData = s1.Draw(savew, saveh, vts, fd);//Pixeldaten mit je 4 Byte/Pixel   
            }
            if (modus == 1)
            {
                Sierpinski s1 = new Sierpinski();
                pixelData = s1.Draw5(savew, saveh, vts, fd);
            }
            if (modus == 2)
            {
                Feigenbaum fb = new Feigenbaum();
                pixelData = fb.Draw(0.5, vts, savew, saveh, 0, 1, 3, 4, fd);
            }
            if (modus == 3)
            {
                pixelData = Mandelbrot.DrawMandelbrotGraph(savew, saveh);
            }
            if (modus == 4)
            {
                Zellulär z1 = new Zellulär();
                pixelData = z1.Draw(savew, saveh);
            }
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(BitmapSource.Create(savew, saveh, 96D, 96D, PixelFormats.Bgr32, BitmapPalettes.WebPalette, pixelData, imgw * 4)));//Anhand der Pixeldaten erzeugen
            using (var sw = File.Create(file))//Stream erzeugen
                encoder.Save(sw);//Bild speichern
            StatusBox.Text = "";
        }
        private void SpeichernCick(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Fraktal"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Pictures (.png)|*.png"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                Saving((int)(GetTiefe()), Int32.Parse(SaveX.Text), Int32.Parse(SaveY.Text), filename);
            }
            StatusBox.Text = "";
        }
    }
}
