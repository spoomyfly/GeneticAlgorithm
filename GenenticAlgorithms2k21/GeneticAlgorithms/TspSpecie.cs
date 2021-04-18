using System;

namespace G2FunctionOptimalization
{
    public class TspSpecie : Specie
    {
        public Point[] OrderedPoints { get; set; }
        public override double Fenotype()
        {
            double length = 0;
            for (int i = 1; i < OrderedPoints.Length; i++) //todo in one loop
            {
                length += OrderedPoints[i].Distance(OrderedPoints[i - 1]);
            }
            length += OrderedPoints[0].Distance(OrderedPoints[OrderedPoints.Length - 1]);
            return length;
        }
        /// <summary>
        /// we are trying to minimize function, so less Fenotype values shoud get higher fitnessFunction value.
        /// </summary>
        /// <returns></returns>
        public override double FitnessFunction()
        {
            return 1 / Fenotype();
        }
    }
    public class Point :IEquatable<Point>
    {
        public double x { get; set; }
        public double y { get; set; }
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double Distance(Point point)
        {
            return Math.Sqrt(Math.Pow(point.x - x, 2) + Math.Pow(point.y - y,2));
        }
        public override string ToString()
        {
            return $"x = {x}, y = {y}";
        }

        public bool Equals(Point other)
        {
            return other.x == x && other.y == y;
        }
    }
}
