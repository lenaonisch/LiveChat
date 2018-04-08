using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.ChatModels
{
    public class Chat
    {
        public int ID { get; set; }
        public Company Company { get; set; }
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// duration in minutes
        /// </summary>
        public byte Duration { get; set; }
        public ICollection<Message> Messages { get; set; }

        /// <summary>
        /// SignalR group name assigned to chat room
        /// </summary>
	    [NotMapped]
        public string GroupID;

        public Chat() { }
        public Chat(string GroupID)
        {
            this.GroupID = GroupID;
            StartDateTime = DateTime.Now;
            Messages = new List<Message>();
        }
    }
}