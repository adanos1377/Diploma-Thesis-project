using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevelopApp
{
    static class Program
    {
        public static Client client;
        public static Security security = new Security();
        public static string userLogin;
        public static string serverAddress;
        public static byte[] sessionKeyWithServer = null;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LogonForm());
        }
    }
}
