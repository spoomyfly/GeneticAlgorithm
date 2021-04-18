using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G2FunctionOptimalization
{
    public abstract class Specie
    {
        public abstract double Fenotype();
        public abstract double FitnessFunction();
        //public abstract Specie RankingTournament(this IEnumerable<Specie> population);
        //public abstract Specie ValueTournament(this IEnumerable<Specie> population);
        //public abstract Specie RandomSelection(this IEnumerable<Specie> population);
        //public abstract void Mutate(double chance);
        //public virtual static Specie Recombinate(Specie s1, Specie s2);
    }
}
