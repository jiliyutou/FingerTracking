using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace HandTracking
{
    public class KinectSensorPlus
    {
        private KinectSensor sensor;

        private WriteableBitmap depthBitmap;
        private WriteableBitmap colorBitmap;
        private DepthImagePixel[] depthPixels;
        private byte[] colorPixels;
        private byte[] depthColor;
        private Skeleton[] skeletonData = new Skeleton[0];
        private bool skeletonReadyFlag = false;
        private int DepthFrameWidth;
        private int DepthFrameHeight;

        const int BLUE = 0;
        const int GREEN = 1;
        const int RED = 2;
        const int AlPHA = 3;

        const int rectWidth = 120;
        const int rectHeight = 120;
        private FingerDetection detector;
        private PointDepth3D[] rectDepth3D;

        public KinectReadyEvent readyEvent { get; set; }


        #region Dev
        private static int timeStamp = 0;   //记录帧号，每3000帧循环一次，100秒
        PointSkeleton3D rightWrist;
        PointSkeleton3D rightHand;
        PointSkeleton3D rightElbow;
        #endregion

        public FingerIdentification Fingers { get; set; }

        /// <summary>
        /// Constructor, object Initialize
        /// </summary>
        public KinectSensorPlus() 
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }
            if (null != this.sensor)
            {
                this.detector = new FingerDetection(this.sensor);
                this.Fingers = new FingerIdentification(this.sensor);
                this.rectDepth3D = new PointDepth3D[rectWidth * rectHeight];
                this.readyEvent = new KinectReadyEvent();

                this.sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
                this.sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
                this.sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];
                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];
                this.depthColor = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                this.depthBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                this.DepthFrameWidth = this.sensor.DepthStream.FrameWidth;
                this.DepthFrameHeight = this.sensor.DepthStream.FrameHeight;
            }
        }

        /// <summary>
        /// Depth Frame Ready
        /// Kinect tracks skeletons,  localize the Skeleton Hand Joint Position
        /// Map Skeleton point given by Kinect to PointDepth3D , Add result to HandTracker's List buffer for computing every frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if (skeletonData.Length != skeletonFrame.SkeletonArrayLength)
                    {
                        skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    int personCount = 0;
                    foreach (Skeleton sk in skeletonData)
                    {
                        if (sk.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            //Skeleton point map to depth point given by Kinect
                            CoordinateMapper mapper = new CoordinateMapper(this.sensor);
                            DepthImagePoint left = mapper.MapSkeletonPointToDepthPoint(
                                sk.Joints[JointType.HandLeft].Position,
                                this.sensor.DepthStream.Format);

                            DepthImagePoint right = mapper.MapSkeletonPointToDepthPoint(
                                sk.Joints[JointType.HandRight].Position,
                                this.sensor.DepthStream.Format);

                            rightWrist = new PointSkeleton3D(sk.Joints[JointType.WristRight].Position);
                            rightHand = new PointSkeleton3D(sk.Joints[JointType.HandRight].Position);
                            rightElbow = new PointSkeleton3D(sk.Joints[JointType.ElbowRight].Position);

                            personCount++; 

                        }
                    }
                    skeletonReadyFlag = (personCount == 0 ? false : true);
                }
            }
        }



        /// <summary>
        /// Depth Frame Ready
        /// According to the Hand position tracked by Kinect, extract small Rectangular pixels matrix for hand tracking
        /// Extract pixels for each frame, each pixel in matrix has the X,Y,Depth information
        /// Construct Hand instance, do hand tracking, draw with returning positions of fingertips and palm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            if (!skeletonReadyFlag)
                return;
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                    CoordinateMapperPlus mapper = new CoordinateMapperPlus(this.sensor);
                    PointDepth3D palmDepth3D = mapper.MapSkeletonPointToDepthPoint(rightHand, this.sensor.DepthStream.Format);

                    int index = palmDepth3D.Y * DepthFrameWidth + palmDepth3D.X;
                    if (index < 0 || index > this.depthPixels.Length)
                        return;
                    short currDepth = depthPixels[index].Depth;

                    int indexColor = 0;
                    int rectSize = 0;
                    for (int i = 0; i < this.DepthFrameWidth; i++)
                    {
                        for (int j = 0; j < this.DepthFrameHeight; j++)
                        {
                            indexColor = (j * this.DepthFrameWidth + i) * 4;
                            if (KinectUtil.isInTrackRegion(i, j, palmDepth3D.X, palmDepth3D.Y))
                            {
                                int indexDepthPixels = j * this.DepthFrameWidth + i;
                                rectDepth3D[rectSize++] = new PointDepth3D(i, j, depthPixels[indexDepthPixels].Depth);

                                //Draw depth pixel according to different distance
                                this.depthColor[indexColor + BLUE] = 255;
                                this.depthColor[indexColor + GREEN] = 0;
                                this.depthColor[indexColor + RED] = 0;
                                this.depthColor[indexColor + AlPHA] = 0;

                                if (depthPixels[indexDepthPixels].Depth >= currDepth - 100
                                    && depthPixels[indexDepthPixels].Depth <= currDepth + 100)
                                {
                                    this.depthColor[indexColor + BLUE] = 255;
                                    this.depthColor[indexColor + GREEN] = 255;
                                    this.depthColor[indexColor + RED] = 255;
                                    this.depthColor[indexColor + AlPHA] = 0;
                                }
                            }
                            else
                            {
                                this.depthColor[indexColor + BLUE] = 0;
                                this.depthColor[indexColor + GREEN] = 0;
                                this.depthColor[indexColor + RED] = 0;
                                this.depthColor[indexColor + AlPHA] = 0;
                            }
                        }
                    }
                    #region
                    //for (int i = 0,depthColorIndex = 0; i < this.depthPixels.Length; i++, depthColorIndex += 4)
                    //{
                    //    if (pointRight.X >= (int)(i % this.sensor.DepthStream.FrameWidth) - 50 &&
                    //        pointRight.X <= (int)(i % this.sensor.DepthStream.FrameWidth) + 50 &&
                    //        pointRight.Y >= (int)(i / this.sensor.DepthStream.FrameWidth) - 40 &&
                    //        pointRight.Y <= (int)(i / this.sensor.DepthStream.FrameWidth) + 60)
                    //    {
                    //        this.depthColor[depthColorIndex + BLUE] = 255;
                    //        this.depthColor[depthColorIndex + GREEN] = 0;
                    //        this.depthColor[depthColorIndex + RED] = 0;
                    //        this.depthColor[depthColorIndex + AlPHA] = 0;
                    //        if (depthPixels[i].Depth >= nearDepth - 100 && depthPixels[i].Depth <= nearDepth + 100)
                    //        {
                    //            this.depthColor[depthColorIndex + BLUE] = 255;
                    //            this.depthColor[depthColorIndex + GREEN] = 255;
                    //            this.depthColor[depthColorIndex + RED] = 255;
                    //            this.depthColor[depthColorIndex + AlPHA] = 0;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        this.depthColor[depthColorIndex + BLUE] = 0;
                    //        this.depthColor[depthColorIndex + GREEN] = 0;
                    //        this.depthColor[depthColorIndex + RED] = 0;
                    //        this.depthColor[depthColorIndex + AlPHA] = 0;

                    //    }
                    //}
                    #endregion

                    //Construct Hand instance for tracker, then do hand tracking
                    if (rectSize == rectWidth * rectHeight)
                    {
                        detector.RightHand = new Hand(rectDepth3D, palmDepth3D, rectWidth, rectHeight);
                        detector.DetectAndSmooth(timeStamp);
                        
                        //Fingers.Identify(this.Sketelon3DFingerTips,timeStamp);
                        //Fingers.Identify2(this.Sketelon3DFingerTips,rightHand, rightWrist);

                        Fingers.Identify3(this.Sketelon3DFingerTips,timeStamp);
                        timeStamp = (++timeStamp) % KinectUtil.LOOP_TIMES;
                        foreach (var element in detector.Depth3DFingerTips)
                        {
                            int k = (element.Y * DepthFrameWidth + element.X) * 4;
                            for (int i = k - 20; i < k + 20; i += 4)
                            {
                                depthColor[i + BLUE] = 0;
                                depthColor[i + GREEN] = 0;
                                depthColor[i + RED] = 255;
                            }
                        }
                    }
                    this.depthBitmap.WritePixels(
                         new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                         this.depthColor,
                         this.depthBitmap.PixelWidth * sizeof(int),
                         0);

                    readyEvent.afterDepthReady();
                }
            }
        }

        /// <summary>
        /// Color Frame Ready
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    colorFrame.CopyPixelDataTo(this.colorPixels);
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }


        /// <summary>
        /// Enable all the streams, and start the Kinect sensor
        /// </summary>
        public void Start()
        {
            if (null != this.sensor)
            {
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                this.sensor.SkeletonStream.Enable();
                try{
                    this.sensor.Start();
                }
                catch (Exception) { }
            }
        }
        
        /// <summary>
        /// Disable all streams, and stop the Kinect sensor
        /// </summary>
        public void Stop()
        {
            if (null != this.sensor)
            {
                this.sensor.ColorStream.Disable();
                this.sensor.DepthStream.Disable();
                this.sensor.SkeletonStream.Disable();
                this.sensor.Stop();
            }
        }


        #region public property for WPF UI
        public WriteableBitmap ColorBitmap
        {
            get {
                return colorBitmap;
            }
        }
        public WriteableBitmap DepthBitmap
        {
            get {
                return depthBitmap;
            }
        }

        public List<PointSkeleton3D> Sketelon3DFingerTips
        {
            get {
                return this.detector.Skeleton3DFingerTips;
            }
        }
        public List<PointDepth3D> Depth3DFingerTips
        {
            get{
                return detector.Depth3DFingerTips;
            }
        }


        public PointDepth3D Palm
        {
            get {
                CoordinateMapperPlus mapper = new CoordinateMapperPlus(this.sensor);
                return mapper.MapSkeletonPointToDepthPoint(rightHand, this.sensor.DepthStream.Format);
            }
        }

        public PointDepth3D Wrist
        {
            get {
                CoordinateMapperPlus mapper = new CoordinateMapperPlus(this.sensor);
                return mapper.MapSkeletonPointToDepthPoint(rightWrist, this.sensor.DepthStream.Format);
            }
        }

        #endregion
    }
}
