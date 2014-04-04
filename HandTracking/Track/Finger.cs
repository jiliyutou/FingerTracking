using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandTracking
{
    public enum FingerTrackingState
    {
        Tracked,
        NotTracked
    }

    public class Finger
    {
        public PointSkeleton3D Position { get; set; }
        public FingerTrackingState TrackingState { get; set; }

        public Finger()
        {
            Position = new PointSkeleton3D();
            TrackingState = FingerTrackingState.NotTracked;
        }

    }
}
