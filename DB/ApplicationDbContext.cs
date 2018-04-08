using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DB
{
    public class ApplicationDbContext : IdentityDbContext<Models.ModelsMVC.ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Models.ChatModels.UserProfile> UserProfiles { get; set; }
        public DbSet<Models.ChatModels.Chat> Chats { get; set; }
        public DbSet<Models.ChatModels.Company> Companies { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}