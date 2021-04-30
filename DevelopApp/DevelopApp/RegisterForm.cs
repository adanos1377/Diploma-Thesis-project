using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DevelopApp
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new LogonForm();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            if (LoginText.Text == "Login" || PasswordText.Text == "Password" || RepeatPasswordText.Text == "Password" || serverIPText.Text == "Server IP")
            {
                MessageBox.Show("You need to fill all the fields.", "Error");
            }
            else if (PasswordText.Text != RepeatPasswordText.Text)
            {
                MessageBox.Show("The passwords don't match.", "Error");
            }
            else if (LoginText.Text.Length > 16)
            {
                MessageBox.Show("The username is too long, it has to be 16 characters or less.", "Error");
            }
            else
            {
                // Password must contain:
                // Minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character
                Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}$");

                Match match = regex.Match(PasswordText.Text);
                if (match.Success)
                {
                    try
                    {
                        Program.client = new Client(serverIPText.Text);
                        var serverKey = Communication.KeyExchange();
                        if (serverKey != null)
                        {
                            var sessKey = Program.security.SetSessionKey(serverKey);
                            if (Communication.Register(LoginText.Text, PasswordText.Text, sessKey) == true)
                            {
                                MessageBox.Show("User registration complete.", "Success");
                                DialogResult = DialogResult.No;
                                this.Hide();
                                var form2 = new LogonForm();
                                form2.Closed += (s, args) => this.Close();
                                form2.Show();
                            }
                            else
                            {
                                MessageBox.Show("The username is taken already, please enter another one.", "Error");
                                Program.client.Disconnect();
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
                else
                {
                    MessageBox.Show("The password must contain minimum eight characters," +
                        " at least one uppercase letter, one lowercase letter, one number and one special character.", "Error");
                }

            }
        }
    }
}
