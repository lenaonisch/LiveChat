using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DraftDBDiagram
{
    /// <summary>
    /// taken from LiveChat.Models.ApplicationUser : IdentityUser
    /// Roles are build-in in MVC for this class, but I just point it here
    ///   to demonstrate on ClassDiagram
    /// </summary>
    public class ApplicationUser
    {
        public Contact Contact { get; set; }
        public BaseUser BaseUser { get; set; }
        /// <summary>
        /// Superoperator, Operator, User...
        /// </summary>
        public string Role { get; set; }
    }
}