using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace HandTracking
{
    /// <summary>
    /// Package Microsoft.Kinect.CoordniateMapper
    /// Map user-define Point Class; PointDepth3D to PointSkeleton3D, and reverse
    /// 
    /// 2013-3-5 Add MapDepthPointsToSketelonPoints and MapSkeletonPointsToDepthPoints two functions
    /// </summary>
    public class CoordinateMapperPlus
    {
        private CoordinateMapper mapper;

        public CoordinateMapperPlus(KinectSensor sensor)
        {
            if(null != sensor)
                mapper = new CoordinateMapper(sensor);
        }

        /// <summary>
        /// Map PointSkeleton3D to Point2D
        /// </summary>
        /// <param name="pointSkleton3D"></param>
        /// <param name="colorImageFormat"></param>
        /// <returns></returns>
        public Point2D MapSkeletonPointToColorPoint(PointSkeleton3D pointSkleton3D, ColorImageFormat colorImageFormat)
        {
            SkeletonPoint point = new SkeletonPoint();
            point.X = pointSkleton3D.X;
            point.Y = pointSkleton3D.Y;
            point.Z = pointSkleton3D.Z;
            ColorImagePoint ImgPoint = mapper.MapSkeletonPointToColorPoint(point, colorImageFormat);
            return new Point2D(ImgPoint.X, ImgPoint.Y);
        }

        /// <summary>
        /// Map PointSkeleton3D to PointDepth3D
        /// </summary>
        /// <param name="pointSkleton3D"></param>
        /// <param name="depthImageFormat"></param>
        /// <returns></returns>
        public PointDepth3D MapSkeletonPointToDepthPoint(PointSkeleton3D pointSkleton3D,DepthImageFormat depthImageFormat)
        {
            SkeletonPoint point = new SkeletonPoint();
            point.X = pointSkleton3D.X;
            point.Y = pointSkleton3D.Y;
            point.Z = pointSkleton3D.Z;

            return new PointDepth3D(mapper.MapSkeletonPointToDepthPoint(point,depthImageFormat));
        }

        /// <summary>
        /// Map PointDepth3D to PointSkeleton3D
        /// </summary>
        /// <param name="depthImageFormat"></param>
        /// <param name="pointDepth3D"></param>
        /// <returns></returns>
        public PointSkeleton3D MapDepthPointToSketelonPoint(DepthImageFormat depthImageFormat, PointDepth3D pointDepth3D)
        {
            DepthImagePoint point = new DepthImagePoint();
            point.X = pointDepth3D.X;
            point.Y = pointDepth3D.Y;
            point.Depth = pointDepth3D.Depth;

            return new PointSkeleton3D(mapper.MapDepthPointToSkeletonPoint(depthImageFormat, point));
        }


        /// <summary>
        /// Map PointSkeleton3D List to Point2 List
        /// </summary>
        /// <param name="pointSkeleton3D"></param>
        /// <param name="colorImageFormat"></param>
        /// <returns></returns>
        public List<Point2D> MapSkeletonPointsToColorPoints(List<PointSkeleton3D> pointSkeleton3D, ColorImageFormat colorImageFormat)
        {
            List<Point2D> ret = new List<Point2D>();
            foreach (var element in pointSkeleton3D)
            {
                ret.Add(MapSkeletonPointToColorPoint(element, colorImageFormat));
            }
            return ret;
        }

        /// <summary>
        /// Map PointSkeleton3D List to PointDepth3D List
        /// </summary>
        /// <param name="pointSkleton3D"></param>
        /// <param name="depthImageFormat"></param>
        /// <returns></returns>
        public List<PointDepth3D> MapSkeletonPointsToDepthPoints(List<PointSkeleton3D> pointSkeleton3D, DepthImageFormat depthImageFormat)
        {
            List<PointDepth3D> ret = new List<PointDepth3D>();
            foreach (var element in pointSkeleton3D)
            {
                ret.Add(MapSkeletonPointToDepthPoint(element,depthImageFormat));
            }
            return ret;
        }

        /// <summary>
        /// Map PointDepth3D List to PointSkeleton3D List
        /// </summary>
        /// <param name="depthImageFormat"></param>
        /// <param name="pointDepth3D"></param>
        /// <returns></returns>
        public List<PointSkeleton3D> MapDepthPointsToSketelonPoints(DepthImageFormat depthImageFormat, List<PointDepth3D> pointDepth3D)
        {
            List<PointSkeleton3D> ret = new List<PointSkeleton3D>();
            foreach (var element in pointDepth3D)
            {
                ret.Add(MapDepthPointToSketelonPoint(depthImageFormat, element));
            }
            return ret;
        }

    }
}
