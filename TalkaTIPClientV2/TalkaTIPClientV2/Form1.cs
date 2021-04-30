using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TalkaTIPClientV2
{
    public partial class game : Form
    {
        //string ServerIP;
        //int ServerPort;
        //string LobbyID;
        APIHandle.Server server;
        private HttpClient httpClient = new HttpClient();
        private string ChoosedPlayer = "None";
        private string PlayerNickName;
        private static Pen pen;
        private static Graphics g;
        private static string[,] cell = new string[9,2];
        private static string currentplayer = "X";
        private static Random r = new Random();
        private static TcpClient client = new TcpClient();
        int inandout = 0;
        Thread t;
        int MyStep = 0;

        public int CheckVictory()
        {
            
            //CHECK X
            if (cell[0, 1] == "X" && cell[1, 1] == "X" && cell[2, 1] == "X")
            {
                return 1;
            }
            if (cell[3, 1] == "X" && cell[4, 1] == "X" && cell[5, 1] == "X")
            {
                return 1;
            }
            if (cell[6, 1] == "X" && cell[7, 1] == "X" && cell[8, 1] == "X")
            {
                return 1;
            }
            if (cell[0, 1] == "X" && cell[3, 1] == "X" && cell[6, 1] == "X")
            {
                return 1;
            }
            if (cell[1, 1] == "X" && cell[4, 1] == "X" && cell[7, 1] == "X")
            {
                return 1;
            }
            if (cell[2, 1] == "X" && cell[5, 1] == "X" && cell[8, 1] == "X")
            {
                return 1;
            }
            if (cell[0, 1] == "X" && cell[4, 1] == "X" && cell[8, 1] == "X")
            {
                return 1;
            }
            if (cell[2, 1] == "X" && cell[4, 1] == "X" && cell[6, 1] == "X")
            {
                return 1;
            }
            //CHECK 0
            if (cell[0, 1] == "0" && cell[1, 1] == "0" && cell[2, 1] == "0")
            {
                return 2;
            }
            if (cell[3, 1] == "0" && cell[4, 1] == "0" && cell[5, 1] == "0")
            {
                return 2;
            }
            if (cell[6, 1] == "0" && cell[7, 1] == "0" && cell[8, 1] == "0")
            {
                return 2;
            }
            if (cell[0, 1] == "0" && cell[3, 1] == "0" && cell[6, 1] == "0")
            {
                return 2;
            }
            if (cell[1, 1] == "0" && cell[4, 1] == "0" && cell[7, 1] == "0")
            {
                return 2;
            }
            if (cell[2, 1] == "0" && cell[5, 1] == "0" && cell[8, 1] == "0")
            {
                return 2;
            }
            if (cell[0, 1] == "0" && cell[4, 1] == "0" && cell[8, 1] == "0")
            {
                return 2;
            }
            if (cell[2, 1] == "0" && cell[4, 1] == "0" && cell[6, 1] == "0")
            {
                return 2;
            }

            //CHECK TIE
            int db = 0;
            for (int i = 0; i < 9; i++)
            {
                if (cell[i, 0] == "1") db++;
            }
            if (db == 9)
            {
                return 0;
            }



            return 3;
        }
        private void DrawO(Pen pen, Graphics g, int cells)
        {
            pen = new Pen(Color.Red, 4);
            pen.Brush = new HatchBrush(HatchStyle.OutlinedDiamond, Color.Blue);
            g = panel1.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            switch (cells)
            {
                case 1:
                    if (cell[0, 0] != "1")
                    {
                        g.DrawEllipse(pen,0,0,98,78);
                        cell[0, 0] = "1";
                        cell[0, 1] = "0";
                        Cplayer.Text = "X";
                        currentplayer = "X";
                    }
                    break;
                case 2:
                    if (cell[1, 0] != "1")
                    {
                        g.DrawEllipse(pen, 100, 0, 98, 78);
                        cell[1, 0] = "1";
                        cell[1, 1] = "0";
                        Cplayer.Text = "X";
                        currentplayer = "X";
                    }
                    break;
                case 3:
                    if (cell[2, 0] != "1")
                    {
                        g.DrawEllipse(pen, 200, 0, 98, 78);
                        cell[2, 0] = "1";
                        cell[2, 1] = "0";
                        Cplayer.Text = "X";
                        currentplayer = "X";
                    }
                    break;
                case 4:
                    if (cell[3, 0] != "1")
                    {
                        g.DrawEllipse(pen, 0, 80, 98, 78);
                        cell[3, 0] = "1";
                        cell[3, 1] = "0";
                        Cplayer.Text = "X";
                        currentplayer = "X";
                    }
                    break;
                case 5:
                    if (cell[4, 0] != "1")
                    {
                        g.DrawEllipse(pen, 100, 80, 98, 78);
                        cell[4, 0] = "1";
                        cell[4, 1] = "0";
                        Cplayer.Text = "X";
                        currentplayer = "X";
                    }
                    break;
                case 6:
                    if (cell[5, 0] != "1")
                    {
                        g.DrawEllipse(pen, 200, 80, 98, 78);
                        cell[5, 0] = "1";
                        cell[5, 1] = "0";
                        Cplayer.Text = "X";
                        currentplayer = "X";
                    }
                    break;
                case 7:
                    if (cell[6, 0] != "1")
                    {
                        g.DrawEllipse(pen, 0, 160, 98, 78);
                        cell[6, 0] = "1";
                        cell[6, 1] = "0";
                        Cplayer.Text = "X";
                        currentplayer = "X";
                    }
                    break;
                case 8:
                    if (cell[7, 0] != "1")
                    {
                        g.DrawEllipse(pen, 100, 160, 98, 78);
                        cell[7, 0] = "1";
                        cell[7, 1] = "0";
                        Cplayer.Text = "X";
                        currentplayer = "X";
                    }
                    break;
                case 9:
                    if (cell[8, 0] != "1")
                    {
                        g.DrawEllipse(pen, 200, 160, 98, 78);
                        cell[8, 0] = "1";
                        cell[8, 1] = "0";
                        Cplayer.Text = "X";
                        currentplayer = "X";
                    }
                    break;
                default:
                    break;
            }
        }
        private void DrawX(Pen pen,Graphics g,int cells)
        {
            pen = new Pen(Color.DarkOrange, 4);
            pen.Brush = new HatchBrush(HatchStyle.Cross,Color.Chocolate);
            g = panel1.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            switch (cells)
            {
                case 1:
                    if (cell[0, 0] != "1")
                    {
                        g.DrawLine(pen, 0, 0, 100, 80);
                        g.DrawLine(pen, 100, 0, 0, 80);
                        cell[0, 0] = "1";
                        cell[0, 1] = "X";
                        currentplayer = "0";
                        Cplayer.Text = "Circle";
                    }
                    break;
                case 2:
                    if (cell[1, 0] != "1")
                    {
                        g.DrawLine(pen, 100, 0, 200, 80);
                        g.DrawLine(pen, 200, 0, 100, 80);
                        cell[1, 0] = "1";
                        cell[1, 1] = "X";
                        currentplayer = "0";
                        Cplayer.Text = "Circle";
                    }
                    break;
                case 3:
                    if (cell[2, 0] != "1")
                    {
                        g.DrawLine(pen, 200, 0, 300, 80);
                        g.DrawLine(pen, 300, 0, 200, 80);
                        cell[2, 0] = "1";
                        cell[2, 1] = "X";
                        currentplayer = "0";
                        Cplayer.Text = "Circle";
                    }
                    break;
                case 4:
                    if (cell[3, 0] != "1")
                    {
                        g.DrawLine(pen, 0, 80, 100, 160);
                        g.DrawLine(pen, 100, 80, 0, 160);
                        cell[3, 0] = "1";
                        cell[3, 1] = "X";
                        currentplayer = "0";
                        Cplayer.Text = "Circle";
                    }
                    break;
                case 5:
                    if (cell[4, 0] != "1")
                    {
                        g.DrawLine(pen, 100, 80, 200, 160);
                        g.DrawLine(pen, 200, 80, 100, 160);
                        cell[4, 0] = "1";
                        cell[4, 1] = "X";
                        currentplayer = "0";
                        Cplayer.Text = "Circle";
                    }
                    break;
                case 6:
                    if (cell[5, 0] != "1")
                    {
                        g.DrawLine(pen, 200, 80, 300, 160);
                        g.DrawLine(pen, 300, 80, 200, 160);
                        cell[5, 0] = "1";
                        cell[5, 1] = "X";
                        currentplayer = "0";
                        Cplayer.Text = "Circle";
                    }
                    break;
                case 7:
                    if (cell[6, 0] != "1")
                    {
                        g.DrawLine(pen, 0, 160, 100, 240);
                        g.DrawLine(pen, 100, 160, 0, 240);
                        cell[6, 0] = "1";
                        cell[6, 1] = "X";
                        currentplayer = "0";
                        Cplayer.Text = "Circle";
                    }
                    break;
                case 8:
                    if (cell[7, 0] != "1")
                    {
                        g.DrawLine(pen, 100, 160, 200, 240);
                        g.DrawLine(pen, 200, 160, 100, 240);
                        cell[7, 0] = "1";
                        cell[7, 1] = "X";
                        currentplayer = "0";
                        Cplayer.Text = "Circle";
                    }
                    break;
                case 9:
                    if (cell[8, 0] != "1")
                    {
                        g.DrawLine(pen, 200, 160, 300, 240);
                        g.DrawLine(pen, 300, 160, 200, 240);
                        cell[8, 0] = "1";
                        cell[8, 1] = "X";
                        currentplayer = "0";
                        Cplayer.Text = "Circle";
                    }
                    break;
                default:
                break;
            }
          
        }

        private void NewGame()
        {
            panel1.Invalidate();
            for (int i = 0; i < 9; i++)
            {
                cell[i, 0] = "0";
                cell[i, 1] = null;
            }
        }
        
        public game(string name)
        {
            InitializeComponent();
            PlayerNickName = name;
            //textBox1.Text
            Player1.Text = PlayerNickName;
            Play.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            pen = new Pen(Color.Black);
            g = panel1.CreateGraphics();
            for (int i = 0; i < 9; i++)
            {
                cell[i, 0] = null;
            }
            g.DrawLine(pen, 100, 0, 100, 250);
            g.DrawLine(pen, 200, 0, 200, 250);
            g.DrawLine(pen, 0, 80, 300, 80);
            g.DrawLine(pen, 0, 160, 300, 160);
            pen.Dispose();
            g.Dispose();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {

            bool clicked = false;
            if (currentplayer == ChoosedPlayer && e.X > 0 && e.Y > 0 && e.X < 100 && e.Y < 80)
            {
                if (ChoosedPlayer == "X")
                {
                    DrawX(pen, g, 1);
                    MyStep = 1;
                }
                else
                {
                    DrawO(pen, g, 1);
                    MyStep = 1;
                }
                
                clicked = true;
            }
            else if (currentplayer == ChoosedPlayer && e.X > 0 && e.Y > 0 && e.X < 100 && e.Y < 80 && cell[0, 0] == "1")
            {
                clicked = true;
                return;
            }
            if (currentplayer == ChoosedPlayer && e.X > 100 && e.Y > 0 && e.X < 200 && e.Y < 80)
            {
                clicked = true;
                if (ChoosedPlayer == "X")
                {
                    DrawX(pen, g, 2);
                    MyStep = 2;
                }
                else
                {
                    DrawO(pen, g, 2);
                    MyStep = 2;
                }
            }
            else if (currentplayer == ChoosedPlayer && e.X > 100 && e.Y > 0 && e.X < 200 && e.Y < 80 && cell[1, 0] == "1")
            {
                clicked = true;
                return;
            }
            if (currentplayer == ChoosedPlayer && e.X > 200 && e.Y > 0 && e.X < 300 && e.Y < 80)
            {
                clicked = true;
                if (ChoosedPlayer == "X")
                {
                    DrawX(pen, g, 3);
                    MyStep = 3;
                }
                else
                {
                    DrawO(pen, g, 3);
                    MyStep = 3;
                }
            }
            else if (currentplayer == ChoosedPlayer && e.X > 200 && e.Y > 0 && e.X < 300 && e.Y < 80 && cell[2, 0] == "1")
            {
                clicked = true;
                return;
            }
            if (currentplayer == ChoosedPlayer && e.X > 0 && e.Y > 80 && e.X < 100 && e.Y < 160)
            {
                clicked = true;
                if (ChoosedPlayer == "X")
                {
                    DrawX(pen, g, 4);
                    MyStep = 4;
                }
                else
                {
                    DrawO(pen, g, 4);
                    MyStep = 4;
                }
            }
            else if (currentplayer == ChoosedPlayer && e.X > 0 && e.Y > 80 && e.X < 100 && e.Y < 160 && cell[3, 0] == "1")
            {
                clicked = true;
                return;
            }
            if (currentplayer == ChoosedPlayer && e.X > 100 && e.Y > 80 && e.X < 200 && e.Y < 160)
            {
                clicked = true;
                if (ChoosedPlayer == "X")
                {
                    DrawX(pen, g, 5);
                    MyStep = 5;
                }
                else
                {
                    DrawO(pen, g, 5);
                    MyStep = 5;
                }
            }
            else if (currentplayer == ChoosedPlayer && e.X > 100 && e.Y > 80 && e.X < 200 && e.Y < 160 && cell[4, 0] == "1")
            {
                clicked = true;
                return;
            }
            if (currentplayer == ChoosedPlayer && e.X > 200 && e.Y > 80 && e.X < 300 && e.Y < 160)
            {
                clicked = true;
                if (ChoosedPlayer == "X")
                {
                    DrawX(pen, g, 6);
                    MyStep = 6;
                }
                else
                {
                    DrawO(pen, g, 6);
                    MyStep = 6;
                }
            }
            else if (currentplayer == ChoosedPlayer && e.X > 200 && e.Y > 80 && e.X < 300 && e.Y < 160 && cell[5, 0] == "1")
            {
                clicked = true;
                return;
            }
            if (currentplayer == ChoosedPlayer && e.X > 0 && e.Y > 160 && e.X < 100 && e.Y < 240)
            {
                clicked = true;
                if (ChoosedPlayer == "X")
                {
                    DrawX(pen, g, 7);
                    MyStep = 7;
                }
                else
                {
                    DrawO(pen, g, 7);
                    MyStep = 7;
                }
            }
            else if (currentplayer == ChoosedPlayer && e.X > 0 && e.Y > 160 && e.X < 100 && e.Y < 240 && cell[6, 0] == "1")
            {
                clicked = true;
                return;
            }
            if (currentplayer == ChoosedPlayer && e.X > 100 && e.Y > 160 && e.X < 200 && e.Y < 240)
            {
                clicked = true;
                if (ChoosedPlayer == "X")
                {
                    DrawX(pen, g, 8);
                    MyStep = 8;
                }
                else
                {
                    DrawO(pen, g, 8);
                    MyStep = 8;
                }
            }
            else if (currentplayer == ChoosedPlayer && e.X > 100 && e.Y > 160 && e.X < 200 && e.Y < 240 && cell[7, 0] == "1")
            {
                clicked = true;
                return;
            }
            if (currentplayer == ChoosedPlayer && e.X > 200 && e.Y > 160 && e.X < 300 && e.Y < 240)
            {
                clicked = true;
                if (ChoosedPlayer == "X")
                {
                    DrawX(pen, g, 9);
                    MyStep = 9;
                }
                else
                {
                    DrawO(pen, g, 9);
                    MyStep = 9;
                }
            }
            else if (currentplayer == ChoosedPlayer && e.X > 200 && e.Y > 160 && e.X < 300 && e.Y < 240 && cell[8, 0] == "1")
            {
                clicked = true;
                return;
            }
            if (clicked)
            {
                SendMove();
                CheckEndGame();


            }
        }
        void CheckEndGame()
        {
            if (CheckVictory() == 0)
            {
                
                if (inandout == 0)
                {
                    Cplayer.Text = "0";
                    currentplayer = "0";
                    inandout = 1;
                }
                else
                {
                    Cplayer.Text = "X";
                    currentplayer = "X";
                    inandout = 0;
                }
                MessageBox.Show("It's a tie");
                string textToSend = "sc"+ "Tie" + "," + server.lobby_ID;
                byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
                var nwStream = client.GetStream();
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                NewGame();
            }
            else if (CheckVictory() == 1)
            {
                
                if (inandout == 0)
                {
                    Cplayer.Text = "0";
                    currentplayer = "0";
                    inandout = 1;
                }
                else
                {
                    Cplayer.Text = "X";
                    currentplayer = "X";
                    inandout = 0;
                }
                int score = int.Parse(Xsc.Text);
                score++;
                Xsc.Text = score.ToString();
                MessageBox.Show("X Won");
                string textToSend = "sc"+ "Xwon" + "," + server.lobby_ID;
                byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
                var nwStream = client.GetStream();
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                NewGame();

            }
            else if (CheckVictory() == 2)
            {
                
                if (inandout == 0)
                {
                    Cplayer.Text = "0";
                    currentplayer = "0";
                    inandout = 1;
                }
                else
                {
                    Cplayer.Text = "X";
                    currentplayer = "X";
                    inandout = 0;
                }
                int score = int.Parse(Csc.Text);
                score++;
                Csc.Text = score.ToString();
                MessageBox.Show("0 Won");
                string textToSend = "sc"+ "0won" + "," +server.lobby_ID;
                byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
                var nwStream = client.GetStream();
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                NewGame();
            }
        }
        //game

        private void SendMove()
        {
            var nwStream = client.GetStream();
            string textToSend = "mv" + server.lobby_ID + "," + MyStep;
            byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }
        private void SendQuit()
        {
            var nwStream = client.GetStream();
            string textToSend = "ex" + server.lobby_ID + "," + PlayerNickName;
            byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                SendQuit();
            }
            
            try
            {
                t.Abort();
            }
            catch
            {

            }
            Application.Exit();
        }

        //private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        //{
        //   label1.Text = checkedListBox1.Items[e.Index].ToString();
        //    for (int ix = 0; ix < checkedListBox1.Items.Count; ++ix)
        //    {
        //        if (ix != e.Index) checkedListBox1.SetItemChecked(ix, false);
        //    }
        //}

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void game_Load(object sender, EventArgs e)
        {

        }



        int listplre = 0;

        private void ListendForCommand()
        {
            while (client.Connected)
            {
                NetworkStream nwStream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
                string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (dataReceived != "")
                {
                    string RecivedCommand = dataReceived.Substring(0, 2);
                    string RecivedMessage = "";
                    if (dataReceived.Length > 2) RecivedMessage = dataReceived.Remove(0, 2);
                    string[] temp = RecivedMessage.Split(',');
                    if (RecivedCommand == "up")//aktualizaja boxa z listą graczy do wyboru
                    {
                        if (checkedListBox1.Enabled)
                        {
                            Invoke((MethodInvoker)(() => checkedListBox1.Items.Clear()));
                            string[] tmp = RecivedMessage.Split(',');
                            for (int i = 0; i < tmp.Length; i++)
                            {
                                if (tmp[i] != "")
                                {
                                    Invoke((MethodInvoker)(() => checkedListBox1.Items.Add(tmp[i])));
                                }

                            }
                        }
                    }
                    if (RecivedCommand == "pa")
                    {
                        if (temp[1] == "second")
                        {
                            //Invoke((MethodInvoker)(() => Player1.Text = temp[0]));
                            //Invoke((MethodInvoker)(() => Player2.Text = PlayerNickName));
                            Invoke((MethodInvoker)(() => Player2.Text = temp[0]));
                            //Invoke((MethodInvoker)(() => Connect.Enabled = false));
                            Invoke((MethodInvoker)(() => Play.Enabled = false));
                            Invoke((MethodInvoker)(() => checkedListBox1.Enabled = false));
                            currentplayer = "X";
                            ChoosedPlayer = "0";
                            Invoke((MethodInvoker)(() => youwith.Text = "Circle"));
                            Invoke((MethodInvoker)(() => Cplayer.Text = "X"));
                        }
                        else
                        {
                            //Invoke((MethodInvoker)(() => Player1.Text = PlayerNickName));
                            Invoke((MethodInvoker)(() => Player2.Text = temp[0]));
                            //Invoke((MethodInvoker)(() => Connect.Enabled = false));
                            Invoke((MethodInvoker)(() => Play.Enabled = false));
                            Invoke((MethodInvoker)(() => checkedListBox1.Enabled = false));
                            currentplayer = "X";
                            ChoosedPlayer = "X";
                            Invoke((MethodInvoker)(() => youwith.Text = "X"));
                            Invoke((MethodInvoker)(() => Cplayer.Text = "X"));
                        }

                    }
                    if(RecivedCommand == "ex")
                    {

                    }
                    if(RecivedCommand == "mv")
                    {
                        int Step = int.Parse(RecivedMessage);
                        switch (Step)
                        {
                            case 1:
                                if(ChoosedPlayer == "X")
                                {
                                    DrawO(pen, g, Step);
                                }
                                else if (ChoosedPlayer == "0")
                                {
                                    DrawX(pen, g, Step);
                                }
                                break;
                            case 2:
                                if (ChoosedPlayer == "X")
                                {
                                    DrawO(pen, g, Step);
                                }
                                else if (ChoosedPlayer == "0")
                                {
                                    DrawX(pen, g, Step);
                                }
                                break;
                            case 3:
                                if (ChoosedPlayer == "X")
                                {
                                    DrawO(pen, g, Step);
                                }
                                else if (ChoosedPlayer == "0")
                                {
                                    DrawX(pen, g, Step);
                                }
                                break;
                            case 4:
                                if (ChoosedPlayer == "X")
                                {
                                    DrawO(pen, g, Step);
                                }
                                else if (ChoosedPlayer == "0")
                                {
                                    DrawX(pen, g, Step);
                                }
                                break;
                            case 5:
                                if (ChoosedPlayer == "X")
                                {
                                    DrawO(pen, g, Step);
                                }
                                else if (ChoosedPlayer == "0")
                                {
                                    DrawX(pen, g, Step);
                                }
                                break;
                            case 6:
                                if (ChoosedPlayer == "X")
                                {
                                    DrawO(pen, g, Step);
                                }
                                else if (ChoosedPlayer == "0")
                                {
                                    DrawX(pen, g, Step);
                                }
                                break;
                            case 7:
                                if (ChoosedPlayer == "X")
                                {
                                    DrawO(pen, g, Step);
                                }
                                else if (ChoosedPlayer == "0")
                                {
                                    DrawX(pen, g, Step);
                                }
                                break;
                            case 8:
                                if (ChoosedPlayer == "X")
                                {
                                    DrawO(pen, g, Step);
                                }
                                else if (ChoosedPlayer == "0")
                                {
                                    DrawX(pen, g, Step);
                                }
                                break;
                            case 9:
                                if (ChoosedPlayer == "X")
                                {
                                    DrawO(pen, g, Step);
                                }
                                else if (ChoosedPlayer == "0")
                                {
                                    DrawX(pen, g, Step);
                                }
                                break;

                            default:
                                break;
                        }
                        CheckEndGame();
                    }

                }
                
            }
        }

        //private void Connect_Click(object sender, EventArgs e) //połączenie
        //{
        //    Connect.Enabled = false;
        //    client.Connect("127.0.0.1", 12000); //ip adress here
        //    t = new Thread(() => ListendForCommand());
        //    t.Start();
        //    string textToSend = "lo" + PlayerNickName;
        //    byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
        //    var nwStream = client.GetStream();
        //    nwStream.Write(bytesToSend, 0, bytesToSend.Length);



        //}

        private void game_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                    // The Exit method of the Application class was called.
                    client.Close();
                    break;
                case CloseReason.FormOwnerClosing:
                    // The owner form is closing. 
                    client.Close();
                    break;
                case CloseReason.MdiFormClosing:
                    // The parent form is closing.
                    client.Close();
                    break;
                case CloseReason.None:
                    // Unknown closing reason. 
                    client.Close();
                    break;
                case CloseReason.TaskManagerClosing:
                    // The application is being closed from the TaskManager.  
                    client.Close();
                    break;
                case CloseReason.UserClosing:
                    // The user is closing the form through the UI.  
                    client.Close();
                    break;
                case CloseReason.WindowsShutDown:
                    // Windows is closing the application because it is shutting down.  
                    client.Close();
                    break;
            }
        }

        private async void Play_Click(object sender, EventArgs e) //rozpoczęcie gry
        {
            //
            //TU funkcja wysyłająca do API GET z zapytaniem o znalezienie meczu, na razie hardcoded
            await ApiGETMatchmaking();
                //ServerIP = "127.0.0.1";
                //ServerPort = 12000;
                //LobbyID = "test_lobby";
                //
            client.Connect(server.adress, server.port); //ip adress here
            t = new Thread(() => ListendForCommand());
            t.Start();

            //Player1.Text = PlayerNickName;

            string textToSend = "lo" + PlayerNickName + "," + server.lobby_ID+','+server.max_size;
            byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
            var nwStream = client.GetStream();
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                Play.Enabled = false;
                checkedListBox1.Enabled = false; 
        }

        private async Task<string> ApiGETMatchmaking() //Matching
        {
            string result = null;
            try
            {
                //var my_jsondata = new
                //{
                //    NickName = PlayerNickName
                //};
                //string json = JsonConvert.SerializeObject(my_jsondata);
                //var request = new HttpRequestMessage
                //{
                //    Method = HttpMethod.Get,
                //    RequestUri = new Uri("https://localhost:5001/api/Matching"),
                //    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                //};
                //HttpResponseMessage response = await httpClient.SendAsync(request);//.ConfigureAwait(continueOnCapturedContext: false);
                HttpResponseMessage response = await httpClient.GetAsync("https://localhost:5001/api/Matching/Match/"+PlayerNickName);
                response.EnsureSuccessStatusCode();
                var objekt = await response.Content.ReadAsStringAsync();
                server = JsonConvert.DeserializeObject<APIHandle.Server>(objekt);
            }
            catch (Exception)
            {
                return null;
            }

            return result;
        }
        private void label1_TextChanged(object sender, EventArgs e)
        {
            Play.Enabled = true;
        }

        private void Player1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void Cplayer_Click(object sender, EventArgs e)
        {

        }

        private void youwith_Click(object sender, EventArgs e)
        {

        }

        private void Player2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}
