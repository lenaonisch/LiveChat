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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Models.ModelsMVC;
using DB;
using Microsoft.AspNet.SignalR.Client.Http;

namespace LiveChat
{

    public class ChatHub : Hub
    {

        [Authorize(Roles = "Operator")]
        public void Send(string group, string message)
        {
            BaseUser bu = StaticData.Users[Context.ConnectionId].BaseUser;
            Clients.Group(group).addNewMessageToPage(bu.NickName, group, message).Wait();
            StaticData.Groups[bu.Company][group].Messages.Add(new Message(message, bu, DateTime.Now));
        }


        public void Send(string message)
        {
            BaseUser bu = StaticData.Users[Context.ConnectionId].BaseUser;
            string group = StaticData.UsersInGroups[bu.Company][Context.ConnectionId].First().GroupID;
            Clients.Group(group).addNewMessageToPage(bu.NickName, group, message).Wait();
            StaticData.Groups[bu.Company][group].Messages.Add(new Message(message, bu, DateTime.Now));
        }

        public void RegisterOperator(string user, Company company)
        {
            Logger.LogMessage("Operator register start");
            UserProfile op = new UserProfile() { BaseUser = new BaseUser(user, company, Context.ConnectionId) };
            HashSet<Chat> chats = new HashSet<Chat>();
            string cId = Context.ConnectionId;
            //lock (StaticData.lockobj)
            {
                StaticData.UsersInGroups[company].Add(cId, chats);
                StaticData.Users.Add(cId, op);
                StaticData.Operators[company].LAdd(op);
                //if the operator is the first who enters chat - he should be added to all rooms
                if (StaticData.Operators[company].Count == 1)
                {
                    ConcurrentDictionary<string, Chat> groups = StaticData.Groups[company];
                    foreach (KeyValuePair<string, Chat> kvPair in groups)
                    {
                        string group = kvPair.Key;
                        JoinOperator(op, group, company);
                    }
                }
            }
            Logger.LogMessage("Operator register end");
        }

        public void RegisterUser(string user, Company company)
        {
            string cId = Context.ConnectionId;

            UserProfile up = new UserProfile() { BaseUser = new BaseUser(user, company, cId) };
            //lock (StaticData.lockobj)
            {
                if (StaticData.Users.ContainsKey(cId) == false)
                {
                    StaticData.Users.Add(cId, up);//Add to User parameter Identity values
                }

                string group = StaticData.GetRoomID().ToString();
                Chat chat = new Chat(group, company);

                //Adding group if it doesn't exist
                StaticData.Groups[company].Add(group, chat);

                //Adding user to group
                StaticData.UsersInGroups[company].Add(cId, new HashSet<Chat>());

                Groups.Add(cId, group).Wait();
                StaticData.UsersInGroups[company][cId].LAdd(chat);
                StaticData.GroupsForUsers[company].Add(group, new HashSet<UserProfile>());
                StaticData.GroupsForUsers[company][group].LAdd(up);

                Logger.LogMessage("User " + user + " with ID " + cId + " enter to room " + group);
                UserProfile op = StaticData.GetMostFreeOperator(company);
                if (op != null)
                {
                    JoinOperator(op, group, company);
                }
                else
                    Clients.Caller.blockUser();
            }
        }

        public void RemoveUser(string connectionID, bool isOperator)
        {
            lock (StaticData.lockobj)
            {
                Logger.LogMessage("Remove user with ID " + connectionID);
                if (StaticData.Users.ContainsKey(connectionID) == false)
                {
                    Logger.LogMessage("User with ID= " + connectionID + " has already been removed");
                    return;
                }
                var userProfile = StaticData.Users[connectionID];
                var company = userProfile.BaseUser.Company;
                var userName = userProfile.BaseUser.NickName;
                HashSet<Chat> chats = StaticData.UsersInGroups[company][connectionID];

                foreach (var chat in chats)
                {
                    Clients.Group(chat.GroupID).blockUser().Wait();
                    Clients.Group(chat.GroupID).closeGroup(chat.GroupID).Wait();
                    foreach (var user in StaticData.GroupsForUsers[company][chat.GroupID])
                    {
                        Logger.LogMessage("User " + user.BaseUser.NickName + " is disconnected from group" + chat.GroupID);
                        Clients.Group(chat.GroupID).addNewMessageToPage(user.BaseUser.NickName, chat.GroupID, "The room is closed").Wait();
                        if (user.BaseUser.ConnectionID != connectionID)
                        {
                            Groups.Remove(user.BaseUser.ConnectionID, chat.GroupID).Wait();
                            StaticData.UsersInGroups[company][user.BaseUser.ConnectionID].Remove(chat);
                        }
                        if (StaticData.Operators[company].Contains(user)) continue;
                        StaticData.RemoveConnectionID(company, user.BaseUser.ConnectionID);
                    }
                    StaticData.GroupsForUsers[company].Remove(chat.GroupID);
                    StaticData.Groups[company].Remove(chat.GroupID);
                }

                if (isOperator)
                {
                    StaticData.Operators[company].LRemove(userProfile);
                    StaticData.RemoveConnectionID(company, connectionID);
                }
            }
        }

        protected void JoinOperator(UserProfile op, string roomName, Company company)
        {
            Groups.Add(op.BaseUser.ConnectionID, roomName).Wait();
            Clients.Group(roomName).registerUserInRoom(roomName).Wait();
            Chat chat = StaticData.Groups[company][roomName];
            StaticData.UsersInGroups[company][op.BaseUser.ConnectionID].LAdd(chat);
            StaticData.GroupsForUsers[company][roomName].LAdd(op);
            Clients.Group(roomName).addNewMessageToPage(op.BaseUser.NickName, roomName, "joined room" + roomName).Wait();
            Clients.Group(roomName).unblockUser().Wait();
        }


        private void AddConnection()
        {
            lock (StaticData.lockobj)
            {
                //Add Operator:
                if (DatabaseOperations.ContainsRole(Context.User.Identity, "Operator"))
                {
                    Company company;
                    using (var context = new ApplicationDbContext())
                    {
                        var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                        var user = UserManager.FindById(Context.User.Identity.GetUserId());
                        var selCompanies = StaticData.Companies.Values.Where(c => c.ID == user.CompanyID);
                        if (selCompanies.Count() == 0)
                        {
                            Logger.LogMessage("Error company ID for user " + user.UserName);
                            return;
                        }
                        company = selCompanies.First();
                    }
                    RegisterOperator(Context.User.Identity.Name, company);
                }
                //Add simple User
                else
                {
                    bool correctCompany = false;
                    string genName = DateTime.Now.ToString();
                    string companyIDs = Context.QueryString["companyID"];
                    if (companyIDs != null)
                        try
                        {
                            int companyID = Convert.ToInt32(companyIDs);
                            var sel = StaticData.Companies.Values.Where(c => c.ID == companyID);
                            if (sel.Count() == 1)
                            {
                                var company = sel.First();
                                RegisterUser(genName, company);
                                correctCompany = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogMessage(e.Message);
                        }

                    if (!correctCompany)
                    {
                        Logger.LogMessage("Error company ID for user " + genName);
                        Clients.Client(Context.ConnectionId).blockUser().Wait();
                        return;
                    }
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
            RemoveUser(this.Context.ConnectionId, Context.User.IsInRole(Strings.Strings.RoleOperator));
            return base.OnDisconnected(stopCalled);
        }
    }
}