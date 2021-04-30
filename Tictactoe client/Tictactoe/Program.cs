using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;
using System.Threading;

namespace Tictactoe
{
    static class Program
    {
        [STAThread]
        public static bool IsRunning(this Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            try
            {
                Process.GetProcessById(process.Id);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           
            Application.Run(new game("Henry"));
        }
    }

}