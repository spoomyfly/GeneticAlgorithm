using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G2FunctionOptimalization
{
    public class G2Problem
    {
        private readonly long _populationVolume;
        private readonly long _populationNumber;
        public void Start(string[] args)
        {
            ParseArgs(args);
            Specie[] population = GenerateStartingPopulation();
            var populationNew = new Specie[population.Length];
            for (int i = 0; i < _populationNumber; i++)
            {
                Specie parent1 = ValueTournament(population);
                Specie parent2 = ValueTournament(population);

                Specie child = Recombinate(parent1, parent2);

                Mutate(child);

                populationNew[i] = child;
            }
        }

        private void Mutate(Specie child)
        {
            throw new NotImplementedException();
        }

        private Specie Recombinate(Specie parent1, Specie parent2)
        {
            throw new NotImplementedException();
        }

        private Specie ValueTournament(object population)
        {
            throw new NotImplementedException();
        }
        private Specie RankingTournament(object population)
        {
            throw new NotImplementedException();
        }
        private Specie SimpleTournament(object population)
        {
            throw new NotImplementedException();
        }

        private Specie[] GenerateStartingPopulation()
        {
            throw new NotImplementedException();
        }

        private void ParseArgs(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
