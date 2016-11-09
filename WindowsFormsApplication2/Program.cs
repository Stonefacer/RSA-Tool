using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    static class Program
    {
        public const double Version = 0.001;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
