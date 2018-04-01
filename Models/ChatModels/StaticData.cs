using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ChatModels
{
    public static class StaticData
    {

        public static object lockobj = new object();

        /// <summary>
        /// Last free RoomID
        /// </summary>
        private static int roomId = 0;

        /// <summary>
        /// Key - Company name, Value - Company
        /// </summary>
        public static Dictionary<string, Company> Companies = new Dictionary<string, Company>();

        /// <summary>
        /// All operators which are online
        /// </summary>
        public static Dictionary<Company, HashSet<UserProfile>> Operators = new Dictionary<Company, HashSet<UserProfile>>();

        //According to https://stackoverflow.com/questions/19351493/getting-all-group-names-in-signalr it is needed to manage groups manually
        /// <summary>
        /// Dictionary groups
        /// </summary>
        public static Dictionary<Company, Dictionary<string, Chat>> Groups = new Dictionary<Company, Dictionary<string, Chat>>();

        /// <summary>
        /// Connects username and groupname. The key is the username, the value is group
        /// </summary>
        public static Dictionary<Company, Dictionary<string, HashSet<Chat>>> UsersInGroups = new Dictionary<Company, Dictionary<string, HashSet<Chat>>>();

        /// <summary>
        /// Connects username and ConnectionId. The key is the username, the value is the ConnectionId
        /// </summary>
        public static Dictionary<Company, Dictionary<string, UserProfile>> Users = new Dictionary<Company, Dictionary<string, UserProfile>>();

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
                Groups.Add(company, new Dictionary<string, Chat>());
                Users.Add(company, new Dictionary<string, UserProfile>());
                UsersInGroups.Add(company, new Dictionary<string, HashSet<Chat>>());
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
            Dictionary<string, HashSet<Chat>> uig = UsersInGroups[company];
            int minRooms = uig[op.BaseUser.NickName].Count;

            foreach(UserProfile op1 in ops)
            {
                if (uig[op1.BaseUser.NickName].Count < minRooms)
                {
                    minRooms = uig[op1.BaseUser.NickName].Count;
                    op = op1;
                }
            }

            return op;
        }

        public static int GetRoomID()
        {
            return roomId++;
        }
    }
}
