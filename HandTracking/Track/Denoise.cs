using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandTracking
{
    /// <summary>
    /// class for Nosisy Reduction
    /// call static funtionDilation first;
    /// then call static function Erosion;
    /// </summary>
    public class Denoise
    {
        public static bool[][] Dilation(bool[][] pixels, int mask = 1)
        {
            // Alloc the dilated image
            bool[][] dilatePixels = new bool[pixels.Length][];
            for (int i = 0; i < pixels.Length; i++)
            {
                dilatePixels[i] = new bool[pixels[i].Length];
            }

            // Distances matrix
            int[][] distance = ManhattanDistance(pixels, true);

            // Dilate the image
            for (int i = 0; i < pixels.Length; i++)
            {
                for (int j = 0; j < pixels[i].Length; j++)
                {
                    dilatePixels[i][j] = ((distance[i][j] <= mask) ? true : false);
                }
            }

            return dilatePixels;
        }

        public static bool[][] Erosion(bool[][] pixels, int mask = 1)
        {
            // Alloc eroded image
            bool[][] erodePixels = new bool[pixels.Length][];
            for (int i = 0; i < pixels.Length; i++)
            {
                erodePixels[i] = new bool[pixels[i].Length];
            }

            // Distances matrix
            int[][] distance = ManhattanDistance(pixels, false);

            // Dilate the image
            for (int i = 0; i < pixels.Length; i++)
            {
                for (int j = 0; j < pixels[i].Length; j++)
                {
                    erodePixels[i][j] = ((distance[i][j] > mask) ? true : false);
                }
            }
            return erodePixels;
        }

        /// <summary>
        /// Calculate the Manhattan Distance, traverses distanceMatrix twice
        /// Distance between (x1,y1) to (x2,y2) is |x1-x2| + |y1-y2|
        /// </summary>
        /// <param name="image">image[i][j] = true represents pixel(i,j) is HandPixel </param>
        /// <param name="zeroDistanceValue">set distanceMatrix[i][j] = 0, where image[i][j] == zeroDistanceValue</param>
        /// <returns>Manhattan Distance Matrix</returns>
        private static int[][] ManhattanDistance(bool[][] pixels, bool zeroDistanceValue)
        {
            int[][] distanceMatrix = new int[pixels.Length][];
            for (int i = 0; i < distanceMatrix.Length; i++)
            {
                distanceMatrix[i] = new int[pixels[i].Length];
            }

            // traverse from top left to bottom right
            for (int i = 0; i < distanceMatrix.Length; i++)
            {
                for (int j = 0; j < distanceMatrix[i].Length; j++)
                {
                    if ((pixels[i][j] && zeroDistanceValue) || (!pixels[i][j] && !zeroDistanceValue))
                    {
                        // first pass and pixel was on, it gets a zero
                        distanceMatrix[i][j] = 0;
                    }
                    else
                    {
                        // pixel was off
                        // It is at most the sum of the lengths of the array
                        // away from a pixel that is on
                        distanceMatrix[i][j] = pixels.Length + pixels[i].Length;
                        // or one more than the pixel to the north
                        if (i > 0) distanceMatrix[i][j] = Math.Min(distanceMatrix[i][j], distanceMatrix[i - 1][j] + 1);
                        // or one more than the pixel to the west
                        if (j > 0) distanceMatrix[i][j] = Math.Min(distanceMatrix[i][j], distanceMatrix[i][j - 1] + 1);
                    }
                }
            }
            // traverse from bottom right to top left
            for (int i = distanceMatrix.Length - 1; i >= 0; i--)
            {
                for (int j = distanceMatrix[i].Length - 1; j >= 0; j--)
                {
                    // either what we had on the first pass
                    // or one more than the pixel to the south
                    if (i + 1 < distanceMatrix.Length)
                        distanceMatrix[i][j] = Math.Min(distanceMatrix[i][j], distanceMatrix[i + 1][j] + 1);
                    // or one more than the pixel to the east
                    if (j + 1 < distanceMatrix[i].Length)
                        distanceMatrix[i][j] = Math.Min(distanceMatrix[i][j], distanceMatrix[i][j + 1] + 1);
                }
            }

            return distanceMatrix;
        }

    }
}
