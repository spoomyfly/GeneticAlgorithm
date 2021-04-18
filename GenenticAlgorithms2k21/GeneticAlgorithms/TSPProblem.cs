using System;
using System.Collections.Generic;
using System.Globalization;

namespace G2FunctionOptimalization
{
    public class TSPProblem
    {
        private static string _pathToFile;
        private static long _populationVolume;
        private static long _populationNumber;
        private static SelectionType _selectionType;
        private Point[] parsedPoints;
        private Random random = new Random();
        private TspSpecie bestSpecie;
        private double[] rouletteWheelSelectionShares;
        private double[] rankTournamentSelectionShares;

        /// <summary>
        /// Minimizing Full Length
        /// </summary>
        /// <param name="args"></param>
        public void Start(string[] args)
        {
            ParseArgs(args);
            TspSpecie[] population = GenerateStartingPopulation();
            for (int j = 0; j < _populationNumber; j++)
            {

            var populationNew = new TspSpecie[population.Length];
            for (int i = 0; i < _populationVolume; i++)
            {
                TspSpecie parent1;
                TspSpecie parent2;
                switch (_selectionType)
                {
                    case SelectionType.basic:
                        parent1 = SimpleTournament(population);
                        parent2 = SimpleTournament(population);
                        break;
                    case SelectionType.rankingTournament:
                        rankTournamentSelectionShares = CalculateRankTournamentShares(population);
                        parent1 = RankTournament(population);
                        parent2 = RankTournament(population);
                        break;
                    case SelectionType.valueTournament:
                        rouletteWheelSelectionShares = CalculateRouletteWheelShares(population);
                        parent1 = RouletteWheelSelection(population);
                        parent2 = RouletteWheelSelection(population);
                        break;
                    default:
                        parent1 = RandomSelection(population);
                        parent2 = RandomSelection(population);
                        break;
                }

                TspSpecie child = Recombinate(parent1, parent2);

                Mutate(child);
                if (bestSpecie is null)
                    bestSpecie = child;
                else if (bestSpecie.FitnessFunction() < child.FitnessFunction())
                {
                    bestSpecie = child;
                    Console.WriteLine("kolejny dobry powstał w pokoleniu numer {0} dla osobnika numer {1}, i ma długość trasy: {2}", j+1,i, bestSpecie.Fenotype());
                }
                populationNew[i] = child;
            }
                Console.WriteLine("{0};{1}", j+1, CalculateAverageFenotype(populationNew));
                population = populationNew;
            }
        }

        private double CalculateAverageFenotype(TspSpecie[] populationNew)
        {
            double sum = 0;
            foreach (var item in populationNew)
            {
                sum += item.Fenotype();
            }
            return sum / populationNew.Length;
        }

        public double[] CalculateRankTournamentShares(TspSpecie[] population)
        {
            double[] fitnessFunctionValues = new double[population.Length];
            for (int i = 0; i < population.Length; i++)
            {
                fitnessFunctionValues[i] = population[i].FitnessFunction();
            }
            Array.Sort(fitnessFunctionValues);
            double[] chances = new double[population.Length];
            double rankSum = population.Length * (population.Length + 1) / 2;
            for (int i = 0; i < population.Length; i++)
            {
                chances[i] = (Array.IndexOf(fitnessFunctionValues, population[i].FitnessFunction()) + 1) / rankSum;
                if (i!=0)
                {
                    chances[i] += chances[i - 1];
                }
            }
            return chances;
        }

        public TspSpecie RandomSelection(TspSpecie[] population)
        {
            return population[random.Next(0, population.Length - 1)];
        }

        private void Mutate(TspSpecie child)
        {
            if (random.NextDouble()>0.05)
            {
                var index1 = random.Next(child.OrderedPoints.Length);
                var index2 = random.Next(child.OrderedPoints.Length);
                var tmp = child.OrderedPoints[index1];
                child.OrderedPoints[index1] = child.OrderedPoints[index2];
                child.OrderedPoints[index2] = tmp;
            }
        }

        private TspSpecie Recombinate(TspSpecie parent1, TspSpecie parent2)
        {
            //todo different crossover types
            return CrossoverOnePoint(parent1, parent2);
        }

        private TspSpecie CrossoverOnePoint(TspSpecie parent1, TspSpecie parent2)
        {
            Point[] childPoints = CrossoverExtensions.OnePointCrossover(parent1.OrderedPoints, parent2.OrderedPoints, random.Next(parent1.OrderedPoints.Length));
            return new TspSpecie() { OrderedPoints = childPoints };
        }

        private TspSpecie CrossoverPMX(TspSpecie parent1, TspSpecie parent2)
        {
            int firstCutPoint = random.Next(parent1.OrderedPoints.Length-1);
            int secondCutPoint = random.Next(parent1.OrderedPoints.Length-1);
            if (firstCutPoint > secondCutPoint)
            {
                int tmp = firstCutPoint;
                firstCutPoint = secondCutPoint;
                secondCutPoint = tmp;
            }
            TspSpecie child = new TspSpecie();
            Point[] childPoints = new Point[parent1.OrderedPoints.Length];

            for (int i = firstCutPoint; i <= secondCutPoint; i++)
            {
                childPoints[i] = parent1.OrderedPoints[i];
            }
            int[] notCopiedValuesIndexesInParent2 = new int[secondCutPoint - firstCutPoint];  
            for (int i = firstCutPoint; i < secondCutPoint; i++)
            {
                if (Array.Find(childPoints, x=>parent2.OrderedPoints[i].x == x.x && parent2.OrderedPoints[i].y == x.y) is object)
                {
                    // notCopiedValues add [i] 
                }
            }

            throw new NotImplementedException();
        }
        public static T[] PMX<T>(T[] p1, T[] p2, int firstCut, int secondCut)
        {
            T[] child = new T[p1.Length]; 

            return null;
        }
        
        public TspSpecie RouletteWheelSelection(TspSpecie[] population)
        {
            
            //wybieramy osobnika 
            double chance = random.NextDouble();
            for (int i = 0; i < rouletteWheelSelectionShares.Length; i++)
            {
                if (chance < rouletteWheelSelectionShares[i])
                {
                    if (i != 0)
                    {
                        return population[i - 1];
                    }
                    return population[0];
                }
            }
            Console.WriteLine("Błąd w implementacji");
            return population[0];
        }

        private static double[] CalculateRouletteWheelShares(TspSpecie[] population)
        {
            double sum = 0;

            double[] share = new double[population.Length];
            for (int i = 0; i < population.Length; i++)
            {
                sum += population[i].FitnessFunction();
            }
            for (int i = 0; i < population.Length; i++)
            {
                if (i == 0)
                {
                    share[i] = population[i].FitnessFunction() / sum;
                }
                else
                {
                    share[i] = share[i - 1] + population[i].FitnessFunction() / sum;
                }
            }

            return share;
        }

        public TspSpecie RankTournament(TspSpecie[] population)
        {
            //wybieramy osobnika 
            double chance = random.NextDouble();
            for (int i = 0; i < rankTournamentSelectionShares.Length; i++)
            {
                if (chance < rankTournamentSelectionShares[i])
                {
                    if (i != 0)
                    {
                        return population[i - 1];
                    }
                    return population[0];
                }
            }
            Console.WriteLine("Błąd w implementacji");
            return population[0];
        }
        public TspSpecie SimpleTournament(TspSpecie[] population)
        {
            var candiadte1 = population[random.Next(0, population.Length - 1)];
            var candiadte2 = population[random.Next(0, population.Length - 1)];
            if (candiadte1.FitnessFunction() > candiadte2.FitnessFunction())
                return candiadte1;
            return candiadte2;
        }

        private TspSpecie[] GenerateStartingPopulation()
        {
            TspSpecie[] population = new TspSpecie[_populationVolume];
            for (int j = 0; j < _populationVolume; j++)
            {
                Point[] points = Randomize(parsedPoints);
                population[j] = new TspSpecie() { OrderedPoints = points };
            }
            return population;
        }

        private Point[] Randomize(Point[] parsedPoints)
        {
            random.Shuffle(parsedPoints);
            Point[] points = new Point[parsedPoints.Length];
            Array.Copy(parsedPoints, points, parsedPoints.Length);
            return points;
        }

        private void ParseArgs(string[] args)
        {
            if (args.Length>3)
            {
                _pathToFile = args[0];
                _populationNumber = Int32.Parse(args[1]);
                _populationVolume = int.Parse(args[2]);
                _selectionType =(SelectionType) int.Parse(args[3]);
            }
            else
            {
                _pathToFile = "qa194.tsp";
                _populationNumber = 1000;
                _populationVolume = 2500;
                _selectionType = SelectionType.random;
            }
            string[][] parsedString = Parser.Parser.Parse(_pathToFile);
            Point[] points = new Point[parsedString.Length];
            for (int i = 0; i < parsedString.Length; i++)
            {
                var x = double.Parse(parsedString[i][1], CultureInfo.InvariantCulture);
                var y = double.Parse(parsedString[i][2], CultureInfo.InvariantCulture);
                Point point = new Point(x, y);
                points[i] = point;
            }
            parsedPoints = points;
        }
    }
}
