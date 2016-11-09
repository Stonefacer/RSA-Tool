using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace AbvMathLib
{
    public class AbvMath
    {
        public static BigInteger GetGCDBinary(BigInteger m, BigInteger n)
        {
            BigInteger k = 1;
            while (true)
            {
                if (m.IsZero)
                    return n*k;
                if (n.IsZero)
                    return m*k;
                if (n == m)
                    return n*k;
                if (n.IsOne || m.IsOne)
                    return k;
                if(m.IsEven)
                {
                    m >>= 1;
                    if (n.IsEven)
                    {
                        n >>= 1;
                        k <<= 1;
                    }
                }
                else if(n.IsEven)
                    n >>= 1;
                else if(n>m)
                    n = (n - m) >> 1;
                else
                    m = (m - n) >> 1;
            }
        }

        public static int GetJacobiSymbolValue(BigInteger a, BigInteger b)
        {
            if (GetGCDBinary(a, b) != 1)
                return 0;
            int res = 1;
            if(a.Sign==-1)
            {
                a = -a;
                if (b % 4 == 3)
                    res = -res;
            }
            int t;
            BigInteger c;
            do
            {
                t = 0;
                while(a.IsEven)
                {
                    t++;
                    a >>= 1;
                }
                if(((t&1)==1))
                {
                    int i = (int)(b%8);
                    if(i==5||i==3)
                        res = -res;
                }
                if (a % 4 == 3 && b % 4 == 3)
                    res = -res;
                c = a;
                a = b % c;
                b = c;
            }
            while(!a.IsZero);
            return res;
        }


        public static int GetLegendreSymbolValue(BigInteger n, BigInteger p) {
            BigInteger p1 = p - 1;
            var res = BigInteger.ModPow(n, (p1)>>1, p);
            if (res == p1)
                return -1;
            else
                return res.Sign;
        }

        /// <summary>
        /// Get value exp(sqrt(ln(n)*ln(ln(n))))
        /// </summary>
        public static double GetLN(BigInteger n) {
            var bi = BigInteger.Log(n);
            double ln;
            if (!double.TryParse(bi.ToString(), out ln))
                return 0;
            return Math.Exp(Math.Sqrt(ln*Math.Log(ln)));
        }

        /// <summary>
        /// Very slow method
        /// </summary>
        public static BigInteger SimpleSqrt(BigInteger n) {
            BigInteger buf = 1, res = 0;
            while (n >= buf) {
                n -= buf;
                buf += 2;
                res++;
            }
            return res;
        }

        private static BigInteger SqrtGetStartValue(BigInteger n) {
            //return n >> 1;
            return BigInteger.Pow(2, n.GetBitCount()>>1);
        }

        public static BigInteger SqrtHero(BigInteger n) {
            BigInteger x=0, xp = SqrtGetStartValue(n);
            x = (xp + BigInteger.Divide(n, xp))>>1;
            while (x != xp) {
                xp = x;
                x = (xp + BigInteger.Divide(n, xp))>>1;
            }
            return x;
        }

        public static BigInteger SqrtHero(BigInteger n, int maxIterations) {
            BigInteger x = 0, xp = SqrtGetStartValue(n);
            x = (xp + BigInteger.Divide(n, xp)) >> 1;
            while (x != xp&&(maxIterations--)>1) {
                xp = x;
                x = (xp + BigInteger.Divide(n, xp)) >> 1;
            }
            return x;
        }

        public static bool IsSqrtWithoutPointExist(BigInteger n) {
            string check = n.ToString();
            if (check.EndsWith("2") || check.EndsWith("3") || check.EndsWith("7") || check.EndsWith("8"))
                return false;
            return BigInteger.Pow(SqrtHero(n), 2)==n;
        }

        public static int IsQuadraticResidue(BigInteger num, BigInteger mod) {
            BigInteger buf = mod;
            for (int i = 0; i < 0x7fffffff; i++, buf*=mod) {
                if (IsSqrtWithoutPointExist(buf + num))
                    return i;
            }
            return 0;
        }

        public static bool IsQuadraticResidue(BigInteger num, BigInteger mod, int Accuary) {
            BigInteger buf = mod;
            for (int i = 1; i < Accuary; i++) {
                if (IsSqrtWithoutPointExist(buf + num))
                    return true;
            }
            return false;
        }


    }
}
