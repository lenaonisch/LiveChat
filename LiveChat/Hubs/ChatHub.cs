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
using System.Security.Claims;
using LiveChat.Hubs;

namespace LiveChat
{
    // [Authorize(Roles = "Operator")]
    public class ChatHub : Hub
    {
        /// <summary>
        /// Temporary variable to describe some company
        /// </summary>
        private static Company tmpComp;


        [Authorize(Roles = "Operator")]
        public void Send(string group, string message)
        {
            BaseUser bu = StaticData.Users[tmpComp][Context.ConnectionId].BaseUser;
            Clients.Group(group).addNewMessageToPage(bu.NickName, group, message).Wait();
            StaticData.Groups[tmpComp][group].Messages.Add(new Message(message, bu, DateTime.Now));
        }


        public void Send(string message)
        {
            BaseUser bu = StaticData.Users[tmpComp][Context.ConnectionId].BaseUser;
            string group = StaticData.UsersInGroups[tmpComp][Context.ConnectionId].First().GroupID;
            Clients.Group(group).addNewMessageToPage(bu.NickName, group, message).Wait();
            StaticData.Groups[tmpComp][group].Messages.Add(new Message(message, bu, DateTime.Now));
        }

        public void RegisterOperator(string user)
        {
            Logger.LogMessage("Operator register start");
            UserProfile op = new UserProfile() { BaseUser = new BaseUser(user, tmpComp, Context.ConnectionId) };
            HashSet<Chat> chats = new HashSet<Chat>();
            string cId = Context.ConnectionId;

            //lock (StaticData.lockobj)
            {
                StaticData.UsersInGroups[tmpComp].Add(cId, chats);
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
                        Groups.Add(op.BaseUser.ConnectionID, group).Wait();
                        Clients.Group(group).addNewMessageToPage(op.BaseUser.NickName, group, "joined room" + group).Wait();
                        Clients.Caller.registerUserInRoom(group).Wait();
                    }
                }
            }
            Logger.LogMessage("Operator register end");
        }

        public void RegisterUser(string user)
        {
            string cId = Context.ConnectionId;

            ////this code should be changed, when Identity is used
            //if (user == "Operator")
            //{
            //    RegisterOperator(user);
            //    return;
            //}

            //lock (StaticData.lockobj)
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
                    StaticData.Groups[tmpComp].Add(group, chat);
                }

                //Adding user to group
                //if (StaticData.UsersInGroups[tmpComp].ContainsKey(user) == false)
                {
                    StaticData.UsersInGroups[tmpComp].Add(cId, new HashSet<Chat>());
                }

                //if (StaticData.UsersInGroups[tmpComp][user].Contains(StaticData.Groups[tmpComp][group]) == false)
                {
                    Groups.Add(cId, group).Wait();
                    StaticData.UsersInGroups[tmpComp][cId].LAdd(chat);
                }
                Logger.LogMessage("User " + user + " with ID " + cId + " enter to room " + group);
                JoinOperator(group);
                Clients.OthersInGroup(group).registerUserInRoom(group).Wait();
                Clients.Group(group).addNewMessageToPage(user, group, "joined room" + group).Wait();

            }
        }

        public void RemoveUser(string connectionID)
        {
            lock (StaticData.lockobj)
            {
                Logger.LogMessage("Remove user with ID " + connectionID);
                var userProfile = StaticData.Users[tmpComp][connectionID];
                var userName = userProfile.BaseUser.NickName;
                HashSet<Chat> chats = StaticData.UsersInGroups[tmpComp][connectionID];
                
                if (IdentityOperations.ContainsRole(Context.User.Identity, "Operator"))
                {
                    StaticData.Operators[tmpComp].LRemove(userProfile);
                }

                StaticData.Users[tmpComp].Remove(connectionID);
                StaticData.UsersInGroups[tmpComp].Remove(connectionID);
                StaticData.Groups[tmpComp].Remove(connectionID);

                foreach (var chat in chats)
                {
                    Clients.Group(chat.GroupID).addNewMessageToPage(userName, chat.GroupID, "The room is closed").Wait();
                    Clients.OthersInGroup(chat.GroupID).closeGroup(chat.GroupID).Wait();

                    //Remove operator from room:
                    var sel = StaticData.Operators[tmpComp].Where(op => { return StaticData.UsersInGroups[tmpComp][op.BaseUser.ConnectionID].Contains(chat); });
                    if (sel.Count() > 0)
                    {
                        var oper = sel.First().BaseUser;
                        Groups.Remove(oper.ConnectionID, chat.GroupID).Wait();
                        StaticData.UsersInGroups[tmpComp][oper.ConnectionID].LRemove(chat);
                    }
                    Logger.LogMessage("User " + userName + " is disconnected from group" + chat.GroupID);
                }
            }
        }

        public void JoinOperator(string roomName)
        {
            UserProfile op = StaticData.GetMostFreeOperator(tmpComp);

            if (op == null) return;
            Groups.Add(op.BaseUser.ConnectionID, roomName).Wait();
            Chat chat = StaticData.Groups[tmpComp][roomName];
            StaticData.UsersInGroups[tmpComp][op.BaseUser.ConnectionID].LAdd(chat);
            Clients.Group(roomName).addNewMessageToPage(op.BaseUser.NickName, roomName, "joined room" + roomName).Wait();
        }


        private void AddConnection()
        {
            lock (StaticData.lockobj)
            {
                //string name = Context.User.Identity.Name;
                //if (name == "") name = DateTime.Now.ToString();
                if (IdentityOperations.ContainsRole(Context.User.Identity, "Operator"))
                    RegisterOperator(Context.User.Identity.Name);
                //process situation when 2 users added in one moment of time(it seems to me, adding milliseconds will not solve the problem fully)
                else
                {
                    string genName = DateTime.Now.ToString();
                    RegisterUser(genName);
                }
            }
        }

        public override Task OnConnected()
        {
            AddConnection();
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

        public override Task OnReconnected()
        {
            Logger.LogMessage("Reconnection used!!!");
            AddConnection();
            return base.OnReconnected();
        }

        public ChatHub()
        {
            tmpComp = StaticData.Companies.First().Value;
        }
    }
}