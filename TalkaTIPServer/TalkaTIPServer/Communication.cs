using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using TalkaTIPServer;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Threading;

namespace TalkaTIPSerwer
{
    class Communication
    {
        static Dictionary<int, Delegate> communiqueDictionary = new Dictionary<int, Delegate>();

        public static void AddDelegateToDictionary()
        {
            // in, out
            communiqueDictionary[0] = new Func<List<string>, string>(Register);
            communiqueDictionary[1] = new Func<List<string>, byte[], Socket, string>(LogIn);
            communiqueDictionary[2] = new Func<List<string>, string>(LogOut);
            //communiqueDictionary[3] = new Func<List<string>, string>(DeleteAccount);
            communiqueDictionary[4] = new Func<List<string>, string>(ChangePassword);
            communiqueDictionary[8] = new Func<List<string>, string>(AddFriend);
            communiqueDictionary[9] = new Func<List<string>, string>(DelFriend);
            communiqueDictionary[11] = new Func<List<string>, string>(CallState);
            communiqueDictionary[13] = new Func<long, string>(History);
            communiqueDictionary[15] = new Func<List<string>, string>(Iam);
            communiqueDictionary[20] = new Func<List<string>, string>(ChatMessage);
            communiqueDictionary[21] = new Func<List<string>, string>(BlockUser);
            communiqueDictionary[22] = new Func<List<string>, string>(UnblockUser);
            communiqueDictionary[23] = new Func<List<string>, string>(GetChatMessages);
            communiqueDictionary[24] = new Func<List<string>, string>(CreateGroupChat);
            communiqueDictionary[25] = new Func<List<string>, string>(AddUserToGroupChat);
            communiqueDictionary[26] = new Func<List<string>, string>(LeaveGroupChat);
            communiqueDictionary[27] = new Func<List<string>, string>(GroupChatMessage);
            communiqueDictionary[28] = new Func<List<string>, string>(GetAllGroupChatMessages);
            communiqueDictionary[29] = new Func<List<string>, string>(AddApiToUser);
            communiqueDictionary[30] = new Func<List<string>, string>(DeleteApiFromUser);
        }

        // Incoming messages
        public static string Register(List<string> param)
        {
            // Communication with DB
            using (var ctx = new TalkaTipDB())
            {
                var newUser = new Users();
                string login = param[0]; // login

                int loginUnique = ctx.Users.Where(x => x.Login == login).Count(); // Check if the login is unique
                if (loginUnique == 0)
                {
                    newUser.Login = login;
                    newUser.Password = param[1];         
                    newUser.LastLogoutDate = DateTime.Now;
                    newUser.RegistrationDate = DateTime.Now;

                    ctx.Users.Add(newUser);
                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Fail(); ;
                    }
                }
                else
                {
                    return Fail();
                }
            }
            return OK();
        }

        public static string LogIn(List<string> param, byte[] sessionKey, Socket client)
        {
            string login = param[0];

            string password = param[1];

            // users -> online
            using (var ctx = new TalkaTipDB())
            {
                var user = ctx.Users.Where(x => x.Login == login && x.Password == password).FirstOrDefault();
                if (user != null)
                {
                    if (Program.onlineUsers.ContainsKey(user.UserID))
                    {
                        return Fail();
                    }
                    ClientClass connectedClient = new ClientClass();
                    connectedClient.login = login;
                    connectedClient.iAM = DateTime.Now;
                    connectedClient.addressIP = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();

                    connectedClient.sessionKey = sessionKey;
                    connectedClient.friendWithStateDict = new Dictionary<string, string>();
                    connectedClient.blockedUsersDict = new Dictionary<string, string>();

                    // Add to dictionary
                    Program.onlineUsers[user.UserID] = connectedClient;
                    
                    // Find people who have friend that is logged in
                    IQueryable<long> friends = ctx.Friends.Where(x => x.UserID2 == user.UserID).Select(x => x.UserID1);

                    // Find friends in the online dict and add/change the IP address
                    foreach (long item in friends)
                    {
                        if (Program.onlineUsers.ContainsKey(item))
                        {
                            Program.onlineUsers[item].friendWithStateDict[login] = connectedClient.addressIP;
                        }
                    }

                    IQueryable<long> blocked = ctx.Blocked.Where(x => x.UserID2 == user.UserID).Select(x => x.UserID1);
                    
                    foreach (long item in blocked)
                    {
                        if (Program.onlineUsers.ContainsKey(item))
                        {
                            Program.onlineUsers[item].blockedUsersDict[login] = connectedClient.addressIP;
                        }
                    }

                    return OK();
                }
                else
                {
                    return Fail();
                }
            }
        }

        public static string LogOut(List<string> param)
        {
            string login = param[0];

            using (var ctx = new TalkaTipDB())
            {
                var userToLogOut = ctx.Users.Where(x => x.Login == login).FirstOrDefault();
                if (userToLogOut != null)
                {
                    userToLogOut.LastLogoutDate = DateTime.Now;
                    try
                    {
                        Program.onlineUsers.Remove(userToLogOut.UserID); // Delete from the dictionary
                        ctx.Entry(userToLogOut).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();

                        // Find people who have friend that is logged in
                        IQueryable<long> friends = ctx.Friends.Where(x => x.UserID2 == userToLogOut.UserID).Select(x => x.UserID1);

                        // Find friends in the online dict and add/change the IP address
                        foreach (long item in friends)
                        {
                            if (Program.onlineUsers.ContainsKey(item))
                            {
                                Program.onlineUsers[item].friendWithStateDict[login] = "0";
                            }
                        }

                        IQueryable<long> blocked = ctx.Blocked.Where(x => x.UserID2 == userToLogOut.UserID).Select(x => x.UserID1); ;

                        foreach (long item in blocked)
                        {
                            if (Program.onlineUsers.ContainsKey(item))
                            {
                                Program.onlineUsers[item].blockedUsersDict[login] = "0";
                            }
                        }

                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
            return OK();
        }

        public static string ChangePassword(List<string> param)
        {
            string login = param[0];
            string password1 = param[1];
            using (var ctx = new TalkaTipDB())
            {
                var userToChngPasswd = ctx.Users.Where(x => x.Login == login && x.Password == password1).FirstOrDefault();
                if (userToChngPasswd != null)
                {
                    userToChngPasswd.Password = param[2];
                    try
                    {
                        ctx.Entry(userToChngPasswd).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
            return OK();
        }

        public static string AddFriend(List<string> param)
        {
            string login1 = param[0]; // User logged in
            string login2 = param[1]; // Friend of the logged in user
            using (var ctx = new TalkaTipDB())
            {
                var acquaintance = new Friends();

                // Find the proper users IDs
                var userID1 = ctx.Users.Where(x => x.Login == login1).Select(x => x.UserID).FirstOrDefault();
                if (userID1 != 0)
                {
                    var user2 = ctx.Users.Where(x => x.Login == login2).Select(x => new { x.UserID, x.Login }).FirstOrDefault();
                    if (user2 != null)
                    {
                        var acq = ctx.Friends.Where(x => x.UserID1 == userID1 && x.UserID2 == user2.UserID).FirstOrDefault();
                        if (acq == null)
                        {
                            var block = ctx.Blocked.Where(x => x.UserID1 == userID1 && x.UserID2 == user2.UserID).FirstOrDefault();
                            if (block == null)
                            {
                                acquaintance.UserID1 = userID1;
                                acquaintance.UserID2 = user2.UserID;
                                try
                                {
                                    ctx.Friends.Add(acquaintance);
                                    ctx.SaveChanges();
                                    string acqLogin = user2.Login;
                                    if (Program.onlineUsers.ContainsKey(user2.UserID))
                                    {
                                        Program.onlineUsers[userID1].friendWithStateDict.Add(acqLogin, Program.onlineUsers[user2.UserID].addressIP);
                                    }
                                    else
                                    {
                                        Program.onlineUsers[userID1].friendWithStateDict.Add(acqLogin, "0");
                                    }

                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return Fail();
                                }
                            }
                            else
                            {
                                Blocked blockedUser = ctx.Blocked.Where(x => x.UserID1 == userID1 && x.UserID2 == user2.UserID).FirstOrDefault();
                                ctx.Entry(blockedUser).State = System.Data.Entity.EntityState.Deleted;
                                try
                                {
                                    ctx.SaveChanges();
                                    Program.onlineUsers[userID1].blockedUsersDict.Remove(user2.Login);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return Fail();
                                }
                                acquaintance.UserID1 = userID1;
                                acquaintance.UserID2 = user2.UserID;
                                try
                                {
                                    ctx.Friends.Add(acquaintance);
                                    ctx.SaveChanges();
                                    string acqLogin = user2.Login;
                                    if (Program.onlineUsers.ContainsKey(user2.UserID))
                                    {
                                        Program.onlineUsers[userID1].friendWithStateDict.Add(acqLogin, Program.onlineUsers[user2.UserID].addressIP);
                                    }
                                    else
                                    {
                                        Program.onlineUsers[userID1].friendWithStateDict.Add(acqLogin, "0");
                                    }

                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return Fail();
                                }
                            }
                        }
                        else
                        {
                            return Fail();
                        }
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
            return OK();
        }

        public static string DelFriend(List<string> param)
        {
            string login1 = param[0]; // Logged in user
            string login2 = param[1];

            using (var ctx = new TalkaTipDB())
            {
                var userLoggedInID1 = ctx.Users.Where(x => x.Login == login1).Select(x => x.UserID).FirstOrDefault();
                if (userLoggedInID1 != 0)
                {
                    var user1Friend = ctx.Users.Where(x => x.Login == login2).Select(x => new { x.UserID, x.Login }).FirstOrDefault();
                    if (user1Friend.UserID != 0)
                    {
                        var friend = ctx.Friends.Where(x => x.UserID1 == userLoggedInID1 && x.UserID2 == user1Friend.UserID).FirstOrDefault();
                        if (friend != null)
                        {
                            ctx.Entry(friend).State = System.Data.Entity.EntityState.Deleted;
                            try
                            {
                                ctx.SaveChanges();
                                Program.onlineUsers[userLoggedInID1].friendWithStateDict.Remove(user1Friend.Login);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Fail();
                            }
                        }
                        else
                        {
                            friend = ctx.Friends.Where(x => x.UserID1 == user1Friend.UserID && x.UserID2 == userLoggedInID1).FirstOrDefault();
                            if (friend != null)
                            {
                                ctx.Entry(friend).State = System.Data.Entity.EntityState.Deleted;
                                try
                                {
                                    ctx.SaveChanges();
                                    Program.onlineUsers[userLoggedInID1].friendWithStateDict.Remove(user1Friend.Login);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    return Fail();
                                }
                            }
                        }
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
            return OK();
        }

        public static string BlockUser(List<string> param)
        {
            string login1 = param[0]; // User logged in
            string login2 = param[1]; // Friend of the logged in user
            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                Blocked block = new Blocked();
                // Find the proper users IDs
                long userID1 = ctx.Users.Where(x => x.Login == login1).Select(x => x.UserID).FirstOrDefault();
                if (userID1 != 0)
                {
                    var user2 = ctx.Users.Where(x => x.Login == login2).Select(x => new { x.UserID, x.Login }).FirstOrDefault();
                    if (user2 != null)
                    {
                        var blck = ctx.Blocked.Where(x => x.UserID1 == userID1 && x.UserID2 == user2.UserID).FirstOrDefault();
                        if (blck == null)
                        {
                            block.UserID1 = userID1;
                            block.UserID2 = user2.UserID;
                            try
                            {
                                ctx.Blocked.Add(block);
                                ctx.SaveChanges();
                                string blockedLogin = user2.Login;

                                if (Program.onlineUsers.ContainsKey(user2.UserID))
                                {
                                    Program.onlineUsers[userID1].blockedUsersDict.Add(blockedLogin, Program.onlineUsers[user2.UserID].addressIP);
                                }
                                else
                                {
                                    Program.onlineUsers[userID1].blockedUsersDict.Add(blockedLogin, "0");
                                }
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Fail();
                            }
                        }
                        else
                        {
                            return Fail();
                        }
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
            return OK();
        }

        public static string UnblockUser(List<string> param)
        {
            string login1 = param[0]; // Logged in user
            string login2 = param[1];

            using (var ctx = new TalkaTipDB())
            {
                var userLoggedInID1 = ctx.Users.Where(x => x.Login == login1).Select(x => x.UserID).FirstOrDefault();
                if (userLoggedInID1 != 0)
                {
                    var user1Blocked = ctx.Users.Where(x => x.Login == login2).Select(x => new { x.UserID, x.Login }).FirstOrDefault();
                    if (user1Blocked.UserID != 0)
                    {
                        Blocked blockedUser = ctx.Blocked.Where(x => x.UserID1 == userLoggedInID1 && x.UserID2 == user1Blocked.UserID).FirstOrDefault();
                        ctx.Entry(blockedUser).State = System.Data.Entity.EntityState.Deleted;
                        try
                        {
                            ctx.SaveChanges();
                            Program.onlineUsers[userLoggedInID1].blockedUsersDict.Remove(user1Blocked.Login);
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return Fail();
                        }
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
            return OK();
        }

        public static string CallState(List<string> param)
        {
            string login1 = param[0];
            string login2 = param[1];

            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                long SenderID = ctx.Users.Where(x => x.Login == login1).Select(x => x.UserID).FirstOrDefault();
                if (SenderID != 0)
                {
                    long ReceiverID = ctx.Users.Where(x => x.Login == login2).Select(x => x.UserID).FirstOrDefault();
                    if (ReceiverID != 0)
                    {
                        Histories history = new Histories();
                        history.UserSenderID = SenderID;
                        history.UserReceiverID = ReceiverID;
                        
                        //yyyy-MM-dd-HH:mm:ss
                        history.Start = DateTime.ParseExact(param[2], "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);

                        //HH:mm:ss
                        history.Duration = new DateTime().Add(TimeSpan.Parse(param[3]));
                        try
                        {
                            ctx.Histories.Add(history);
                            ctx.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return Fail();
                        }
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
            return OK();
        }

        public static string Iam(List<string> param)
        {
            string login = string.Empty;
            login = param[0];

            // Check if the user exists
            using (var ctx = new TalkaTipDB())
            {
                var user = ctx.Users.Where(x => x.Login == login).FirstOrDefault();
                if (user != null)
                {
                    if (Program.onlineUsers.ContainsKey(user.UserID))
                    {
                        Program.onlineUsers[user.UserID].iAM = DateTime.Now;
                        return StateChange(user.UserID);
                    }
                }
            }
            return " <EOF>";
        }

        // Function for recieving and saving chat messages between users
        public static string ChatMessage(List<string> param)
        {
            string loginFrom = param[0];
            string loginTo = param[1];
            DateTime msgSentTime = DateTime.ParseExact(param[2], "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);

            StringBuilder builder = new StringBuilder();
            for(int i = 3; i < param.Count; i++)
            {
                // Append each string to the StringBuilder overload.
                builder.Append(param[i]).Append(" ");
            }
            string message = builder.ToString();

            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                long SenderID = ctx.Users.Where(x => x.Login == loginFrom).Select(x => x.UserID).FirstOrDefault();
                if (SenderID != 0)
                {
                    long ReceiverID = ctx.Users.Where(x => x.Login == loginTo).Select(x => x.UserID).FirstOrDefault();
                    if (ReceiverID != 0)
                    {
                        Messages msg = new Messages();
                        msg.UserSenderID = SenderID;
                        msg.UserReceiverID = ReceiverID;
                        msg.SendTime = DateTime.ParseExact(param[2], "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);
                        msg.Message = message;

                        try
                        {
                            ctx.Messages.Add(msg);
                            ctx.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return Fail();
                        }
                        var blck = ctx.Blocked.Where(x => x.UserID1 == ReceiverID && x.UserID2 == SenderID).FirstOrDefault();
                        if (blck == null)
                        {
                            // Send the message to reciever
                            if(Program.onlineUsers.ContainsKey(ReceiverID))
                            {
                                ThreadStart work = delegate {
                                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(Program.onlineUsers[ReceiverID].addressIP), 14450);
                                    builder = new StringBuilder();
                                    for (int i = 0; i < param.Count; i++)
                                    {
                                        // Append each string to the StringBuilder overload.
                                        builder.Append(param[i]).Append(" ");
                                    }
                                    message = builder.ToString();

                                    Console.WriteLine("SEND MESSAGE: {0}", message);

                                    message = Program.security.EncryptMessage(Program.onlineUsers[SenderID].sessionKey, message);
                                    message = ((char)24).ToString() + ' ' + message;
                                    message += " <EOF>";

                                    Thread.Sleep(200);
                                    AsynchronousServer.SendMessage(message, endPoint);
                                };
                                new Thread(work).Start();
                                
                            }
                            return OK();
                        }
                        else
                        {
                            return Fail();
                        }
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
        }

        // Return all chat messages with a user in a format ready to display
        public static string GetChatMessages(List<string> param)
        {
            string allChatMessages = string.Empty;
            string loginFrom = param[0];
            string loginTo = param[1];

            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                long User1ID = ctx.Users.Where(x => x.Login == loginFrom).Select(x => x.UserID).FirstOrDefault();
                if (User1ID != 0)
                {
                    long User2ID = ctx.Users.Where(x => x.Login == loginTo).Select(x => x.UserID).FirstOrDefault();
                    if (User2ID != 0)
                    {
                        var block = ctx.Blocked.Where(x => x.UserID1 == User1ID && x.UserID2 == User2ID).FirstOrDefault();
                        if (block == null)
                        {
                            var selectedRows = ctx.Messages
                                .Where(x => (x.UserReceiverID == User1ID && x.UserSenderID == User2ID)
                                || (x.UserReceiverID == User2ID && x.UserSenderID == User1ID)).Select(x => x);

                            if (selectedRows != null)
                            {
                                foreach (Messages msg in selectedRows)
                                {
                                    if (msg.UserSenderID == User1ID)
                                    {
                                        allChatMessages = allChatMessages + "\n" + loginFrom + " " + msg.SendTime.ToString() + "\n" + msg.Message + "\n";
                                    }
                                    else
                                    {
                                        allChatMessages = allChatMessages + "\n" + loginTo + " " + msg.SendTime.ToString() + "\n" + msg.Message + "\n";
                                    }
                                }

                                allChatMessages = Program.security.EncryptMessage(
                                    Program.onlineUsers[User1ID].sessionKey, allChatMessages);
                                allChatMessages = ((char)25).ToString() + ' ' + allChatMessages;
                                allChatMessages += " <EOF>";

                                return allChatMessages;
                            }
                            else
                            {
                                return Fail();
                            }
                        }
                        else
                        {
                            allChatMessages = "Blocked chat.";
                            allChatMessages = Program.security.EncryptMessage(
                                    Program.onlineUsers[User1ID].sessionKey, allChatMessages);
                            allChatMessages = ((char)25).ToString() + ' ' + allChatMessages;
                            allChatMessages += " <EOF>";

                            return allChatMessages;
                        }
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
        }


        // Creating a group chat (also adds the user who created it)
        public static string CreateGroupChat(List<string> param)
        {
            string chatName = param[0];
            string chatCreatorUN = param[1];    // UN = user name

            // param[3] = is API controlled, param[2] = date time

            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                int nameUnique = ctx.GroupChat.Where(x => x.GroupChatName == chatName).Count(); // Check if the name is unique

                if (nameUnique == 0)
                {
                    long creatorID = ctx.Users.Where(x => x.Login == chatCreatorUN).Select(x => x.UserID).FirstOrDefault();
                    if (creatorID != 0)
                    {
                        GroupChat groupChat = new GroupChat();
                        groupChat.GroupChatName = chatName;
                        groupChat.IsApiControlled = int.Parse(param[3]);

                        try
                        {
                            ctx.GroupChat.Add(groupChat);
                            ctx.SaveChanges();
                            return AddUserToGroupChat(param);
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return Fail();
                        }
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
        }

        // Adding a user to group chat
        public static string AddUserToGroupChat(List<string> param)
        {
            string chatName = param[0];
            string userToAdd = param[1];
            int isApiControlled = int.Parse(param[3]);

            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                long userID = ctx.Users.Where(x => x.Login == userToAdd).Select(x => x.UserID).FirstOrDefault();
                GroupChat chat = ctx.GroupChat.Where(x => x.GroupChatName == chatName).FirstOrDefault();
                if (chat.IsApiControlled == isApiControlled)
                {
                    if (userID != 0 && chat.GroupChatID != 0 && chat.IsApiControlled == 0)
                    {
                        // Check if the user is already inside
                        int isInChat = ctx.GroupChatUsers.Where(x => x.UserInChatID == userID && x.JoinedGroupChatID == chat.GroupChatID).Count();

                        if (isInChat == 0)
                        {
                            GroupChatUsers joiningUser = new GroupChatUsers();
                            joiningUser.JoinTime = DateTime.ParseExact(param[2], "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);
                            joiningUser.JoinedGroupChatID = chat.GroupChatID;
                            joiningUser.UserInChatID = userID;

                            try
                            {
                                ctx.GroupChatUsers.Add(joiningUser);
                                ctx.SaveChanges();
                                return OK();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Fail();
                            }
                        }
                        else
                        {
                            return Fail();
                        }

                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
        }

        // Leaving a group chat (deletes the chat if everyone leaves)
        public static string LeaveGroupChat(List<string> param)
        {
            string chatName = param[0];
            string userLeaving = param[1];

            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                long leavingID = ctx.Users.Where(x => x.Login == userLeaving).Select(x => x.UserID).FirstOrDefault();
                long chatID = ctx.GroupChat.Where(x => x.GroupChatName == chatName).Select(x => x.GroupChatID).FirstOrDefault();

                if (leavingID != 0 && chatID != 0)
                {
                    GroupChatUsers removedUser = ctx.GroupChatUsers
                        .Where(x => x.JoinedGroupChatID == chatID && x.UserInChatID == leavingID).FirstOrDefault();

                    try
                    {
                        ctx.GroupChatUsers.Remove(removedUser);
                        ctx.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Fail();
                    }

                    // Remove the chat if it's empty
                    int isChatEmpty = ctx.GroupChatUsers.Where(x => x.JoinedGroupChatID == chatID).Count();
                    if(isChatEmpty == 0)
                    {
                        GroupChat chatToRemove = ctx.GroupChat.Where(x => x.GroupChatID == chatID).FirstOrDefault();

                        try
                        {
                            ctx.GroupChat.Remove(chatToRemove);
                            ctx.SaveChanges();
                        }
                        catch(DbUpdateConcurrencyException)
                        {
                            return Fail();
                        }
                    }

                    return OK();
                }
                else
                {
                    return Fail();
                }
            }
        }

        public static string GroupChatMessage(List<string> param)
        {
            string loginFrom = param[0];
            string chatName = param[1];
            DateTime msgSentTime = DateTime.ParseExact(param[2], "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture);

            StringBuilder builder = new StringBuilder();
            for (int i = 3; i < param.Count; i++)
            {
                // Append each string to the StringBuilder overload.
                builder.Append(param[i]).Append(" ");
            }
            string message = builder.ToString();

            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                long senderID = ctx.Users.Where(x => x.Login == loginFrom).Select(x => x.UserID).FirstOrDefault();
                if (senderID != 0)
                {
                    long chatID = ctx.GroupChat.Where(x => x.GroupChatName == chatName).Select(x => x.GroupChatID).FirstOrDefault();
                    if(chatID != 0)
                    {
                        GroupChatMessages chatMessage = new GroupChatMessages();
                        chatMessage.ChatID = chatID;
                        chatMessage.UserSenderID = senderID;
                        chatMessage.SendTime = msgSentTime;
                        chatMessage.Message = message;

                        try
                        {
                            ctx.GroupChatMessages.Add(chatMessage);
                            ctx.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return Fail();
                        }

                        // Send the message to all online users in the chat
                        var allChatUsers = ctx.GroupChatUsers.Where(x => x.JoinedGroupChatID == chatID);

                        foreach(var user in allChatUsers)
                        {
                            // Send the message to reciever
                            if (Program.onlineUsers.ContainsKey(user.UserInChatID))
                            {
                                try
                                {
                                    ThreadStart work = delegate {
                                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(Program.onlineUsers[user.UserInChatID].addressIP), 14450);
                                        builder = new StringBuilder();
                                        for (int i = 0; i < param.Count; i++)
                                        {
                                            // Append each string to the StringBuilder overload.
                                            builder.Append(param[i]).Append(" ");
                                        }
                                        message = chatName + " " + builder.ToString();

                                        message = Program.security.EncryptMessage(Program.onlineUsers[senderID].sessionKey, message);
                                        message = ((char)27).ToString() + ' ' + message;
                                        message += " <EOF>";

                                        Thread.Sleep(200);
                                        AsynchronousServer.SendMessage(message, endPoint);
                                    };
                                    new Thread(work).Start();
                                }
                                catch(Exception)
                                {
                                    return Fail();
                                }
                            }
                        }
                        return OK();
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
        }

        public static string GetAllGroupChatMessages(List<string> param)
        {
            string allChatMessages = string.Empty;
            string loginFrom = param[0];
            string chatName = param[1];

            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                long userID = ctx.Users.Where(x => x.Login == loginFrom).Select(x => x.UserID).FirstOrDefault();
                if (userID != 0)
                {
                    long chatID = ctx.GroupChat.Where(x => x.GroupChatName == chatName).Select(x => x.GroupChatID).FirstOrDefault();
                    if (chatID != 0)
                    {
                        var selectedRows = ctx.GroupChatMessages.Where(x => x.ChatID == chatID);

                        if (selectedRows != null)
                        {
                            foreach (GroupChatMessages msg in selectedRows)
                            {
                                string senderName = ctx.Users.Where(x => x.UserID == msg.UserSenderID).Select(x => x.Login).FirstOrDefault();
                                allChatMessages = allChatMessages + "\n" + senderName + " " + msg.SendTime.ToString() + "\n" + msg.Message + "\n";
                            }

                            allChatMessages = Program.security.EncryptMessage(
                                Program.onlineUsers[userID].sessionKey, allChatMessages);
                            allChatMessages = ((char)28).ToString() + ' ' + allChatMessages;
                            allChatMessages += " <EOF>";

                            return allChatMessages;
                        }
                        else
                        {
                            return Fail();
                        }
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
        }

        public static string AddApiToUser(List<string> param)
        {
            string login = param[0];
            string apiUri = param[1];
            string apiName = param[2];
            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                long userID = ctx.Users.Where(x => x.Login == login).Select(x => x.UserID).FirstOrDefault();
                if (userID != 0)
                {
                    UsersAPIs usersAPI = new UsersAPIs();
                    usersAPI.CorrelatedUserID = userID;
                    usersAPI.ApiUri = apiUri;
                    usersAPI.ApiName = apiName;

                    try
                    {
                        ctx.UsersAPIs.Add(usersAPI);
                        ctx.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Fail();
                    }

                    return OK();
                }
                else
                {
                    return Fail();
                }
            }
        }

        public static string DeleteApiFromUser(List<string> param)
        {
            string login = param[0];
            string apiUri = param[1];

            using(TalkaTipDB ctx = new TalkaTipDB())
            {
                long userID = ctx.Users.Where(x => x.Login == login).Select(x => x.UserID).FirstOrDefault();
                if (userID != 0)
                {
                    UsersAPIs apiToRemove = ctx.UsersAPIs.Where(
                        x => x.CorrelatedUserID == userID && x.ApiUri == apiUri).FirstOrDefault();

                    if(apiToRemove != null && apiToRemove.ApiUri == apiUri)
                    {
                        try
                        {
                            ctx.UsersAPIs.Remove(apiToRemove);
                            ctx.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return Fail();
                        }

                        return OK();
                    }
                    else
                    {
                        return Fail();
                    }
                }
                else
                {
                    return Fail();
                }
            }
        }

        // Outgoing messages
        public static string OK()
        {
            Console.WriteLine("OK");
            return ((char)5).ToString() + " <EOF>";
        }

        public static string Fail()
        {
            Console.WriteLine("Fail");
            return ((char)6).ToString() + " <EOF>";

        }

        public static string LogIP(long userID)
        {
            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                var user = ctx.Users.Where(x => x.UserID == userID).FirstOrDefault();
                if (user != null)
                {
                    string message = string.Empty;

                    // 0111 login_1 status_1 IP_1 login_2 status_2 IP_2...login_n status_n IP_n)
                    var friends = ctx.Friends.Where(x => x.UserID1 == userID);  // Returns all friends the user added

                    foreach (var item in friends)
                    {
                        var friendLogin = ctx.Users.Where(x => x.UserID == item.UserID2).Select(x => x.Login).FirstOrDefault();
                        if (friendLogin != null)
                        {
                            message += friendLogin + " ";
                            if (Program.onlineUsers.ContainsKey(item.UserID2))
                            {
                                message += "1 ";
                                message += Program.onlineUsers[item.UserID2].addressIP + " ";
                            }
                            else
                            {
                                message += "0 ";
                                message += "0 ";
                            }
                        }
                        else
                        {
                            return Fail();
                        }
                    }

                    friends = ctx.Friends.Where(x => x.UserID2 == userID);  // Returns all friends the user added

                    foreach (var item in friends)
                    {
                        var friendLogin = ctx.Users.Where(x => x.UserID == item.UserID1).Select(x => x.Login).FirstOrDefault();

                        var blck = ctx.Blocked.Where(x => x.UserID1 == userID && x.UserID2 == item.UserID1).FirstOrDefault();
                        if (blck == null) // Skip if the user was blocked
                        {
                            if (friendLogin != null)
                            {
                                if (!message.Contains(friendLogin)) // Skip if the user added both sides
                                {
                                    message += friendLogin + " ";
                                    if (Program.onlineUsers.ContainsKey(item.UserID1))
                                    {
                                        message += "1 ";
                                        message += Program.onlineUsers[item.UserID1].addressIP + " ";
                                    }
                                    else
                                    {
                                        message += "0 ";
                                        message += "0 ";
                                    }
                                }
                            }
                            else
                            {
                                return Fail();
                            }
                        }
                    }

                    message = Program.security.EncryptMessage(Program.onlineUsers[userID].sessionKey, message);
                    message = ((char)7).ToString() + ' ' + message;
                    message += " <EOF>";
                    return message;
                }
                else
                {
                    return Fail();
                }
            }
        }

        public static string History(long userID)
        {
            string history = string.Empty;
            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                var histories = ctx.Histories.Where(x => x.UserSenderID == userID || x.UserReceiverID == userID).OrderBy(x => x.Start).ToList();
                if (histories.Count != 0)
                {
                    foreach (var item in histories)
                    {
                        if (item.UserSenderID == userID)    // userID is the sender
                        {
                            var friendLoginR = ctx.Users.Where(x => x.UserID == item.UserReceiverID).Select(x => x.Login).FirstOrDefault();
                            if (friendLoginR != null)
                            {
                                history += friendLoginR + " " + item.Start.ToString() + " " + item.Duration.ToString() + " ";
                            }
                        }
                        else // userID is the receiver
                        {
                            var friendLoginS = ctx.Users.Where(x => x.UserID == item.UserSenderID).Select(x => x.Login).FirstOrDefault();
                            if (friendLoginS != null)
                            {
                                history += friendLoginS + " " + item.Start.ToString() + " " + item.Duration.ToString() + " ";
                            }
                        }
                    }
                }
                history = Program.security.EncryptMessage(Program.onlineUsers[userID].sessionKey, history);
                history = ((char)13).ToString() + ' ' + history;
                return history + " <EOF>";
            }
        }

        public static string GroupChats(long userID)
        {
            string message = string.Empty;

            using(TalkaTipDB ctx = new TalkaTipDB())
            {
                var groupChats = ctx.GroupChatUsers.Where(x => x.UserInChatID == userID);
                foreach(var chatUser in groupChats)
                {
                    var chat = ctx.GroupChat.Where(x => x.GroupChatID == chatUser.JoinedGroupChatID).FirstOrDefault();
                    message += chat.GroupChatName + " " + chat.IsApiControlled + " ";
                }
            }

            message = Program.security.EncryptMessage(Program.onlineUsers[userID].sessionKey, message);
            message = (char)19 + " " + message;
            message += " <EOF>";
            return message;
        }

        public static string ApiList(long userID)
        {
            string message = string.Empty;

            using (TalkaTipDB ctx = new TalkaTipDB())
            {
                var apiList = ctx.UsersAPIs.Where(x => x.CorrelatedUserID == userID);
                foreach (var api in apiList)
                {
                    message += api.ApiName + " " + api.ApiUri + " ";
                }
            }

            message = Program.security.EncryptMessage(Program.onlineUsers[userID].sessionKey, message);
            message = (char)31 + " " + message;
            message += " <EOF>";
            return message;
        }

        public static string StateChange(long userID)
        {
            string message = string.Empty;
            foreach (var item in Program.onlineUsers[userID].friendWithStateDict)
            {
                message += item.Key + " " + item.Value + " ";
            }

            message = Program.security.EncryptMessage(Program.onlineUsers[userID].sessionKey, message);
            message = (char)14 + " " + message;
            message += " <EOF>";

            // Remove from the dictionary
            Program.onlineUsers[userID].friendWithStateDict.Clear();

            return message;
        }

        public static string ChooseCommunique(string message, byte[] sessionKey, Socket client)
        {
            byte[] sessKey;
            long userID;
            string result;
            //char mess = message[0];

            if (message[0] == (char)1 || message[0] == (char)0)
            {
                sessKey = sessionKey;
                if (sessKey == null) { return Fail(); }
            }
            else
            {
                // Session key is in the onlineUsers dictionary
                userID = getUserIDByIPAddress(((IPEndPoint)client.RemoteEndPoint).Address.ToString());
                if (userID > 0)
                {
                    sessKey = Program.onlineUsers[userID].sessionKey;
                    if (sessKey == null) { return Fail(); }
                }
                else
                {
                    return Fail();
                }
            }

            // Decipher   
            string decipheredMessage = Program.security.DecryptMessage(message.Substring(2, message.Length - 8), sessKey);

            Console.WriteLine("Header: {0}, Deciphered content: {1}", message[0], decipheredMessage);

            // Take 1 byte to get the header
            int header = message[0];

            // Parameters to send
            string[] sendParameters = decipheredMessage.Split(' ');
            List<string> parameters = new List<string>();
            for (int i = 0; i < sendParameters.Length; i++)
            {
                parameters.Add(sendParameters[i]);
            }

            // Login requires 3 parameters, in other case use one
            if (header != 1) 
            {
                // Use the dictionary of delegates to choose the proper method to respond
                result = (string)communiqueDictionary[header].DynamicInvoke(parameters);
            }
            else
            {
                result = (string)communiqueDictionary[header].DynamicInvoke(parameters, sessionKey, client);
            }
            return result;
        }

        public static long getUserIDByIPAddress(string addressIP)
        {
            long userID = -1;

            foreach (KeyValuePair<long, ClientClass> item in Program.onlineUsers)
            {
                if (item.Value.addressIP == addressIP)
                {
                    userID = item.Key;
                    return userID;
                }
            }
            return userID;
        }
    }
}
