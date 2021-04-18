using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G2FunctionOptimalization
{
    public static class Basic
    {
        //task: maximize function
        public static double G2Function(IEnumerable<double> arguments)
        { 
            var args = arguments.ToList();
            double sum = 0;
            double product = 1;
            double divisorSum = 0;
            for (int i = 0; i <args.Count; i++)
            {
                sum += Math.Pow(Math.Cos(args[i]), 4);
                product *= Math.Pow(Math.Cos(args[i]), 2);
                divisorSum += i* Math.Pow(args[i], 2);
            }
            product  = product * 2;

            var divider = sum - product;
            var divisor = Math.Sqrt(divisorSum);
            return Math.Abs(divider / divisor);
        }

        public static bool IfLimited(IEnumerable<double> arguments)
        {
            var args = arguments.ToList();
            double sum = 0;
            double product = 1;
            for (int i = 0; i < args.Count; i++)
            {
                product *= args[i];
                sum += args[i];
            }
            if (product<=0.75 || sum>= 7.5 * args.Count)
            {
                return false;
            }
            return true;
        }
    }
}
