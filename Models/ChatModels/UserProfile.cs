using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Models.ChatModels
{
    public class UserProfile
    {
        public ModelsMVC.ApplicationUser User { get; set; } // contains Role here: User (end-users) / Operator / 'SuperOperator'
        /// <summary>
        /// this class may be used in SignalRMvc.Hubs.ChatHub in Users property:
        ///     static List<User> Users = new List<User>();   
        /// see example on https://metanit.com/sharp/mvc5/16.2.php
        /// </summary>
        public BaseUser BaseUser { get; set; }

        /// <summary>
        /// As soon as chat will be supplied for multiple companies, this field should be stored not only for operators, but for registered end-users, too
        /// </summary>
        public Company Company { get; set; }
        public Contact Contact { get; set; }

        /// <summary>
        /// nullable field
        /// </summary>
        public byte[] Avatar { get; set; }
    }
}