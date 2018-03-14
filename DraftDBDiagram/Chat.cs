using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DraftDBDiagram
{
    public class Chat
    {
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// duration in minutes
        /// </summary>
        public byte Duration { get; set; }

        public List<Message> Messages
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