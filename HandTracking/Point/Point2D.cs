using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandTracking
{
    public class Point2D
    {
        public int X { get; set;}
        public int Y { get; set;}

        #region Constructor
        /// <summary>
        /// Two different constructors to construct Point2D Object
        /// </summary>
        /// <param name="_x">default 0 value for Coordinate X</param>
        /// <param name="_y">default 0 value for Coordinate Y</param>
        public Point2D(int _x = 0, int _y = 0)
        {
            X = _x;
            Y = _y;
        }

        public Point2D(Point2D point)
        {
            this.X = point.X;
            this.Y = point.Y;
        }

        #endregion 


        #region Operator Override
       
        /// <summary>
        /// Add two point
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Point2D operator +(Point2D point1, Point2D point2)
        {
            Point2D ret = new Point2D();
            ret.X = point1.X + point2.X;
            ret.Y = point1.Y + point2.Y;
            return ret;
        }
       
        /// <summary>
        /// minus two point
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Point2D operator -(Point2D point1, Point2D point2)
        {
            Point2D ret = new Point2D();
            ret.X = point1.X - point2.X;
            ret.Y = point1.Y - point2.Y;
            return ret;
        }
        
        /// <summary>
        /// A point multipy with a double value
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Point2D operator *(Point2D point1, double value)
        {
            Point2D ret = new Point2D();
            ret.X = (int)(point1.X * value);
            ret.Y = (int)(point1.Y * value);
            return ret;
        }

        /// <summary>
        /// == Operator
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static bool operator ==(Point2D point1, Point2D point2)
        {
            if (point1.X == point2.X && point1.Y == point2.Y)
                return true;
            return false;
        }
        
        /// <summary>
        /// != Operator
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static bool operator !=(Point2D point1, Point2D point2)
        {
            return !(point1 == point2);
        }

        #endregion 

        /// <summary>
        /// Get the Euclidean Distance between two point in 2D space
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float EuclideanDistance(Point2D point1,Point2D point2)
        {
            return (float)Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) 
                                        + (point1.Y - point2.Y) * (point1.Y - point2.Y));
        }

        /// <summary>
        /// Get the vector length from point1 to point(0,0)
        /// </summary>
        /// <param name="point1"></param>
        /// <returns></returns>
        public static float Length(Point2D point1)
        {
            return (float)(Math.Sqrt(point1.X * point1.X + point1.Y * point1.Y));
        }

        /// <summary>
        /// get angle between two vectors
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns>radian discription for the return angle</returns>
        public static float Angle(Point2D point1, Point2D point2)
        {
            return (float)Math.Acos((point1.X * point2.X + point1.Y * point2.Y) / (Length(point1) * Length(point2)));
        }


        /** Operator override,
         * so, need to override Equle and GetHansCode for Point2D class**/

        public override bool Equals(object obj)
        {
            Point2D other = (Point2D)obj;
            if (this.X == other.X && this.Y == other.Y)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
    }
}
