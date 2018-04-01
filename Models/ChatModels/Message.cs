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
        public DateTime DateTime { get;}
        [Required]
        public string Text { get;}
        public BaseUser User { get;}

        public Message(string message, BaseUser user, DateTime dateTime)
        {
            Text = message;
            User = user;
            DateTime = dateTime;
        }
    }
}