using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HandTracking
{
    public class PointDepth3D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public short Depth { get; set; }

        #region Constructor
        public PointDepth3D(int _x = 0, int _y = 0, short depth = 0)
        {
            this.X = _x;
            this.Y = _y;
            this.Depth = depth;
        }

        public PointDepth3D(Point2D point, short depth)
        {
            this.X = point.X;
            this.Y = point.Y;
            this.Depth = depth;
        }

        public PointDepth3D(PointDepth3D point)
        {
            this.X = point.X;
            this.Y = point.Y;
            this.Depth = point.Depth;
        }

        public PointDepth3D(Microsoft.Kinect.DepthImagePoint depthImagePoint)
        {
            this.X = depthImagePoint.X;
            this.Y = depthImagePoint.Y;
            this.Depth = (short)depthImagePoint.Depth;
        }
        #endregion Constructor
    }
}
