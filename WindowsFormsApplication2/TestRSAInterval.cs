using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;

namespace WindowsFormsApplication2
{
    public partial class TestRSAInterval : Form
    {
        private RSA test;
        private BigInteger min, max, Current;
        private double Total
        {
            get
            {
                return 0;
            }
            set
            {
                lbTotal.Text = value.ToString("F02")+" %";
            }
        }

        private int _Fails;
        private int Fails
        {
            get
            {
                return _Fails;
            }
            set
            {
                lbFails.Text = String.Format("Fails ({0}):", value);
                _Fails = value;
            }
        }

        private Queue<BigInteger> Failures;
        private bool bl;

        public TestRSAInterval(RSA ts, BigInteger min, BigInteger max)
        {
            test = ts;
            this.min = min;
            this.max = max;
            bl = true;
            InitializeComponent();
        }

        private void TestRSAInterval_Load(object sender, EventArgs e)
        {
            tbFrom.Text = Convert.ToString(min);
            tbTo.Text = Convert.ToString(max);
            tbN.Text = Convert.ToString(test.N);
            tbE.Text = Convert.ToString(test.E);
            tbD.Text = Convert.ToString(test.D);
            tbCur.Text = Convert.ToString(min);
            tbPocessed.Text = "0";
            lbTotal.Text = "0 %";
            Fails = 0;
            Failures = new Queue<BigInteger>();
            (new System.Threading.Thread(ThTest)).Start();
            timer1.Start();
        }

        private void ThTest()
        {
            for (Current = min; (Current <= max)&&bl;Current++)
            {
                if (test.Decrypt(test.Encrypt(Current)) != Current)
                    Failures.Enqueue(Current);
            }
        }

        private void AddInLog(String Format, params object[] obj)
        {
            tbLog.Text += String.Format(Format, obj);
            tbLog.SelectionStart = tbLog.Text.Length;
            tbLog.ScrollToCaret();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Total = Double.Parse(Convert.ToString((Current-min)*10000/(max+1)))/100.00;
                tbCur.Text = Convert.ToString(Current);
                if (Failures.Count > 0)
                {
                    AddInLog("FAILED: {0}\r\n", Convert.ToString(Failures.Dequeue()));
                    Fails++;
                }
            }
            catch(Exception ex)
            {
                AddInLog("TIMER EXCEPTION: {0}\r\n", ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bl = false;
        }

        private void TestRSAInterval_FormClosing(object sender, FormClosingEventArgs e)
        {
            bl = false;
        }
    }
}
