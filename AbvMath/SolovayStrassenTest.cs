using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace AbvMathLib
{
    public class SolovayStrassenTest : IDisposable
    {

        static SolovayStrassenTest()
        {
            Working = false;
        }

        /// <summary>
        /// Test is it num prime
        /// </summary>
        /// <param name="n">Number to test</param>
        /// <param name="k">Count of iterations (accuary = 1-2^-k)</param>
        /// <returns>true if n prime and false otherwise</returns>
        public static bool IsPrime(ref BigInteger n, int k)
        {
            Random rn = new Random((int)DateTime.Now.Ticks);
            int min = 0x7fffffff;
            if (n < min)
                min = int.Parse(n.ToString())-1;
            for (int i = 0; i < k;i++ )
            {
                BigInteger a = rn.Next(0, min);
                if (AbvMath.GetGCDBinary(n, a)>1)
                    return false;
                BigInteger buf = BigInteger.ModPow(a, (n - 1) >> 1, n);
                switch(AbvMath.GetLegendreSymbolValue(a, n))
                {
                    case 0:
                        return false;
                    case 1:
                        if (buf != 1)
                            return false;
                        break;
                    case -1:
                        if (buf != n - 1)
                            return false;
                        break;
                }
            }
            return true;
        }

        public static bool IsPrimeLowNumber(int n, int k) {
            Random rn = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < k; i++) {
                int a = rn.Next(0, n-1);
                if (AbvMath.GetGCDBinary(n, a) > 1)
                    return false;
                BigInteger buf = BigInteger.ModPow(a, (n - 1) >> 1, n);
                switch (AbvMath.GetLegendreSymbolValue(a, n)) {
                    case 0:
                        return false;
                    case 1:
                        if (buf != 1)
                            return false;
                        break;
                    case -1:
                        if (buf != n - 1)
                            return false;
                        break;
                }
            }
            return true;
        }

        public static Double GetAccuary(int k)
        {
            return 1.0/Math.Pow(2.0, (double)k);
        }

        public static int Accuary { get; set; }
        public static bool Working { get; set; }

        public delegate BigInteger ChangeCurrentNumber();
        public delegate void GetNextNumber(ref BigInteger bi);

        private ChangeCurrentNumber _OnNumFound = null;
        private GetNextNumber _GetNext;
        private BigInteger _Current;
        private Thread th;
        private int _CurrentState;


        public delegate void StatusChanged(SolovayStrassenTest obj);
        public event StatusChanged OnStatusChanged;

        public delegate void PrimeNumberFound(SolovayStrassenTest obj, BigInteger Prime);
        public event PrimeNumberFound OnPrimeNumberFound;

        public delegate void TestStoped(SolovayStrassenTest obj);
        public event TestStoped OnTestStoped;

        public Label lb;

        public int Id { get; private set; }

        public int CurrentState
        {
            get
            {
                return _CurrentState;
            }
            private set
            {
                _CurrentState = value;
            }
        }

        public BigInteger Current
        {
            get
            {
                return _Current;
            }
            private set
            {

            }
        }

        public SolovayStrassenTest(int id, GetNextNumber GetNext, ChangeCurrentNumber OnNumFound)
        {
            GetNext(ref _Current);
            _GetNext = GetNext;
            _OnNumFound = OnNumFound;
            Id = id;
            th = new Thread(ThreadTest);
            th.Priority = ThreadPriority.BelowNormal;
        }

        public void Start()
        {
            th.Start();
        }

        public void Stop()
        {
            th.Abort();
        }

        private bool PrivateIsPrime(ref BigInteger n, int k)
        {
            Random rn = new Random();
            BigInteger one = BigInteger.One;
            for (_CurrentState = 0; _CurrentState < k; _CurrentState++)
            {
                OnStatusChanged(this);
                BigInteger a = rn.Next(0, 2147000000);
                if (AbvMath.GetGCDBinary(n, a) > 1)
                    return false;
                BigInteger buf = BigInteger.ModPow(a, (n - 1) >> 1, n);
                switch (AbvMath.GetJacobiSymbolValue(a, n))
                {
                    case 0:
                        return false;
                    case 1:
                        if (buf != 1)
                            return false;
                        break;
                    case -1:
                        if (buf != n - 1)
                            return false;
                        break;
                }
            }
            return true;
        }

        private void ThreadTest()
        {
            while(Working)
            {
                if (this.PrivateIsPrime(ref _Current, Accuary))
                {
                    OnPrimeNumberFound(this, _Current);
                    if (_OnNumFound != null)
                        _OnNumFound();
                }
                _GetNext(ref _Current);
            }
            OnTestStoped(this);
        }

        public void Dispose()
        {
            if (th.IsAlive)
                th.Abort();
            th = null;
            this.OnPrimeNumberFound = null;
            this.OnStatusChanged = null;
            this.OnTestStoped = null;
            this._Current = 0;
        }
    }
}
