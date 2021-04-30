using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevelopApp
{
    public partial class LogonForm : Form
    {
        public LogonForm()
        {
            InitializeComponent();
        }

        private void LogInButton_Click(object sender, EventArgs e)
        {
            if (LoginText.Text == "Login" || PasswordText.Text == "Password" || serverIPText.Text == "Server IP")
            {
                MessageBox.Show("You need to enter both login and password.", "Error");
            }
            else
            {
                try
                {
                    Program.client = new Client(serverIPText.Text);
                    var serverKey = Communication.KeyExchange();
                    if (serverKey != null)
                    {
                        var sessKey = Program.security.SetSessionKey(serverKey);

                        if (Communication.LogIn(LoginText.Text, PasswordText.Text, sessKey) == true)
                        {
                            Program.userLogin = LoginText.Text;
                            Program.serverAddress = serverIPText.Text;
                            DialogResult = DialogResult.Yes;
                            Program.sessionKeyWithServer = sessKey;
                            this.Hide();
                            var form2 = new Form1();
                            form2.FormClosed += new FormClosedEventHandler(f_FormClosed);
                            form2.Closed += (s, args) => this.Close();
                            form2.Show();
                        }
                        else
                        {
                            MessageBox.Show("Username or password incorrect.", "Error");
                        }
                    }
                    else
                    {
                        Program.client.Disconnect();
                        MessageBox.Show("Server connection attempt has failed.", "Error");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Server connection attempt has failed.", "Error");
                }
            }
        }

        private void f_FormClosed(object sender, EventArgs e)
        {
            Program.client = new Client(Program.serverAddress);
            Communication.LogOut(Program.userLogin);
            Program.client.Disconnect();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new RegisterForm();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }
    }
}
