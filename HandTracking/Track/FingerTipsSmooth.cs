using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandTracking
{
    /// <summary>
    /// Smooth FingerTips Class
    /// 2013-3-5 Added 
    /// </summary>
    public class FingerTipsSmooth
    {
        const float DIS_THRESHOLD = 0.015f;
        const int FRAME_INTERVAL = 5;
        const float ALPHA = 0.15f;
        const float FILTER_THRESHOLD = 0.06f;

        private List<PointSkeleton3D> fingerTips;

        public FingerTipsSmooth()
        {
            fingerTips = new List<PointSkeleton3D>();
        }

        /// <summary>
        /// Smooth FingerTips with current computed result and time stamp
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="timeStamp"></param>
        public void Smooth(List<PointSkeleton3D> curr,int timeStamp)
        {
            if (curr == null || curr.Count > 6)
            {
                return;
            }
            NoiseFilter(curr);
            if (fingerTips.Count >= curr.Count)
            {
                List<PointSkeleton3D> candinateDelete = new List<PointSkeleton3D>();
                foreach (var i in fingerTips)
                {
                    PointSkeleton3D res = new PointSkeleton3D();
                    float min = float.MaxValue;

                    //Find nearest cooresponding point in two Lists
                    foreach (var j in curr)
                    {
                        float temp = PointSkeleton3D.EuclideanDistance(i, j);
                        if (temp < DIS_THRESHOLD && min > temp)
                        {
                            min = temp;
                            res = new PointSkeleton3D(j);
                        }
                    }
                    //Update result with weighted method
                    if (min != float.MaxValue)  
                    {
                        if (min < 0.008f)
                        {
                            i.TimeStamp = timeStamp;
                        }
                        else
                        {
                            i.X = i.X * ALPHA + (1 - ALPHA) * res.X;
                            i.Y = i.Y * ALPHA + (1 - ALPHA) * res.Y;
                            i.Z = i.Z * ALPHA + (1 - ALPHA) * res.Z;
                            i.TimeStamp = timeStamp;
                        }
                    }
                    //the point lost, Check the timestamp difference
                    else if ((timeStamp > i.TimeStamp && timeStamp - i.TimeStamp >= FRAME_INTERVAL)
                            || (timeStamp < i.TimeStamp && (timeStamp + KinectUtil.LOOP_TIMES) - i.TimeStamp >= FRAME_INTERVAL))
                    {
                        candinateDelete.Add(i);
                    }
                }
                foreach (var k in candinateDelete)
                {
                    fingerTips.Remove(k);
                }
            }
            else 
            {
                fingerTips.Clear();
                foreach (var i in curr)
                {
                    PointSkeleton3D point = new PointSkeleton3D(i,timeStamp);
                    fingerTips.Add(point);
                }
            }
        }

        /// <summary>
        /// Sort Fingertips list with depth increasing
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private int CompareByDepth(PointSkeleton3D p1, PointSkeleton3D p2)
        {
            if (p1.Z == p2.Z)
                return 0;
            return (p1.Z > p2.Z) ? 1 : -1;
        }

        /// <summary>
        /// Noise Point Filtering (near wrist point when palm rotates)
        /// Succeed only finger count bigger than 2, otherwize fail
        /// </summary>
        /// <param name="currFingerTips">FingerTips list from current frame</param>
        private void NoiseFilter(List<PointSkeleton3D> currFingerTips)
        {
            if (currFingerTips == null || currFingerTips.Count > 6)
                return;
            currFingerTips.Sort(CompareByDepth);
            if (currFingerTips.Count >= 2)
            {
                int index = currFingerTips.Count - 1;
                float different = currFingerTips[index].Z - currFingerTips[index - 1].Z;
                if (different > FILTER_THRESHOLD)
                {
                    currFingerTips.Remove(currFingerTips[index]);
                }
            }
        }

        #region Property
        /// <summary>
        /// FingerTips after smoothing and filtering, no finger indentification involved
        /// </summary>
        public List<PointSkeleton3D> SmoothFingerTips
        {
            get {
                return fingerTips;
            }
        }
        #endregion 

    }
}
