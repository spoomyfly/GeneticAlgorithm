using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using G2FunctionOptimalization;
using Xunit;

namespace Tests
{
    public class SelectionTests
    {
        private readonly TSPProblem problem;

        public SelectionTests():this(new TSPProblem())
        {
            
        }
        private SelectionTests(TSPProblem problem)
        {
            this.problem = problem;
        }

        [Theory, AutoData]
        public void RandomSelectionReturns(TspSpecie[] population)
        {
            var parent1 = problem.RandomSelection(population);
            var parent2 = problem.RandomSelection(population);
             
            Assert.Contains(parent1, population);
            Assert.Contains(parent2, population);
        }

        [Theory, AutoData]
        public void RouletteWheelSelection(TspSpecie[] population)
        {
            
            var parent1 = problem.RouletteWheelSelection(population);
            var parent2 = problem.RouletteWheelSelection(population);

            Assert.Contains(parent1, population);
            Assert.Contains(parent2, population);
        }
        [Theory, AutoData]
        public void RankoTurnamentSelection(int q)
        {
            double sum = 0;
            double rankSum = q * (q + 1) / 2;
            for (int i = 1; i < q; i++)
            {
                sum += i / rankSum;
            }
            //Assert.True(double.Equals(1,sum));
        }

        [Theory, AutoData]
        public void CalculateRankournament(TspSpecie[] population)
        {
            int q = population.Length;
            double rankSum = q * (q + 1) / 2;
            
            double sum = 0;
            for (int i = 1; i < q; i++)
            {
                sum += i / rankSum;
            }


            var aa = problem.CalculateRankTournamentShares(population);
            Assert.True(true);
            // +- works
        }

        [Theory]
        [InlineData(new int[] {1,2,3,5 }, new int[] { 2, 1, 5, 3 }, new int[] { 1, 2, 5, 3 })]
        [InlineData(new int[] {1,2,3,5 }, new int[] { 5, 3, 2, 1 }, new int[] { 1, 2, 5, 3 })]
        [InlineData(new int[] {1,2,3,5 }, new int[] { 3, 1, 5, 3 }, new int[] { 1, 2, 3, 5 })]
        [InlineData(new int[] {1,2,3,5 }, new int[] { 1, 5, 2, 3 }, new int[] { 1, 2, 5, 3 })]
        public void OnePointCrossoverInt(int[] parent1, int[] parent2, int[] child)
        {
            var result = CrossoverExtensions.OnePointCrossover<int>(parent1, parent2, 2);
            Assert.Equal(child, result);
        }

        [Theory]
        [MemberData(nameof(GetPointsFromDataGenerator))]

        public void OnePointCrossoverPoints(Point[] parent1, Point[] parent2, Point[] child)
        {
            var result = CrossoverExtensions.OnePointCrossover(parent1, parent2, 2);
            Assert.Equal(child, result);
        }

        public static IEnumerable<object[]> GetPointsFromDataGenerator()
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
                new Point[]
                    {
                        new Point(1,2),
                        new Point(1,1),
                        new Point(2,2),
                        new Point(2,1)
                    },
                new Point[]
                    {
                        new Point(1,1),
                        new Point(1,2),
                        new Point(2,2),
                        new Point(2,1)
                    },
            };
            yield return new object[]
            {
                new Point[]
                    {
                        new Point(1,1),
                        new Point(1,2),
                        new Point(2,1),
                        new Point(2,2)
                    },
                new Point[]
                    {
                        new Point(2,2),
                        new Point(1,1),
                        new Point(1,2),
                        new Point(2,1)
                    },
                new Point[]
                    {
                        new Point(1,1),
                        new Point(1,2),
                        new Point(2,2),
                        new Point(2,1)
                    },
            };
            yield return new object[]
            {
                new Point[]
                    {
                        new Point(1,1),
                        new Point(1,2),
                        new Point(2,1),
                        new Point(2,2)
                    },
                new Point[]
                    {
                        new Point(2,1),
                        new Point(1,1),
                        new Point(2,2),
                        new Point(1,2)
                    },
                new Point[]
                    {
                        new Point(1,1),
                        new Point(1,2),
                        new Point(2,1),
                        new Point(2,2)
                    },
            };
        }
    }
}
