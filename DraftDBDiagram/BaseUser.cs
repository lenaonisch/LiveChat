using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DraftDBDiagram
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

        /// <summary>
        /// nullable field
        /// </summary>
        public byte[] Avatar
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        /// <summary>
        /// As soon as chat will be supplied for multiple companies, this field should be stored not only for operators, but for registered end-users, too
        /// </summary>
        public Company Company
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }
    }
}