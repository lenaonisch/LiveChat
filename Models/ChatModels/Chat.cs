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

        /// <summary>
        /// SignalR group name assigned to chat room
        /// </summary>
	    [NotMapped]
        public string GroupID;


        public DateTime StartDateTime { get; set; }

        private List<Message> messages = new List<Message>();

        /// <summary>
        /// duration in minutes
        /// </summary>
        public byte Duration { get; set; }

        public List<Message> Messages
        {
            get
            {
                return messages;
            }
        }

        public Chat(string GroupID)
        {
            this.GroupID = GroupID;
            StartDateTime = DateTime.Now;
        }

        public void AddMessage(string message, BaseUser user)
        {
            messages.Add(new Message(message, user, DateTime.Now));
        }
    }
}