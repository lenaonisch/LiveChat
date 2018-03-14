using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DraftDBDiagram
{
    public class UserProfile
    {
        public ApplicationUser User { get; set; }
        public Contact Contact { get; set; }
    }
}