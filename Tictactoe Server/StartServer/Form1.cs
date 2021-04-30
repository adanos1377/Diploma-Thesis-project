using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using RestSharp;

namespace StartServer
{

    public partial class Form1 : Form
    {
        private HttpClient httpClient = new HttpClient();
        List<TcpClient> Clients = new List<TcpClient>();
        Thread t;
        TcpListener listener;
        List<PlayerBound> Players = new List<PlayerBound>();
        static IList<ClientIDs> ClientList = new List<ClientIDs>();
        public List<Lobby> Lobbies = new List<Lobby>();
        public async Task<string> ApiPOST(Lobby lobby)
        {
            string result;
            try
            {
                var my_jsondata = new
                {
                    players = lobby.Nicknames,
                    ranked = true,
                    lobby_id = lobby.lobbyName,
                };
                string json = JsonConvert.SerializeObject(my_jsondata);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:5001/api/Matches/solo"),
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
                HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(continueOnCapturedContext: false);
                response.EnsureSuccessStatusCode();
                var test = response.Content;
                result = await test.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return null;
            }

            return result;
        }
        public async Task<string> ApiPUT(Lobby lobby)
        {
            string result;
            List<int> scores2=new List<int>();
            foreach (var x in lobby.Players) scores2.Add(x.score);
            try
            {
                var my_jsondata = new
                {
                    scores = scores2,
                    match_id = Int32.Parse(lobby.matchID)
                };
                string json = JsonConvert.SerializeObject(my_jsondata);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync("https://localhost:5001/api/Matches/solo", stringContent)
                    .ConfigureAwait(continueOnCapturedContext: false);
                response.EnsureSuccessStatusCode();
                result = await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return null;
            }

            return result;
        }
        static bool IsClientConneted(TcpClient client)
        {
            if (client.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] checkConn = new byte[1];
                if (client.Client.Receive(checkConn, SocketFlags.Peek) == 0)
                {
                    return false;
                }
            }
            return true;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public class PlayerBound
        {
            public string Player1;
            public string Player2;
        }
        public class ClientIDs
        {
            public string ClientID { get; set; }
            public EndPoint Ip { get; set; }
        }

        private async void ThreadProc(object obj)
        {
            var client = (TcpClient)obj;
            while (client.Connected)
            {
                if (!IsClientConneted(client))
                {
                    Invoke((MethodInvoker)(() => listBox2.Items.Add("Client disconnected :" + client.Client.RemoteEndPoint)));
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        if (client.Client.RemoteEndPoint.ToString() == listBox1.Items[i].ToString())
                        {
                            Invoke((MethodInvoker)(() => listBox1.Items.Remove(client.Client.RemoteEndPoint.ToString())));
                        }
                    }
                    client.Close();
                    break;
                }
                else
                {
                    NetworkStream nwStream = client.GetStream();
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
                    string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (dataReceived != "")
                    {
                       
                        NetworkStream p1 = nwStream;
                        NetworkStream p2 = null;
                        string RecivedCommand = dataReceived.Substring(0, 2);
                        string RecivedMessage = "";
                        if (dataReceived.Length > 2) RecivedMessage = dataReceived.Remove(0, 2);
                        string[] temp = RecivedMessage.Split(',');
                        if (RecivedCommand == "lo")
                        {
                            var lob=Lobbies.Exists(x => x.lobbyName == temp[1]);
                            NetworkStream p = client.GetStream();
                            if (lob == true)
                            {
                                Lobby templobby = Lobbies.Find(x => x.lobbyName == temp[1]);
                                templobby.Players.Add(new Player { Nickname=temp[0],network=p, order="second"});
                                templobby.Nicknames.Add(temp[0]);

                                //
                                int j = 1;
                                for (int i = 0; i < templobby.Players.Count; i++)
                                {
                                    byte[] bufif = Encoding.UTF8.GetBytes("pa" + templobby.Players[j].Nickname + "," + templobby.Players[i].order);
                                    templobby.Players[i].network.Write(bufif, 0, bufif.Length);
                                    j--;
                                }
                                templobby.matchID=await ApiPOST(templobby);
                                Invoke((MethodInvoker)(() => listBox2.Items.Add("match id received: " + templobby.matchID)));
                            }
                            else 
                            {
                                Lobbies.Add(new Lobby(temp[1],temp[2],new Player { Nickname=temp[0],network=p,order="first"}));
                            }

                        }
                        if (RecivedCommand == "sc")
                        {
                            Invoke((MethodInvoker)(() => listBox2.Items.Add("match result: " + temp[0])));
                            NetworkStream source = client.GetStream();
                            Lobby templobby = Lobbies.Find(x => x.lobbyName == temp[1]);
                            if (temp[0]=="Xwon")
                            {

                                for (int i = 0; i < templobby.Players.Count; i++)
                                {

                                        if (templobby.Players[i].order == "first")
                                        {
                                            templobby.Players[i].score = 1;
                                        }
                                        else
                                        {
                                            templobby.Players[i].score = 0;
                                        }

                                }

                                //await ApiPUT(templobby);
                            }
                            else if (temp[0] == "0won")
                            {
                                for (int i = 0; i < templobby.Players.Count; i++)
                                {
                                    if (templobby.Players[i].order == "first")
                                    {
                                        templobby.Players[i].score = 0;
                                    }
                                    else
                                    {
                                        templobby.Players[i].score = 1;
                                    }
                                    //if (templobby.Players[i].network == source)
                                    //{
                                    //    if (templobby.Players[i].order == "first")
                                    //    {
                                    //        templobby.Players[i].score = 0;
                                    //    }
                                    //    else
                                    //    {
                                    //        templobby.Players[i].score = 1;
                                    //    }
                                    //}
                                }
                                //await ApiPUT(templobby);
                            }
                            else
                            {
                                for (int i = 0; i < templobby.Players.Count; i++)
                                {
                                    if (templobby.Players[i].network == source)
                                    {
                                        if (templobby.Players[i].order == "first")
                                        {
                                            templobby.Players[i].score = 0;
                                        }
                                        else
                                        {
                                            templobby.Players[i].score = 0;
                                        }
                                    }
                                }
                                //
                            }
                            if (!templobby.sent)
                            {
                                await ApiPUT(templobby);
                                templobby.sent = true;
                            }
                            
                        }
                        if (RecivedCommand == "pl")
                        {
                            string[] tmp = RecivedMessage.Split(',');
                            string Player1 = tmp[0];
                            string Player2 = tmp[1];
                            Players.Add(new PlayerBound { Player1 = Player1, Player2 = Player2 });
                            List<string> ingame = new List<string>();
                            for (int i = 0; i < Players.Count; i++)
                            {
                                ingame.Add(Players[i].Player1);
                                ingame.Add(Players[i].Player2);

                            }
                            string SendClientList = "up";
                            for (int i = 0; i < ClientList.Count; i++)
                            {
                                if (!ingame.Contains(ClientList[i].ClientID))
                                {
                                    SendClientList += ClientList[i].ClientID + ",";
                                }
                            }
                            byte[] bufi = Encoding.UTF8.GetBytes(SendClientList);
                            for (int i = 0; i < Clients.Count; i++)
                            {
                                NetworkStream Clientsstream = Clients[i].GetStream();
                                Clientsstream.Write(bufi, 0, bufi.Length);
                            }
                            EndPoint ip = null;

                            byte[] bufif = Encoding.UTF8.GetBytes("pa" + Player1);
                            for (int i = 0; i < ClientList.Count; i++)
                            {
                                if (ClientList[i].ClientID == Player2)
                                {
                                    ip = ClientList[i].Ip;
                                }
                            }

                            for (int i = 0; i < Clients.Count; i++)
                            {
                                if (Clients[i].Client.RemoteEndPoint == ip)
                                {
                                    p2 = Clients[i].GetStream();
                                    p2.Write(bufif, 0, bufif.Length);
                                }
                            }
                        }
                        if (RecivedCommand == "ex")
                        {
                            var lob = Lobbies.Exists(x => x.lobbyName == temp[0]);
                            NetworkStream p = client.GetStream();
                            if (lob == true)
                            {
                                Lobby templobby = Lobbies.Find(x => x.lobbyName == temp[0]);
                                templobby.Players.Remove(templobby.Players.Find(x=>x.Nickname==temp[1]));
                                if (templobby.Players.Count < 1)
                                {
                                    Lobbies.Remove(templobby);
                                }
                            }
                        }
                        if (RecivedCommand == "mv")
                        {
                            string Player1 = "";
                            string Player2 = "";
                            string[] tmp = RecivedMessage.Split(',');
                            string Player = tmp[0];
                            string Step = tmp[1];
                            EndPoint ip = null;
                            NetworkStream source = client.GetStream();
                            Lobby templobby = Lobbies.Find(x => x.lobbyName == temp[0]);
                            for (int i = 0; i < templobby.Players.Count; i++)
                            {
                                if (templobby.Players[i].network != source)
                                {
                                    NetworkStream outcome = templobby.Players[i].network;
                                    string textToSend = "mv" + Step;
                                    byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
                                    outcome.Write(bytesToSend, 0, bytesToSend.Length);
                                }
                            }

                        }
                    }
                }
            }
        }
       
        private void Listening()
        {
            Invoke((MethodInvoker)(() => label3.Text = "ON"));
            
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            const int PORT_NO = 12000;
            //---listen at the specified IP and port no.---
            listener = new TcpListener(ipAddress, PORT_NO);
            Invoke((MethodInvoker)(() => listBox2.Items.Add("Server Online! Listening..")));
            TcpClient client;
            listener.Start();
            while (label3.Text == "ON") // Add your exit flag here
            {
                client = listener.AcceptTcpClient();
                Clients.Add(client);
                Invoke((MethodInvoker)(() => listBox2.Items.Add("Client connected: " + client.Client.RemoteEndPoint)));
                Invoke((MethodInvoker)(() => listBox1.Items.Add(client.Client.RemoteEndPoint.ToString())));
                int ind = Clients.IndexOf(Clients.Last());
                ThreadPool.QueueUserWorkItem(ThreadProc, Clients[ind]);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            t = new Thread(() => Listening());
            t.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label3.Text = "OFF";
            button1.Enabled = true;
            listener.Stop();
            
            t.Abort();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                listener.Stop();
                t.Abort();
            }
            catch
            {

            }
            
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //IsGame = false;
        }
    }
    public class Lobby
    {
        public string lobbyName { get; set; }
        public List<Player> Players = new List<Player>();
        public List<string> Nicknames = new List<string>();
        public bool sent;
        public int maxSize;
        public string matchID;
        public Lobby(string name, string maxsize, Player player)
        {
            sent = false;
            lobbyName = name;
            Players.Add(player);
            Nicknames.Add(player.Nickname);
            maxSize = Int32.Parse(maxsize);
        }
    }
    public class Player
    {
        public string Nickname { get; set; }
        public NetworkStream network { get; set; }
        public string order { get; set; }
        public int score;
    }
    public class MatchSoloCreate
    {
        public List<string> players { get; set; }
        public bool ranked { get; set; }
        public string lobby_id { get; set; }
    }
}
