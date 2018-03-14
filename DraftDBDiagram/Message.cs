using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DraftDBDiagram
{
    public class Message
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        public string Text { get; set; }
        public BaseUser User { get; set; }
    }
}