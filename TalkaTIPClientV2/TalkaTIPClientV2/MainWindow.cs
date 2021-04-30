using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TalkaTIPClientV2
{
    public partial class MainWindow : Form
    {
        Thread commThread;
        public EndPoint callerEndPoint;
        public DateTime begin;
        public DateTime end;
        public bool isReceiver = false;

        private WaveIn recorder;
        private BufferedWaveProvider bufferedWaveProvider;
        private WaveOut player;
        private WaveFormat waveFormat;
        private UdpClient receiveClient = new UdpClient();
        private UdpClient sendClient = new UdpClient();
        private IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 1550);
        private bool isOnGroupsTab = false;

        public MainWindow()
        {
            InitializeComponent();
            timerIAM.Start();
        }


        // Recieve all basic user data from the server
        void waitForCommuniques()
        {
            string response = string.Empty;
            int numberOfComm = 0;
            string[] comms = null;
            while (!Program.mainWindow.Visible) { }
            while (numberOfComm < 4)
            {
                try
                {
                    response = Program.client.Receive();
                    if (response.Length > 0)
                    {
                        comms = response.Split(new string[] { " <EOF>" }, StringSplitOptions.None);
                        comms = comms.Take(comms.Length - 1).ToArray();
                        foreach (var comm in comms)
                        {
                            Communication.commFromServer(comm);
                            numberOfComm++;
                        }
                    }
                }
                catch (Exception)
                {
                    numberOfComm += comms.Length;
                }
            }
            Program.client.Disconnect();
        }

        private void BlockButton_Click(object sender, EventArgs e)
        {
            string promptValue;

            if (listView2.SelectedItems.Count == 0 || listView2.SelectedItems.Count > 1)
            {
                if (listView1.SelectedItems.Count == 0 || listView1.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Please select one user to add to your blocked list.", "Error");
                }
                else
                {
                    promptValue = listView1.SelectedItems[0].Text;
                    deleteFriendButton.PerformClick();
                    BlockSelectedUser(promptValue);
                }
            }
            else
            {
                promptValue = listView2.SelectedItems[0].Text;
                BlockSelectedUser(promptValue);
            }
        }

        private void BlockSelectedUser(string promptValue)
        {
            try
            {
                Program.client = new Client(Program.serverAddress);
                if (Communication.BlockUser(Program.userLogin, promptValue) == true)
                {
                    MessageBox.Show(promptValue + " was successfully added to your blocked list.", "Success");
                    BlockButton.Visible = false;
                    UnblockButton.Visible = true;
                }
                else
                {
                    MessageBox.Show("The user wasn't found or is in your blocked list already.", "Error");
                }
                Program.client.Disconnect();
            }
            catch (Exception)
            {
                MessageBox.Show("Server connection error.", "Error");
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            string promptValue = UserSearch.Text;
            if (promptValue != "" && promptValue != Program.userLogin)
            {
                try
                {
                    Program.client = new Client(Program.serverAddress);
                    if (Communication.AddFriend(Program.userLogin, promptValue) == true)
                    {
                        MessageBox.Show(promptValue + " was successfully added to your contacts list.", "Success");
                        string[] friendDetails = { promptValue, "0" };
                        ListViewItem friend = new ListViewItem(friendDetails, 0);
                        Program.mainWindow.listView1.Items.Add(friend);
                        listView1.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("The user wasn't found or is in your contacts list already.", "Error");
                    }
                    Program.client.Disconnect();
                }
                catch (Exception)
                {
                    MessageBox.Show("Server connection error.", "Error");
                }
            }
        }

        private void FriendButton_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count == 0 || listView2.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please select one user to add to your contacts list.", "Error");
            }
            else
            {
                string promptValue = listView2.SelectedItems[0].Text;
                try
                {
                    Program.client = new Client(Program.serverAddress);
                    if (Communication.AddFriend(Program.userLogin, promptValue) == true)
                    {
                        MessageBox.Show(promptValue + " was successfully added to your contacts list.", "Success");
                        string[] friendDetails = { promptValue, "0" };
                        ListViewItem friend = new ListViewItem(friendDetails, 0);
                        Program.mainWindow.listView1.Items.Add(friend);
                        listView1.Refresh();
                        FriendButton.Visible = false;
                        deleteFriendButton.Visible = true;
                    }
                    else
                    {
                        MessageBox.Show("The user wasn't found or is in your contacts list already.", "Error");
                    }
                    Program.client.Disconnect();
                }
                catch (Exception)
                {
                    MessageBox.Show("Server connection error.", "Error");
                }
            }
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            Program.loginAndMessage.Clear();
            Program.apiNameAndHandle.Clear();
            Program.chatNameAndMessage.Clear();
            DialogResult = DialogResult.No;
            Close();
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            commThread = new Thread(waitForCommuniques);
            commThread.Start();

            receiveClient.ExclusiveAddressUse = false;
            sendClient.ExclusiveAddressUse = false;

            receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);



            Thread t = new Thread(Service);
            t.IsBackground = true;
            t.Start();
        }

        void Service()
        {
            //sourcestream = new NAudio.Wave.WaveIn();
            waveFormat = new WaveFormat(22050, 16, 1);

            bufferedWaveProvider = new BufferedWaveProvider(waveFormat);

            player = new WaveOut();

            player.Init(bufferedWaveProvider);
            player.Play();
            receiveClient.Client.Bind(remoteEP);

            while (true)
            {
                byte[] byteData = receiveClient.Receive(ref remoteEP);

                bufferedWaveProvider.AddSamples(byteData, 0, byteData.Length);
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerIAM.Stop();
            if (commThread.IsAlive)
            {
                Program.client.Disconnect();
                commThread.Abort();
            }
            try
            {
                if (gbAnswerCall.Visible == true)
                {
                    declineButton_Click(null, null);
                }
                else if (gbCall.Visible == true && cancelCallButton.Visible == true)
                {
                    cancelCallButton_Click(null, null);
                }
                else if (gbInCall.Visible == true)
                {
                    disconnectButton_Click(null, null);
                }
                Program.client = new Client(Program.serverAddress);
                Communication.LogOut(Program.userLogin);
                Program.client.Disconnect();
            }
            catch (Exception)
            {
                MessageBox.Show("Server connection error.", "Error");
            }
            Program.userLogin = "";
            Program.serverAddress = "";
            Program.sessionKeyWithServer = null;

            if (Program.callHandler.player != null)
            {
                Program.callHandler.player.Stop();
            }
            if (Program.callHandler.clientSocket != null)
            {
                Program.callHandler.clientSocket.Shutdown(SocketShutdown.Both);
                Program.callHandler.clientSocket.Close();
            }
            Program.callHandler = null;
        }

        private void RecorderOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        {
            byte[] buffer = waveInEventArgs.Buffer;
            sendClient.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Loopback, 1550));
        }

        private void CallButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a user to call.", "Error");
            }
            else if (listView1.SelectedItems[0].Text == Program.userLogin)
            {
                sendClient.Client.Bind(remoteEP);

                recorder = new WaveIn();
                recorder.DataAvailable += RecorderOnDataAvailable;
                recorder.WaveFormat = waveFormat;
                recorder.StartRecording();
                labelCallUN.Text = listView1.SelectedItems[0].Text.ToString();
                cancelCallButton.Visible = true;
                CallButton.Visible = false;
            }
            else if (listView1.SelectedItems[0].ImageIndex == 0)
            {
                try
                {
                    Program.client = new Client(Program.serverAddress);
                    Communication.CallState(Program.userLogin, listView1.SelectedItems[0].Text, DateTime.Now, TimeSpan.Zero);
                    string[] historyDetails = new string[3];
                    historyDetails[0] = listView1.SelectedItems[0].Text;
                    historyDetails[1] = DateTime.Now.ToString();
                    historyDetails[2] = "missed call";
                    listView2.Items.Insert(0, (new ListViewItem(historyDetails)));
                    listView2.Refresh();
                    MessageBox.Show("Selected user is not available at the moment. Try calling them later or wait for their response."
                        , "User unavailable.");
                    Program.client.Disconnect();
                }
                catch (Exception)
                {
                    MessageBox.Show("Server connection error.", "Error");
                }
            }
            else
            {
                labelCallUN.Text = listView1.SelectedItems[0].Text.ToString();
                cancelCallButton.Visible = true;
                CallButton.Visible = false;
                Program.callHandler.Call();
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                end = DateTime.Now;
                Program.callHandler.DropCall();

                // Send to server
                Program.client = new Client(Program.serverAddress);
                if (isReceiver == true)
                {
                    Communication.CallState(labelInCallUN.Text.Remove(0, 2), Program.userLogin, begin, (end - begin));
                }
                else
                {
                    Communication.CallState(Program.userLogin, labelInCallUN.Text.Remove(0, 2), begin, (end - begin));
                }
                Program.client.Disconnect();
                isReceiver = false;
                gbAnswerCall.Visible = false;
                gbCall.Visible = true;
                gbAnswerCall.Visible = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Server connection error.", "Error");
            }
        }

        private void declineButton_Click(object sender, EventArgs e)
        {
            gbAnswerCall.Visible = false;
            gbCall.Visible = true;
            gbAnswerCall.Visible = false;
            cancelCallButton.Visible = false;
            CallButton.Visible = true;
            Program.callHandler.DeclineCall(callerEndPoint);
            //gBChangePass.Enabled = true;
            //gBDelAcc.Enabled = true;
        }

        private void answerButton_Click(object sender, EventArgs e)
        {
            gbInCall.Visible = true;
            gbAnswerCall.Visible = false;
            Program.callHandler.AcceptCall(callerEndPoint);
        }

        private void cancelCallButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count != 0)
                {
                    if (listView1.SelectedItems[0].Text == Program.userLogin)
                    {
                        recorder.StopRecording();
                        // stop playback
                        player.Stop();
                        cancelCallButton.Visible = false;
                        CallButton.Visible = true;
                    }
                    else
                    {
                        //timerCallOut.Stop();
                        Program.callHandler.CancelCall();
                        Program.client = new Client(Program.serverAddress);
                        Communication.CallState(Program.userLogin, listView1.SelectedItems[0].Text, DateTime.Now, TimeSpan.Zero);
                        string[] historyDetails = new string[3];
                        historyDetails[0] = listView1.SelectedItems[0].Text;
                        historyDetails[1] = DateTime.Now.ToString();
                        historyDetails[2] = "missed call";
                        listView2.Items.Insert(0, (new ListViewItem(historyDetails)));
                        listView2.Refresh();
                        Program.client.Disconnect();
                        cancelCallButton.Visible = false;
                        CallButton.Visible = true;
                        //gBChangePass.Enabled = true;
                        //gBDelAcc.Enabled = true;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Server connection error.", "Error");
            }
        }

        private void deleteFriendButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0 || listView1.SelectedItems.Count > 1)
            {
                MessageBox.Show("You need to select one user from the friends list.", "Error");
            }
            else
            {
                try
                {
                    Program.client = new Client(Program.serverAddress);
                    if (Communication.DelFriend(Program.userLogin, listView1.SelectedItems[0].Text) == true)
                    {
                        MessageBox.Show("The user: " + listView1.SelectedItems[0].Text + " was successfully deleted from your list.", "Success");
                        listView1.Items.Remove(listView1.SelectedItems[0]);
                        listView1.Refresh();
                        deleteFriendButton.Visible = false;
                        FriendButton.Visible = true;
                    }
                    else
                    {
                        MessageBox.Show("Unknown error occured while deleting the user from your list.", "Error");
                    }
                    Program.client.Disconnect();
                }
                catch (Exception)
                {
                    MessageBox.Show("Server connection error.", "Error");
                }
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if(InsertMessageText.Text != string.Empty)
            {
                if(InsertMessageText.Text.Length < 200)
                {
                    if (!isOnGroupsTab && listView1.SelectedItems[0] != null)
                    {
                        try
                        {
                            string recieverLogin = listView1.SelectedItems[0].Text;
                            Program.client = new Client(Program.serverAddress);
                            if (Communication.ChatMessage(Program.userLogin, recieverLogin, InsertMessageText.Text) == true)
                            {
                                AllMessages.Text += "\r\n" + Program.userLogin + " " +
                                    DateTime.Now.ToString() + "\r\n" + InsertMessageText.Text + "\r\n";
                                InsertMessageText.Text = string.Empty;
                            }
                            else
                            {
                                InsertMessageText.Text = string.Empty;
                                MessageBox.Show("Sending the message failed or blocked by the user.", "Error");
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Server connection error.", "Error");
                        }
                        Program.client.Disconnect();
                    }
                    else
                    {
                        if(listViewGroups.SelectedItems[0] != null)
                        {
                            try
                            {
                                string chatName = listViewGroups.SelectedItems[0].Text;
                                Program.client = new Client(Program.serverAddress);
                                if (Communication.GroupChatMessage(Program.userLogin, chatName, InsertMessageText.Text) == true)
                                {
                                    AllMessages.Text += "\r\n" + Program.userLogin + " " +
                                        DateTime.Now.ToString() + "\r\n" + InsertMessageText.Text + "\r\n";
                                    InsertMessageText.Text = string.Empty;
                                }
                                else
                                {
                                    InsertMessageText.Text = string.Empty;
                                    MessageBox.Show("Sending the message failed.", "Error");
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Server connection error.", "Error");
                            }
                            Program.client.Disconnect();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Your text message length may not be higher than 200 characters.", "Error");
                }
            }
        }

        private void InsertMessageText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendButton.PerformClick();
            }
        }

        private void UnblockButton_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count == 0 || listView2.SelectedItems.Count > 1)
            {
                MessageBox.Show("You need to select one user from the history.", "Error");
            }
            else
            {
                try
                {
                    Program.client = new Client(Program.serverAddress);
                    if (Communication.UnblockUser(Program.userLogin, listView2.SelectedItems[0].Text) == true)
                    {
                        MessageBox.Show("The user: " + listView2.SelectedItems[0].Text + " was successfully deleted from your blocked list.", "Success");
                        //listView2.Items.Remove(listView1.SelectedItems[0]);
                        //listView2.Refresh();
                        BlockButton.Visible = false;
                        UnblockButton.Visible = true;
                    }
                    else
                    {
                        MessageBox.Show("Unknown error occured while deleting the user from your blocked list.", "Error");
                    }
                    Program.client.Disconnect();
                }
                catch (Exception)
                {
                    MessageBox.Show("Server connection error.", "Error");
                }
            }
        }

        public void UpdateChatText(string updatedText)
        {
            AllMessages.Text = updatedText.Replace("\n", Environment.NewLine);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0 || listView1.SelectedItems.Count > 1)
            {
                //MessageBox.Show("You need to select one user from the friends list.", "Error");
            }
            else
            {
                isOnGroupsTab = false;
                try
                {
                    FriendButton.Visible = false;
                    deleteFriendButton.Visible = true;
                    BlockButton.Visible = true;
                    UnblockButton.Visible = false;
                    buttonLeaveChat.Visible = false;
                    buttonInviteToChat.Visible = false;

                    if (listView1.SelectedItems[0].ForeColor == Color.Red)
                    {
                        listView1.SelectedItems[0].ForeColor = Color.Black;
                    }

                    if (Program.loginAndMessage.ContainsKey(listView1.SelectedItems[0].Text))
                    {
                        UpdateChatText(Program.loginAndMessage[listView1.SelectedItems[0].Text]);
                    }
                    else
                    {
                        Program.client = new Client(Program.serverAddress);

                        // Load the messages from server if available   
                        Communication.GetAllChatMessages(Program.userLogin, listView1.SelectedItems[0].Text);

                        Thread.Sleep(100);
                        Program.client.Disconnect();
                    }

                    ContactName.Text = listView1.SelectedItems[0].Text;
                }
                catch (Exception)
                {
                    MessageBox.Show("Server connection error.", "Error");
                }
            }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            isOnGroupsTab = false;
        }

        private void listViewGroups_ItemActivate(object sender, EventArgs e)
        {
            isOnGroupsTab = true;
        }

        private void listViewGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewGroups.SelectedItems.Count == 0 || listViewGroups.SelectedItems.Count > 1)
            {
                //MessageBox.Show("You need to select one user from the friends list.", "Error");
            }
            else
            {
                isOnGroupsTab = true;
                try
                {
                    FriendButton.Visible = false;
                    deleteFriendButton.Visible = false;
                    BlockButton.Visible = false;
                    UnblockButton.Visible = false;
                    buttonLeaveChat.Visible = true;
                    buttonInviteToChat.Visible = true;

                    if (listViewGroups.SelectedItems[0].ForeColor == Color.Red)
                    {
                        listViewGroups.SelectedItems[0].ForeColor = Color.Black;
                    }

                    if (Program.chatNameAndMessage.ContainsKey(listViewGroups.SelectedItems[0].Text))
                    {
                        UpdateChatText(Program.chatNameAndMessage[listViewGroups.SelectedItems[0].Text]);
                    }
                    else
                    {
                        Program.client = new Client(Program.serverAddress);

                        // Load the messages from server if available   
                        Communication.GetAllGroupChatMessages(Program.userLogin, listViewGroups.SelectedItems[0].Text);

                        Thread.Sleep(100);
                        Program.client.Disconnect();
                    }

                    ContactName.Text = listViewGroups.SelectedItems[0].Text;
                }
                catch (Exception err)
                {
                    MessageBox.Show("Server connection error.\n" + err, "Error");
                }
            }
        }

        private void buttonCreateChat_Click(object sender, EventArgs e)
        {
            using (TextDialog f = new TextDialog())
            {
                f.labelDesc.Text = "Type in the chat name:";

                if (f.ShowDialog() == DialogResult.OK)
                {
                    if(f.textContent.Text != string.Empty)
                    {
                        try
                        {
                            Program.client = new Client(Program.serverAddress);
                            if (!Communication.CreateGroupChat(f.textContent.Text, Program.userLogin))
                            {
                                MessageBox.Show("Chat name already taken or task failed.", "Error");
                                f.Dispose();
                                buttonCreateChat.PerformClick();
                            }
                            else
                            {
                                MessageBox.Show("Chat created successfully.", "Success");
                                ListViewItem chat = new ListViewItem(f.textContent.Text, 0);
                                Program.mainWindow.listViewGroups.Items.Add(chat);
                                listViewGroups.Refresh();
                                //f.Dispose();
                            }
                            Program.client.Disconnect();
                        }
                        catch(Exception)
                        {
                            MessageBox.Show("Server connection error.", "Error");
                            f.Dispose();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Chat name can't be empty.", "Error");
                        f.Dispose();
                        buttonCreateChat.PerformClick();
                    }
                }
            }
        }

        private void buttonLeaveChat_Click(object sender, EventArgs e)
        {
            if (listViewGroups.SelectedItems.Count == 0 || listViewGroups.SelectedItems.Count > 1 || listViewGroups.SelectedItems[0] == null)
            {
                //MessageBox.Show("You need to select one user from the friends list.", "Error");
            }
            else
            {
                try
                {
                    Program.client = new Client(Program.serverAddress);
                    if (!Communication.LeaveGroupChat(listViewGroups.SelectedItems[0].Text, Program.userLogin))
                    {
                        MessageBox.Show("Failed to leave the chat.", "Error");
                    }
                    else
                    {
                        listViewGroups.Items.Remove(listViewGroups.SelectedItems[0]);
                        MessageBox.Show("Success!");
                    }
                    Program.client.Disconnect();
                }
                catch(Exception)
                {
                    MessageBox.Show("Server connection error.", "Error");
                }
            }
        }

        private void buttonInviteToChat_Click(object sender, EventArgs e)
        {
            if (!(listViewGroups.SelectedItems.Count == 0 || listViewGroups.SelectedItems.Count > 1 || listViewGroups.SelectedItems[0] == null))
            {
                using (TextDialog f = new TextDialog())
                {
                    f.labelDesc.Text = "Type in the user name:";

                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        if (f.textContent.Text != string.Empty)
                        {
                            try
                            {
                                Program.client = new Client(Program.serverAddress);
                                if (!Communication.AddUserToGroupChat(listViewGroups.SelectedItems[0].Text, f.textContent.Text))
                                {
                                    MessageBox.Show("Task failed.", "Error");
                                    f.Dispose();
                                    buttonInviteToChat.PerformClick();
                                }
                                else
                                {
                                    MessageBox.Show("User added successfully.", "Success");
                                    //f.Dispose();
                                }
                                Program.client.Disconnect();
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Server connection error.", "Error");
                                f.Dispose();
                            }
                        }
                        else
                        {
                            MessageBox.Show("User name can't be empty.", "Error");
                            f.Dispose();
                            buttonInviteToChat.PerformClick();
                        }
                    }
                }
            }
        }

        private void timerIAM_Tick(object sender, EventArgs e)
        {
            try
            {
                Program.client = new Client(Program.serverAddress);
                Communication.Iam(Program.userLogin);
                Program.client.Disconnect();
            }
            catch (Exception)
            { }
        }

        private void addAPIButton_Click(object sender, EventArgs e)
        {
            string promptValue = addGameAPIText.Text;
            if (promptValue != string.Empty)
            {
                APIHandle aPIHandle = new APIHandle(promptValue);
                if (Program.apiNameAndHandle.ContainsValue(aPIHandle))
                {
                    MessageBox.Show("The API is already on the list.", "Error");
                }
                else
                {
                    string result = aPIHandle.ApiPOST().GetAwaiter().GetResult();

                    if (result != null)
                    {
                        try
                        {
                            Program.client = new Client(Program.serverAddress);
                            Communication.AddApiToUser(Program.userLogin, promptValue, result);

                            string[] apiDetails = new string[2];
                            apiDetails[0] = result;
                            apiDetails[1] = promptValue;
                            gameAPIListView.Items.Insert(0, new ListViewItem(apiDetails));

                            Program.apiNameAndHandle.Add(result, aPIHandle);
                            MessageBox.Show(result, "OK");
                            Program.client.Disconnect();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Server connection error.", "Error");
                        }
                    }
                }
            }
        }

        private void playerStatisticsButton_Click(object sender, EventArgs e)
        {
            if (gameAPIListView.SelectedItems.Count == 0 || gameAPIListView.SelectedItems.Count > 1)
            { }
            else
            {
                string apiName = gameAPIListView.SelectedItems[0].Text;
                if (Program.apiNameAndHandle.ContainsKey(apiName))
                {
                    string stats = Program.apiNameAndHandle[apiName].ApiGETUserStatistics().GetAwaiter().GetResult();

                    // Display statistics on the main screen
                    UpdateChatText(stats);
                }
                else
                {
                    MessageBox.Show("API not in the main class variable.", "Error");
                }
            }
        }

        private void searchOpponentButton_Click(object sender, EventArgs e)
        {
            if (gameAPIListView.SelectedItems.Count == 0 || gameAPIListView.SelectedItems.Count > 1)
            { }
            else
            {
                string apiName = gameAPIListView.SelectedItems[0].Text;
                if (Program.apiNameAndHandle.ContainsKey(apiName))
                {
                    if (apiName == "Tic-Tac-Toe")
                    {
                        // Initialize the game in a second window
                        Thread myThread = new Thread((ThreadStart)delegate { Application.Run(new game(Program.userLogin)); });
                        //myThread.TrySetApartmentState(ApartmentState.STA); //If you receive errors, comment this out; use this when doing interop with STA COM objects.
                        myThread.Start(); //Start the thread; Run the form
                    }
                    else
                    {
                        string chatName = Program.apiNameAndHandle[apiName].ApiGETMatchmaking().GetAwaiter().GetResult();

                        // Create/join the proper chat
                        if (Communication.CreateAPIGroupChatAndAddUser(chatName, Program.userLogin))
                        {
                            string str = "Select chat: " + chatName + " from your group chat list.";
                            MessageBox.Show(str, "Success.");

                            // Add the chat to group chats list
                            string[] chatDetails = new string[2];
                            chatDetails[0] = chatName;
                            chatDetails[1] = "1";

                            Program.mainWindow.listViewGroups.Items.Insert(0, new ListViewItem(chatDetails));
                        }
                        else
                        {
                            MessageBox.Show("Joining or creating chat failed.", "Error");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("API not in the main class variable.", "Error");
                }
            }
        }

        private void InsertMessageText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                InsertMessageText.Text = string.Empty;
            }
        }

        private void changePasswordButton_Click(object sender, EventArgs e)
        {
            using (TextDialog f = new TextDialog())
            {
                f.labelDesc.Text = "Type in your old password:";
                f.textContent.UseSystemPasswordChar = true;

                if (f.ShowDialog() == DialogResult.OK)
                {
                    if (f.textContent.Text != string.Empty)
                    {
                        string oldPassword = f.textContent.Text;
                        f.Dispose();
                        using (TextDialog f2 = new TextDialog())
                        {

                            f2.labelDesc.Text = "Type in your new password:";
                            f2.textContent.UseSystemPasswordChar = true;
                            if (f2.ShowDialog() == DialogResult.OK)
                            {
                                if (f2.textContent.Text != string.Empty)
                                {
                                    try
                                    {
                                        Program.client = new Client(Program.serverAddress);
                                        if (!Communication.ChangePassword(Program.userLogin, oldPassword, f2.textContent.Text))
                                        {
                                            MessageBox.Show("Task failed.", "Error");
                                            f2.Dispose();
                                            changePasswordButton.PerformClick();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Password change successful.", "Success");
                                            //f.Dispose();
                                        }
                                        Program.client.Disconnect();
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("Server connection error.", "Error");
                                        f2.textContent.UseSystemPasswordChar = false;
                                        f2.Dispose();
                                    }
                                }
                            }
                        } 
                    }
                    else
                    {
                        MessageBox.Show("User name can't be empty.", "Error");
                        f.Dispose();
                        changePasswordButton.PerformClick();
                    }
                }
                f.textContent.UseSystemPasswordChar = false;
            }
        }
    }
}
