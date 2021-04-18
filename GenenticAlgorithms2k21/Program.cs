using System;

namespace GenenticAlgorithms2k21
{
    public class Params
    {
        public static Specie bestSpecie;
        public static Random random = new Random();
        public static int _populationVolume = 100;
        public static int _numberOfPopulation = 100;
    }
    class Program
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [0] - number of species per population. Default is 100
        /// [1] - number of populations. Defult is 100
        /// </param>
        static void Main(string[] args)
        {
            #region init
            if (args.Length>1)
            {
                int.TryParse(args[0], out Params._populationVolume);
                int.TryParse(args[1], out Params._numberOfPopulation);
            }

            Params.bestSpecie = new Specie();
            #endregion
            Console.WriteLine($"Start reprezentacji bitowej: {DateTime.Now}");
            DoItWithBitRepresentation(args);
            Params.bestSpecie = null;
            Console.WriteLine($"Koniec reprezentacji bitowej i start reprezentacji zmiennoprzecinkowej: {DateTime.Now}");
            DoItWithDoubleRepresentation(args);
            Console.WriteLine($"Koniec reprezentacji zmiennoprzecinkowej: {DateTime.Now}");

        }

        private static void DoItWithBitRepresentation(string[] args)
        {
            TOsobnik.Start();
        }

        private static void DoItWithDoubleRepresentation(string[] args)
        {
            Specie[] population = Specie.GenerateStartPopulation(Params._populationVolume);
            for (int i = 1; i < Params._numberOfPopulation; i++)
            {
                population = Specie.GenerateNextPopulation(population);
            }
        }

        
    }


    public class Specie
    {
        // range [-2,2]
        public double Val { get; set; }


        public static Specie[] GenerateNextPopulation(Specie[] species)
        {
            Specie[] population = new Specie[species.Length];
            //ValueTournament 
            double[] shares = CountForPopulation(species); 

            for (int i = 0; i < species.Length; i++)
            {
                Specie dad = species[SelectSpecie(shares)];
                Specie mom = species[SelectSpecie(shares)];

                Specie child = Recombinate(dad, mom);
                while (!CheckIfMatchesLimitation(child))
                {
                    child = Recombinate(dad, mom);
                }
                if (Params.bestSpecie is null || Methods.Ocena(child.Val)> Methods.Ocena(Params.bestSpecie.Val))
                {
                    Params.bestSpecie = child;
                    Console.WriteLine("powstało nowe niebo " + Methods.Ocena(Params.bestSpecie.Val) + " w kroku " + i);
                }
                population[i] = child;
            }
            return population;
        }

        private static bool CheckIfMatchesLimitation(Specie child)
        {
            return child.Val <= 2 && child.Val >= -2;
                }

        private static void Mutate(Specie child)
        {
            long maska_mutacji = 1u << Params.random.Next(64);
            var xx = BitConverter.DoubleToInt64Bits(child.Val);
            child.Val = BitConverter.Int64BitsToDouble(xx ^= maska_mutacji);
        }

        private static Specie Recombinate(Specie dad, Specie mom)
        {
            var x = BitConverter.DoubleToInt64Bits(dad.Val);
            var y = BitConverter.DoubleToInt64Bits(mom.Val);

            Specie child = new Specie();

            int punkt_podzialu = Params.random.Next(1, 32);
            uint maska_podzialu = ~0u;
            maska_podzialu <<= punkt_podzialu;

            long childVal = x & maska_podzialu | (y & ~maska_podzialu);
            child.Val = BitConverter.Int64BitsToDouble(childVal);

            if (Params.random.NextDouble() < 0.05)
            {
                Mutate(child);
            }

            return child;
        }

        private static int SelectSpecie(double[] shares)
        {
            var chance = Params.random.NextDouble();
            for (int i = 0; i < shares.Length; i++)
            {
                if (chance < shares[i])
                {
                    if (i != 0)
                    {
                        return i - 1;
                    }
                    return 0;
                }
            }
            return 0;
        }

        private static double[] CountForPopulation(Specie[] species)
        {
            // suma fenotypów wszystkich kandydatów;
            double sum = 0;
            //tabela z udziałami
            double[] share = new double[species.Length];
            for (int i = 0; i < species.Length; i++)
            {
                // ponieważ fenotyp może byćwartośćią z zakresu (-2,0) , do każdego fenotypu dodaję 3.
                sum += species[i].Val + 3;
            }
            for (int i = 0; i < species.Length; i++)
            {
                //formula na obliczenie udziałów
                if (i == 0)
                {
                    share[i] = (species[i].Val + 3) / sum;
                }
                else
                {
                    share[i] = share[i - 1] + (species[i].Val + 3) / sum;
                }
            }
            return share;
        }

        public static Specie[] GenerateStartPopulation(int populationVolume)
        {
            Specie[] population = new Specie[populationVolume];
            for (int i = 0; i < populationVolume; i++)
            {
                Specie specie = new Specie
                {
                    Val = Params.random.NextDouble() * 4 - 2
                };
                population[i] = specie;
            }
            return population;
        }

    }
    public struct TOsobnik
    {
        static readonly Random los = new Random();
        uint mGen;

        public static void Start()
        {
            TOsobnik niebo = new TOsobnik();
            // populacja startowa
            TOsobnik[] populacja = PopulacjaStartowa(Params._populationVolume);
            // stop?
            for (int nr_pokolenia = 0; nr_pokolenia < Params._numberOfPopulation; ++nr_pokolenia)
            {
                TOsobnik[] nowa_populacja = new TOsobnik[populacja.Length];
                for (int i_nowy = 0; i_nowy < nowa_populacja.Length; ++i_nowy)
                {
                    // selekcja
                    //TOsobnik mama = Turniej(populacja);
                    //TOsobnik tata = Turniej(populacja);
                    TOsobnik mama = ValueTournament(populacja);
                    TOsobnik tata = ValueTournament(populacja);
                    // rekombinacja
                    nowa_populacja[i_nowy] = mama.Rekombinaja(tata);

                    while (!CheckLimits(nowa_populacja[i_nowy]))
                    {
                        Console.Write("zamiast " + nowa_populacja[i_nowy].Fenotyp());
                        nowa_populacja[i_nowy] = mama.Rekombinaja(tata);
                        Console.Write(" wygenerowano " + nowa_populacja[i_nowy].Fenotyp());
                        Console.WriteLine();
                    }

                    if (Methods.Ocena(niebo.Fenotyp()) < Methods.Ocena(nowa_populacja[i_nowy].Fenotyp()))
                    {
                        niebo = nowa_populacja[i_nowy];
                        Console.WriteLine("powstało nowe niebo " + Methods.Ocena(niebo.Fenotyp()) + " w kroku " + nr_pokolenia);
                    }
                }
                // nowa populacja
                populacja = nowa_populacja;
            }
        }
        static bool CheckLimits(TOsobnik tOsobnik)
        {
            if (tOsobnik.Fenotyp() < -2 || tOsobnik.Fenotyp() > 2)
                return false;
            return true;
        }
        public double Fenotyp()
        {
            return -2 + mGen / 1000000000.0;
        }

        public void Losowy()
        {
            mGen = (uint)los.Next(int.MinValue, int.MaxValue);
        }

        public TOsobnik Rekombinaja(TOsobnik tata)
        {
            TOsobnik dziecko;

            int punkt_podzialu = los.Next(1, 32);
            uint maska_podzialu = ~0u;
            maska_podzialu <<= punkt_podzialu;

            dziecko.mGen = (tata.mGen & maska_podzialu);
            var xx = (mGen & ~maska_podzialu);
            dziecko.mGen |= xx;
            if (los.NextDouble() < 0.1)
            { // mutacja
                dziecko = Mutate(dziecko);
            }
            return dziecko;
        }

        private static TOsobnik Mutate(TOsobnik dziecko)
        {
            uint maska_mutacji = 1u << los.Next(32);
            dziecko.mGen ^= maska_mutacji;
            return dziecko;
        }

        public static TOsobnik[] PopulacjaStartowa(int rozmiar)
        {
            TOsobnik[] populacja = new TOsobnik[rozmiar];
            for (int i = 0; i < rozmiar; i++)
                populacja[i].Losowy();

            return populacja;
        }

        public static TOsobnik ValueTournament(TOsobnik[] kandydaci)
        {
            // suma fenotypów wszystkich kandydatów;
            double sum = 0;
            //tabela z udziałami
            double[] share = new double[kandydaci.Length];
            for (int i = 0; i < kandydaci.Length; i++)
            {
                // ponieważ fenotyp może byćwartośćią z zakresu (-2,0) , do każdego fenotypu dodaję 3.
                sum += kandydaci[i].Fenotyp() + 3;
            }
            for (int i = 0; i < kandydaci.Length; i++)
            {
                //formula na obliczenie udziałów
                if (i == 0)
                {
                    share[i] = (kandydaci[i].Fenotyp() + 3) / sum;
                }
                else
                {

                    share[i] = share[i - 1] + (kandydaci[i].Fenotyp() + 3) / sum;
                }
            }
            //całość kodu powyżej może być obliczona 1 raz per gotową populację - w celu zmniejszenia nakładów obliczeniowych.

            //wybieramy osobnika 
            double chance = los.NextDouble();
            for (int i = 0; i < share.Length; i++)
            {
                if (chance < share[i])
                {
                    if (i != 0)
                    {
                        return kandydaci[i - 1];
                    }
                    return kandydaci[0];
                }
            }
            Console.WriteLine("Błąd w implementacji");
            return kandydaci[0];

        }
        public static TOsobnik RankingTournament(TOsobnik[] kandydaci)
        {
            Array.Sort(kandydaci);
            //suma nieskończonego ciągu dla Σ(1/2)^n = 1
            double chance = los.NextDouble();
            for (int i = 0; i < kandydaci.Length; i++)
            {
                if (chance < SequenceSum(i + 1))
                {
                    if (i == 0)
                    {
                        return kandydaci[0];
                    }
                    return kandydaci[i - 1];
                }
            }

            Console.WriteLine("Błąd w implementacji");
            return kandydaci[0];

        }

        private static double SequenceSum(int index)
        {
            double sum = 0;
            for (int i = 1; i <= index; i++)
            {
                sum += Math.Pow(0.5, i);
            }
            return sum;
        }
    }

    public static class Methods
    {
        public static double Ocena(double x) { return x * Math.Sin(x) * Math.Sin(10 * x); }
    }
}
