using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
//using System.Diagnostics;

using AbvMathLib;
using AbvMathLib.Primes;
using System.Collections;

namespace AbvMathLib.Factorization {
    public class QS:IDisposable {

        #region InnerDataTypes

        public class Row {

            public class Element{
                public BigInteger p;
                public int BetaPow;
            }

            public static Row Create(BigInteger p, BigInteger x) {
                Row buf = new Row();
                buf.BaseT = p;
                buf.SourceT = p;
                buf.X = x;
                return buf;
            }

            public BigInteger BaseT;
            public BigInteger SourceT;
            public BigInteger X;
            public List<Element> Values;

            public static List<Element> CreateBase(List<BigInteger> PValues) {
                var Values = new List<Element>();
                Values.AddRange(PValues.Select(x => new Element() { BetaPow = 0, p = x }));
                return Values;
            }

            public void Init(List<Element> PValues) {
                Values = PValues;
            }

            public void Sieve() {
                for (int i = 0; i < Values.Count;i++) {
                    var buf = Values[i];
                    while (BaseT % Values[i].p == 0) { // low performence, check is it really so fucking neccesary
                        BaseT /= Values[i].p;
                        buf.BetaPow++;
                    }
                }
            }
        }

        public class Value {
            private bool _value;
            private List<Value> _vals = new List<Value>();
            public ValueType tp{get;private set;}
            public bool val {
                get {
                    if (tp == ValueType.Free)
                        return _value;
                    if (_vals.Count == 0)
                        return false;
                    var res = _vals[0].val;
                    for (int i = 1; i < _vals.Count; res ^= _vals[i++].val) ;
                    return res;
                }
            }

            private Value(ValueType tp) {
                this.tp = tp;
            }

            private Value(ValueType tp, bool value):this(tp) {
                _value = value;
            }

            public void SetValue(bool val) {
                if (tp == ValueType.Base)
                    return;
                _value = val;
            }

            public void AddConnection(Value val) {
                _vals.Add(val);
            }

            public static Value FreeOne {
                get {
                    return new Value(ValueType.Free, true);
                }
            }

            public static Value FreeZero {
                get {
                    return new Value(ValueType.Free, false);
                }
            }

            public static Value Base {
                get {
                    return new Value(ValueType.Base);
                }
            }
        }

        public enum ValueType {
            Free = 0,
            Base
        }

        public enum Operations:int {
            None = 0,
            CreatingTable = 1,
            CreatingFactorBase = 3,
            Sieve = 4,
            Normalize = 5,
            Solve0 = 7,
            Solve1 = 9
        }

        #endregion

        #region Static

        public static double GetPrimesAllocation(BigInteger N) {
            return BigInteger.Log(N);
        }

        public static BigInteger GetPrimesCount(BigInteger N) {
            var div = GetPrimesAllocation(N);
            if (div == 0)
                return -1;
            return N / (new BigInteger(div));
        }

        #endregion

        public BigInteger P, A, N;

        public BigInteger IterationsComleted { get; private set; }
        public BigInteger IterationsTotal { get; private set; }
        public Operations CurrentOperation { get; private set; }
        public int MatrixSizeX { get; private set; }
        public int MatrixSizeY { get; private set; }
        public string MatRes { get; private set; }
        public Boolean BruteP;

        private static List<BigInteger> DivisorsTop1000 = FermatTest.GetPrimes(1000);

        private static List<BigInteger> GetPrimeDivisiors1000(ref BigInteger N) {
            List<BigInteger> res = new List<BigInteger>();
            int id=0;
            while (N!=1&&id<DivisorsTop1000.Count) {
                if (N % DivisorsTop1000[id] == 0) {
                    N /= DivisorsTop1000[id];
                    res.Add(DivisorsTop1000[id]);
                    id = 0;
                }
                else
                    id++;
            }
            return res;
        }

        public static BigInteger[] GetAllPrimeDivisors(BigInteger N, QS qs) {
            List<BigInteger> answer = GetPrimeDivisiors1000(ref N);
            if (N == 1)
                return answer.ToArray();
            var mod = new List<BigInteger>() { 2, 3, 5, 7, 11, 13, 17, 19};
            if (FermatTest.IsPrime(N, mod)) {
                answer.Add(N);
                return answer.ToArray();
            }
            //QS qs = new QS() { N = N, A = (int)(AbvMath.GetLN(N)*1.3), BruteP = true, P=(int)(AbvMath.GetLN(N)*0.05+0.5) };
            var LN = AbvMath.GetLN(N);
            qs.N = N;
            BigInteger PStart = (int)(LN * 0.1 + 0.5);
            BigInteger[] AValues = new BigInteger[] {(int)LN};
            int AValId = 0;
            qs.P = PStart+1;
            qs.A = AValues[0];
            var bs = qs.GetFactorBase();
            List<BigInteger> res = null;
            do {
                qs.P = qs.GetFactorBaseNext(bs);
                if (qs.P >= qs.A/3) {
                    if (AValId == AValues.Length-1) {
                        answer.Add(N);
                        return answer.ToArray();
                    }
                    qs.P = PStart;
                    qs.A = AValues[++AValId];
                    bs = bs.Where(x=>x<=qs.P).ToList();
                }
                try {
                    res = qs.GetPrimeDivisors(bs);
                }
                catch (Exception) {

                }
            } while (res == null || res.Any(x => x.IsOne));
            if (FermatTest.IsPrime(res[0], mod))
                answer.Add(res[0]);
            else
                answer.AddRange(GetAllPrimeDivisors(res[0], qs));
            if (FermatTest.IsPrime(res[1], mod))
                answer.Add(res[1]);
            else
                answer.AddRange(GetAllPrimeDivisors(res[1], qs));
            var ans = answer.ToArray();
            Array.Sort(ans);
            return ans;
        }

        public List<BigInteger> GetPrimeDivisors() {
            BigInteger SQ = AbvMath.SqrtHero(N);
            List<Row> MainTable = new List<Row>();

            for (IterationsTotal = A, IterationsComleted = 1, CurrentOperation = Operations.CreatingTable; IterationsComleted <= IterationsTotal; IterationsComleted++) {
                BigInteger X = SQ+IterationsComleted;
                MainTable.Add(Row.Create(BigInteger.Pow(X, 2) - N, X));
            }
            IterationsComleted = 0;
            CurrentOperation = Operations.CreatingFactorBase;
            List<BigInteger> FactorBase = GetFactorBase();
            var buf = Row.CreateBase(FactorBase);
            MainTable.ForEach(x => x.Init(buf));
            FactorBase = null;
            GC.Collect();

            IterationsComleted = 0;
            IterationsTotal = MainTable.Count;
            CurrentOperation = Operations.Sieve;
            Sieve(MainTable);

            //MainTable.ForEach(x=>Trace.Write(x.BaseT.ToString()+" "));
            //Trace.WriteLine("");
            MainTable = MainTable.Where(x=>x.BaseT==1).ToList();
            GC.Collect();

            //MainTable.ForEach(x => Trace.WriteLine(x.SourceT.ToString()));
            //Trace.WriteLine(string.Join(" ", FactorBase.Select(x => x.ToString()).ToArray()));
            //Trace.WriteLine("------------Before normalizing------------");
            //MainTable.ForEach(x => Trace.WriteLine(string.Join(" ", x.Values.Select(y => y.BetaPow.ToString()).ToArray())));
            //IterationsComleted = 0;
            //IterationsTotal = MainTable[0].Values.Count;
            //CurrentOperation = Operations.Normalize;
            //NormalizeTable(MainTable);
            //Trace.WriteLine("------------After normalizing------------");
            //Trace.WriteLine(string.Join(" ", MainTable[0].Values.Select(x => x.p.ToString())));
            //MainTable.ForEach(x => Trace.WriteLine(string.Join(" ", x.Values.Select(y => y.BetaPow.ToString()).ToArray())));


            IterationsComleted = 0;
            CurrentOperation = Operations.Solve0;
            BigInteger N1 = 1, N2 = 1, r1 = 1;
            var S = Solve(MainTable);
            if (S.Count == 0)
                throw new Exception("Incorrect parameters");
            //Trace.WriteLine(string.Join("\r\n", S));
            CurrentOperation = Operations.Solve1;
            IterationsTotal = S.Count;
            IterationsComleted = 0;
            for (int si = 0; si < S.Count; si++, IterationsComleted++) {
                int it = 0;
                var CurRes = MainTable.Where(x => S[si][it++]).ToList();
                CurRes.ForEach(x => N1 *= x.SourceT);
                N1 = AbvMath.SqrtHero(N1);

                CurRes.ForEach(x => N2 *= x.X * x.X);
                N2 = AbvMath.SqrtHero(N2);

                r1 = AbvMath.GetGCDBinary(N2 - N1, N);
                if (r1 != 1 && N != r1)
                    break;
            }
            MainTable = null;
            FactorBase = null;
            GC.Collect();
            return new List<BigInteger>() { r1, N / r1 };
        }

        // bug: 1705289942053

        public List<BigInteger> GetPrimeDivisors(List<BigInteger> FactorBase) {
            BigInteger SQ = AbvMath.SqrtHero(N);
            List<Row> MainTable = new List<Row>();

            for (IterationsTotal = A, IterationsComleted = 1, CurrentOperation = Operations.CreatingTable; IterationsComleted <= IterationsTotal; IterationsComleted++) {
                BigInteger X = SQ + IterationsComleted;
                MainTable.Add(Row.Create(BigInteger.Pow(X, 2) - N, X));
            }
            IterationsComleted = 0;
            var buf = Row.CreateBase(FactorBase);
            MainTable.ForEach(x => x.Init(buf));

            IterationsComleted = 0;
            IterationsTotal = MainTable.Count;
            CurrentOperation = Operations.Sieve;
            Sieve(MainTable);

            //MainTable.ForEach(x => Trace.Write(x.BaseT.ToString() + " "));
            //Trace.WriteLine("");
            MainTable = MainTable.Where(x => x.BaseT == 1).ToList();
            GC.Collect();

            //MainTable.ForEach(x => Trace.WriteLine(x.SourceT.ToString()));
            //Trace.WriteLine(string.Join(" ", FactorBase.Select(x => x.ToString()).ToArray()));
            //Trace.WriteLine("------------Before normalizing------------");
            //MainTable.ForEach(x => Trace.WriteLine(string.Join(" ", x.Values.Select(y => y.BetaPow.ToString()).ToArray())));
            //IterationsComleted = 0;
            //IterationsTotal = MainTable[0].Values.Count;
            //CurrentOperation = Operations.Normalize;
            //NormalizeTable(MainTable);
            //Trace.WriteLine("------------After normalizing------------");
            //Trace.WriteLine(string.Join(" ", MainTable[0].Values.Select(x => x.p.ToString())));
            //MainTable.ForEach(x => Trace.WriteLine(string.Join(" ", x.Values.Select(y => y.BetaPow.ToString()).ToArray())));


            IterationsComleted = 0;
            CurrentOperation = Operations.Solve0;
            BigInteger N1 = 1, N2 = 1, r1=1;
            var S = Solve(MainTable);
            if (S.Count == 0)
                throw new Exception("Incorrect parameters");
            //Trace.WriteLine(string.Join("\r\n", S));


            for (IterationsComleted = 1, IterationsTotal = S.Count; IterationsComleted < IterationsTotal; IterationsComleted++) {
                int it = 0;
                var CurRes = MainTable.Where(x => S[(int)(IterationsTotal-IterationsComleted)][it++]).ToList();
                CurRes.ForEach(x => N1 *= x.SourceT);
                N1 = AbvMath.SqrtHero(N1);

                CurRes.ForEach(x => N2 *= x.X * x.X);
                N2 = AbvMath.SqrtHero(N2);

                r1 = AbvMath.GetGCDBinary(N2 - N1, N);
                if (r1 != 1 && N != r1)
                    break;
            }
            S = null;
            MainTable = null;
            GC.Collect();
            return new List<BigInteger>() { r1, N / r1 };
        }

        public static string GetVector(BigInteger val, int bitsCount) {
            StringBuilder str = new StringBuilder();
            var data = val.ToByteArray();
            for (int i = data.Length - 1; i >= 0;i-- ) {
                var buf = Convert.ToString(data[i], 2);
                while (buf.Length != 8)
                    buf = buf.Insert(0, "0");
                str.Append(buf);
            }
            int dif = str.Length - bitsCount;
            if (dif > 0)
                str = str.Remove(0, dif);
            else if (dif < 0) {
                while (dif++ != 0)
                    str = str.Insert(0, "0");
            }
            return str.ToString();
        }

        private List<BitArray> Solve(List<Row> lst, int Step=-1) {
            int m = lst.Count,              // columns
                n = lst[0].Values.Count;    // rows
            MatrixSizeX = n;
            MatrixSizeY = m;
            BitArray[] Matrix = new BitArray[n];
            for (int i = 0; i < n; i++) {
                Matrix[i] = new BitArray(m);
                for(int j=0; j < m; j++) {
                    Matrix[i].Set(j, lst[j].Values[i].BetaPow % 2 == 1);
                }
            }
            //Trace.WriteLine(string.Join("\r\n", Matrix.Select(x => string.Join(" ", x.Select(y => y.ToString())))));
            var Xs = ConvertMatrix(Matrix);
            //Trace.WriteLine("");
            //Trace.WriteLine(string.Join("\r\n", Matrix.Select(x => string.Join(" ", x.Select(y => y.ToString())))));
            Value [] res = new Value[Matrix[0].Length];
            Array.Clear(res, 0, res.Length);
            int t = 0;
            // fill base vars
            for (int i = 0; i < Xs.Length; i++) {
                if (Xs[i] == 0)
                    continue;
                res[Xs[i] - 1] = Value.Base;
            }
            // fill free vars
            for (int i = 0; i < res.Length; i++) {
                if (res[i] == null)
                    res[i] = Value.FreeOne;
            }
            if (!res.Any(x => x.tp == ValueType.Free))
                throw new InvalidOperationException();
            // add relations between vars
            for (int bs = 0; t < Matrix.Length; t++, bs = 0) {
                bs = Xs[t] - 1;
                if (bs == -1)
                    continue;
                var Base = res[bs];
                for (int i = bs+1; i < res.Length; i++) {
                    if (Matrix[t].Get(i))
                        Base.AddConnection(res[i]);
                }
            }
            // get result
            var lstStr = new List<BitArray>();
            do {
                lstStr.Add(new BitArray(res.Select(x => x.val).ToArray()));
            }
            while (GetCurrentBit(res) != -1);
            return lstStr;
        }

        private static int GetCurrentBit(Value[] res) {
            var data = res.Where(x=>x.tp==ValueType.Free).ToList();
            for (int i = 0; i < data.Count; i++) {
                if (data[i].val) {
                    // reset previous digits
                    for (int j = 0; j < i; j++) {
                        if (!data[i].val) {
                            data[i].SetValue(true);
                        }
                    }
                    data[i].SetValue(false);
                    return i;
                }
            }
            return -1;
        }

        public static int[] ConvertMatrix(BitArray[] Matrix) {
            int m = Matrix[0].Length,
                n = Matrix.Length;
            int [] Used = new int[n];
            Array.Clear(Used, 0, Used.Length);
            for (int i = 0; i < m; i++) {
                for (int j = 0; j < n; j++) {
                    if (Matrix[j].Get(i) && Used[j] == 0) {
                        for (int r = 0; r < n; r++) {
                            if (Matrix[r].Get(i) && r != j) {
                                for (int k = 0; k < m; Matrix[r][k]^=Matrix[j][k++]) ;
                            }
                        }
                        Used[j] = i+1;
                    }
                }
            }
            return Used;
        }

        private static int GetSumOfColumn(int id, List<Row> lst) {
            int sum = 0;
            lst.ForEach(x=>sum+=x.Values[id].BetaPow);
            return sum;
        }

        private static void DeleteColumn(int id, List<Row> lst) {
            lst.ForEach(x=>x.Values.RemoveAt(id));
        }

        private void NormalizeTable(List<Row> lst) {
            IterationsTotal = lst[0].Values.Count;
            for (int i = 0; i < IterationsTotal;i++) {
                IterationsComleted = i;
                if (GetSumOfColumn(i, lst) == 0) {
                    DeleteColumn(i--, lst);
                    IterationsTotal--;
                }
            }
        }

        public List<BigInteger> GetFactorBase() {
            //return new List<BigInteger>() {2, 17, 23, 29 };
            List<BigInteger> mod = new List<BigInteger>() { 2, 3, 5, 7, 11 };
            List<BigInteger> Res = new List<BigInteger>();
            if (AbvMath.GetLegendreSymbolValue(2, N)==1)
                Res.Add(2);
            BigInteger Cur = 3;
            IterationsComleted = 0;
            IterationsTotal = P;
            while (Cur<P) {
                if (FermatTest.IsPrime(Cur, mod) && (AbvMath.GetLegendreSymbolValue(N, Cur) == 1)) {
                    Res.Add(Cur);
                    IterationsComleted++;
                }
                Cur += 2;
            }
            return Res;
        }

        public BigInteger GetFactorBaseNext(List<BigInteger> bs) {
            List<BigInteger> mod = new List<BigInteger>() { 2, 3, 5, 7, 11 };
            BigInteger Cur = bs[bs.Count - 1]+2;
            if (Cur == 4)
                Cur++;
            while (true) {
                if (FermatTest.IsPrime(Cur, mod) && (AbvMath.GetLegendreSymbolValue(N, Cur) == 1)) {
                    bs.Add(Cur);
                    return Cur;
                }
                Cur += 2;
            }
        }

        private void Sieve(List<Row> MainTable) {
            foreach (var cur in MainTable) {
                cur.Sieve();
                IterationsComleted++;
            }
        }

        private void GetBeta(List<Row> MainTable, BigInteger p, out int Beta, out BigInteger t1, out BigInteger t2) {
            bool work = true;
            int BBeta = 1;
            Beta = 0;
            t1 = 0;
            t2 = 0;
            BigInteger NMod, pProp=p;
            while (true) {
                work = false;
                NMod = N%pProp;
                foreach (var el in MainTable) {
                    if (BigInteger.ModPow(el.X, 2, pProp) == NMod) {
                        t1 = el.X;
                        work = true;
                        Beta = BBeta;
                        break;
                    }
                }
                if (!work) {
                    if (pProp > t1)
                        t2 = pProp - t1;
                    else
                        t2 = pProp - t1 % pProp;
                    break;
                }
                BBeta++;
                pProp *= p;
            }
        }

        public void Dispose() {

        }
    }
}
