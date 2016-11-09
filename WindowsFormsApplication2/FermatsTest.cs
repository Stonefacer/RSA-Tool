using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading;

namespace WindowsFormsApplication2
{
    class FermatsTest
    {
        private bool _IsWorking;
        public bool IsWorking
        {
            get
            {
                return _IsWorking;
            }
            set
            {
                if (_IsWorking == true && value == false)
                    Stop();
                else if (_IsWorking == false && value == true)
                    Start();
                _IsWorking = value;
            }
        }

        public BigInteger A;
        public BigInteger Current;
        public BigInteger IteratorMain{private set;get;}
        
        private Thread th;

        private List<BigInteger> Primes;

        public delegate BigInteger ChangeCurrentNumber();
        public delegate void GetNextNumber(ref BigInteger bi);

        private ChangeCurrentNumber OnNumFound=null;
        private GetNextNumber GetNext;

        public int MaskLen
        {
            get
            {
                if (Mask == null)
                    return 0;
                return Mask.Count;
            }
            private set
            {

            }
        }

        public static FermatsTest Start(BigInteger bi, BigInteger Iters, GetNextNumber gn)
        {
            FermatsTest nw = new FermatsTest(bi, Iters);
            nw.GetNext = gn;
            nw.Start();
            return nw;
        }

        public static FermatsTest Start(BigInteger bi, BigInteger Iters, GetNextNumber gn, ChangeCurrentNumber OnFound)
        {
            FermatsTest nw = new FermatsTest(bi, Iters, OnFound);
            nw.GetNext = gn;
            nw.Start();
            return nw;
        }

        private FermatsTest(BigInteger Cur, BigInteger MaxA)
        {
            Current = Cur;
            if (Current % 2 == 0)
                Current++;
            Primes = new List<BigInteger>();
            _IsWorking = false;
            A = MaxA;
        }

        private FermatsTest(BigInteger Cur, BigInteger MaxA, ChangeCurrentNumber OnFound):this(Cur, MaxA)
        {
            this.OnNumFound = OnFound;
        }

        public void Start()
        {
            if (IsWorking)
                return;
            Mask = GetPrimes(A);
            _IsWorking = true;
            th=new Thread(ThreadTest);
            th.Priority = ThreadPriority.BelowNormal;
            th.Start();
        }

        public static bool IsPrime(BigInteger bi, BigInteger A)
        {
            BigInteger buf, buf2 = bi-1;
            if (A < 2)
                return false;
            for (buf = 2; buf < A; buf++)
            {
                if (BigInteger.ModPow(buf, buf2, bi) != 1)
                    return false;
            }
            return true;
        }

        public static bool IsPrime(BigInteger bi, List<BigInteger> Mask)
        {
            BigInteger buf2 = bi - 1;
            foreach(BigInteger buf in Mask)
            {
                if (bi < buf)
                    return true;
                if (BigInteger.ModPow(buf, buf2, bi) != 1)
                    return false;
            }
            return true;
        }

        public bool IsPrime(ref BigInteger bi)
        {
            BigInteger buf2 = bi - 1;
            IteratorMain = 0;
            foreach (BigInteger buf in Mask)
            {
                if (BigInteger.ModPow(buf, buf2, bi) != 1)
                    return false;
                IteratorMain++;
            }
            return true;
        }

        public void ThreadTest()
        {
            BigInteger cur = 0;
            while(_IsWorking)
            {
                GetNext(ref cur);
                try
                {
                    if (IsPrime(ref cur))
                    {
                        Primes.Add(cur);
                        if (OnNumFound != null)
                            OnNumFound();
                    }
                }
                catch(Exception)
                {
                }
            }
        }

        public void Stop()
        {
            _IsWorking = false;
        }

        public BigInteger this [int i]
        {
            get
            {
                if (i >= Primes.Count)
                    return 0;
                BigInteger bi = Primes[i];
                Primes.RemoveAt(i);
                return bi;
            }
            private set
            {
                if (i < Primes.Count)
                    Primes.Insert(i, value);
                else
                    Primes.Add(value);
            }
        }

        private List<BigInteger> Mask;

        public static List<BigInteger> GetPrimes(BigInteger n)
        {
            List<BigInteger> lst = new List<BigInteger>();
            lst.Add(2);
            lst.Add(3);
            BigInteger i = 5;
            for(;lst.Count<n;i++)
            {
                if (IsPrime(i, lst))
                    lst.Add(i);
            }
            return lst;
        }
    }
}
