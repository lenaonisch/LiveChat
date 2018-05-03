using LiveChat.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Models.ChatModels
{
    public static class StaticData
    {

        public static object lockobj = new object();

        public const int SleepTime = 100;

        /// <summary>
        /// Last free RoomID
        /// </summary>
        private static int roomId = 0;

        /// <summary>
        /// Key - Company name, Value - Company
        /// </summary>
        public static ConcurrentDictionary<string, Company> Companies = new ConcurrentDictionary<string, Company>();

        /// <summary>
        /// All operators which are online
        /// </summary>
        public static ConcurrentDictionary<Company, HashSet<UserProfile>> Operators = new ConcurrentDictionary<Company, HashSet<UserProfile>>();

        //According to https://stackoverflow.com/questions/19351493/getting-all-group-names-in-signalr it is needed to manage groups manually
        /// <summary>
        /// Dictionary groups
        /// </summary>
        public static ConcurrentDictionary<Company, ConcurrentDictionary<string, Chat>> Groups = new ConcurrentDictionary<Company, ConcurrentDictionary<string, Chat>>();

        /// <summary>
        /// Connects username and groupname. The key is the ConnectionId, the value is group
        /// </summary>
        public static ConcurrentDictionary<Company, ConcurrentDictionary<string, HashSet<Chat>>> UsersInGroups = new ConcurrentDictionary<Company, ConcurrentDictionary<string, HashSet<Chat>>>();

        /// <summary>
        /// Connects ConnectionId  and UserProfile. The key is the ConnectionId, the value is the UserProfile 
        /// </summary>
        public static ConcurrentDictionary<Company, ConcurrentDictionary<string, UserProfile>> Users = new ConcurrentDictionary<Company, ConcurrentDictionary<string, UserProfile>>();

        static StaticData()
        {
            Company c = new Company("Default");
            AddCompany(c);
        }

        /// <summary>
        /// Generate variables for company, when first chat visitor appears
        /// </summary>
        /// <param name="company"></param>
        public static void AddCompany(Company company)
        {
            lock (lockobj)
            {
                string name = company.Name;
                Companies.Add(name, company);
                Groups.Add(company, new ConcurrentDictionary<string, Chat>());
                Users.Add(company, new ConcurrentDictionary<string, UserProfile>());
                UsersInGroups.Add(company, new ConcurrentDictionary<string, HashSet<Chat>>());
                Operators.Add(company, new HashSet<UserProfile>());
            }
        }

        /// <summary>
        /// Get Operator which has the smallest number of active rooms
        /// </summary>
        /// <param name="company">Company</param>
        /// <returns></returns>
        public static UserProfile GetMostFreeOperator(Company company)
        {
            if (Operators[company].Count == 0) return null;

            HashSet<UserProfile> ops = Operators[company];
            UserProfile op=Operators[company].First();
            ConcurrentDictionary<string, HashSet<Chat>> uig = UsersInGroups[company];
            int minRooms = uig[op.BaseUser.ConnectionID].Count;

            foreach(UserProfile op1 in ops)
            {
                if (uig[op1.BaseUser.ConnectionID].Count < minRooms)
                {
                    minRooms = uig[op1.BaseUser.ConnectionID].Count;
                    op = op1;
                }
            }

            return op;
        }

        //public static void AddUser(Company company, string userName, string connectionID)
        //{
        //    if (Users[company].ContainsKey(connectionID) == false)
        //    {
        //        Users[company].TryAdd(connectionID, new UserProfile() { BaseUser = new BaseUser(userName, company, connectionID) });//Add to User parameter Identity values
        //    }

        //    string group = StaticData.GetRoomID().ToString();

        //    //Adding group if it doesn't exist
        //    if (Groups[company].ContainsKey(group) == false)
        //    {
        //        Groups[company].TryAdd(group, new Chat(group));
        //    }

        //}

        public static int GetRoomID()
        {
            return roomId++;
        }
    }
}
