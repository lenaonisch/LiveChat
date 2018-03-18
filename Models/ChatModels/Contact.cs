using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ChatModels
{
    public class Contact
    {
        public string Country { get; set; }
        public string Town { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Flat { get; set; }
        public List<string> Phones { get; set; }
        public string Notes { get; set; }
    }
}
