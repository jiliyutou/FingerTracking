using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace HandTracking
{
    public enum HandType
    {
        TypeLeftHand,
        TypeRightHand
    }
    public class FingerDetection
    {
        private KinectSensor sensor;

        public Hand LeftHand { get; set; }
        public Hand RightHand { get; set; }

        private FingerTipsSmooth smoother;


        public FingerDetection(KinectSensor sensor)
        {
            if (null != sensor)
                this.sensor = sensor;

            smoother = new FingerTipsSmooth();
        }

        /// <summary>
        /// Call Core Track Algo and Smooth Algo to Track and Smooth FingerTips
        /// </summary>
        /// <param name="timeStamp"></param>
        public void DetectAndSmooth(int timeStamp)
        {
            if(null == sensor) 
                return;
            RightHand.FingerDetect(0);
            CoordinateMapperPlus mapper = new CoordinateMapperPlus (this.sensor);
            List<PointSkeleton3D> currFingerTips = mapper.MapDepthPointsToSketelonPoints(sensor.DepthStream.Format, RightHand.FingerTips);
            smoother.Smooth(currFingerTips,timeStamp);
        }

        #region Property

        /// <summary>
        /// FingerTips with PointSkeleton3D representation After Smoothing
        /// </summary>
        public List<PointSkeleton3D> Skeleton3DFingerTips
        {
            get {
                return smoother.SmoothFingerTips;
            }
        }

        /// <summary>
        /// FingerTips With PointDepth3D representation After Smoothing
        /// </summary>
        public List<PointDepth3D> Depth3DFingerTips
        {
            get {
                CoordinateMapperPlus mapper = new CoordinateMapperPlus(this.sensor);
                return mapper.MapSkeletonPointsToDepthPoints(Skeleton3DFingerTips, this.sensor.DepthStream.Format);
            }
        }

       
        #endregion
    }
}
