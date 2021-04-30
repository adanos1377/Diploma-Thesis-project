using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TalkaTIPClientV2
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
            string message = comm + " " + Program.security.EncryptMessage(key, login + " " +
                Security.hashPassword(password + login)) + " <EOF>";

            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static bool LogIn(string login, string password, byte[] key)
        {
            char comm = (char)1;
            string message = comm + " " + Program.security.EncryptMessage(key, login + " " +
                Security.hashPassword(password + login)) + " <EOF>";

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

        public static bool ChangePassword(string login, string oldPassword, string newPassword)
        {
            char comm = (char)4;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                login + " " + Security.hashPassword(oldPassword + login) + " " +
                Security.hashPassword(newPassword + login)) + " <EOF>";
            Program.client.Send(message);
            var ans = Program.client.Receive();
            return Response(ans[0]);
        }

        public static bool AddFriend(string login, string friendLogin)
        {
            char comm = (char)8;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                login + " " + friendLogin) + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static bool DelFriend(string login, string friendLogin)
        {
            char comm = (char)9;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                login + " " + friendLogin) + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static bool BlockUser(string login, string userToBlock)
        {
            char comm = (char)21;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                login + " " + userToBlock) + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static bool UnblockUser(string login, string userToUnblock)
        {
            char comm = (char)22;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                login + " " + userToUnblock) + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static void CallState(string callerLogin, string receiverLogin, DateTime date, TimeSpan callTime)
        {
            char comm = (char)11;
            string dateString = date.ToString("yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);
            string callTimeString = string.Format("{0:D2}:{1:D2}:{2:D2}", callTime.Hours, callTime.Minutes, callTime.Seconds);
            string message = Program.security.EncryptMessage(Program.sessionKeyWithServer,
                callerLogin + " " + receiverLogin + " " + dateString + " " + callTimeString);
            message = comm + " " + message + " <EOF>";
            Program.client.Send(message);
        }

        public static bool ChatMessage(string senderLogin, string receiverLogin, string chatMessage)
        {
            char comm = (char)20;
            string dateString = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);
            string message = Program.security.EncryptMessage(Program.sessionKeyWithServer,
                senderLogin + " " + receiverLogin + " " + dateString + " " + chatMessage);
            message = comm + " " + message + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
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

        public static void GetAllChatMessages(string senderLogin, string receiverLogin)
        {
            char comm = (char)23;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                senderLogin + " " + receiverLogin) + " <EOF>";
            Program.client.Send(message);
            message = Program.client.Receive();
            commFromServer(message.Substring(0, message.Length - 6));
        }

        public static bool CreateGroupChat(string chatName, string userLogin)
        {
            char comm = (char)24;
            string dateString = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                chatName + " " + userLogin + " " + dateString + " 0") + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        /// <summary>
        /// If the chat is already created, only adds user
        /// </summary>
        /// <param name="chatName"></param>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public static bool CreateAPIGroupChatAndAddUser(string chatName, string userLogin)
        {
            char comm = (char)24;
            string dateString = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);
            string message = Program.security.EncryptMessage(Program.sessionKeyWithServer,
                chatName + " " + userLogin + " " + dateString + " 1") + " <EOF>";
            Program.client.Send(comm + " " + message);
            bool response = Response(Program.client.Receive()[0]);

            if (response)
            {
                return true;
            }
            else
            {
                comm = (char)25;
                Program.client.Send(comm + " " + message);
                return Response(Program.client.Receive()[0]);
            }
        }

        public static bool AddUserToGroupChat(string chatName, string userLogin)
        {
            char comm = (char)25;
            string dateString = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                chatName + " " + userLogin + " " + dateString + " 0") + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static bool LeaveGroupChat(string chatName, string userLogin)
        {
            char comm = (char)26;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer,
                chatName + " " + userLogin) + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static bool GroupChatMessage(string senderLogin, string chatName, string chatMessage)
        {
            char comm = (char)27;
            string dateString = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);
            string message = Program.security.EncryptMessage(Program.sessionKeyWithServer,
                senderLogin + " " + chatName + " " + dateString + " " + chatMessage);
            message = comm + " " + message + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }


        public static void GetAllGroupChatMessages(string senderLogin, string receiverLogin)
        {
            char comm = (char)28;
            string message = comm + " " + Program.security.EncryptMessage(
                Program.sessionKeyWithServer, senderLogin + " " + receiverLogin) + " <EOF>";
            Program.client.Send(message);
            message = Program.client.Receive();
            commFromServer(message.Substring(0, message.Length - 6));
        }

        public static bool AddApiToUser(string senderLogin, string apiUri, string apiName)
        {
            char comm = (char)29;
            string message = comm + " " + Program.security.EncryptMessage(
                Program.sessionKeyWithServer,senderLogin + " " + apiUri + " " + apiName) + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static bool DeleteApiFromUser(string senderLogin, string apiUri)
        {
            char comm = (char)29;
            string message = comm + " " + Program.security.EncryptMessage(
                Program.sessionKeyWithServer, senderLogin + " " + apiUri) + " <EOF>";
            Program.client.Send(message);
            return Response(Program.client.Receive()[0]);
        }

        public static void Iam(string login)
        {
            char comm = (char)15;
            string message = comm + " " + Program.security.EncryptMessage(Program.sessionKeyWithServer, login) + " <EOF>";

            Program.client.Send(message);
            message = Program.client.Receive();
            commFromServer(message.Substring(0, message.Length - 6));
        }

        public static void commFromServer(string messageFromServer)
        {
            // Decipher the message
            int comm = (int)messageFromServer[0];
            string message;

            if (comm == 24 || comm == 27)
            {
                message = Program.security.DecryptMessage(messageFromServer.Substring(2,
                    messageFromServer.Length - 8), Program.sessionKeyWithServer);
            }
            else
            {
                message = Program.security.DecryptMessage(messageFromServer.Substring(2), Program.sessionKeyWithServer);
            }

            switch (comm)
            {
                case 7:
                    LogIP(message);
                    break;
                case 13:
                    History(message);
                    break;
                case 14:
                    StateChange(message);
                    break;
                case 19:
                    GroupChats(message);
                    break;
                case 24:
                    RecieveChatMessage(message);
                    break;
                case 25:
                    RecieveAllChatMessages(message);
                    break;
                case 27:
                    RecieveGroupChatMessage(message);
                    break;
                case 28:
                    RecieveAllGroupChatMessages(message);
                    break;
                case 31:
                    ApiList(message);
                    break;
                default:
                    break;
            }
        }

        private static void RecieveAllChatMessages(string recievedMessages)
        {
            Program.mainWindow.Invoke((MethodInvoker)delegate
            {
                if (!Program.loginAndMessage.ContainsKey(Program.mainWindow.listView1.SelectedItems[0].Text))
                {
                    Program.loginAndMessage.Add(Program.mainWindow.listView1.SelectedItems[0].Text, recievedMessages);
                    Program.mainWindow.UpdateChatText(recievedMessages);
                }
                else
                {
                    if (Program.loginAndMessage[Program.mainWindow.listView1.SelectedItems[0].Text] != recievedMessages)
                    {
                        Program.mainWindow.UpdateChatText(recievedMessages);
                    }
                }
            });
        }

        // Recieve and display the message
        private static void RecieveChatMessage(string chatMessage)
        {
            string[] param = chatMessage.Split(' ');

            string loginFrom = param[0];
            string loginTo = param[1];

            // Only recieve the messages meant for you
            if (loginTo == Program.userLogin)
            {
                DateTime msgSentTime = DateTime.ParseExact(param[2], "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);

                StringBuilder builder = new StringBuilder();
                for (int i = 3; i < param.Length - 1; i++)  // Ignore the <EOF>
                {
                    // Append each string to the StringBuilder overload
                    builder.Append(param[i]).Append(" ");
                }

                // Preparing the display format
                string message = builder.ToString();
                message = "\n" + loginFrom + " " + msgSentTime.ToString() + "\n" + message + "\n";

                // Saving messages to memory
                if (Program.loginAndMessage.ContainsKey(loginFrom))
                {
                    string temp = Program.loginAndMessage[loginFrom] + message;
                    Program.loginAndMessage[loginFrom] = temp;
                }
                else
                {
                    Program.loginAndMessage.Add(loginFrom, message);
                }

                // Display the messages or inform user
                Program.mainWindow.Invoke((MethodInvoker)delegate
                {
                    if (!(Program.mainWindow.listView1.SelectedItems.Count == 0 || Program.mainWindow.listView1.SelectedItems.Count > 1)
                        && Program.mainWindow.listView1.SelectedItems[0].Text == loginFrom)
                    {
                            Program.mainWindow.AllMessages.Text += message.Replace("\n", Environment.NewLine);
                    }
                    else
                    {
                        bool found = false;
                        int index = 0;
                        foreach (ListViewItem item in Program.mainWindow.listView1.Items)
                        {
                            if (item.Text == loginFrom)
                            {
                                item.ForeColor = Color.Red;
                                found = true;
                                Program.mainWindow.listView1.Refresh();
                                break;
                            }
                            index++;
                        }

                        if (!found)
                        {
                            string[] friendDetails = { loginFrom, "0" };
                            ListViewItem friend = new ListViewItem(friendDetails, 0);
                            friend.ForeColor = Color.Red;
                            Program.mainWindow.listView1.Items.Add(friend);
                            Program.mainWindow.listView1.Refresh();
                        }
                    }
                });
            }
        }

        private static void RecieveAllGroupChatMessages(string recievedMessages)
        {
            Program.mainWindow.Invoke((MethodInvoker)delegate
            {
                if (!Program.chatNameAndMessage.ContainsKey(Program.mainWindow.listViewGroups.SelectedItems[0].Text))
                {
                    Program.chatNameAndMessage.Add(Program.mainWindow.listViewGroups.SelectedItems[0].Text, recievedMessages);
                    Program.mainWindow.UpdateChatText(recievedMessages);
                }
                else
                {
                    if (Program.chatNameAndMessage[Program.mainWindow.listViewGroups.SelectedItems[0].Text] != recievedMessages)
                    {
                        Program.mainWindow.UpdateChatText(recievedMessages);
                    }
                }
            });
        }

        // Recieving messages in real time
        private static void RecieveGroupChatMessage(string chatMessage)
        {
            string[] param = chatMessage.Split(' ');

            string loginFrom = param[0];
            string chatName = param[1];

            if (loginFrom != Program.userLogin)
            {
                DateTime msgSentTime = DateTime.ParseExact(param[2], "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);

                StringBuilder builder = new StringBuilder();
                for (int i = 3; i < param.Length - 1; i++)  // Ignore the <EOF>
                {
                    // Append each string to the StringBuilder overload
                    builder.Append(param[i]).Append(" ");
                }

                // Preparing the display format
                string message = builder.ToString();
                message = "\n" + loginFrom + " " + msgSentTime.ToString() + "\n" + message + "\n";

                // Saving messages to memory
                if (Program.chatNameAndMessage.ContainsKey(chatName))
                {
                    string temp = Program.chatNameAndMessage[loginFrom] + message;
                    Program.chatNameAndMessage[loginFrom] = temp;
                }
                else
                {
                    Program.chatNameAndMessage.Add(chatName, message);
                }

                // Display the messages or inform user
                Program.mainWindow.Invoke((MethodInvoker)delegate
                {
                    if (!(Program.mainWindow.listViewGroups.SelectedItems.Count == 0 || Program.mainWindow.listViewGroups.SelectedItems.Count > 1)
                        && Program.mainWindow.listViewGroups.SelectedItems[0].Text == chatName)
                    {
                        Program.mainWindow.AllMessages.Text += message.Replace("\n", Environment.NewLine); ;
                    }
                    else
                    {
                        bool found = false;
                        int index = 0;
                        foreach (ListViewItem item in Program.mainWindow.listViewGroups.Items)
                        {
                            if (item.Text == chatName)
                            {
                                item.ForeColor = Color.Red;
                                found = true;
                                Program.mainWindow.listViewGroups.Refresh();
                                break;
                            }
                            index++;
                        }

                        if (!found)
                        {
                            string[] chatDetails = { chatName, "0" };
                            ListViewItem chat = new ListViewItem(chatDetails, 0);
                            chat.ForeColor = Color.Red;
                            Program.mainWindow.listViewGroups.Items.Add(chat);
                            Program.mainWindow.listViewGroups.Refresh();
                        }
                    }
                });
            }
        }

        private static void LogIP(string messageFromServer)
        {
            string[] friends = messageFromServer.Split(' ');
            ListViewItem friend;
            for (int i = 0; i < friends.Length - 1; i += 3)
            {
                // friends[i] login
                // friends[i+1] status
                // friends[i+2] IP
                string[] friendDetails = { friends[i], friends[i + 2] };
                if (friends[i + 1] == "0") // Unavailable
                {

                    friend = new ListViewItem(friendDetails, 0);
                }
                else
                {
                    friend = new ListViewItem(friendDetails, 1);
                }
                Program.mainWindow.Invoke((MethodInvoker)delegate
                {
                    Program.mainWindow.listView1.Items.Add(friend);
                });
            }
        }

        private static void History(string messageFromServer)
        {
            string[] history = messageFromServer.Split(' ');
            string[] historyDetails = new string[3];
            for (int i = 0; i < history.Length - 1; i += 5)
            {
                historyDetails[0] = history[i];
                historyDetails[1] = history[i + 1] + " " + history[i + 2];
                historyDetails[2] = history[i + 4] == "00:00:00" ? "missed call" : history[i + 4];
                Program.mainWindow.Invoke((MethodInvoker)delegate
                {
                    Program.mainWindow.listView2.Items.Insert(0, new ListViewItem(historyDetails));
                });
            }
        }

        private static void GroupChats(string messageFromServer)
        {
            string[] chatNames = messageFromServer.Split(' ');
            string[] chatDetails = new string[2];
            for(int i = 0; i < chatNames.Length - 1; i += 2)
            {
                chatDetails[0] = chatNames[i];
                chatDetails[1] = chatNames[i + 1];
                Program.mainWindow.Invoke((MethodInvoker)delegate
                {
                    Program.mainWindow.listViewGroups.Items.Insert(0, new ListViewItem(chatDetails));
                });
            }
        }

        private static void ApiList(string messageFromServer)
        {
            string[] apiNameUri = messageFromServer.Split(' ');
            string[] apiDetails = new string[2];
            for (int i = 0; i < apiNameUri.Length - 1; i += 2)
            {
                apiDetails[0] = apiNameUri[i];
                apiDetails[1] = apiNameUri[i + 1];
                Program.apiNameAndHandle[apiDetails[0]] = new APIHandle(apiDetails[1]);
                Program.mainWindow.Invoke((MethodInvoker)delegate
                {
                    Program.mainWindow.gameAPIListView.Items.Insert(0, new ListViewItem(apiDetails));
                });
            }
        }

        private static void StateChange(string messageFromServer)
        {
            string[] friends = messageFromServer.Split(' ');
            for (int i = 0; i < friends.Length - 1; i += 2)
            {
                int index = Program.mainWindow.listView1.FindItemWithText(friends[i]).Index;
                Program.mainWindow.listView1.Items[index].SubItems[1].Text = friends[i + 1];
                if (friends[i + 1] == "0")
                {
                    Program.mainWindow.listView1.Items[index].ImageIndex = 0;
                }
                else
                {
                    Program.mainWindow.listView1.Items[index].ImageIndex = 1;
                }
            }
            Program.mainWindow.listView1.Refresh();
        }
    }
}
