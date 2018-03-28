﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.ChatModels
{
    public class Chat
    {
        public int ID { get; set; }
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// duration in minutes
        /// </summary>
        public byte Duration { get; set; }

        public List<Message> Messages { get; set; }
        
    }
}