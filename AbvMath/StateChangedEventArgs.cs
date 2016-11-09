using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace AbvMathLib.Factorization {
    public class StateChangedEventArgs {
        public string State;
        public int Percents;

        public StateChangedEventArgs() {
            State = "";
            Percents = 0;
        }

        public StateChangedEventArgs(string state, int percents) {
            this.State = state;
            this.Percents = percents;
        }
    }

    class DoneEventArgs {
        public List<BigInteger> Divisors;

        public DoneEventArgs() {
            Divisors = new List<BigInteger>();
        }

        public DoneEventArgs(IEnumerable<BigInteger> lst):this() {
            Divisors.AddRange(lst);
        }
    }
}
