using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace AbvMathLib.Primes
{
    public class FermatTest
    {
        public static bool IsPrime(BigInteger bi, List<BigInteger> Mask, int accuary)
        {
            BigInteger buf2 = bi - 1;
            foreach (BigInteger buf in Mask)
            {
                if (bi < buf || accuary==0)
                    return true;
                if (BigInteger.ModPow(buf, buf2, bi) != 1)
                    return false;
                accuary--;
            }
            return true;
        }

        public static bool IsPrime(BigInteger bi, List<BigInteger> Mask)
        {
            BigInteger buf2 = bi - 1;
            foreach (BigInteger buf in Mask)
            {
                if (bi <= buf)
                    return true;
                if (BigInteger.ModPow(buf, buf2, bi) != 1)
                    return false;
            }
            return true;
        }

        public static List<BigInteger> GetPrimes(int n)
        {
            List<BigInteger> lst = new List<BigInteger>();
            lst.Add(2);
            lst.Add(3);
            BigInteger i = 5;
            for (; lst.Count < n; i++)
            {
                if (IsPrime(i, lst))
                    lst.Add(i);
            }
            return lst;
        }

        public static List<BigInteger> GetPrimes(BigInteger min, BigInteger max, int accuary)
        {
            List<BigInteger> lst = GetPrimes(accuary);
            if (min.IsEven)
                min++;
            for (BigInteger i = min; i < max; i+=2)
            {
                if (IsPrime(i, lst, accuary))
                    lst.Add(i);
            }
            return lst;
        }
    }
}
