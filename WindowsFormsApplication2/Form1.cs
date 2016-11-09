using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using System.IO;
using System.Threading;
using System.Linq;

using Ext.System.Core;
using Ext.System.Security;

using AbvMathLib;
using AbvMathLib.Factorization;

using Microsoft.Win32;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BigInteger[] bi = { new BigInteger(2), new BigInteger(5), new BigInteger() };
            for (int i = 0; i < 2048;i++)
            {
                bi[0] *= 2;
            }
            textBox3.Text = Convert.ToString(bi[0]%bi[1]);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label1.Text = textBox3.Text.Length.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public BigInteger FastMod(BigInteger num, BigInteger mod, int pow)
        {
            bool bl = (pow%2==1);
            BigInteger buf = num;
            while(pow>1)
            {
                buf *= num;
                buf %= mod;
                pow >>= 1;
            }
            if (bl)
                buf = (buf * num) % mod;
            return buf;
        }


        public int SpecialBinaryOr(int i1, int i2, int bits=8)
        {
            int mask = 0, fullmask = (1<<bits)-1;
            bits--;
            for(int i=(1<<bits);i>0;i>>=1)
            {
                mask |= i;
            }
            mask = (~mask)&(fullmask);
            return i1 & mask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BytesCount">1024 bits = 128 bytes</param>
        /// <returns></returns>

        static private Random rnd = new Random();

        public BigInteger RandBigInteger(int BytesCount, int HigerByte=256)
        {
            Random rn = new Random();
            byte [] bt = new byte[BytesCount];
            rn.NextBytes(bt);
            bt[BytesCount - 1] = (byte)SpecialBinaryOr(rn.Next(0, 256), HigerByte);
            return new BigInteger(bt);
        }

        public BigInteger RandBigInteger(int BytesCount)
        {
            Random rn = new Random();
            byte[] bt = new byte[BytesCount];
            rn.NextBytes(bt);
            bt[BytesCount - 1] &= 0x7F;
            return new BigInteger(bt);
        }

        public BigInteger RandBigInteger(BigInteger max, BigInteger min)
        {
            max -= min;
            byte [] bt = max.ToByteArray();
            int len = bt.Length;
            rnd.NextBytes(bt);
            bt[0] &= 0x7f;
            return (new BigInteger(bt)%max)+min;
        }

        public struct Info
        {
            public static int i, j, maxI, maxJ, Bits, Rounds;
            public static BigInteger MainI, PrevMainI;
        }

        public bool IsPrime(BigInteger num, int S, BigInteger t, int r)
        {
            if((t&1)==0)
                return false;
            //BigInteger.Pow(new BigInteger(2), S);
            BigInteger num1 = num - 1, num2=num1-1, a, x;
            Info.maxI = r;
            for(Info.i=0;Info.i<Info.maxI;Info.i++)
            {
                a = RandBigInteger(num2, 2);
                x = BigInteger.ModPow(a, t, num);
                if (x == BigInteger.One || x == num1)
                    continue;
                for(Info.j=0, Info.maxJ=S-1;Info.j<Info.maxJ;Info.j++)
                {
                    x = BigInteger.ModPow(x, 2, num);
                    if (x == 1)
                        return false;
                    if (x == num1)
                        goto GotoBack;
                }
                return false;
            GotoBack: ;
            }
            return true;
        }

        public bool IsPrime(BigInteger num)
        {
            BigInteger num1 = num - 1;
            int s=1;
            BigInteger t;
            byte[] bt = num1.ToByteArray();
            for(int i=0;i<8;i++)
            {
                if ((bt[bt.Length - 1] & (1 << i))!=0)
                    s = i;
            }
            s+=(bt.Length-1)*8;
            for (int i = s; i >=0; i--)
            {
                t = num1 / BigInteger.Pow(2, i);
                if (t % 2 != 0)
                {
                    if (BigInteger.Pow(2, i) * t == num1)
                        return IsPrime(num, i, t, s);
                }
            }
            throw new Exception("Что за ХРЕНЬ?!");
        }

        private void AddInQueue(PrimeNum bi)
        {
            mt.WaitOne();
            primes.Enqueue(bi);
            mt.Release();
        }

        private object GetLast()
        {
            Object obj = null;
            mt.WaitOne();
            if (primes.Count != 0)
                obj = primes.Dequeue();
            mt.Release();
            return obj;
        }

        private void IncreaseMin()
        {
            mt.WaitOne();
            min++;
            mt.Release();
        }

        public class PrimeNum
        {
            public String str;
            public int S;
            public BigInteger bi, K;
            public PrimeNum(BigInteger bi, int s, BigInteger k)
            {
                str = Convert.ToString(bi);
                S = s;
                K = k;
                this.bi = bi;
            }
        }

        private BigInteger min, buf;
        private Queue<PrimeNum> primes = new Queue<PrimeNum>();
        private System.Threading.Thread th = null;
        private System.Threading.Semaphore mt = new System.Threading.Semaphore(1, 1);
        private bool work = true;

        private void Start()
        {
            min = BigInteger.Pow(2, Info.Bits);
            while(work)
            {
                try
                {
                    buf = min * Info.MainI + 1;
                    if (IsPrime(buf, Info.Bits, Info.MainI, Info.Rounds))
                    {
                        AddInQueue(new PrimeNum(buf, Info.Bits, Info.MainI));
                    }
                }
                catch(Exception ex)
                {
                    tsError.Text = ex.Message;
                }
                Info.MainI += 2;
            }
            tsError.Text = "Done!";
        }



        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Info.Bits = int.Parse(textBox1.Text);
                Info.Rounds = int.Parse(textBox5.Text);
                Info.MainI = BigInteger.Parse(textBox6.Text);
                Info.PrevMainI = 0;
                //textBox4.Text = "";
                if(th==null)
                {
                    th = new System.Threading.Thread(Start);
                    th.Start();
                    timer1.Start();
                }
                else
                {
                    th.Abort();
                    th = null;
                    timer1.Stop();
                    primes.Clear();
                }

                /*for (int i = 0; i < 100000000; i++)
                {
                    bi = RandBigInteger(max, min);
                    if (bi < min || bi > max)
                    {
                        MessageBox.Show(Convert.ToString(bi)+"\r\nIteration: "+i.ToString());
                        break;
                    }
                }*/
                //MessageBox.Show("SUCCESS!!!");
                //textBox3.Text = Convert.ToString(RandBigInteger(max, min));
            }
            catch(Exception ex)
            {
                th = null;
                MessageBox.Show(ex.Message, "Unhandled exception...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = Convert.ToString(RandBigInteger(256, 127));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //textBox2.Text = Convert.ToString(RandBigInteger(256, 127));
        }

        private void AddInLog(String str)
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream("log.txt", System.IO.FileMode.Append);
                byte[] bt = Encoding.ASCII.GetBytes(str);
                fs.Write(bt, 0, bt.Length);
                fs.Close();
            }
            catch(Exception)
            {

            }
        }

        public String GetHEX(BigInteger bi)
        {
            byte[] bt = bi.ToByteArray();
            StringBuilder str = new StringBuilder();

            for (int i = bt.Length - 1; i >= 0; i--)
            {
                str.Append(bt[i].ToString("X02"));
            }
            return str.ToString();
        }

        public String GetArray(BigInteger bi)
        {
            byte[] bt = bi.ToByteArray();
            StringBuilder str = new StringBuilder();
            str.Append("byte [] key = {");
            int i = 0;
            str.Append(bt[i++].ToString());
            for (; i < bt.Length;i++ )
            {
                str.Append(", " + bt[i].ToString());
            }
            str.Append("}");
            return str.ToString();
        }

        private int GetBitCount(BigInteger bi)
        {
            byte[] bt = bi.ToByteArray();
            int i = 1 << 7, res = bt.Length * 8;
            while((bt[0]&i)==0)
            {
                res--;
                i >>= 1;
                if (i == 0)
                {
                    res++;
                    break;
                }
            }
            return res;
        }

        private int GetRealBitCount(BigInteger bi)
        {
            int i = (bi.ToByteArray().Length);
            if (i == 0)
                i = 1;
            i *= 8;
            return i;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                tsStatus.Text = String.Format("{0} Num/Sec I:{1}/{2} ({3}%) J:{4}/{5} ({6}%)",
                    (Info.MainI - Info.PrevMainI)/2,
                    Info.i, Info.maxI, (((double)Info.i) * 100.00 / ((double)Info.maxI)).ToString("F02"),
                    Info.j, Info.maxJ, (((double)Info.j) * 100.00 / ((double)Info.maxJ)).ToString("F02"));
                Info.PrevMainI = Info.MainI;
                textBox3.Text = Convert.ToString(buf);
                label1.Text = String.Format("{0} ({1} bit)", textBox3.Text.Length, GetBitCount(buf));
                textBox6.Text = Convert.ToString(Info.MainI);
                PrimeNum obj = (PrimeNum)GetLast();
                int i = 0;
                while (obj != null)
                {
                    String str = String.Format("2^{0}*{1}:{2}\r\n0x{3}\r\n{4}\r\n", obj.S, obj.K, obj.str, GetHEX(obj.bi), GetArray(obj.bi));
                    textBox4.Text += str;
                    AddInLog(str);
                    i++;
                    if (i == 100)
                        break;
                    obj = (PrimeNum)GetLast();
                }
            }
            catch(Exception)
            {
                timer1.Stop();
                MessageBox.Show("Таймер дробнул исключение");
            }
        }

        private void UpdateRegistryKoef()
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\PrimesSeacher", "Koef", textBox6.Text);
        }

        private void LoadRegisrty()
        {
            try
            {
                textBox6.Text = Registry.GetValue(@"HKEY_CURRENT_USER\Software\PrimesSeacher", "Koef", 1).ToString();
            }
            catch(Exception)
            {

            }
        }

        private void Test(int i1, int i2)
        {
            MessageBox.Show(String.Format("{0};{1};{2}", i1, i2, Convert.ToString(GetHOD(i1, i2))));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadRegisrty();
            Cur = 0;
            textBox7.Text = "100";
            textBox19.Text = "100";
            UpdateLenForAll();
            button16_Click(null, null);
            int cores = WMI.WMIInfo.CPU.GetNumberOfLogicalPocessors();
            label16.Text = "Проц потянет: " + Convert.ToString(cores) + " потока(ов)";
            label41.Text = "Проц потянет: " + Convert.ToString(cores) + " потока(ов)";
            cores >>= 1;
            if (cores < 1||cores>100)
                cores = 1;
            numericUpDown1.Value = cores;
            label50.Text = Program.Version.ToString();
            UpdateListView();
            textBox34.Text = Path.Combine(Directory.GetCurrentDirectory(), "numericDataBase.bin");
            numericUpDown3_ValueChanged(this, new EventArgs());
            label58.Text = "";
            label63.Text = "";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                UpdateRegistryKoef();
                if (th != null)
                    th.Abort();
                if (ft != null)
                {
                    foreach (FermatsTest t in ft)
                    {
                        t.IsWorking = false;
                    }
                }
                InfoGetD.work = false;
                FermatsNumbers.Work = false;
                SolovayStrassenTest.Working = false;
                if (Factor != null)
                    Factor.Abort();
            }
            catch(Exception)
            {

            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                label5.Text = textBox6.Text.Length.ToString();
            }
            catch(Exception)
            {

            }
        }

        private string RandString(int len)
        {
            string str = "";
            const string Nums = "13579";
            for(int i =0;i<len-1;i++)
            {
                str += (char)(rnd.Next(0, 10)+(int)'0');
            }
            str += Nums[rnd.Next(0, Nums.Length)];
            return str;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            textBox6.Text += RandString(10);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if(textBox6.Text.Length<11)
            {
                textBox6.Text = "";
            }
            else
            {
                textBox6.Text = textBox6.Text.Substring(0, textBox6.Text.Length - 10);
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double db = double.Parse(textBox7.Text);
                db = 1.00/Math.Pow(2.00, db);
                if(db==0)
                    label9.Text = "Погрешность: 0+";
                else
                    label9.Text = "Погрешность: " + db;
            }
            catch (Exception)
            {
                label9.Text = "Погрешность: xз";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Cur += 777;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Cur *= 777;
        }

        private BigInteger _Cur;

        private BigInteger Cur
        {
            get
            {
                return _Cur;
            }
            set
            {
                if (value < 0)
                    _Cur = 0;
                else
                    _Cur = value;
                textBox2.Text = Convert.ToString(_Cur);
                textBox33.Text = textBox2.Text;
                UpdateInfo();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void UpdateInfo()
        {
            try
            {
                label7.Text = String.Format("{0} decimal symbols; Size:{1} bit", textBox2.Text.Length, GetRealBitCount(_Cur));
                label47.Text = label7.Text;
            }
            catch (Exception ex)
            {

            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Cur -= 777;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Cur /= 777;
        }

        private void AddBytesCount(int count)
        {
            if (count < 1)
                return;
            byte [] bt = _Cur.ToByteArray();
            byte [] bt2 = new byte[bt.Length+count];
            bt.CopyTo(bt2, 0);
            for (int i = 0; i < count; i++)
            {
                bt2[bt.Length+i] = (byte)rnd.Next(0, 0x7F);
            }
            Cur = new BigInteger(bt2);
        }

        private void MinusBytesCount(int count)
        {
            if (count < 1)
                return;
            byte[] bt = _Cur.ToByteArray();
            if(count>=bt.Length)
            {
                Cur = 0;
                return;
            }
            byte[] bt2 = new byte[bt.Length - count];
            Array.Copy(bt, bt2, bt.Length - count);
            Cur = new BigInteger(bt2);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            AddBytesCount(1);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            AddBytesCount(16);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MinusBytesCount(1);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            MinusBytesCount(16);
        }

        Semaphore sem = new Semaphore(1, 1);

        private BigInteger Change()
        {
            sem.WaitOne();
            BigInteger bi = _Cur;
            int len = _Cur.ToByteArray().Length;
            if (len > 128)
            {
                bi = RandBigInteger(len);
                if (bi % 2 == 0)
                    bi++;
                _Cur = bi;
            }
            sem.Release();
            return bi;
        }

        private void GetNext(ref BigInteger abi)
        {
            sem.WaitOne();
            abi = _Cur;
            _Cur += 2;
            sem.Release();
        }


        private FermatsTest [] ft = null;

        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                if (ft != null)
                {
                    timer2.Stop();
                    foreach (FermatsTest t in ft)
                    {
                        t.Stop();
                    }
                    ft = null;
                    sc.Clear();
                    button13.Text = "Старт";
                }
                else
                {
                    PrevBi = 0;
                    if (_Cur % 2 == 0)
                        _Cur++;
                    int TCount = Convert.ToInt32(numericUpDown1.Value);
                    ft = new FermatsTest[TCount];
                    for (int i = 0; i < TCount; i++)
                    {
                        ft[i] = FermatsTest.Start(_Cur, BigInteger.Parse(textBox7.Text), GetNext, Change);
                    }
                    button13.Text = "Стоп";
                    timer2.Start();
                }
            }
            catch(Exception ex)
            {
                tsError.Text = ex.Message;
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }


        class SpeedCounter
        {
            private int Size, _begin, _RSize;

            struct TimePoint
            {
                public double value;
                public long Ticks;
            };

            TimePoint[] History;

            public double Speed
            {
                get
                {
                    return GetOverageSpeed();
                }
                private set
                {

                }
            }

            private int Begin
            {
                get
                {
                    return _begin;
                }
                set
                {
                    if (value >= Size)
                        value = 0;
                    _begin = value;
                }
            }

            private int RSize
            {
                get
                {
                    return _RSize;
                }
                set
                {
                    if (value > Size)
                        value = Size;
                    _RSize = value;
                }
            }

            public double GetInterval()
            {
                long min = History[0].Ticks, max = History[0].Ticks;
                for(int i=0;i<RSize;i++)
                {
                    if (History[i].Ticks < min)
                        min = History[i].Ticks;
                    else if (History[i].Ticks > max)
                        max = History[i].Ticks;
                }
                double db = max - min;
                db /= TimeSpan.TicksPerSecond;
                if (db<=0)
                    db = 1;
                return db;
            }

            public double GetOverageSpeed()
            {
                double db = History[0].value;
                for (int i = 1; i < RSize;i++)
                {
                    db += History[i].value;
                }
                return db / GetInterval();
            }

            public void Add(double db, long ticks)
            {
                History[Begin].value = db;
                History[Begin].Ticks = ticks;
                Begin++;
                RSize++;
            }

            public SpeedCounter(int Size)
            {
                this.Size = Size;
                History = new TimePoint[Size];
                _RSize = 0;
                _begin = 0;
                for (int i = 0; i < Size;i++ )
                {
                    History[i].value = 0;
                    History[i].Ticks = 0;
                }
            }

            public void Clear()
            {
                Begin = 0;
                RSize = 0;
            }
        }

        BigInteger PrevBi = 0;
        SpeedCounter sc = new SpeedCounter(100);

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (ft == null)
            {
                timer2.Stop();
                return;
            }
            textBox8.Text = Convert.ToString(_Cur);
            label10.Text = String.Format("{0} decimal symbols Size:{1} bit", textBox8.Text.Length, GetRealBitCount(_Cur));
            BigInteger speed = (_Cur - PrevBi) / 2;
            PrevBi = _Cur;
            if (speed >= 0 && speed <= 100000)
                sc.Add(double.Parse(Convert.ToString(speed)), DateTime.Now.Ticks);
            StringBuilder sb = new StringBuilder();
            int id = 0;
            sb.AppendFormat("Speed: {0} NPS ", sc.Speed.ToString("F03"));
            foreach (FermatsTest test in ft)
            {
                sb.Append(String.Format("Thread {0}: Current Number: {1}/{2} ({3} %); ", id++,
                    test.IteratorMain, test.MaskLen, (((double)test.IteratorMain * 100.00) / ((double)test.MaskLen)).ToString("F02")));
                BigInteger buf = test[0];
                String str;
                while ((!buf.IsZero))
                {
                    str = String.Format("{0}\r\n0x{1}\r\n{2}; // {3} bit\r\n\r\n", Convert.ToString(buf), GetHEX(buf), GetArray(buf), GetRealBitCount(buf));
                    if (checkBox3.Checked) {
                        textBox9.Text += str;
                        textBox9.SelectionStart = textBox9.Text.Length;
                        textBox9.ScrollToCaret();
                        AddInLog(str);
                    }
                    buf = test[0];
                    Application.DoEvents();
                }
            }
            tsStatus.Text = sb.ToString();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                BigInteger P, Q, E, D, T, F;
                P = BigInteger.Parse(textBox11.Text);
                Q = BigInteger.Parse(textBox12.Text);
                E = BigInteger.Parse(textBox15.Text);
                D = BigInteger.Parse(textBox20.Text);
                F = BigInteger.Parse(textBox10.Text);
                T = BigInteger.Parse(textBox22.Text);
                UpdateLenForAll();

                (new TestRSAInterval(RSA.Instance(P, Q, E, D), F, T)).Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private int ___i;
        private const int max____i = 10000;

        public void WritePrimesCountAllocation()
        {
            FileStream fs = new FileStream("Primes Allocation.txt", FileMode.Create);
            BigInteger bi3 = 3, bi2 = 2;
            String str;
            byte[] bt;
            for (___i = 1; ___i < max____i; ___i++)
            {
                str = String.Format("{0}\t\t\t{1}-{2}\r\n", ___i, Convert.ToString(bi2).Length, Convert.ToString(bi2).Length);
                bt = Encoding.ASCII.GetBytes(str);
                fs.Write(bt, 0, bt.Length);
                bi2 *= 2;
                bi3 *= 3;
            }
            fs.Close();
        }

        public BigInteger GetHOD(BigInteger a, BigInteger b)
        {
            BigInteger preva;
            if(a<b)
            {
                preva = a;
                a = b;
                b = preva;
            }
            while(true)
            {
                preva = b;
                b = a % b;
                a = preva;
                if (b == 0)
                    return a;
            }
        }

        private struct FermatsNumbers
        {
            public static int PowMin, PowCur, PowMax, Miss, Accuary;
            public static bool Work;
            public static int id;
            public static List<BigInteger> Mask;
            public static Queue<BigInteger> Primes = new Queue<BigInteger>();
        }

        private void Searcher()
        {
            int id = FermatsNumbers.id;
            FermatsNumbers.PowCur = FermatsNumbers.PowMin;
            BigInteger bi, bi2;
            while(id==FermatsNumbers.id&&FermatsNumbers.Work)
            {
                if (FermatsNumbers.PowCur >= FermatsNumbers.PowMax)
                {
                    FermatsNumbers.Work = false;
                    break;
                }
                bi = BigInteger.Pow(2, FermatsNumbers.PowCur++);
                for(int i=1;i<FermatsNumbers.Miss;i+=2)
                {
                    bi2 = bi + i;
                    if(FermatsTest.IsPrime(bi2, FermatsNumbers.Mask))
                    {
                        FermatsNumbers.Primes.Enqueue(bi2);
                    }
                }
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FermatsNumbers.Work)
                {
                    FermatsNumbers.PowMin = int.Parse(textBox18.Text);
                    FermatsNumbers.PowMax = int.Parse(textBox16.Text);
                    FermatsNumbers.Miss = int.Parse(textBox21.Text);
                    FermatsNumbers.Accuary = int.Parse(textBox19.Text);
                    if (FermatsNumbers.PowMin >= FermatsNumbers.PowMax)
                        throw new Exception("Границы заданы с ошибкой!");
                    FermatsNumbers.Work = true;
                    FermatsNumbers.id = rnd.Next(0, 1000000000);
                    FermatsNumbers.Mask = FermatsTest.GetPrimes(FermatsNumbers.Accuary);
                    FermatsNumbers.Primes.Clear();
                    Thread th = new Thread(Searcher);
                    th.Start();
                    button15.Text = "Стоп";
                    timer4.Interval = 1000;
                    timer4.Start();
                }
                else
                {
                    FermatsNumbers.Work = false;
                    FermatsNumbers.id++;
                    button15.Text = "Старт";
                    timer4.Stop();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            tsStatus.Text = String.Format("{0}/{1} ({2} %)", ___i, max____i, (((double)___i*100.00)/((double)max____i)).ToString("F02"));
            if (___i == max____i)
                timer3.Stop();
        }

        public delegate void Updater();
        const int MaxIter = 1000000000;


        private struct InfoGetD
        {
            public static int i;
            public static BigInteger D, E, FN, N;
            public static bool work;
        }

        public void StartSearching()
        {
            InfoGetD.D = GetDbyE(InfoGetD.E, InfoGetD.FN, InfoGetD.N);
            Updater upd = delegate()
            {
                timer5.Enabled = false;
                textBox20.Text = Convert.ToString(InfoGetD.D);
                if ((InfoGetD.E * InfoGetD.D) % InfoGetD.FN == 1)
                {
                    label22.Text = InfoGetD.i.ToString() + " Correct";
                }
                else
                {
                    label22.Text = InfoGetD.i.ToString() + " Incorrect";
                }
            };
            this.Invoke(upd);
        }

        public BigInteger GetDbyE(BigInteger E, BigInteger FN, BigInteger N)
        {
            //return BigInteger.ModPow(E, FN - 1, FN);
            BigInteger k=FN;
            InfoGetD.i = 0;
            //Updater upd;
            InfoGetD.work = true;
            while (InfoGetD.work)
            {
                if ((k+1) % E == 0)
                {
                    return (k+1) / E;
                }
                k += FN;
                InfoGetD.i++;
                if (InfoGetD.i == MaxIter)
                    return 0;
            }
            return 0;
        }

        /// <summary>
        /// Ceil(a/b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public BigInteger CeilDiv(BigInteger a, BigInteger b)
        {
            BigInteger res = a/b;
            if (a - b * res != 0)
                res++;
            return res;
        }



        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                BigInteger FN = (BigInteger.Parse(textBox11.Text)-1) * (BigInteger.Parse(textBox12.Text)-1), E = BigInteger.Parse(textBox15.Text), D;
                D = RSA.GetD(FN, E);
                textBox20.Text = Convert.ToString(D);
                label22.Text = String.Format("E * D = {0} (mod f(N))", (E*D)%FN);
                UpdateLenForAll();
                /*if(InfoGetD.work)
                {
                    InfoGetD.work = false;
                    return;
                }
                BigInteger p = BigInteger.Parse(textBox11.Text), q = BigInteger.Parse(textBox12.Text);
                InfoGetD.N = p * q;
                InfoGetD.FN = (p - 1) * (q - 1);
                InfoGetD.E = BigInteger.Parse(textBox15.Text);

                Thread th = new Thread(StartSearching);
                timer5.Enabled = true;
                th.Start();*/
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Double db = Double.Parse(textBox19.Text);
                db = 1.00 / Math.Pow(2.00, db);
                if (db == 0)
                    label24.Text = "Погрешность: 0+";
                else
                    label24.Text = "Погрешность: " + db.ToString();
            }
            catch(Exception)
            {

            }
        }

        private int GetPositiveBitCount(byte bt)
        {
            int s = 0;
            for(int i=1;i<256;i<<=1)
            {
                if ((bt & i) != 0)
                    s++;
            }
            return s;
        }

        private int GetPositiveBitCount(byte[] bt)
        {
            int s = 0;
            foreach(byte btt in bt)
            {
                s += GetPositiveBitCount(btt);
            }
            return s;
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            double progress = FermatsNumbers.PowCur-FermatsNumbers.PowMin, summary = FermatsNumbers.PowMax-FermatsNumbers.PowMin;
            tsStatus.Text = String.Format("{0}/{1} ({2} %)", progress.ToString("F0"), summary.ToString("F0"), (progress*100.00/summary).ToString("F03"));
            String str;
            while(FermatsNumbers.Primes.Count != 0)
            {
                BigInteger bi = FermatsNumbers.Primes.Dequeue();
                int count = GetPositiveBitCount(bi.ToByteArray());
                if (count == 2)
                    str = "**********************************************************************************************\r\n";
                else
                    str = "";
                str += string.Format("{0}\r\n0x{1}\r\n{2}; // {3} bit 1:{4}\r\n\r\n", Convert.ToString(bi), GetHEX(bi), GetArray(bi), 
                    GetRealBitCount(bi), count);
                textBox17.Text += str;
                textBox17.SelectionStart = textBox17.Text.Length;
                textBox17.ScrollToCaret();
                AddInLog(str);
                Application.DoEvents();
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            label22.Text = String.Format("{0}/{1} ({2} %)", InfoGetD.i, MaxIter, ((double)InfoGetD.i*100.00/(double)MaxIter).ToString());
        }

        private void UpdateLength(TextBox src, Label size)
        {
            BigInteger bi = 0;
            if (BigInteger.TryParse(src.Text, out bi))
            {
                size.Text = GetRealBitCount(bi) + " bit";
            }
            else
            {
                size.Text = "";
            }
        }

        /*
         * 25 - 10
         * 26 - 11
         * 27 - 12
         * 28 - 15
         * 29 - 20
         * 30 - 13
         * 31 - 14
         */

        private void UpdateLenForAll()
        {
            UpdateLength(textBox10, label25);
            UpdateLength(textBox11, label26);
            UpdateLength(textBox12, label27);
            UpdateLength(textBox15, label28);
            UpdateLength(textBox20, label29);
            UpdateLength(textBox29, label39);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            UpdateLenForAll();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        bool IsEmpty = true;




        private void CreateList()
        {
            StreamWriter sw = new StreamWriter("arr.txt");
            String str = "const String [][] PrimeNumbersLocationArr = new String[][]\r\n{";
            sw.WriteLine(str);
            BigInteger b0, b1;
            int l = 16384*32;
            for (int i = 256; i <= l; i += 256)
            {
                b0 = BigInteger.Pow(2, i - 8);
                b1 = BigInteger.Pow(2, i) - 1;
                str = "new String[]{";
                str += String.Format("\"{0}\", \"{1}\", \"{2}\", \"{3}\"",
                    i.ToString(),
                    (i / 8).ToString(),
                    BigInteger.Log(b0).ToString("F0") + " - " + BigInteger.Log(b1).ToString("F0"),
                    Convert.ToString(b0).Length.ToString() + " - " + Convert.ToString(b1).Length.ToString());
                str += "}";
                if (i != l)
                    str += ",";
                sw.WriteLine(str);
                this.Invoke((Action)(delegate() { tsStatus.Text = String.Format("{0}/{1}({2} %)", i, l, (((double)i) * 100.00 / ((double)l)).ToString("F03")); }));
            }
            str = "};";
            sw.WriteLine(str);
            sw.Flush();
            sw.Close();
            IsEmpty = true;
        }

        private bool lstInited = false;

        private void UpdateListView()
        {
            if (lstInited)
                return;
            lstInited = true;
            ListViewItem itm;
            for(int i=0;i<BigVariables.PrimeNumbersLocationArr.Length;i++)
            {
                itm = new ListViewItem(BigVariables.PrimeNumbersLocationArr[i]);
                if (i % 2 != 0)
                    itm.BackColor = Color.FromArgb(0xe0e0e0);
                listView1.Items.Add(itm);
            }
            //if (!IsEmpty)
            //    return;
            //IsEmpty = false;
            //(new Thread(CreateList)).Start();
            //if (!IsEmpty)
            //    return;
            //String[] obj = new String[4];
            //BigInteger b0, b1;
            //ListViewItem itm;
            //bool bl = false;
            //for (int i = 256; i <= 16384*4; i += 256)
            //{
            //    b0 = BigInteger.Pow(2, i - 8);
            //    b1 = BigInteger.Pow(2, i) - 1;
            //    obj[0] = i.ToString();
            //    obj[1] = (i / 8).ToString();
            //    obj[2] = BigInteger.Log(b0).ToString("F0") + " - " + BigInteger.Log(b1).ToString("F0");
            //    obj[3] = Convert.ToString(b0).Length.ToString() + " - " + Convert.ToString(b1).Length.ToString();
            //    itm = new ListViewItem(obj);
            //    if (i % 1024 == 0)
            //        itm.BackColor = Color.FromArgb(0x9e9e9e);
            //    else if (bl)
            //        itm.BackColor = Color.FromArgb(0xeaeaea);
            //    bl = !bl;
            //    listView1.Items.Add(itm);
            //}
            //IsEmpty = false;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {
            AddBytesCount(64);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            MinusBytesCount(64);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                String msg = textBox13.Text;
                byte[] bt;
                if(checkBox1.Checked)
                    bt = Encoding.Unicode.GetBytes(msg);
                else
                    bt = Encoding.ASCII.GetBytes(msg);
                BigInteger E = BigInteger.Parse(textBox14.Text);
                BigInteger N = BigInteger.Parse(textBox23.Text);
                BigInteger M = new BigInteger(bt);
                if (M > N)
                    throw new Exception("Длинна сообщения привышает длинну ключа!");
                textBox24.Text = RSA.InstanceEnc(N, E).Encrypt(M).ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ОшибкО", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                BigInteger M = BigInteger.Parse(textBox28.Text);
                BigInteger D = BigInteger.Parse(textBox27.Text);
                BigInteger N = BigInteger.Parse(textBox26.Text);
                if(checkBox1.Checked)
                    textBox25.Text = Encoding.Unicode.GetString(RSA.InstanceDec(N, D).Decrypt(M).ToByteArray());
                else
                    textBox25.Text = Encoding.ASCII.GetString(RSA.InstanceDec(N, D).Decrypt(M).ToByteArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ОшибкО", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            BigInteger p = BigInteger.Parse(textBox11.Text);
            BigInteger q = BigInteger.Parse(textBox12.Text);
            textBox29.Text = (p * q).ToString();
            UpdateLength(textBox29, label39);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            AddBytesCount(1);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            MinusBytesCount(1);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            AddBytesCount(16);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            MinusBytesCount(16);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            AddBytesCount(64);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            MinusBytesCount(64);
        }

        private List<SolovayStrassenTest> _SolTestThreads = new List<SolovayStrassenTest>();

        private void button23_Click(object sender, EventArgs e)
        {
            if(SolovayStrassenTest.Working)
            {
                SolovayStrassenTest.Working = false;
                button23.Text = "Старт";
            }
            else
            {
                if (_Cur.IsEven)
                    _Cur++;
                sc.Clear();
                SolovayStrassenTest.Working = true;
                SolovayStrassenTest.Accuary = Convert.ToInt32(textBox32.Text);
                button23.Text = "Стоп";
                int count = Convert.ToInt32(numericUpDown2.Value);
                flowLayoutPanel1.Controls.Clear();
                _SolTestThreads.ForEach(x => x.Dispose());
                _SolTestThreads.Clear();
                LAST_XML_FILE_ID = 0;
                LastNumbers.Clear();
                foreach(var v in Directory.EnumerateFiles(Directory.GetCurrentDirectory())) {
                    var i = Path.GetFileName(v).ToInt(-1);
                    if(i > LAST_XML_FILE_ID)
                        LAST_XML_FILE_ID = i;
                }
                for(int i=0;i<count;i++)
                {
                    AddThread();
                }
            }
        }

        private void AddThread()
        {
            SolovayStrassenTest cur = new SolovayStrassenTest(_SolTestThreads.Count, GetNext, Change);
            cur.lb = new Label();
            flowLayoutPanel1.Controls.Add(cur.lb);
            cur.lb.AutoSize = true;
            cur.OnPrimeNumberFound += NewPrimeFound;
            cur.OnStatusChanged += AfterStatusChanged;
            cur.OnTestStoped += TestStoped;
            cur.Start();
            _SolTestThreads.Add(cur);
        }

        private static long LAST_XML_FILE_ID = 0;
        private static List<BigInteger> LastNumbers = new List<BigInteger>();

        private void NewPrimeFound(SolovayStrassenTest obj, BigInteger num)
        {
            Action ac = delegate()
            {
                String str;
                str = string.Format("{0}\r\n0x{1}\r\n{2}; // {3} bit\r\n\r\n", Convert.ToString(num), GetHEX(num), GetArray(num), GetRealBitCount(num));
                textBox30.Text += str;
                textBox30.SelectionStart = textBox30.Text.Length;
                textBox30.ScrollToCaret();
                AddInLog(str);
                if(checkBox6.Checked && LastNumbers.Count != 0) {
                    var rsa = new advRSA();
                    var p = num;
                    var q = LastNumbers.Where(x=>((x/p)>=3 || (p/x) >=3) && BigInteger.Abs(x-p) > (int)(2.0 * Math.Pow((p * x).GetBitCount(), 1.0/4))).FirstOrDefault();
                    if(q != default(BigInteger)) {
                        rsa.P = p;
                        rsa.Q = q;
                        rsa.E = BigInteger.Parse(textBox15.Text);
                        rsa.D = advRSA.GetD(rsa.FN, rsa.E);
                        if(rsa.E * rsa.D % rsa.FN == 1) {
                            using(StreamWriter sw = new StreamWriter(LAST_XML_FILE_ID.ToString() + ".xml", false)) {
                                sw.Write(rsa.ToXmlString(true));
                            }
                            LAST_XML_FILE_ID++;
                            label64.Text = LAST_XML_FILE_ID.ToString();
                        }
                    }
                }
                LastNumbers.Add(num);
                Application.DoEvents();
            };
            try
            {
                this.Invoke(ac);
            }
            catch(Exception)
            {

            }
        }

        private void AfterStatusChanged(SolovayStrassenTest obj)
        {
            Action ac = delegate()
            { 
                obj.lb.Text = String.Format("Поток {0}: {1}/{2} ({3} %)", obj.Id, obj.CurrentState,
                    SolovayStrassenTest.Accuary, (((double)obj.CurrentState) / ((double)SolovayStrassenTest.Accuary) * 100.0).ToString("F02"));
                BigInteger speed = (_Cur - PrevBi) / 2;
                PrevBi = _Cur;
                if (speed >= 0 && speed <= 100000)
                    sc.Add(double.Parse(Convert.ToString(speed)), DateTime.Now.Ticks);
                tsStatus.Text = String.Format("Speed: {0} NPS ", sc.Speed.ToString("F05"));
                tsError.Text = "";
                textBox31.Text = _Cur.ToString();
                label43.Text = String.Format("Size: {0} bit\tReal: {1}", _Cur.GetBitCount(), _Cur.GetRealBitCount());
            };
            try
            {
                this.Invoke(ac);
            }
            catch (Exception)
            {

            }
        }

        private void TestStoped(SolovayStrassenTest obj)
        {
            try {
                Action ac = delegate() { obj.lb.Text = String.Format("Поток {0}: Stoped", obj.Id); };
                this.Invoke(ac);
            }
            catch (Exception ex) {

            }
        }

        private void textBox32_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            if(int.TryParse(textBox32.Text, out i))
            {
                SolovayStrassenTest.Accuary = i;
                label46.Text = SolovayStrassenTest.GetAccuary(i).ToString();
            }
            else
            {
                label46.Text = "АХЗ";
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (!SolovayStrassenTest.Working)
                return;
            if(numericUpDown2.Value>_SolTestThreads.Count)
            {
                AddThread();
            }
            else if(numericUpDown2.Value<_SolTestThreads.Count)
            {
                SolovayStrassenTest buf = _SolTestThreads[_SolTestThreads.Count-1];
                flowLayoutPanel1.Controls.Remove(buf.lb);
                buf.Dispose();
                _SolTestThreads.Remove(buf);
            }
        }

        //                             F I L E   S T R U C T U R E
        //  __________________________________________________________________________________
        // |      field      |  length  |               comments                              |
        // |-----------------|----------|-----------------------------------------------------|
        // |     version     |    8     |                                                     |
        // |  saved datetime |    8     | count of seconds since start of the UNIX generation |
        // |     reserved    |    32    |         reserved must be filled with zero           |
        // |      length     |    4     |          length of saved number in bytes            |
        // |      data       |   *A3X*  |   length depends of value and must atleast 1 byte   |

        private void button24_Click(object sender, EventArgs e) {
            var dialog = new SaveFileDialog();
            dialog.CheckPathExists = true;
            dialog.Filter = "Binary file|*.bin";
            dialog.FilterIndex = 0;
            dialog.Title = "Сохранить как";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            try {
                using (var fs = new FileStream(dialog.FileName, FileMode.Create)) {
                    byte[] buf = BitConverter.GetBytes(Program.Version);
                    fs.Write(buf, 0, buf.Length);
                    buf = BitConverter.GetBytes(DateTime.Now.Ticks/TimeSpan.TicksPerSecond);
                    fs.Write(buf, 0, buf.Length);
                    for (int i = 0; i < 32; fs.WriteByte(0),i++) ;
                    var num = _Cur;
                    buf = BitConverter.GetBytes(num.GetBytesCount());
                    fs.Write(buf, 0, buf.Length);
                    buf = num.ToByteArray();
                    fs.Write(buf, 0, buf.Length);
                    fs.Close();
                }
            }
            catch (UnauthorizedAccessException ex) {
                MessageBox.Show("Нет доступа к файлу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) {
                MessageBox.Show("Во время сохранения возникла неизвестная ошибка.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button25_Click(object sender, EventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.CheckPathExists = true;
            dialog.Filter = "Binary file|*.bin";
            dialog.FilterIndex = 0;
            dialog.Multiselect = false;
            dialog.CheckFileExists = true;
            dialog.Title = "Выберите файл для загрузки";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            try {
                Int64 ticks;
                double version;
                using (var fs = new FileStream(dialog.FileName, FileMode.Open)) {
                    byte[] bt = new byte[32];
                    fs.Read(bt, 0, 8);
                    version = BitConverter.ToDouble(bt, 0);
                    fs.Read(bt, 0, 8);
                    ticks = BitConverter.ToInt64(bt, 0);
                    fs.Read(bt, 0, 32);
                    fs.Read(bt, 0, 4);
                    int len = BitConverter.ToInt32(bt, 0);
                    bt = new byte[len];
                    fs.Read(bt, 0, len);
                    var msg = string.Format("Вы уверены что хотите загрузить число из этого файла?\r\n\r\nФайл создан: {0}\r\nВерсией программы: {1}\r\nДлинна числа: {2} байт ({3} бит)",
                        new DateTime(ticks*TimeSpan.TicksPerSecond), version, len, len*8);
                    if(MessageBox.Show(msg, "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
                        Cur = new BigInteger(bt);
                }
            }
            catch (UnauthorizedAccessException ex) {
                MessageBox.Show("Нет доступа к файлу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FileNotFoundException ex) {
                MessageBox.Show("Выбранный файл не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) {
                MessageBox.Show("Во время сохранения возникла неизвестная ошибка.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox33_TextChanged(object sender, EventArgs e) {
            try {
                Cur = BigInteger.Parse(textBox33.Text);
            }
            catch (Exception ex) {
                Cur = 0;
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e) {
            label52.Text = string.Format("({0} байт(а,ов))", Convert.ToInt32(numericUpDown3.Value) >> 3);
            var num = BigInteger.Pow(2, Convert.ToInt32(numericUpDown3.Value)) - 1;
            label53.Text = string.Format("MAX: {0}", num.ToString());
            label55.Text = string.Format("Размер файла: {0}", AbvMathLib.Factorization.QS.GetPrimesCount(num).ToString().ToStringWithBytesCount());
        }


        private struct DataBaseInfo{
            public static BigInteger Max, Cur;
            public static FileStream fs;
            public static int lenByte;
            public static bool Working=false;
            public static void Done() {
                Working = false;
                Max = 0;
                Cur = 0;
                if(fs!=null)
                    fs.Close();
                fs = null;
            }
        };

        event Action StatusChanged;
        event Action Done;

        private void NewThread() {
            DataBaseInfo.Working = true;
            List<BigInteger> primes = FermatsTest.GetPrimes(10);
            byte[] buf = new byte[DataBaseInfo.lenByte];
            try {
                while (DataBaseInfo.Working && DataBaseInfo.Cur <= DataBaseInfo.Max) {
                    if (FermatsTest.IsPrime(DataBaseInfo.Cur, primes)) {
                        Array.Clear(buf, 0, buf.Length);
                        var cur = DataBaseInfo.Cur.ToByteArray();
                        Array.Copy(cur, buf, Math.Min(DataBaseInfo.lenByte, cur.Length));
                        DataBaseInfo.fs.Lock(0, long.MaxValue);
                        DataBaseInfo.fs.Write(buf, 0, buf.Length);
                        DataBaseInfo.fs.Unlock(0, long.MaxValue);
                    }
                    DataBaseInfo.Cur += 2;
                    StatusChanged();
                }
                DataBaseInfo.Done();
                Done();
            }
            catch (Exception) {

            }
        }

        private void button33_Click(object sender, EventArgs e) {
            if (DataBaseInfo.Working) {
                this.StatusChanged -= Form1_StatusChanged;
                this.Done -= Form1_Done;
                DataBaseInfo.Done();
                button33.Text = "Старт";
            }
            else {
                try {
                    tsError.Text = "";
                    tsError.ForeColor = Color.Black;
                    DataBaseInfo.Cur = 1;
                    DataBaseInfo.lenByte = Convert.ToInt32(numericUpDown3.Value) >> 3;
                    DataBaseInfo.Max = BigInteger.Pow(2, Convert.ToInt32(numericUpDown3.Value)) - 1;
                    DataBaseInfo.fs = new FileStream(textBox34.Text, FileMode.Create);
                    this.StatusChanged += Form1_StatusChanged;
                    this.Done += Form1_Done;
                    (new Thread(NewThread)).Start();
                    button33.Text = "Стоп";
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Ошибко", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    button33.Text = "Старт";
                }
            }

        }

        void Form1_Done() {
            Action ac = delegate() {
                MessageBox.Show("База создана!");
            };
            this.Invoke(ac);
        }

        private void Form1_StatusChanged() {
            try {
                Action ac = delegate() {
                    label56.Text = string.Format("Осталось итераций: {0}", (DataBaseInfo.Max-DataBaseInfo.Cur).ToString());
                };
                this.Invoke(ac);
            }
            catch (Exception ex) {
                DataBaseInfo.Done();
            }
        }

        private void textBox35_TextChanged(object sender, EventArgs e) {
            try {
                BigInteger bi = BigInteger.Parse(textBox35.Text);
                label58.Text = string.Format("Бит: {0} Байт: {1} L(n): {2}", bi.GetBitCount(), bi.GetBytesCount(), AbvMath.GetLN(bi));
            }
            catch(Exception) { }
        }

        Thread Factor = null;
        QS qs = null;

        private void button34_Click(object sender, EventArgs e) {
            if (Factor == null) {
                Factor = new Thread(FactTh);
                timer6.Enabled = true;
                Factor.Start();
            }
            else {
                Factor.Abort();
                timer6.Enabled = false;
            }
        }

        private void FactTh() {
            try {
                List<BigInteger> res = null;
                if (checkBox5.Checked) {
                    qs = new QS();
                    DateTime dt = DateTime.Now;
                    res = QS.GetAllPrimeDivisors(BigInteger.Parse(textBox35.Text), qs).ToList();
                    Action ac = delegate()
                    {
                        label63.Text = String.Join(";", res.Select(x => x.ToString()));
                        tsError.Text = "";
                        tsStatus.Text = string.Format("Factorized in {0}", (DateTime.Now.AddTicks(-dt.Ticks)).ToString("HH:mm:ss.fffff"));
                    };
                    this.Invoke(ac);
                }
                else {
                    qs = new QS();
                    qs.A = BigInteger.Parse(textBox36.Text);
                    qs.N = BigInteger.Parse(textBox35.Text);
                    if (checkBox4.Checked)
                        qs.P = (int)(AbvMath.GetLN(qs.N) * 0.05 + 0.5);
                    else
                        qs.P = BigInteger.Parse(textBox37.Text);
                    qs.BruteP = checkBox4.Checked;
                    if (checkBox4.Checked) {
                        var bs = qs.GetFactorBase();
                        do {
                            qs.P = qs.GetFactorBaseNext(bs)+1;
                            if (qs.P >= qs.A)
                                throw new ArgumentException();
                            Action acc = delegate() { textBox37.Text = qs.P.ToString(); };
                            this.Invoke(acc);
                            try {
                                res = qs.GetPrimeDivisors(bs);
                            }
                            catch (Exception) {

                            }
                        } while (res == null || res.Any(x => x.IsOne));
                    }
                    else
                        res = qs.GetPrimeDivisors();
                    Action ac = delegate() { label63.Text = String.Join(";", res.Select(x => x.ToString())) + string.Format(" P:{0}", qs.P); tsError.Text = "";tsStatus.Text = "";};
                    this.Invoke(ac);
                }
                Factor = null;
                qs = null;
            }
            catch (Exception ex) {
                Action ac = delegate() { tsError.Text = ex.GetType().ToString(); tsStatus.Text = ""; };
                this.Invoke(ac);
                Factor = null;
                qs = null;
            }
        }

        private void timer6_Tick(object sender, EventArgs e) {
            if (qs != null)
                tsStatus.Text = String.Format("N={2} L(N)={4} A={5} P={3} {0} ({1}%)", qs.CurrentOperation.ToString(), (((double)qs.IterationsComleted) / ((double)qs.IterationsTotal) * 100.00).ToString("F03"), qs.N, qs.P, AbvMath.GetLN(qs.N).ToString("F01"), qs.A);
            else
                timer6.Enabled = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e) {
            textBox37.Enabled = !checkBox4.Checked;
        }

        private void button35_Click(object sender, EventArgs e) {
            BigInteger P, Q, E, D;
            P = BigInteger.Parse(textBox11.Text);
            Q = BigInteger.Parse(textBox12.Text);
            E = BigInteger.Parse(textBox15.Text);
            D = BigInteger.Parse(textBox20.Text);
            UpdateLenForAll();
            SaveFileDialog sfd = new SaveFileDialog() {
                AddExtension = false,
                AutoUpgradeEnabled = true,
                CheckFileExists = false,
                CheckPathExists = true,
                CreatePrompt = false,
                Filter = "xml|*.xml",
                FilterIndex = 0,
                OverwritePrompt = true
            };
            if(sfd.ShowDialog() == DialogResult.Cancel)
                return;
            string FileName = sfd.FileName;
            if(FileName.EndsWith(".xml"))
                FileName = FileName.Substring(0, FileName.Length - 4);
            //var rsa = System.Security.Cryptography.RSA.Create();
            //System.Security.Cryptography.RSAParameters Params;
            //Params.P = P.ToByteArray();
            //Params.Q = Q.ToByteArray();
            //Params.Exponent = E.ToByteArray();
            //Params.Modulus = (P * Q).ToByteArray();
            //Params.D = D.ToByteArray();
            var rsa = RSA.Instance(P, Q, E, D);
            using(StreamWriter sw = new StreamWriter(FileName + "(private).xml"))
                sw.Write(rsa.ToXmlString(true));
            using(StreamWriter sw = new StreamWriter(FileName + "(public).xml"))
                sw.Write(rsa.ToXmlString(false));
        }

        private void button36_Click(object sender, EventArgs e) {
            OpenFileDialog sfd = new OpenFileDialog() {
                AddExtension = false,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "xml|*.xml",
                FilterIndex = 0
            };
            if(sfd.ShowDialog() == DialogResult.Cancel)
                return;
            RSA rsa = new RSA();
            using(StreamReader sr = new StreamReader(sfd.FileName))
                rsa.FromXmlString(sr.ReadToEnd());
            textBox11.Text = rsa.P.ToString();
            textBox12.Text = rsa.Q.ToString();
            textBox15.Text = rsa.E.ToString();
            textBox20.Text = rsa.D.ToString();
            button22_Click(sender, e);
            button16_Click(sender, e);
            UpdateLenForAll();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e) {
            panel1.Enabled = !checkBox5.Checked;
        }
    }
}
