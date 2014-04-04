using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;


namespace HandTracking
{
     ///<summary>
     ///MainWindow.xaml 的交互逻辑
     ///</summary>
     ///
    
    public partial class MainWindow : Window
    {
        private KinectSensorPlus sensor;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Closed += new EventHandler(MainWindow_Closed);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
            Win32.FreeConsole();
        }


        [STAThread]
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = new KinectSensorPlus();
            this.sensor.Start();
            this.colorImg.Source = this.sensor.ColorBitmap;
            this.depthImg.Source = this.sensor.DepthBitmap;

            this.sensor.readyEvent.afterDepthReady = DrawFingerTips;

            // Create a console to debug
            Win32.AllocConsole();
            Console.WriteLine("Debug Console");
        }

        public void DrawFingerTips()
        {
            Color[] C = new Color[5];
            C[0] = Colors.Red;
            C[1] = Colors.Orange;
            C[2] = Colors.Yellow;
            C[3] = Colors.Green;
            C[4] = Colors.Blue;
            int i = 0;

            List<PointDepth3D> list = this.sensor.Depth3DFingerTips;
            Console.WriteLine(list.Count);

            this.canvas.Children.Clear();
            //foreach (var element in list)
            //{
            //    //Console.WriteLine(element.X.ToString() + " " + element.Y.ToString());

            //    Ellipse ellipse = new Ellipse();
            //    ellipse.Stroke = new SolidColorBrush(C[i % 5]);
            //    ellipse.StrokeThickness = 10;
            //    ellipse.Width = 10;
            //    ellipse.Height = 10;
            //    this.canvas.Children.Add(ellipse);
            //    Canvas.SetLeft(ellipse, element.X);
            //    Canvas.SetTop(ellipse, element.Y);
            //}

            if (this.sensor.Fingers[FingerType.ThumbRight].TrackingState == FingerTrackingState.Tracked)
            {
                Ellipse e = new Ellipse();
                e.Stroke = new SolidColorBrush(Colors.Red);
                e.StrokeThickness = 10;
                e.Width = 10;
                e.Height = 10;
                this.canvas.Children.Add(e);
                Canvas.SetLeft(e, this.sensor.Fingers.GetPoint2Position(FingerType.ThumbRight).X);
                Canvas.SetTop(e, this.sensor.Fingers.GetPoint2Position(FingerType.ThumbRight).Y);
            }

            if (this.sensor.Fingers[FingerType.IndexRight].TrackingState == FingerTrackingState.Tracked)
            {
                Ellipse e = new Ellipse();
                e.Stroke = new SolidColorBrush(Colors.Orange);
                e.StrokeThickness = 10;
                e.Width = 10;
                e.Height = 10;
                this.canvas.Children.Add(e);
                Canvas.SetLeft(e, this.sensor.Fingers.GetPoint2Position(FingerType.IndexRight).X);
                Canvas.SetTop(e, this.sensor.Fingers.GetPoint2Position(FingerType.IndexRight).Y);
            }
            if (this.sensor.Fingers[FingerType.MiddleRight].TrackingState == FingerTrackingState.Tracked)
            {
                Ellipse e = new Ellipse();
                e.Stroke = new SolidColorBrush(Colors.Black);
                e.StrokeThickness = 10;
                e.Width = 10;
                e.Height = 10;
                this.canvas.Children.Add(e);
                Canvas.SetLeft(e, this.sensor.Fingers.GetPoint2Position(FingerType.MiddleRight).X);
                Canvas.SetTop(e, this.sensor.Fingers.GetPoint2Position(FingerType.MiddleRight).Y);
            }
            if (this.sensor.Fingers[FingerType.RingRight].TrackingState == FingerTrackingState.Tracked)
            {
                Ellipse e = new Ellipse();
                e.Stroke = new SolidColorBrush(Colors.Green);
                e.StrokeThickness = 10;
                e.Width = 10;
                e.Height = 10;
                this.canvas.Children.Add(e);
                Canvas.SetLeft(e, this.sensor.Fingers.GetPoint2Position(FingerType.RingRight).X);
                Canvas.SetTop(e, this.sensor.Fingers.GetPoint2Position(FingerType.RingRight).Y);
            }
            if (this.sensor.Fingers[FingerType.LittleRight].TrackingState == FingerTrackingState.Tracked)
            {
                Ellipse e = new Ellipse();
                e.Stroke = new SolidColorBrush(Colors.Blue);
                e.StrokeThickness = 10;
                e.Width = 10;
                e.Height = 10;
                this.canvas.Children.Add(e);
                Canvas.SetLeft(e, this.sensor.Fingers.GetPoint2Position(FingerType.LittleRight).X);
                Canvas.SetTop(e, this.sensor.Fingers.GetPoint2Position(FingerType.LittleRight).Y);
            }
        }
    }
}
