using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandTracking
{
    public class Hand
    {
        #region Alloc By Constructor
        private PointDepth3D[] pixelDepth3D;
        private PointDepth3D kinectTrackPalm;
        private short kinectPalmDetph;
        private int rectWidth;
        private int rectHeight;
        #endregion

        private bool[][] validMatrix;
        private bool[][] contourMatrix;
        private List<Point2D> contourPixels;
        private List<Point2D> interiorPixels;

        private Point2D palm = new Point2D();
        private List<Point2D> contour = new List<Point2D>();
        private List<Point2D> fingerTips = new List<Point2D>();

        public Hand(PointDepth3D[] pixel3D, PointDepth3D trackPalm, int RWidth = 100, int RHeight = 100)
        {
            this.pixelDepth3D = pixel3D;
            this.kinectTrackPalm = trackPalm;
            this.kinectPalmDetph = trackPalm.Depth;
            this.rectWidth = RWidth;
            this.rectHeight = RHeight;

            contourPixels = new List<Point2D>();
            interiorPixels = new List<Point2D>();
        }

        public void FingerDetect(int dalitionErosionMask = 1)
        {
            validMatrix = generateValidMatrix(this.pixelDepth3D, this.kinectPalmDetph, dalitionErosionMask); 

            /** Get interiorPixels and contourPixels **/
            contourMatrix = new bool[validMatrix.Length][];
            for (int i = 0; i < validMatrix.Length; i++)
            {
                contourMatrix[i] = new bool[validMatrix[i].Length];
            }
            for (int i = 0; i < validMatrix.Length; i++)
            {
                for (int j = 0; j < validMatrix[i].Length; j++)
                {
                    if (validMatrix[i][j])
                    {
                        if (4 == adjacentValidPixelCount(validMatrix, i, j))
                        {
                            interiorPixels.Add(new Point2D(i, j));
                        }
                        else
                        {
                            contourMatrix[i][j] = true;
                            contourPixels.Add(new Point2D(i, j));
                        }
                    }
                }
            }
            if (contourPixels.Count != 0)
            {
                contour = TurtleSearch(validMatrix, contourPixels.First());
            }
            if (null != contour)
            {
                palm = findPalmCenter(interiorPixels, contour);
                fingerTips = findFingerTips(contour, palm);
            }
        }

        /// <summary>
        /// Depth Slice, generate the valid matrix
        /// </summary>
        /// <param name="pixel3D"></param>
        /// <param name="Depth"></param>
        /// <returns>bool[][] valid</returns>
        private bool[][] generateValidMatrix(PointDepth3D[] pixel3D, float Depth, int dalitionErosionMask)
        {
            bool[][] valid = new bool[rectWidth][];
            for (int i = 0; i < valid.Length; i++)
            {
                valid[i] = new bool[rectHeight];
            }
            for (int i = 0; i < valid.Length; i++)
            {
                for (int j = 0; j < valid[i].Length; j++)
                {
                    int index = j * rectWidth + i;
                    if (pixel3D[index].Depth <= Depth + 100 && pixel3D[index].Depth >= Depth - 100)
                    {
                        valid[i][j] = true;
                    }
                    else
                    {
                        valid[i][j] = false;
                    }
                }
            }
            //Noisy Reduction
            if (dalitionErosionMask > 0)
            {
                valid = Denoise.Dilation(valid, dalitionErosionMask);
                valid = Denoise.Erosion(valid, dalitionErosionMask);
            }
            // Mark as not valid the borders of the matrix to improve the efficiency in some methods
            int m;
            // First row
            for (int j = 0; j < valid[0].Length; ++j)
                valid[0][j] = false;
            // Last row
            m = valid.Length - 1;
            for (int j = 0; j < valid[0].Length; ++j)
                valid[m][j] = false;
            // First column
            for (int i = 0; i < valid.Length; ++i)
                valid[i][0] = false;
            // Last column
            m = valid[0].Length - 1;
            for (int i = 0; i < valid.Length; ++i)
                valid[i][m] = false;

            return valid;
        }

        /// <summary>
        /// count number of the valid pixels for pointed(i,j)
        /// </summary>
        /// <param name="valid"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private int adjacentValidPixelCount(bool[][] valid, int i, int j)
        {
            int count = 0;
            if (valid[i - 1][j]) count++;
            if (valid[i + 1][j]) count++;
            if (valid[i][j - 1]) count++;
            if (valid[i][j + 1]) count++;
            return count;
        }

        /// <summary>
        /// Turtle Search Algorithm, generate contour list in order
        /// </summary>
        /// <param name="valid">validMatrix[][]</param>
        /// <param name="start">start from any Point2D in contour List</param>
        /// <returns>contour pixels in order</returns>
        private List<Point2D> TurtleSearch(bool[][] valid, Point2D start)
        {
            List<Point2D> orderContour = new List<Point2D>();
            Point2D pre = new Point2D(-1, -1);
            Point2D curr = new Point2D(start);
            int dir = 0;
            do
            {
                if (valid[curr.X][curr.Y])
                {
                    dir = (dir + 1) % 4;
                    if (curr != pre)
                    {
                        orderContour.Add(new Point2D(curr));
                        pre = new Point2D(curr);
                    }
                }
                else
                {
                    dir = (dir + 4 - 1) % 4;
                }
                switch (dir)
                {
                    case 0: curr.X += 1; break; //Down
                    case 1: curr.Y += 1; break; //Right
                    case 2: curr.X -= 1; break; //Up
                    case 3: curr.Y -= 1; break; //Left
                }

            } while (curr != start);
            return orderContour;
        }

        /// <summary>
        /// find Palm Center using data in two List: handPixels[] and contourPixels[]
        /// Discription: Traveral every point in handPixels list, 
        /// caculate the Mininal Euclidean Distance between every point in contourPixels list
        /// find the maximize distance in the mininal distances set
        /// </summary>
        /// <param name="handPixels"></param>
        /// <param name="contourPixels"></param>
        /// <returns>Point2D palm</returns>
        private Point2D findPalmCenter(List<Point2D> interiorPixels, List<Point2D> contourPixels)
        {
            Point2D palm = new Point2D();
            float min, minMax, distance = 0.0f;
            minMax = float.MinValue;
            for (int i = 0; i < interiorPixels.Count; i += 8)
            {
                min = float.MaxValue;
                for (int j = 0; j < contourPixels.Count; j += 2)
                {
                    distance = Point2D.EuclideanDistance(interiorPixels[i], contourPixels[j]);
                    if (distance < min)
                    {
                        min = distance;
                    }
                    if (min < minMax) break;
                }
                if (min > minMax && min != float.MinValue)
                {
                    minMax = min;
                    palm = interiorPixels[i];
                }
            }
            return palm;
        }

        /// <summary>
        /// find fingertips by K_curvature Algorithm
        /// </summary>
        /// <param name="contour"></param>
        /// <param name="palm"></param>
        /// <returns></returns>
        private List<Point2D> findFingerTips(List<Point2D> contour, Point2D palm)
        {
            List<Point2D> fingerTips = new List<Point2D>();
            Point2D p1, p2, p3, pMid, v1, v2;
            double angle;

            int size = contour.Count;
            int fingerJump = 2;
            float fingerJumpPec = 0.1f;
            int k = 22;

            //Debug i < size  --->  i < size - k
            //ignore the last k candinate points
            for (int i = 0; i < size - k; i += fingerJump)
            {
                if (i - k + size < 0)   //Need to modify
                   break;
                p1 = contour[(i - k + size) % size];
                p2 = contour[i];
                p3 = contour[(i + k + size) % size];

                v1 = p1 - p2;
                v2 = p3 - p2;

                angle = Point2D.Angle(v1, v2);

                if (angle > 0 && angle < 40 * (Math.PI / 180))
                {
                    pMid = (p1 + p3) * 0.5;
                    if (Point2D.EuclideanDistance(pMid, palm) > Point2D.EuclideanDistance(contour[i], palm))
                        continue;
                    else
                    {
                        fingerTips.Add(new Point2D(contour[i]));
                        i += (int)(fingerJumpPec * size);
                    }
                }
            }
            //Console.WriteLine(fingerTips.Count);
            return fingerTips;
        }


        /// <summary>
        /// Map Point2D to PointDepth3D
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private PointDepth3D Point2DMapToDepth3D(Point2D point)
        {
            return pixelDepth3D[point.Y * rectWidth + point.X];
        }

        /**
         * Interface for Up layer
         * easy process in small rectangular with Point2D class
         * while,return PointDepth3D class for upper process
         **/
        /*
         * Point2D (X,Y) represent for the coordinate in small rectangular[0...rectWith][0...rectHeight]
         * PointDepth3D(X,Y,Depth) represent for the coordinate and depth of a original pixel given by Kinect DepthStream
         */
        #region Public Property

        public PointDepth3D Palm
        {
            get
            {
                return Point2DMapToDepth3D(palm);
            }
        }
        public List<PointDepth3D> Contour
        {
            get {
                if (null == contour)
                    return null;
                List<PointDepth3D> ret = new List<PointDepth3D>();
                foreach(var element in contour)
                {
                    ret.Add(Point2DMapToDepth3D(element));
                }
                return ret;
            }
        }
        public List<PointDepth3D> FingerTips
        {
            get
            {
                if (null == fingerTips)
                    return null;
                List<PointDepth3D> ret = new List<PointDepth3D>();
                foreach (var element in fingerTips)
                {
                    ret.Add(Point2DMapToDepth3D(element));
                }
                return ret;
            }
        }
       
        #endregion
    }
}
