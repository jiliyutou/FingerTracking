using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandTracking
{
    public class KinectUtil
    {

        public const int LOOP_TIMES = 3000; 

        const int LowOffset =50;
        const int UpOffset = 70;
        const int LeftOffset = 60;
        const int RightOffset = 60;

        public static bool isInTrackRegion(int i, int j, int X, int Y)
        {
            if (i >= X - LeftOffset && i < X + RightOffset
                && j >= Y - UpOffset && j < Y + LowOffset)
                return true;
            return false;
        }


    }
}
