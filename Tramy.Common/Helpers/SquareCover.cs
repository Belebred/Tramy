using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tramy.Common.Locations;

namespace Tramy.Common.Helpers
{
    /// <summary>
    /// Class to cover polygon with squares
    /// </summary>
    public class SquareCover
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="side">Side of square</param>
        /// <param name="polygon">Polygon to be covered</param>
        public SquareCover(int side, Polygon polygon)
        {
            Polygon = polygon;
            X[0] = Polygon.Points[0].X;
            X[1] = Polygon.Points[0].X;
            Y[0] = Polygon.Points[0].X;
            Y[1] = Polygon.Points[0].X;
            Side = side;
            foreach (var point in Polygon.Points)
            {
                X[0] = Math.Min(X[0], point.X);
                X[1] = Math.Max(X[0], point.X);
                Y[0] = Math.Min(Y[0], point.Y);
                Y[1] = Math.Max(Y[1], point.Y);
            }
        }

        private int Side;
        private int[] X = new int[2];
        private int[] Y = new int[2];
        private Polygon Polygon;

        /// <summary>
        /// List of points where devices will be installed
        /// </summary>
        public List<Point> resultPoints = new List<Point>();

        /// <summary>
        /// Covers polygon with squares
        /// </summary>
        /// <returns>List of points where devices will be installed</returns>
        public async Task<List<Point>> PolygonSquareCover()
        {
            for(var i = X[0]; i <= X[1]; i += Side)
            {
                for (var j = Y[0]; j <= Y[1]; j+= Side)
                {
                    if(await SquareMask(i, j))
                    {
                        resultPoints.Add(new Point()
                        {
                            X = i + Side / 2,
                            Y = j + Side / 2
                        });
                    }
                }
            }
            return resultPoints;
        }

        private async Task<bool> SquareMask(int x, int y)
        {
            for (; x < x+Side; ++x)
            {
                for (; y < y + Side; ++y)
                {
                    if (await CheckPoly(Polygon.Points, x, y))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if point in polygon
        /// </summary>
        /// <param name="polygonPoint">List of polygon points</param>
        /// <param name="x">X coord of point</param>
        /// <param name="y">Y coord of point</param>
        public async Task<bool> CheckPoly(List<Point> polygonPoint, int x, int y)
        {
            bool c = false;
            for (int i = 0, j = polygonPoint.Count - 1; i < polygonPoint.Count; j = i++)
            {
                if ((((polygonPoint[i].Y <= y) && (y < polygonPoint[j].Y)) ||
                    ((polygonPoint[j].Y <= y) && (y < polygonPoint[i].Y))) &&
                  (((polygonPoint[j].Y - polygonPoint[i].Y) != 0) &&
                  (x > ((polygonPoint[j].X - polygonPoint[i].X) * (y - polygonPoint[i].Y) / (polygonPoint[j].Y - polygonPoint[i].Y) + polygonPoint[i].X))))
                    c = !c;
            }
            return c;
        }
    }
}
