using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AbvMathLib;
using AbvMathLib.Factorization;
using AbvMathLib.Primes;

namespace ClassLibrary1 {
    [TestClass]

    public class Class1 {
        public void WriteLog(String str) {
            Debug.WriteLine(String.Format("{0}:\t{1}", DateTime.Now.ToString("HH:mm:ss.fffff dd/MM/yyyy"), str));
        }

        public void WriteLog(String format, params object[] objs) {
            Debug.WriteLine(String.Format("{0}:\t{1}", DateTime.Now.ToString("HH:mm:ss.fffff dd/MM/yyyy"), String.Format(format, objs)));
        }

        public void WriteLogIf(Boolean condition, String str) {
            Debug.WriteLineIf(condition, String.Format("{0}:\t{1}", DateTime.Now.ToString("HH:mm:ss.fffff dd/MM/yyyy"), str));
        }

        public void WriteLogIf(Boolean condition, String format, params object[] objs) {
            Debug.WriteLineIf(condition, String.Format("{0}:\t{1}", DateTime.Now.ToString("HH:mm:ss.fffff dd/MM/yyyy"), String.Format(format, objs)));
        }

        //[TestMethod]
        //public void TestBinaryGCD() {
        //    WriteLog("Test Started");
        //    List<BigInteger> primes = FermatTest.GetPrimes(BigInteger.Parse("2000000000"), BigInteger.Parse("2001000000"), 5);
        //    WriteLog("List of primes numbers are successfully filled");
        //    Random rnd = new Random();
        //    BigInteger k1, k2, k3, n1, n2, res, rl;
        //    int ident = 0;
        //    for (int i = 0; i < 100; i++) {
        //        k1 = primes[rnd.Next(0, primes.Count)];
        //        k2 = primes[rnd.Next(0, primes.Count)];
        //        k3 = primes[rnd.Next(0, primes.Count)];
        //        n1 = k1 * k3;
        //        n2 = k2 * k3;
        //        if (n1 == n2) {
        //            ident++;
        //            rl = n1;
        //        }
        //        else
        //            rl = k3;
        //        res = AbvMathLib.AbvMath.GetGCDBinary(n1, n2);
        //        WriteLogIf(rl != res, String.Format("FAILED: k1 = {0}; k2 = {1}; k3 = {2}; n1 = {3}; n2 = {4}; res = {5}", k1, k2, k3, n1, n2, res));
        //        Assert.AreEqual(rl, res);
        //    }
        //    WriteLog("Test succesfully passed. Identical numbers: " + ident.ToString());
        //}

        //[TestMethod]

        //public void TestBIRand() {
        //    WriteLog("Test started");
        //    BigInteger bi;
        //    String str;
        //    for (int j = 0; j < 100; j++) {
        //        for (int i = 100; i < 200; i++) {
        //            bi = Extends.Rand(i);
        //            str = bi.ToString();
        //            Assert.IsTrue(str.IsMatch("^[0-9]+$"), str);
        //            Assert.IsTrue(bi.GetBytesCount() == i, str + " Incorrect length. i=" + i.ToString() + " real=" + bi.GetBytesCount());
        //        }
        //    }
        //    WriteLog("Test passed");
        //}

        //[TestMethod]

        //public void TestSol() {
        //    WriteLog("Test started");
        //    Double SumDelay = 0;
        //    BigInteger bi = BigInteger.Parse("9479566894563398353735191602311311877687839275287939275776403855928537278827349362041969385516255210296762750068014770859990038788988534603235150717921046274131529023964700524052122193185569803163670204047385452575816321545784560679537468878138816081031909783626125006371752845178960726487531540883972892661612752890057519389231544703137130505092829698691865283908289521965336764508213483168811239802098969592163909274343300552307869425266130527105215396069564740114421200835556382585668705038005740089223326979926731253057421303556430945105276473184699780826182022674185639541442324533096161643104408868277833284609");
        //    for (int i = 0; i < 10; i++) {
        //        DateTime dt = DateTime.Now;
        //        SolovayStrassenTest.IsPrime(ref bi, 6);
        //        SumDelay += DateTime.Now.Subtract(dt).TotalMilliseconds;
        //    }
        //    WriteLog("SummaryDelay: {0} Overage: {1}", SumDelay, SumDelay / 1000.0 / 100.0);
        //    WriteLog("Test finished");
        //}

        //[TestMethod]

        //public void TestFer() {
        //    WriteLog("Test started");
        //    Double SumDelay = 0;
        //    BigInteger bi = BigInteger.Parse("9479566894563398353735191602311311877687839275287939275776403855928537278827349362041969385516255210296762750068014770859990038788988534603235150717921046274131529023964700524052122193185569803163670204047385452575816321545784560679537468878138816081031909783626125006371752845178960726487531540883972892661612752890057519389231544703137130505092829698691865283908289521965336764508213483168811239802098969592163909274343300552307869425266130527105215396069564740114421200835556382585668705038005740089223326979926731253057421303556430945105276473184699780826182022674185639541442324533096161643104408868277833284609");
        //    List<BigInteger> pr = FermatTest.GetPrimes(10);
        //    for (int i = 0; i < 10; i++) {
        //        DateTime dt = DateTime.Now;
        //        FermatTest.IsPrime(bi, pr);
        //        SumDelay += DateTime.Now.Subtract(dt).TotalMilliseconds;
        //    }
        //    WriteLog("SummaryDelay: {0} Overage: {1}", SumDelay, SumDelay / 1000.0 / 100.0);
        //    WriteLog("Test finished");
        //}

        //public void Test() {
        //    BigInteger bi = BigInteger.Parse("9479566894563398353735191602311311877687839275287939275776403855928537278827349362041969385516255210296762750068014770859990038788988534603235150717921046274131529023964700524052122193185569803163670204047385452575816321545784560679537468878138816081031909783626125006371752845178960726487531540883972892661612752890057519389231544703137130505092829698691865283908289521965336764508213483168811239802098969592163909274343300552307869425266130527105215396069564740114421200835556382585668705038005740089223326979926731253057421303556430945105276473184699780826182022674185639541442324533096161643104408868277833284609");
        //}

        //public int TestJacobiSymbolLogger(BigInteger a, BigInteger b) {
        //    return AbvMathLib.AbvMath.GetJacobiSymbolValue(a, b);
        //}

        //[TestMethod]
        //public void TestJacobiSymbol() {
        //    int res = 0;
        //    //res = TestJacobiSymbolLogger(143, 257);
        //    //Assert.AreEqual(1, res, "143;257");
        //    //res = TestJacobiSymbolLogger(18, 19);
        //    //Assert.AreEqual(-1, res, "18;19");
        //    //res = TestJacobiSymbolLogger(17, 19);
        //    //Assert.AreEqual(1, res, "17;19");
        //    //res = TestJacobiSymbolLogger(48, 263);
        //    //Assert.AreEqual(1, res, "48;263");
        //    //res = TestJacobiSymbolLogger(45, 263);
        //    //Assert.AreEqual(-1, res, "45;263");
        //    BigInteger a = BigInteger.Parse("29770582111437908908315998826818325524538949605339641932652590532610312825649894880201393922396850278695527935203688057078610146566098342004263212953949430514234948495011478803140688766392137122412918821650383317224995453252297010492164355796407803070700837690500575485721656218960578493184848420942677227008");
        //    BigInteger b = BigInteger.Parse("15688205076389538456820394134239037678817602472657741215171433365615334116795513770279733928265816747149412553390178848162442493564424357189980843799256483779360977378407741234532980573622976662030722892062421803276575142560534428922584543744238095153386871185559814679237952937253818313527187901701756594361666766764099827067509445922038083967258154369428690337877127719299918431706751056156594510022066608874807267467085901071810424704427260142315742811459980206236832109022596653453303576448415630295489625740403981241652232351264667837932321318292744211619275625817670195720666640412087216974832623693317572741651");
        //    for (int i = 0; i < 100; i++) {
        //        res = 0;
        //        while (res != 1) {
        //            DateTime dtb = DateTime.Now;
        //            res = TestJacobiSymbolLogger(a, b);
        //            if (res == 1)
        //                Debug.WriteLine(String.Format("Time={0}; Res={1}\r\n\r\n", DateTime.Now.Subtract(dtb).TotalMilliseconds, res));
        //            a++;
        //        }
        //    }
        //}

        //[TestMethod]
        //public void TestLegendreSybmol() {
        //    int res = 0;
        //    BigInteger a = BigInteger.Parse("29770582111437908908315998826818325524538949605339641932652590532610312825649894880201393922396850278695527935203688057078610146566098342004263212953949430514234948495011478803140688766392137122412918821650383317224995453252297010492164355796407803070700837690500575485721656218960578493184848420942677227008");
        //    BigInteger b = BigInteger.Parse("15688205076389538456820394134239037678817602472657741215171433365615334116795513770279733928265816747149412553390178848162442493564424357189980843799256483779360977378407741234532980573622976662030722892062421803276575142560534428922584543744238095153386871185559814679237952937253818313527187901701756594361666766764099827067509445922038083967258154369428690337877127719299918431706751056156594510022066608874807267467085901071810424704427260142315742811459980206236832109022596653453303576448415630295489625740403981241652232351264667837932321318292744211619275625817670195720666640412087216974832623693317572741651");
        //    double sum = 0;
        //    double count = 10;
        //    for (int i = 0; i < count; i++) {
        //        res = 0;
        //        DateTime dtb = DateTime.Now;
        //        res = AbvMathLib.AbvMath.GetLegendreSymbolValue(a, b);
        //        double tm = DateTime.Now.Subtract(dtb).TotalMilliseconds;
        //        Debug.WriteLine(String.Format("Time={0}; Res={1}\r\n", tm, res));
        //        sum += tm;
        //    }
        //    Debug.WriteLine(String.Format("Overage time={0}\r\n\r\n", (sum/count).ToString("F03")));
        //}


        //[TestMethod]
        //public void TestRand() {
        //    WriteLog("Test Started!");
        //    Double SumTime = 0;
        //    Double Iters = 0;
        //    Int32 NumDecLen1 = 2500;
        //    Int32 NumDecLen2 = 3000;
        //    for (Iters = 0; NumDecLen1 < NumDecLen2; NumDecLen1++, Iters++) {
        //        BigInteger a = Extends.Rand(NumDecLen1), b = Extends.Rand(NumDecLen2);
        //        DateTime st = DateTime.Now;
        //        BigInteger c = Extends.Rand(ref a, ref b);
        //        Assert.IsTrue(((c >= a) && (c < b)));
        //        SumTime += DateTime.Now.Subtract(st).TotalMilliseconds;
        //    }
        //    WriteLog("Overage delay: {0} Summary: {1}", SumTime / Iters, SumTime);
        //    WriteLog("Test Completed!");
        //}

        //[TestMethod]

        //public void TestQuadraticResidueCheker() {
        //    //10573 = 97 * 109
        //    //42593=191*223
        //    try {
        //        QS obj = new QS() { N = 42593, A = 1000, P = 40 };
        //        Trace.WriteLine(string.Format("Ln(N) = {0} N = {1} A = {2} P = {3}", AbvMath.GetLN(obj.N).ToString("F01"), obj.N, obj.A, obj.P));
        //        Trace.WriteLine(string.Format("RESULT: {0}", string.Join(" ", obj.GetPrimeDivisors().Select(x => x.ToString()))));
        //    }
        //    catch (Exception) { }
        //    //for (BigInteger i = 100; i < 10000; i += 100) {
        //    //    for (BigInteger j = 10; j < 100; j ++) {
        //    //        try {
        //    //            QS obj = new QS() { N = 14549, A = i, P = j };
        //    //            //Trace.WriteLine(string.Format("Ln(N) = {0} N = {1} A = {2} P = {3}", AbvMath.GetLN(obj.N).ToString("F01"), obj.N, obj.A, obj.P));
        //    //            Trace.WriteLine(string.Format("RESULT: {0}", string.Join(" ", obj.GetPrimeDivisors().Select(x => x.ToString()))));
        //    //            return;
        //    //        }
        //    //        catch (Exception) {}
        //    //    }
        //    //}
        //}

        //[TestMethod]

        //public void TestQuadraticResidue() {
        //    Trace.WriteLine("STARTED");
        //    for (int i = 4; i < 10000000; i++) {
        //        if (BigInteger.ModPow(i, 2, 15347) == 29) {
        //            Trace.WriteLine(i.ToString());
        //            break;
        //        }
        //    }
        //    Trace.WriteLine("STOPED");
        //}

        //[TestMethod]
        //public void TestToByteArrayEndianType() {
        //    Trace.WriteLine(string.Join(" ", (new BigInteger(0x01020304).ToByteArray().Select(x=>x.ToString("X")))));
        //}

        //[TestMethod]
        //public void TestQsGetVector() {
        //    Assert.AreEqual("00000001001001001000", QS.GetVector(0x1248, 20));
        //    Assert.AreEqual("0001001001001000", QS.GetVector(0x1248, 16));
        //    Assert.AreEqual("01001001001000", QS.GetVector(0x1248, 14));
        //}

        //[TestMethod]

        //public void TestSomething() {
        //    byte[][] Matrix = new byte[][] { new byte[] { 0, 1, 1 }, new byte[] { 0, 1, 1 }, new byte[] { 0, 1, 1 }, new byte[] { 1, 0, 1 } };
        //    Trace.WriteLine("Before");
        //    Trace.WriteLine(string.Join("\r\n", Matrix.Select(x => string.Join(" ", x.Select(y => y.ToString())))));
        //    var res = QS.ConvertMatrix(Matrix);
        //    Trace.WriteLine("\r\nAfter");
        //    Trace.WriteLine(string.Join("\r\n", Matrix.Select(x=>string.Join(" ", x.Select(y=>y.ToString())))));
        //    Trace.WriteLine(string.Join(" ", res.Select(x=>x.ToString())));
        //}
    }
}
