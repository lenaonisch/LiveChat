using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Models.ChatModels
{
    public class Contact
    {
        public int ID { get; set; }   
        [Required]
        [StringLength(50)]
        public string Country { get; set; }
        [StringLength(50)]
        public string Town { get; set; }
        [StringLength(100)]
        public string District { get; set; }
        [StringLength(100)]
        public string Street { get; set; }
        [StringLength(10)]
        public string House { get; set; }
        [StringLength(10)]
        public string Flat { get; set; }
        [Phone]
        public List<string> Phones { get; set; }
        public string Notes { get; set; }
    }
}
