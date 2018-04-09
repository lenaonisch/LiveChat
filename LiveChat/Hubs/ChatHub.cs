using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Models.ChatModels;

namespace LiveChat
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// Temporary variable to describe some company
        /// </summary>
        private static Company tmpComp;

        ////TODO: Delete this method
        ///// <summary>
        /////  Register user in the group
        ///// </summary>
        ///// <param name="user"></param>
        ///// <param name="group"></param>
        //public void Register(string user, string group)
        //{
        //    string cId = Context.ConnectionId;

        //    if (StaticData.Users[tmpComp].ContainsKey(user) == false)
        //    {
        //        StaticData.Users[tmpComp].Add(user, new UserProfile() { BaseUser = new BaseUser() { NickName = user } });
        //    }

        //    //Adding group if it doesn't exist
        //    if (StaticData.Groups[tmpComp].ContainsKey(group) == false)
        //    {
        //        StaticData.Groups[tmpComp].Add(group, new Chat(StaticData.GetRoomID().ToString()));
        //    }

        //    //Adding user to group
        //    if (StaticData.UsersInGroups[tmpComp].ContainsKey(user) == false)
        //    {
        //        StaticData.UsersInGroups[tmpComp].Add(user, new List<Chat>());
        //    }

        //    if (StaticData.UsersInGroups[tmpComp][user].Contains(StaticData.Groups[tmpComp][group]) == false)
        //    {
        //        Task t = Groups.Add(cId, group);
        //        Chat chat = new Chat(group);
        //        StaticData.UsersInGroups[tmpComp][user].Add(chat);
        //        while (t.IsCompleted == false)
        //        {
        //            if (t.IsCanceled || t.IsFaulted) throw new Exception("User was not added into the group");
        //            Thread.Sleep(20);
        //        }
        //    }
        //    Clients.Group(group).addNewMessageToPage(user, group, "joined room" + group);
        //}

        //TODO: Add [Authorize] after authentification is present
        public void Send(string group, string message)
        {

            BaseUser bu = StaticData.Users[tmpComp][Context.ConnectionId].BaseUser;
            Clients.Group(group).addNewMessageToPage(bu.NickName, group, message);
            StaticData.Groups[tmpComp][group].Messages.Add(new Message(message, bu, DateTime.Now));
        }


        public void Send(string message)
        {
            BaseUser bu = StaticData.Users[tmpComp][Context.ConnectionId].BaseUser;
            string group = StaticData.UsersInGroups[tmpComp][bu.NickName].First().GroupID;
            Clients.Group(group).addNewMessageToPage(bu.NickName, group, message);
            StaticData.Groups[tmpComp][group].Messages.Add(new Message(message, bu, DateTime.Now));
        }

        public void RegisterOperator(string user)
        {
            UserProfile op = new UserProfile() { BaseUser = new BaseUser(user, tmpComp, Context.ConnectionId) };
            HashSet<Chat> chats = new HashSet<Chat>();
            string cId = Context.ConnectionId;
            

            lock (StaticData.lockobj)
            {
                StaticData.UsersInGroups[tmpComp].Add(user, chats);
                StaticData.Users[tmpComp].Add(cId, op);
                StaticData.Operators[tmpComp].Add(op);
                //if the operator is the first who enters chat - he should be added to all rooms
                if (StaticData.Operators[tmpComp].Count == 1)
                {
                    Dictionary<string, Chat> groups = StaticData.Groups[tmpComp];

                    foreach (KeyValuePair<string, Chat> kvPair in groups)
                    {
                        string group = kvPair.Key;
                        Chat chat = kvPair.Value;
                        chats.Add(chat);
                        Task t = Groups.Add(op.BaseUser.ConnectionID, group);
                        //StaticData.UsersInGroups[tmpComp][user].Add(chat);
                        while (t.IsCompleted == false)
                        {
                            if (t.IsCanceled || t.IsFaulted) throw new Exception("User was not added into the group");
                            Thread.Sleep(20);
                        }

                        Clients.Group(group).addNewMessageToPage(op.BaseUser.NickName, group, "joined room" + group);
                    }
                }
            }
        }

        public void RegisterUser(string user)
        {
            string cId = Context.ConnectionId;

            //this code should be changed, when Identity is used
            if (user == "Operator")
            {
                RegisterOperator(user);
                return;
            }
            lock (StaticData.lockobj)
            {
                if (StaticData.Users[tmpComp].ContainsKey(cId) == false)
                {
                    StaticData.Users[tmpComp].Add(cId, new UserProfile() { BaseUser = new BaseUser(user, tmpComp, cId) });//Add to User parameter Identity values
                }

                string group = StaticData.GetRoomID().ToString();

                //Adding group if it doesn't exist
                if (StaticData.Groups[tmpComp].ContainsKey(group) == false)
                {
                    StaticData.Groups[tmpComp].Add(group, new Chat(group));
                }

                //Adding user to group
                if (StaticData.UsersInGroups[tmpComp].ContainsKey(user) == false)
                {
                    StaticData.UsersInGroups[tmpComp].Add(user, new HashSet<Chat>());
                }

                if (StaticData.UsersInGroups[tmpComp][user].Contains(StaticData.Groups[tmpComp][group]) == false)
                {
                    Task t = Groups.Add(cId, group);
                    Chat chat = new Chat(group);
                    StaticData.UsersInGroups[tmpComp][user].Add(chat);
                    while (t.IsCompleted == false)
                    {
                        if (t.IsCanceled || t.IsFaulted) throw new Exception("User was not added into the group");
                        Thread.Sleep(20);
                    }
                }

                JoinOperator(group);
                Clients.Group(group).addNewMessageToPage(user, group, "joined room" + group);
            }
        }

        public void JoinOperator(string roomName)
        {
            UserProfile op = StaticData.GetMostFreeOperator(tmpComp);

            if (op == null) return;
            Task t = Groups.Add(op.BaseUser.ConnectionID, roomName);
            Chat chat = new Chat(roomName);
            StaticData.UsersInGroups[tmpComp][op.BaseUser.NickName].Add(chat);
            while (t.IsCompleted == false)
            {
                if (t.IsCanceled || t.IsFaulted) throw new Exception("User was not added into the group");
                Thread.Sleep(20);
            }
            Clients.Group(roomName).addNewMessageToPage(op.BaseUser.NickName, roomName, "joined room" + roomName);
        }

        public override Task OnConnected()
        {
            //string name = Context.User.Identity.Name;
            //if (name == "") name = DateTime.Now.ToString();
            if (Context.Headers["Referer"].ToLower() == (MvcApplication.GetCentralChatHub() + "/Home/OperatorChat").ToLower())
                RegisterOperator("Operator");
            else
                RegisterUser(DateTime.Now.ToString());
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public ChatHub()
        {
            tmpComp = StaticData.Companies.First().Value;
        }
    }
}