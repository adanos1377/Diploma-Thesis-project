using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevelopApp
{
    class Communication
    {
        static bool Response(char answer)
        {
            if (answer == (char)5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Register(string login, string password, byte[] key)
        {
            char comm = (char)0;
            string message = comm + " " + Program.security.EncryptMessage(key, login + " " + password) + " <EOF>";

            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static bool LogIn(string login, string password, byte[] key)
        {
            char comm = (char)1;
            string message = comm + " " + Program.security.EncryptMessage(key, login + " " + password) + " <EOF>";

            Program.client.Send(message);

            return Response(Program.client.Receive()[0]);
        }

        public static void LogOut(string login)
        {
            char comm = (char)2;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer, login) + " <EOF>";

            Program.client.Send(message);
            return;
        }

        public static bool AccDel(string login, string password)
        {
            char comm = (char)3;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                login + " " + password) + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static bool PassChng(string login, string oldPassword, string newPassword)
        {
            char comm = (char)4;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                login + " " + oldPassword + " " + newPassword) + " <EOF>";
            Program.client.Send(message);
            var ans = Program.client.Receive();
            return Response(ans[0]);
        }

        // Leave Base64 encoding here
        public static byte[] KeyExchange()
        {
            var byteArray = Program.security.GetOwnerPublicKey().ToByteArray();

            string message = (char)17 + " " + Convert.ToBase64String(byteArray) + " <EOF>";
            Program.client.Send(message);
            message = Program.client.Receive();
            return Convert.FromBase64String(message.Substring(2, message.Length - 8));
        }
    }
}
