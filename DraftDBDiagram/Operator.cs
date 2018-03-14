using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DraftDBDiagram
{
    public class Operator
    {
        public ApplicationUser User { get; set; }
        public Department Department { get; set; }
        public Contact Contact { get; set; }
        public BaseUser BaseUser { get; set; }
    }
}