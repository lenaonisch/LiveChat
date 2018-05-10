using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveChat.Exceptions
{
    public class ChatHubException:Exception
    {
        public ChatHubException(string message):base(message)
        {

        }
     
    }
}