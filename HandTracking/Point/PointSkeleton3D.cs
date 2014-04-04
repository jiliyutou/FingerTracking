using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandTracking
{
    public class PointSkeleton3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int TimeStamp { get; set; }

        #region Constructor
        public PointSkeleton3D(float _x = 0, float _y = 0, float _z = 0,int timeStamp = 0)
        {
            this.X = _x;
            this.Y = _y;
            this.Z = _z;
            this.TimeStamp = timeStamp;
        }

        public PointSkeleton3D(PointSkeleton3D point)
        {
            this.X = point.X;
            this.Y = point.Y;
            this.Z = point.Z;
            this.TimeStamp = point.TimeStamp;
        }

        public PointSkeleton3D(PointSkeleton3D point,int timeStamp)
        {
            this.X = point.X;
            this.Y = point.Y;
            this.Z = point.Z;
            this.TimeStamp = timeStamp;
        }

        public PointSkeleton3D(Microsoft.Kinect.SkeletonPoint skeltonPoint)
        {
            this.X = skeltonPoint.X;
            this.Y = skeltonPoint.Y;
            this.Z = skeltonPoint.Z;
        }

        #endregion Constructor

        public static PointSkeleton3D operator -(PointSkeleton3D point1, PointSkeleton3D point2)
        {
            PointSkeleton3D ret = new PointSkeleton3D();
            ret.X = point1.X - point2.X;
            ret.Y = point1.Y - point2.Y;
            ret.Z = point1.Z - point2.Z;
            return ret;
        }

        public static bool operator ==(PointSkeleton3D point1, PointSkeleton3D point2)
        {
            if (point1.X == point2.X && point1.Y == point2.Y
              && point1.Z == point2.Z && point1.TimeStamp == point2.TimeStamp)
                return true;
            return false;
        }
        public static bool operator !=(PointSkeleton3D point1, PointSkeleton3D point2)
        {
            return !(point1 == point2);
        }

        public static float Length(PointSkeleton3D point1)
        {
            return (float)(Math.Sqrt(point1.X * point1.X + point1.Y * point1.Y + point1.Z * point1.Z));
        }

        public static float Angle(PointSkeleton3D point1, PointSkeleton3D point2)
        {
            return (float)Math.Acos((point1.X * point2.X + point1.Y * point2.Y + point1.Z * point2.Z) / (Length(point1) * Length(point2)));
        }

        /// <summary>
        /// Get the Euclidean Distance between two point in Skeleton Space
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float EuclideanDistance(PointSkeleton3D point1, PointSkeleton3D point2)
        {
            return (float)Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X)
                                        + (point1.Y - point2.Y) * (point1.Y - point2.Y)
                                        + (point1.Z - point2.Z) * (point1.Z - point2.Z));
        }

        /// <summary>
        /// Need to override if you override operator == and !=
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            PointSkeleton3D other = (PointSkeleton3D)obj;
            if (this.X == other.X && this.Y == other.Y
                && this.Z == other.Z && this.TimeStamp == other.TimeStamp)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
