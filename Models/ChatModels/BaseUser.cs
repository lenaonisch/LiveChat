using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Models.ChatModels
{
    /// <summary>
    /// this class may be used in SignalRMvc.Hubs.ChatHub in Users property:
    ///     static List<User> Users = new List<User>();   
    /// see example on https://metanit.com/sharp/mvc5/16.2.php
    /// </summary>
    public class BaseUser
    {
        /// <summary>
        /// ID 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ConnectionID of user
        /// </summary>
        [NotMapped]
        public string ConnectionID { get; set; }

        /// <summary>
        /// For unregistered end-users may be formed from DateTime
        /// </summary>
        public string NickName { get; set; }

        public BaseUser(string nickname, string connectID)
        {
            this.NickName = nickname;
            this.ConnectionID = connectID;
        }

    }
}