using System;
using G2FunctionOptimalization;

namespace GeneticAlgorithms
{
    class Program
    {
        static void Main(string[] args)
        {
            TSPProblem tSPProblem = new TSPProblem();
            tSPProblem.Start(args);
        }
    }
}
