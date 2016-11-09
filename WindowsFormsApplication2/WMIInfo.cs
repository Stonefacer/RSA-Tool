using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Threading;

namespace WMI
{

    public static class AsyncBlock
    {
        public class QueryElement
        {
            public Object res;
            CallBack cb;
            Job<Object> jb;
            public QueryElement(Job<Object> _jb, CallBack _cb = null)
            {
                jb = _jb;
                cb = _cb;
                res = "";
            }

            public void DoJob()
            {
                res = jb();
            }

            public void Done()
            {
                if (cb != null)
                {
                    cb(this);
                }
            }
        };


        public delegate void CallBack(QueryElement qe);
        public delegate RetValue Job<RetValue>();
        /*public class QueryElement
        {
            public String Property, ClassName, dir, res;
            CallBack cb;
            QueryElement(String Class, String Property, String _dir=@"root\cimv2", CallBack _cb=null)
            {
                this.Property = Property;
                this.ClassName = Class;
                dir = _dir;
                cb = _cb;
                res = "";
            }

            public void Done()
            {
                if(cb!=null)
                {
                    cb();
                }
            }
        }*/

        public static Queue<QueryElement> WaitingQueue, DoneQueue;

        private static Thread thParser;
        private static bool ParserWork;

        static AsyncBlock()
        {
            thParser = new Thread(Parser);
            ParserWork = true;
            thParser.Start();
            WaitingQueue = new Queue<QueryElement>();
            DoneQueue = new Queue<QueryElement>();
        }

        public static void Stop()
        {
            ParserWork = false;
        }

        public static void Add(QueryElement qe)
        {
            WaitingQueue.Enqueue(qe);
        }

        private static void Parser()
        {
            QueryElement qe;
            while (ParserWork)
            {
                while (WaitingQueue.Count != 0)
                {
                    qe = WaitingQueue.Dequeue();
                    qe.DoJob();
                    DoneQueue.Enqueue(qe);
                    qe = null;
                }
                Thread.Sleep(50);
            }
        }

        public static bool HasNext()
        {
            return DoneQueue.Count != 0;
        }

        public static QueryElement GetCurrent()
        {
            return DoneQueue.Dequeue();
        }
    }

    public static class WMIInfo
    {
        private static String le, ltn; // properties
        public static String LastError
        {
            get
            {
                return le;
            }
            private set
            {
                if (value == "")
                    le = "<unknown>";
                else
                    le = value;
            }
        }
        public static String LastTypeName
        {
            get
            {
                return ltn;
            }
            private set
            {
                if (value == "")
                    ltn = "<unknown>";
                else
                    ltn = value;
            }
        }

        private static ManagementObjectSearcher MainSeacher;
        static WMIInfo()
        {
            MainSeacher = new ManagementObjectSearcher();
            LastError = "";
            LastTypeName = "";
        }

        public static class ManualMethods
        {
            public static ManagementObjectCollection GetAnswer(String dir, String Query)
            {
                MainSeacher.Scope = new ManagementScope(dir);
                MainSeacher.Query = new ObjectQuery(Query);
                return MainSeacher.Get();
            }

            public static Templ GetProperty<Templ>(String ClassName, String Property, Converter<Object, Templ> Сonv, Templ IfNotExist = default(Templ), String dir = @"root\cimv2")
            {
                try
                {
                    var em = GetAnswer(dir, String.Format("SELECT {0} FROM {1}", Property, ClassName)).GetEnumerator();
                    em.MoveNext();
                    Object obj = em.Current.Properties[Property].Value;
                    LastTypeName = obj.GetType().Name;
                    return Сonv(obj);
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                }
                return IfNotExist;
            }

            public static String GetPropertyString(String ClassName, String Property, String dir = @"root\cimv2")
            {
                try
                {
                    var em = GetAnswer(dir, String.Format("SELECT {0} FROM {1}", Property, ClassName)).GetEnumerator();
                    em.MoveNext();
                    return em.Current.Properties[Property].Value.ToString();
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                }
                return "";
            }
        }

        public static class CPU
        {
            public static double GetTemperature()
            {
                uint i = WMIInfo.ManualMethods.GetProperty<UInt32>("MSAcpi_ThermalZoneTemperature", "CurrentTemperature", Convert.ToUInt32, 0, @"root\WMI");
                if (i == 0)
                    return (double)-2.00;
                return Math.Abs((double)i - 2732.00) / 10.00;
            }

            public static String GetName()
            {
                return WMIInfo.ManualMethods.GetPropertyString("Win32_Processor", "Name");
            }

            public static int GetClockSpeed()
            {
                return WMIInfo.ManualMethods.GetProperty<int>("Win32_Processor", "CurrentClockSpeed", Convert.ToInt32, 0);
            }

            public static int GetLoad()
            {
                return WMIInfo.ManualMethods.GetProperty<int>("Win32_PerfFormattedData_Counters_ProcessorInformation", "PercentProcessorTime", Convert.ToInt32, 0);
            }


            //ROOT\CIMV2
            //SELECT * FROM Win32_Processor
            //NumberOfLogicalProcessors
            public static int GetNumberOfLogicalPocessors()
            {
                return WMIInfo.ManualMethods.GetProperty<int>("Win32_Processor", "NumberOfLogicalProcessors", Convert.ToInt32, 1);
            }
        }
    }
}
