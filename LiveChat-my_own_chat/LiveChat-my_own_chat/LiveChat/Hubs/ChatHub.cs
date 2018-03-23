using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace LiveChat
{
    public class ChatHub : Hub
    {
        //According to https://stackoverflow.com/questions/19351493/getting-all-group-names-in-signalr it is needed to manage groups manually
        /// <summary>
        /// List of groups
        /// </summary>
        private static List<string> groups = new List<string>();

        /// <summary>
        /// Connects username and groupname. The key is the username, the value is group
        /// </summary>
        private static Dictionary<string, string> usersInGroups = new Dictionary<string, string>();

        /// <summary>
        /// Connects username and ConnectionId. The key is the username, the value is the ConnectionId
        /// </summary>
        private static Dictionary<string, string> userConnectionID = new Dictionary<string, string>();

        public void Hello()
        {
            Clients.All.hello();
        }

        /// <summary>
        ///  Register user in the group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public void Register(string user, string group)
        {
            string cId = Context.ConnectionId;

            if (userConnectionID.ContainsKey(user) == false)
                userConnectionID.Add(user, cId);

            //Adding group if it doesn't exist
            if (groups.Contains(group) == false)
            {
                groups.Add(group);
            }

            //Adding user to group
            if (usersInGroups.ContainsKey(user) == false)
            {
                Task t = Groups.Add(cId, group);
                usersInGroups.Add(user, group);
                while (t.IsCompleted == false)
                {
                    if (t.IsCanceled || t.IsFaulted) throw new Exception("Пользователь не был добавлен в комнату");
                    Thread.Sleep(20);
                }
            }
            //if user exists in other group
            else
            {
                string oldGroup = usersInGroups[user];
                //TODO: add check if user has privilegies to change his room
                // Operator can change room. User can't
                Task t1 = Groups.Remove(cId, oldGroup);
                Task t2 = Groups.Add(cId, group);
                usersInGroups.Remove(user);
                usersInGroups.Add(user, group);
                while (t1.IsCompleted == false  && t2.IsCompleted == false)
                {
                    if (t1.IsCanceled || t1.IsFaulted|| t2.IsCanceled || t2.IsFaulted) throw new Exception("Пользователь не был добавлен в комнату");
                    Thread.Sleep(20);
                }
            }
            Clients.Group(group).addNewMessageToPage(user, "joined room" + group);
        }

        public void Send(string name, string message)
        {
            Clients.Group(usersInGroups[name]).addNewMessageToPage(name, message);
        }
    }
}