namespace DB.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models.ChatModels;
    using Models.ModelsMVC;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
  

    internal sealed class Configuration : DbMigrationsConfiguration<DB.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DB.ApplicationDbContext context)
        {
            var roles = Enum.GetNames(typeof(Roles));

            Company company = AddCompany(context);
            CreateRoles(context, roles);

            SetRoleToExistingUser(context, company, (string)roles.GetValue(0));
            CreateOtherUsers(context, roles, new string[] { "operator@somecompany.com", "superoperator@somecompany.com", "mother_dragon@anymail.com" });
        }

        private static void SetRoleToExistingUser(ApplicationDbContext context, Company company, string role)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);

            var registeredUser = context.Users.SingleOrDefault(u => u.Email == "elenaonishhenk0@yandex.ru");
            if (registeredUser != null)
            {
                if (!manager.AddToRole(registeredUser.Id, role).Succeeded)
                {
                    Console.WriteLine("Failed to add elenaonishhenk0 as operator");
                }
            }

            UserProfile user = new UserProfile()
            {
                BaseUser = new BaseUser("Лена Онищенко", context.Companies.First(t => t.Name == company.Name)),
                User = registeredUser,
            };
            context.UserProfiles.AddOrUpdate(user);
        }

        private static Company AddCompany(ApplicationDbContext context)
        {
            Company company = new Company("Рога и копыта");
            if (context.Companies.FirstOrDefault(t => t.Name == "Рога и копыта") == null)
            {
                context.Companies.AddOrUpdate(c => c.Name, company);
                context.SaveChanges();
            }
            return company;
        }

        private static void CreateRoles(ApplicationDbContext context, string[] roles)
        {
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);
            foreach (string role in roles)
            {
                if (!context.Roles.Any(r => r.Name == role))
                {
                    var identityRole = new IdentityRole { Name = role };
                    manager.Create(identityRole);
                }
            }
        }

        private static void CreateOtherUsers(ApplicationDbContext context, string[] roles, string[] emails)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            for(int i = 0; i< roles.Length; i++)
            {
                ApplicationUser user = new ApplicationUser() { Email = (string)emails.GetValue(i), UserName = String.Format("{0}{1}", roles.GetValue(i), i) };
                manager.Create(user);
                manager.AddToRole(user.Id, (string)roles.GetValue(i));
            }
        }
    }
}
