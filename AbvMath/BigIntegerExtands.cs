using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;

namespace AbvMathLib
{
    public static class Extends
    {
        private static Random rnd;

        static Extends()
        {
            rnd = new Random((int)DateTime.Now.Ticks);
        }

        /// <summary>
        /// Create random positive number.
        /// </summary>
        public static BigInteger Rand(ref BigInteger min, ref BigInteger max)
        {
            BigInteger sub = max - min;
            if(sub>524288) // 2 ^ 19
            {
                sub >>= 19;
                sub *= rnd.Next(0, 524288);
                if (rnd.Next(0, 2) == 1)
                    sub++;
                return min+sub;
            }
            else
            {
                int l = (int)sub;
                return min + rnd.Next(0, l);
            }
        }

        //public static BigInteger GetSqrt(this BigInteger num, int pow)
        //{

        //}

        public static BigInteger Rand(int DecimalSymbolsCount)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<DecimalSymbolsCount;i++)
            {
                sb.Append((char)('0'+rnd.Next(10)));
            }
            return BigInteger.Parse(sb.ToString());
        }

        public static bool IsMatch(this string str, String exp)
        {
            return Regex.IsMatch(str, exp);
        }

        public static String ToHexString(this BigInteger bi)
        {
            StringBuilder sb = new StringBuilder();
            byte[] bt = bi.ToByteArray();
            return String.Join(" ", bt.Select(x => x.ToString("X02")));
        }

        public static int GetBytesCount(this BigInteger bi)
        {
            return bi.ToByteArray().Length;
        }

        public static int GetDecimalLength(this BigInteger bi)
        {
            return bi.ToString().Length;
        }

        public static int GetBitCount(this BigInteger bi)
        {
            return bi.GetBytesCount() * 8;
        }

        public static int GetRealBitCount(this BigInteger bi)
        {
            byte[] bt = bi.ToByteArray();
            int len = bt.Length * 8 - 8;
            if (len < 0)
                return 0;
            int buf = bt[bt.Length - 1];
            while(buf!=0)
            {
                buf >>= 1;
                len++;
            }
            return len;
        }
    }
}
