using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.ChatModels
{
    /// <summary>
    /// This entity may be used in payment system
    /// </summary>
    public class Company
    {
        public int ID { get; set; }
        [StringLength(150)]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        public Company(string name)
        {
            this.Name = name;
        }
    }
}