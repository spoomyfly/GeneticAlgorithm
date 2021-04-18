using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G2FunctionOptimalization
{
    public static class CrossoverExtensions
    {
        public static T[] OnePointCrossover<T>(T[] p1, T[] p2, int cut) where T : IEquatable<T>
        {
            T[] child = new T[p1.Length];
            for (int i = 0; i < cut; i++)
            {
                child[i] = p1[i];
            }
            for (int i = cut; i < child.Length; i++)
            {
                for (int j = 0; j < p2.Length; j++)
                {
                    if (!Array.Exists(child,  x => x is not null && x.Equals(p2[j])))
                    {
                        child[i] = p2[j];
                        break; //todo something is not yet
                    }
                }
            }
            return child;
        }
    }
}
