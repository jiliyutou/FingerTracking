using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace HandTracking
{
    /// <summary>
    /// Added 2013-3-9
    /// </summary>
    public class FingerIdentification
    {
        private KinectSensor sensor;

        private Dictionary<FingerType, Finger> Fingers;
        public FingerTrackingState TrackingState { get; set; }

        private const float DIS_THRESHOLD = 0.03f;
        private const int FRAME_INTERVAL = 5;

        public FingerIdentification(KinectSensor sensor)
        {
            if(null != sensor)
                this.sensor = sensor;
            TrackingState = FingerTrackingState.NotTracked;

            Fingers = new Dictionary<FingerType, Finger>();
            Fingers.Add(FingerType.ThumbRight, new Finger());
            Fingers.Add(FingerType.IndexRight, new Finger());
            Fingers.Add(FingerType.MiddleRight, new Finger());
            Fingers.Add(FingerType.RingRight, new Finger());
            Fingers.Add(FingerType.LittleRight, new Finger());
        }

        /// <summary>
        /// Implement the IComparer interface for List<T>.Sort()
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private int CompareByCoord_X(PointSkeleton3D p1, PointSkeleton3D p2)
        {
            if (p1.X == p2.X)
                return 0;
            return (p1.X < p2.X) ? 1 : -1;
        }

        /// <summary>
        /// Finger Identify, sort by x_coordniate first,so many assumptions
        /// </summary>
        /// <param name="fingertips"></param>
        public void Identify(List<PointSkeleton3D> fingertips, int timeStamp)
        {
            if (null == fingertips)
                return;
            if (fingertips.Count == 5)   //目前只能处理掌心比较标准的情况 
            {
                TrackingState = FingerTrackingState.Tracked;
                fingertips.Sort(CompareByCoord_X);

                Fingers[FingerType.MiddleRight].Position = fingertips[2];
                Fingers[FingerType.MiddleRight].TrackingState = FingerTrackingState.Tracked;

                float dis1 = PointSkeleton3D.EuclideanDistance(fingertips[2], fingertips[0]);
                float dis2 = PointSkeleton3D.EuclideanDistance(fingertips[2], fingertips[4]);
                Fingers[FingerType.ThumbRight].Position = (dis1 > dis2 ? fingertips[0] : fingertips[4]);
                Fingers[FingerType.LittleRight].Position = (dis1 > dis2 ? fingertips[4] : fingertips[0]);
                Fingers[FingerType.ThumbRight].TrackingState = FingerTrackingState.Tracked;
                Fingers[FingerType.LittleRight].TrackingState = FingerTrackingState.Tracked;
            }
            FingerTrack(fingertips, timeStamp);

        }

        //timeStamp没啥用？？
        private void FingerTrack(List<PointSkeleton3D> fingertips,int timeStamp)
        {
            foreach (var i in Fingers)
            {
                if (i.Value.TrackingState == FingerTrackingState.Tracked)
                {
                    PointSkeleton3D res = new PointSkeleton3D();
                    float min = float.MaxValue;
                    foreach (var j in fingertips)
                    {
                        float temp = PointSkeleton3D.EuclideanDistance(i.Value.Position, j);
                        if (temp < DIS_THRESHOLD && min > temp)
                        {
                            min = temp;
                            res = j;
                        }
                    }
                    if (min != float.MaxValue)
                    {
                        i.Value.Position.X = res.X;
                        i.Value.Position.Y = res.Y;
                        i.Value.Position.Z = res.Z;
                        i.Value.Position.TimeStamp = res.TimeStamp;
                    }
                    else if ((timeStamp > i.Value.Position.TimeStamp && timeStamp - i.Value.Position.TimeStamp >= FRAME_INTERVAL)
                                || (timeStamp < i.Value.Position.TimeStamp && (timeStamp + KinectUtil.LOOP_TIMES) - i.Value.Position.TimeStamp >= FRAME_INTERVAL))
                    {
                        i.Value.TrackingState = FingerTrackingState.NotTracked;
                    }
                }
            }
        }

        /// <summary>
        /// Finger identification with angle between palm-wrist line and every fingertip 
        /// </summary>
        /// <param name="fingertips"></param>
        /// <param name="palm"></param>
        /// <param name="wrist"></param>
        public void Identify2(List<PointSkeleton3D> fingertips, PointSkeleton3D palm, PointSkeleton3D wrist)
        {
            if (null == fingertips)
                return;
            if (fingertips.Count == 5)
            {
                PointSkeleton3D v1 = palm - wrist;
                float min = float.MaxValue;
                PointSkeleton3D res = new PointSkeleton3D();
                foreach (var i in fingertips)
                {
                    PointSkeleton3D v2 = i - palm;
                    float angle = PointSkeleton3D.Angle(v1, v2);
                    if (angle < min)
                    {
                        min = angle;
                        res = i;
                    }
                }
                Fingers[FingerType.MiddleRight].Position = res;
                Fingers[FingerType.MiddleRight].TrackingState = FingerTrackingState.Tracked;
            }
            else
            {
                Fingers[FingerType.MiddleRight].TrackingState = FingerTrackingState.NotTracked;
            }
        }

        /// <summary>
        /// Finger Identification with two fingertips' max Distance 
        /// </summary>
        /// <param name="fingertips"></param>
        public void Identify3(List<PointSkeleton3D> fingertips, int timeStamp)
        {
            if (null == fingertips)
                return;
            if(fingertips.Count == 5)
            {
                //1.Find two indexs for ThumbFinger and LittleFinger
                int thumbIndex = 0, littleIndex = 0 ;
                float max = float.MinValue;

                for (int i = 0; i < fingertips.Count; i++)
                {
                    for (int j = i + 1; j < fingertips.Count; j++)
                    {
                        float dis = PointSkeleton3D.EuclideanDistance(fingertips[i], fingertips[j]);
                        if (dis > max)
                        {
                            max = dis;
                            thumbIndex = i;
                            littleIndex = j;
                        }
                    }
                }
                //2.S and T Set construction, compute min distance between two Sets
                PointSkeleton3D[] S = new PointSkeleton3D[2];
                S[0] = new PointSkeleton3D(fingertips[littleIndex]);
                S[1] = new PointSkeleton3D(fingertips[thumbIndex]);
                PointSkeleton3D[] T = new PointSkeleton3D[3];
                int k = 0;
                for (int i = 0; i < fingertips.Count; i++)
                {
                    if (fingertips[i] != S[0] && fingertips[i] != S[1])
                        T[k++] = new PointSkeleton3D(fingertips[i]);
                }

                int[] neighbour = new int[S.Length];
                float[] minDis = new float[S.Length];
                for (int i = 0; i < S.Length; i++)
                {
                    float min = float.MaxValue;
                    int temp = 0;
                    for (int j = 0; j < T.Length; j++)
                    {
                        float dis = PointSkeleton3D.EuclideanDistance(S[i], T[j]);
                        if (dis < min)
                        {
                            min = dis;
                            temp = j;
                        }
                    }
                    if (min != float.MaxValue)
                    {
                        neighbour[i] = temp;
                        minDis[i] = min;
                    }
                }
                if (minDis[0] == Math.Max(minDis[0], minDis[1]))
                {
                    //Fingers[FingerType.ThumbRight].Position = S[0];
                    //Fingers[FingerType.ThumbRight].TrackingState = FingerTrackingState.Tracked;
                    //Fingers[FingerType.IndexRight].Position = T[neighbour[0]];
                    //Fingers[FingerType.IndexRight].TrackingState = FingerTrackingState.Tracked;

                    //Fingers[FingerType.LittleRight].Position = S[1];
                    //Fingers[FingerType.LittleRight].TrackingState = FingerTrackingState.Tracked;
                    //Fingers[FingerType.RingRight].Position = T[neighbour[1]];
                    //Fingers[FingerType.RingRight].TrackingState = FingerTrackingState.Tracked;

                    Tracked(FingerType.ThumbRight, S[0]);
                    Tracked(FingerType.IndexRight, T[neighbour[0]]);
                    Tracked(FingerType.LittleRight, S[1]);
                    Tracked(FingerType.RingRight, T[neighbour[1]]);
                }
                else
                {
                    //Fingers[FingerType.ThumbRight].Position = S[1];
                    //Fingers[FingerType.ThumbRight].TrackingState = FingerTrackingState.Tracked;
                    //Fingers[FingerType.IndexRight].Position = T[neighbour[1]];
                    //Fingers[FingerType.IndexRight].TrackingState = FingerTrackingState.Tracked;

                    //Fingers[FingerType.LittleRight].Position = S[0];
                    //Fingers[FingerType.LittleRight].TrackingState = FingerTrackingState.Tracked;
                    //Fingers[FingerType.RingRight].Position = T[neighbour[0]];
                    //Fingers[FingerType.RingRight].TrackingState = FingerTrackingState.Tracked;
                    
                    Tracked(FingerType.LittleRight, S[0]);
                    Tracked(FingerType.RingRight, T[neighbour[0]]);
                    Tracked(FingerType.ThumbRight, S[1]);
                    Tracked(FingerType.IndexRight, T[neighbour[1]]);
                }

                //3.Middle Finger Remaineds
                int remain = 0;
                for (int i = 0; i < T.Length; i++)
                {
                    if (i != neighbour[0] && i != neighbour[1])
                        remain = i; 
                }
                //Fingers[FingerType.MiddleRight].Position = T[remain];
                //Fingers[FingerType.MiddleRight].TrackingState = FingerTrackingState.Tracked;
                Tracked(FingerType.MiddleRight, T[remain]);
            }
            FingerTrack(fingertips, timeStamp);
        }

        private void Tracked(FingerType id, PointSkeleton3D position)
        {
            //if (null != position)
            Fingers[id].Position = new PointSkeleton3D(position);
            Fingers[id].TrackingState = FingerTrackingState.Tracked;

        }
        private void NotTracked(FingerType id)
        {
            Fingers[id].Position = new PointSkeleton3D();
            Fingers[id].TrackingState = FingerTrackingState.NotTracked;
        }

        public PointSkeleton3D GetSkeleton3DPosition(FingerType id)
        {
            if(this[id].TrackingState == FingerTrackingState.Tracked)
                return this[id].Position;
            return new PointSkeleton3D();
        }
        public PointDepth3D GetDepth3DPosition(FingerType id)
        {
            if (this[id].TrackingState == FingerTrackingState.Tracked)
            {
                CoordinateMapperPlus mapper = new CoordinateMapperPlus(this.sensor);
                return mapper.MapSkeletonPointToDepthPoint(this[id].Position, this.sensor.DepthStream.Format);
            }
            return new PointDepth3D();
        }
        public Point2D GetPoint2Position(FingerType id)
        {
            if (this[id].TrackingState == FingerTrackingState.Tracked)
            {
                CoordinateMapperPlus mapper = new CoordinateMapperPlus(this.sensor);
                return mapper.MapSkeletonPointToColorPoint(this[id].Position, this.sensor.ColorStream.Format);
            }
            return new Point2D();
        }

        public int FingerTrackedCount
        {
            get {
                int count = 0;
                foreach (var i in Fingers)
                {
                    if (i.Value.TrackingState == FingerTrackingState.Tracked)
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// Override operator []
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Finger this[FingerType id]
        {
            get
            {
                return Fingers[id];
            }
        }
        
    }
}
