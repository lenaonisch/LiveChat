using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Models.ChatModels
{
    /// <summary>
    /// This entity may be used in payment system
    /// </summary>
    public class Company
    {
        [Key]
        public string Name { get; set; }
    }
}