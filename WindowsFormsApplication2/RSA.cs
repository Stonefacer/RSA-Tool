using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml;

using Ext.System.Core;

namespace WindowsFormsApplication2 {
    public class RSA {
        #region Properties

        private BigInteger _P;
        public BigInteger P
        {
            get
            {
                return _P;
            }
            set
            {
                _P = value;
                UpdateN();
            }
        }

        private BigInteger _Q;
        public BigInteger Q
        {
            get
            {
                return _Q;
            }
            set
            {
                _Q = value;
                UpdateN();
            }
        }

        private BigInteger _N;
        public BigInteger N
        {
            get
            {
                return _N;
            }
            private set
            {
                _N = value;
            }
        }

        private BigInteger _FN;
        public BigInteger FN
        {
            get
            {
                return _FN;
            }
            private set
            {
                _FN = value;
            }
        }

        private BigInteger _E;
        public BigInteger E
        {
            get
            {
                return _E;
            }
            set
            {
                _E = value;
            }
        }

        private BigInteger _D;
        public BigInteger D
        {
            get
            {
                return _D;
            }
            set
            {
                _D = value;
            }
        }

        private void UpdateN() {
            N = P * Q;
            FN = (P - 1) * (Q - 1);
            if(!N.IsZero && !FN.IsZero && !E.IsZero)
                UpdateD();
        }

        private void UpdateD() {
            D = GetD(FN, E);
        }

        private bool IsCorrectED() {
            return (E * D % FN == 1);
        }

        #endregion

        #region Contructors
        public RSA() {

        }

        public static RSA Instance(BigInteger P, BigInteger Q, BigInteger E) {
            RSA buf = new RSA();
            buf.P = P;
            buf.Q = Q;
            buf.E = E;
            return buf;
        }

        public static RSA Instance(byte[] P, byte[] Q, byte[] E) {
            RSA buf = new RSA();
            buf.P = new BigInteger(P);
            buf.Q = new BigInteger(Q);
            buf.E = new BigInteger(E);
            return buf;
        }

        public static RSA Instance(BigInteger P, BigInteger Q, BigInteger E, BigInteger D) {
            RSA buf = new RSA();
            buf.P = P;
            buf.Q = Q;
            buf.E = E;
            buf.D = D;
            return buf;
        }

        public static RSA Instance(byte[] P, byte[] Q, byte[] E, byte[] D) {
            RSA buf = new RSA();
            buf.P = new BigInteger(P);
            buf.Q = new BigInteger(Q);
            buf.E = new BigInteger(E);
            buf.D = new BigInteger(D);
            return buf;
        }

        public static RSA InstanceEnc(BigInteger N, BigInteger E) {
            RSA buf = new RSA();
            buf.N = N;
            buf.E = E;
            return buf;
        }

        public static RSA InstanceEnc(byte[] N, byte[] E) {
            RSA buf = new RSA();
            buf.N = new BigInteger(N);
            buf.E = new BigInteger(E);
            return buf;
        }

        public static RSA InstanceDec(BigInteger N, BigInteger D) {
            RSA buf = new RSA();
            buf.N = N;
            buf.D = D;
            return buf;
        }

        public static RSA InstanceDec(byte[] N, byte[] D) {
            RSA buf = new RSA();
            buf.N = new BigInteger(N);
            buf.D = new BigInteger(D);
            return buf;
        }

        #endregion

        #region Static
        public static int GetVV(BigInteger a, BigInteger b, ref BigInteger ka, ref BigInteger kb) {
            BigInteger buf, prevKa = 1, prevKb = 0, r1 = a, r2 = b, buf2;
            if(a < b) {
                buf = a;
                a = b;
                b = buf;
            }
            ka = 0;
            kb = 1;
            while(true) {
                buf = r1 / r2;
                buf2 = ka;
                ka = prevKa - buf * ka;
                prevKa = buf2;
                buf2 = kb;
                kb = prevKb - buf * kb;
                prevKb = buf2;
                buf = r1 % r2;
                r1 = r2;
                r2 = buf;
                if(r2 < 2) {
                    return int.Parse(Convert.ToString(r2));
                }
            }
        }

        public static BigInteger GetD(BigInteger FN, BigInteger E) {
            BigInteger ka = 0, kb = 0;
            int i = GetVV(FN, E, ref ka, ref kb);
            if(i != 1) {
                throw new Exception("Can't find secure key!");
            }
            if(FN > E)
                return BigInteger.Abs(kb) * (FN - 1);
            else
                return BigInteger.Abs(ka) * (FN - 1);
        }

        #endregion

        public BigInteger Encrypt(BigInteger value) {
            return BigInteger.ModPow(value, E, N);
        }

        public BigInteger Decrypt(BigInteger value) {
            return BigInteger.ModPow(value, D, N);
        }

        public BigInteger[] Encrypt<T>(T lst) where T : System.Collections.IEnumerable, System.Collections.ICollection {
            BigInteger[] bi = new BigInteger[lst.Count];
            int i = 0;
            foreach(BigInteger buf in lst) {
                bi[i++] = Encrypt(buf);
            }
            return bi;
        }

        public BigInteger[] Decrypt<T>(T lst) where T : System.Collections.IEnumerable, System.Collections.ICollection {
            BigInteger[] bi = new BigInteger[lst.Count];
            int i = 0;
            foreach(BigInteger buf in lst) {
                bi[i++] = Decrypt(buf);
            }
            return bi;
        }

        public static BigInteger GetMessageHash(String msg, HashAlgorithm hash, Encoding enc = null) {
            if(enc == null)
                enc = Encoding.ASCII;
            byte[] bt;
            bt = hash.ComputeHash(enc.GetBytes(msg));
            return new BigInteger(bt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="enc">Default ASCII</param>
        /// <returns></returns>

        public BigInteger GetSignature(String message, Encoding enc = null) {
            using(var md5 = MD5.Create())
                return Encrypt(GetMessageHash(message, md5, enc));
        }

        public bool IsCorrectSignature(String message, BigInteger Signature, Encoding enc = null) {
            using(var md5 = MD5.Create())
                return (Decrypt(Signature) == GetMessageHash(message, md5, enc));
        }

        public byte[] GetSignature(byte[] bt) {
            BigInteger data = new BigInteger(bt);
            return Encrypt(data).ToByteArray();
        }

        public bool CheckSignature(byte[] signature, byte[] hash) {
            BigInteger data = new BigInteger(signature);
            var res = Decrypt(data).ToByteArray();
            if(res[res.Length - 1] == 0 && hash.Length == res.Length - 1)
                return res.CompareTo(1, hash, 0, hash.Length) == 0;
            else
                return res.CompareTo(hash) == 0;
        }

        public string ToXmlString(bool ShowPrivateKeys) {
            StringBuilder res = new StringBuilder();
            res.AppendLine("<?xml version=\"1.0\"?>");
            res.AppendLine("<RSAKeys>");
            res.AppendFormat("<Modulus>{0}</Modulus>\r\n", Convert.ToBase64String(N.ToByteArray()));
            res.AppendFormat("<Exponent>{0}</Exponent>\r\n", Convert.ToBase64String(E.ToByteArray()));
            if(ShowPrivateKeys) {
                res.AppendFormat("<P>{0}</P>\r\n", Convert.ToBase64String(P.ToByteArray()));
                res.AppendFormat("<Q>{0}</Q>\r\n", Convert.ToBase64String(Q.ToByteArray()));
                res.AppendFormat("<D>{0}</D>\r\n", Convert.ToBase64String(D.ToByteArray()));
            }
            res.AppendLine("</RSAKeys>");
            return res.ToString();
        }

        public void FromXmlString(string str) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(str);
            var root = doc.GetElementsByTagName("RSAKeys")[0];
            var cur = root.SelectSingleNode("Modulus");
            N = new BigInteger(Convert.FromBase64String(cur.InnerText));
            cur = root.SelectSingleNode("Exponent");
            E = new BigInteger(Convert.FromBase64String(cur.InnerText));
            cur = root.SelectSingleNode("P");
            if(cur == null)
                return;
            P = new BigInteger(Convert.FromBase64String(cur.InnerText));
            cur = root.SelectSingleNode("Q");
            Q = new BigInteger(Convert.FromBase64String(cur.InnerText));
            cur = root.SelectSingleNode("D");
            D = new BigInteger(Convert.FromBase64String(cur.InnerText));
        }

    }
}
