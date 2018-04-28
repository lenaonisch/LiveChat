using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Log;
using Microsoft.AspNet.SignalR;
using Models.ChatModels;
using LiveChat.Extensions;

namespace LiveChat
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// Temporary variable to describe some company
        /// </summary>
        private static Company tmpComp;


        //TODO: Add [Authorize] after authentification is present
        public void Send(string group, string message)
        {
            BaseUser bu = StaticData.Users[tmpComp][Context.ConnectionId].BaseUser;
            Clients.Group(group).addNewMessageToPage(bu.NickName, group, message).Wait();
            StaticData.Groups[tmpComp][group].Messages.Add(new Message(message, bu, DateTime.Now));
        }


        public void Send(string message)
        {
            BaseUser bu = StaticData.Users[tmpComp][Context.ConnectionId].BaseUser;
            string group = StaticData.UsersInGroups[tmpComp][bu.NickName].First().GroupID;
            Clients.Group(group).addNewMessageToPage(bu.NickName, group, message).Wait();
            StaticData.Groups[tmpComp][group].Messages.Add(new Message(message, bu, DateTime.Now));
        }

        public void RegisterOperator(string user)
        {
            Logger.LogMessage("Operator enter");
            UserProfile op = new UserProfile() { BaseUser = new BaseUser(user, tmpComp, Context.ConnectionId) };
            HashSet<Chat> chats = new HashSet<Chat>();
            string cId = Context.ConnectionId;

            lock (StaticData.lockobj)
            {
                StaticData.UsersInGroups[tmpComp].Add(user, chats);
                StaticData.Users[tmpComp].Add(cId, op);
                StaticData.Operators[tmpComp].LAdd(op);
                //if the operator is the first who enters chat - he should be added to all rooms
                if (StaticData.Operators[tmpComp].Count == 1)
                {
                    ConcurrentDictionary<string, Chat> groups = StaticData.Groups[tmpComp];

                    foreach (KeyValuePair<string, Chat> kvPair in groups)
                    {
                        string group = kvPair.Key;
                        Chat chat = kvPair.Value;
                        chats.LAdd(chat);
                        /*Task t =*/
                        Groups.Add(op.BaseUser.ConnectionID, group).Wait();
                        //while (t.IsCompleted == false)
                        //{
                        //    if (t.IsCanceled || t.IsFaulted) throw new Exception("User was not added into the group");
                        //    Thread.Sleep(StaticData.SleepTime);
                        //}

                        //StaticData.UsersInGroups[tmpComp][user].Add(chat);
                        Clients.Group(group).addNewMessageToPage(op.BaseUser.NickName, group, "joined room" + group).Wait();
                        Clients.Caller.registerUserInRoom(group).Wait();
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
                Chat chat = new Chat(group, tmpComp);
                //Adding group if it doesn't exist
                //if (StaticData.Groups[tmpComp].ContainsKey(group) == false)
                {
                    StaticData.Groups[tmpComp].Add(group,chat );
                }

                //Adding user to group
                //if (StaticData.UsersInGroups[tmpComp].ContainsKey(user) == false)
                {
                    StaticData.UsersInGroups[tmpComp].Add(user, new HashSet<Chat>());
                }

                //if (StaticData.UsersInGroups[tmpComp][user].Contains(StaticData.Groups[tmpComp][group]) == false)
                {
                    /* Task t =*/
                    Groups.Add(cId, group).Wait();
                    StaticData.UsersInGroups[tmpComp][user].LAdd(chat);
                }
                Logger.LogMessage("User " + user + " enter to room " + group);
                JoinOperator(group);
                Clients.OthersInGroup(group).registerUserInRoom(group).Wait();
                Clients.Group(group).addNewMessageToPage(user, group, "joined room" + group).Wait();

            }
        }

        public void RemoveUser(string connectionID)
        {
            lock (StaticData.lockobj)
            {
                var userProfile = StaticData.Users[tmpComp][connectionID];
                var userName = userProfile.BaseUser.NickName;
                HashSet<Chat> chats = StaticData.UsersInGroups[tmpComp][userName];
                
                if (userName == "Operator")
                {
                    StaticData.Operators[tmpComp].LRemove(userProfile);
                }

                StaticData.Users[tmpComp].Remove(userName);
                StaticData.UsersInGroups[tmpComp].Remove(userName);
                StaticData.Groups[tmpComp].Remove(connectionID);

                foreach (var chat in chats)
                {
                    Clients.Group(chat.GroupID).addNewMessageToPage(userName, chat.GroupID, "The room is closed").Wait();
                    Clients.OthersInGroup(chat.GroupID).closeGroup(chat.GroupID).Wait();

                    //Remove operator from room:
                    var sel = StaticData.Operators[tmpComp].Where(op => { return StaticData.UsersInGroups[tmpComp][op.BaseUser.NickName].Contains(chat); });
                    var oper = sel.First().BaseUser;
                    Groups.Remove(oper.ConnectionID, chat.GroupID).Wait();
                    StaticData.UsersInGroups[tmpComp][oper.NickName].LRemove(chat);

                    Logger.LogMessage("User " + userName + " is disconnected from group" + chat.GroupID);
                    //This line is not needed and cause an error:
                    /*Task t =*/ /*Groups.Remove(connectionID, chat.GroupID).Wait();*/
                                 // Debug.WriteLine("End disconnect from group" + chat.GroupID);
                                 //while (t.IsCompleted == false)
                                 //{
                                 //    if (t.IsCanceled || t.IsFaulted) throw new Exception("User was not removed from the group");
                                 //    Thread.Sleep(StaticData.SleepTime);
                                 //}
                }
            }
        }

        public void JoinOperator(string roomName)
        {
            UserProfile op = StaticData.GetMostFreeOperator(tmpComp);

            if (op == null) return;
            /*Task t =*/
            Groups.Add(op.BaseUser.ConnectionID, roomName).Wait();
            Chat chat = StaticData.Groups[tmpComp][roomName];
            StaticData.UsersInGroups[tmpComp][op.BaseUser.NickName].LAdd(chat);
            //while (t.IsCompleted == false)
            //{
            //    if (t.IsCanceled || t.IsFaulted) throw new Exception("User was not added into the group");
            //    Thread.Sleep(StaticData.SleepTime);
            //}
            Clients.Group(roomName).addNewMessageToPage(op.BaseUser.NickName, roomName, "joined room" + roomName).Wait();
        }

        public override Task OnConnected()
        {
            //string name = Context.User.Identity.Name;
            //if (name == "") name = DateTime.Now.ToString();
            if (Context.Headers["Referer"].ToLower() == (MvcApplication.GetCentralChatHub() + "/Home/OperatorChat").ToLower())
                RegisterOperator("Operator");
            //process situation when 2 users added in one moment of time(it seems to me, adding miliseconds will not solve the problem fully)
            else
            {
                string genName = DateTime.Now.ToString();
                if (StaticData.Users[tmpComp].ContainsKey(genName))
                {
                    string genNameM = "";
                    int i = 0;
                    do
                    {
                        genNameM = genName + i;
                    } while (StaticData.Users[tmpComp].ContainsKey(genNameM));
                    genName = genNameM;

                }
                RegisterUser(genName);
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            if (stopCalled)
            {
                // We know that Stop() was called on the client,
                // and the connection shut down gracefully.
                RemoveUser(this.Context.ConnectionId);


            }
            else
            {
                // This server hasn't heard from the client in the last ~35 seconds.
                // If SignalR is behind a load balancer with scaleout configured, 
                // the client may still be connected to another SignalR server.
                RemoveUser(this.Context.ConnectionId);
            }
            return base.OnDisconnected(stopCalled);
        }

        public ChatHub()
        {
            tmpComp = StaticData.Companies.First().Value;
        }
    }
}