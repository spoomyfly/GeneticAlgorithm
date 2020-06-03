﻿using srodowisko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPR0206
{
    class Program
    {
        static void Main(string[] args)
        {
            _0106 _0106 = new _0106();
            _0106.Init(args);
            Console.ReadKey();
        }
    }
    class _0106
    {
        #region fields 
        public Random r;
        public static int _popVolume;
        public srodowisko.ProblemKlienta _problemKlienta;
        public static int _genNumber;
        public static int _mutationChance;
        public static DateTime _finishTime;
        public int SpecieLength;
        public double[] _roulette;
        private double _sumFF;
        public Population CurrentPopulation;
        public double MinDistance;
        public Specie MinSpecie;
        #endregion

        public void Init(string[] args)
        {
            foreach (var item in args)
            {
                Console.Write(item + "     ");
            }
            LoadParameters(args); // sprawdzamy wszystkie podane argumenty i laduejmy do pól w #region fields


            CurrentPopulation = GenStartPopulation();
            FillRoulette(CurrentPopulation);


            DateTime iterationStartTime = DateTime.Now;
            var iterationTime = DateTime.Now - iterationStartTime;
            int licznik = 0;
            while (DateTime.Now + iterationTime < _finishTime) // próba rozwiązania problemu ze skończeniem działania programu
            {
                if (licznik > 5000)
                {
                    Console.WriteLine("Numer populacji powyżej limitu i jest równy {0}", licznik);
                    break;
                }
                iterationStartTime = DateTime.Now;
                Specie[] species = new Specie[0];
                for (int j = 0; j < _popVolume; j++) //to jest stałe niezależnie od warunku stopu
                {
                    Specie s1 = Selection(CurrentPopulation);
                    Specie s2 = Selection(CurrentPopulation);

                    Specie p1 = FitnessFunction(s1) > FitnessFunction(s2) ? s2 : s1; // wybieramy rodzica z 2 wylosowanych osobników

                    Specie s3 = Selection(CurrentPopulation);
                    Specie s4 = Selection(CurrentPopulation);

                    Specie p2 = FitnessFunction(s3) > FitnessFunction(s4) ? s4 : s3; // wybieramy 2 rodzica

                    Specie child = Recombination(p1, p2);

                    //check Mutation
                    if (r.Next(0, 100) < _mutationChance)
                    {
                        child = Mutation(child);
                    }
                    //dodajemy dziecko do nowej populacji

                    Array.Resize(ref species, species.Length + 1);
                    species[species.Length - 1] = child;

                    if (FitnessFunction(child) < MinDistance)
                    {
                        MinDistance = FitnessFunction(child);
                        MinSpecie = child;
                        Console.WriteLine("#K W {0} iteracji z najmniejszym do tej pory dystansem w {1} km (?) - czas: {2}", licznik + 1, MinDistance, DateTime.Now);
                    }

                }

                licznik++;
                if (DateTime.Now - iterationStartTime > iterationTime) // próba rozwiązania problemu ze skończeniem działania programu
                                                                       // sprawdzamy ile trwała najwieksza do tej pory iteracja
                {
                    iterationTime = DateTime.Now - iterationStartTime;
                    Console.WriteLine("#t " + iterationTime + " seconds");
                }
                CurrentPopulation.species = species;
                FillRoulette(CurrentPopulation);
            }

            Console.WriteLine("Stop bo przewidywany czas ukonczenia iteracji : " + (DateTime.Now + iterationTime).ToString("dd-MM-yyyy HH:mm:ss.f") + "jest większy od " + _finishTime.ToString("dd-MM-yyyy HH:mm:ss.f") + ". Numer iteracji: " + licznik);
            //DebugFitnessFunction(MinSpecie);
            Console.WriteLine(MinDistance);

        }

        private void FillRoulette(Population population)
        {
            double[] FF = new double[population.species.Length];
            double sumFF = 0;
            for (int i = 0; i < population.species.Length; i++)
            {
                FF[i] = _problemKlienta.Ocena(population.species[i].indexes); //big doublee
                sumFF += FF[i];//sum of big double
            }
            double[] rouletteFF = new double[population.species.Length];
            for (int i = 0; i < population.species.Length; i++)
            {
                rouletteFF[i] = 1 - FF[i] / sumFF; //probability - place where we can reverse pbbb 
            }
            sumFF = 0;
            foreach (var item in rouletteFF)
            {
                sumFF += item;
            }
            _roulette = rouletteFF;
            _sumFF = sumFF;
        }
        private void LoadParameters(string[] args)
        {
            if (!DateTime.TryParse(args[0], out _finishTime))
            {
                throw new Exception("Pierwszy parameter musi być w postaci \"2020-05-19 20:20:00\"");
            }
            r = new Random();
            _problemKlienta = new ProblemKlienta();
            SpecieLength = _problemKlienta.Rozmiar(Int32.Parse(args[1]));

            r = new Random();
            _mutationChance = 2;
            try
            {
                Int32.TryParse(args[3], out _mutationChance);
            }
            catch (Exception)
            {
            }

            _popVolume = 100;
            try
            {
                Int32.TryParse(args[2], out _popVolume);
            }
            catch (Exception)
            {
            }
            _genNumber = 5000;
            MinDistance = Int32.MaxValue;
            //new Location[] { new Location { lat = 1, lon = 1 }, new Location { lat = 2, lon = 3 },
            //new Location {lat=8,lon=22 },new Location { lat = 2, lon = 2 }, new Location(){ lat=23,lon=3} };//*
        }
        public Population GenStartPopulation()
        {
            int[] defspecie = new int[SpecieLength];
            for (int i = 0; i < defspecie.Length; i++)
            {
                defspecie[i] = i;
            }
            Specie defSpecie = new Specie() { indexes = defspecie };

            Population population = new Population();
            Specie[] species = new Specie[0];

            for (int i = 0; i < _popVolume; i++) //wypelniamy populacje startową
            {
                int[] locForSpecie = ShuffleSpecie(r, defSpecie);
                Specie specie = new Specie() { indexes = locForSpecie };
                Array.Resize(ref species, species.Length + 1); // korzystamy tylko z System;
                species[species.Length - 1] = specie;
            }
            population.species = species;
            return population;

        }

        public double FitnessFunction(Specie specie) // representtionType = path
        {
            return _problemKlienta.Ocena(specie.indexes);
        }

        public Specie Selection(Population population)
        {
            double d = r.NextDouble() * _sumFF;
            double tempSum = 0;
            for (int i = 0; i < _roulette.Length; i++)
            {
                if (d < _roulette[i] + tempSum)
                {
                    return population.species[i];
                }
                tempSum += _roulette[i];
            }
            return population.species[0];
        }

        public Specie Recombination(Specie tata, Specie mama)

        {

            int[] potomek = new int[tata.indexes.Length];

            potomek[0] = tata.indexes[0];
            //search for sme val in mama arr, get index of it
            var val = mama.indexes[0];
            int index = Array.IndexOf(tata.indexes, val);
            while (!Array.Exists(potomek, x => x == val))
            {
                potomek[index] = val;
                val = mama.indexes[index];
                index = Array.IndexOf(tata.indexes, val);
            }
            for (int i = 0; i < mama.indexes.Length; i++)
            {
                if (!Array.Exists(potomek, x => x == mama.indexes[i]))
                {
                    potomek[i] = mama.indexes[i];
                }
            }
            return new Specie() { indexes = potomek };
        }

        private Specie Mutation(Specie specie)
        {
            int ran1 = r.Next(0, specie.indexes.Length - 1); // our point
            int ran2 = ran1;
            while (ran1 == ran2)
            {
                ran2 = r.Next(0, specie.indexes.Length - 1); // new place for our mutated point
            }

            int tmp = specie.indexes[ran1];
            specie.indexes[ran1] = specie.indexes[ran2];
            specie.indexes[ran2] = tmp;
            return specie;
            //Specie tmp = population.species[ran1];
            //population.species[ran1] = population.species[ran2];
            //population.species[ran2] = tmp;
            //return population;

        }
        #region classes
        public class Population
        {
            public Specie[] species { get; set; }
        }

        public class Specie
        {
            public int[] indexes;
            public string Fenotype()
            {
                string res = "";
                //foreach (var item in indexes)
                //{
                //    res += string.Format("( {0} , {1} )", _defLocations[item].lat, _defLocations[item].lon);
                //    res += Environment.NewLine;
                //}
                return res;
            }
        }

        public class Location
        {
            public double lat { get; set; }
            public double lon { get; set; }

        }
        public static int[] ShuffleSpecie(Random rng, Specie specie)
        {
            int[] result = (int[])specie.indexes.Clone();
            //Point[] arrres = 
            int n = result.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                int temp = result[n];
                result[n] = result[k];
                result[k] = temp;
            }
            return result;
        }
        #endregion
    }
}