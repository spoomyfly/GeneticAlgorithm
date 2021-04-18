using System;
using System.Collections.Generic;
using G2FunctionOptimalization;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Theory]
        [MemberData(nameof(GetPointFromDataGenerator))]
        public void Point_Distance(Point p1, Point p2, double floor, double ceiling)
        {
            Assert.True(p1.Distance(p2)>= floor && p1.Distance(p2) <= ceiling);
        }

        [Theory]
        [MemberData(nameof(GetPointArray))]
        public void TspSpecie_Fenotype(Point[] species, double length)
        {
            TspSpecie tspSpecie = new TspSpecie();
            tspSpecie.OrderedPoints = species;
            Assert.Equal(tspSpecie.Fenotype(), length);
        }

        public static IEnumerable<object[]> GetPointFromDataGenerator()
        {
            yield return new object[]
            {
            new Point(1,1),
            new Point(1,1),
            0,
            0};
            yield return new object[]
            {
            new Point(24748.3333, 50840.0000),
            new Point( 24758.8889, 51211.944),
            370,
            380};
        }

        public static IEnumerable<object[]> GetPointArray()
        {
            yield return new object[]
                {
                    new Point[]
                    {
                        new Point(1,1),
                        new Point(1,2),
                        new Point(2,1),
                        new Point(2,2)
                    },
                    2+2*Math.Sqrt(2)
                };
        }
    }
}
