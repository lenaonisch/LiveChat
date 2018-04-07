using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Models.ChatModels
{
    public class Message
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        [Required]
        public string Text { get; set; }
        public BaseUser User { get; set; }

        public Message(string message, BaseUser user, DateTime dateTime)
        {
            Text = message;
            User = user;
            DateTime = dateTime;
        }
    }
}