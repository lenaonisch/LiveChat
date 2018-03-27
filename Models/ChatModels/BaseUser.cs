using System;
using System.Collections.Generic;
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
        public int ID { get; set; }
        /// <summary>
        /// For unregistered end-users may be formed from DateTime
        /// </summary>
        public int NickName { get; set; }
    }
}