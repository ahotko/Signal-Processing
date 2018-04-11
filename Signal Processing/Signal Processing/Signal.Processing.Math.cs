using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Annex.Signal.Processing
{
    static class AdditionalMathFunctions
    {
        public static double Factorial(int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException("Factorial function is defined for positive integers.");
            }
            if (n == 0)
            {
                return 1.0;
            }
            double _result = 1.0;
            for (int _cnt = 1; _cnt <= n; _cnt++)
            {
                _result *= _cnt;
            }
            return _result;
        }

        public static double BesselI0(double x)
        {
            double _result = 0.0;
            for (int n = 0; n <= 10; n++)
            {
                _result += (Math.Pow(x / 2.0, 2 * n) / Math.Pow(Factorial(n), 2));
            }
            return _result;
        }

        public static double Sinc(double x)
        {
            if (x == 0.0)
            {
                return 1.0;
            }
            else
            {
                return Math.Sin(Math.PI * x) / (Math.PI * x);
            }
        }
    }
}
