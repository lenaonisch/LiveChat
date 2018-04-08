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
            string companyName = "Рога и копыта";
            Company company = AddCompany(context, companyName);
            CreateRoles(context, roles);

            SetDataToExistingUser(context, "elenaonishhenk0@yandex.ru", companyName, (string)roles.GetValue(0), "Лена Онищенко");
            CreateOtherUsers(context, roles, 
                            new string[] 
                                { "operator@somecompany.com",
                                  "superoperator@somecompany.com",
                                  "mother_dragon@anymail.com"
                                },
                            companyName);
        }

        private static void SetDataToExistingUser(ApplicationDbContext context, string email, string company, string role, string UserName)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);

            var registeredUser = context.Users.SingleOrDefault(u => u.Email == email);
            if (registeredUser != null)
            {
                if (!manager.AddToRole(registeredUser.Id, role).Succeeded)
                {
                    Console.WriteLine("Failed to add {0} as {1}", email, role);
                }
                else
                {
                    registeredUser = manager.FindByEmail(email);
                    UpdateUserProfile(context, registeredUser, company);
                }
            }
        }

        private static Company AddCompany(ApplicationDbContext context, string name)
        {
            Company company = new Company(name);
            if (context.Companies.FirstOrDefault(t => t.Name == name) == null)
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

        private static void CreateOtherUsers(ApplicationDbContext context, string[] roles, string[] emails, string company)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            for(int i = 0; i< roles.Length; i++)
            {
                ApplicationUser user = new ApplicationUser() { Email = (string)emails.GetValue(i), UserName = String.Format("{0}{1}", roles.GetValue(i), i) };
                if (manager.Create(user).Succeeded)
                {
                    manager.AddToRole(user.Id, (string)roles.GetValue(i));
                    UpdateUserProfile(context, user, company);
                }
            }
        }

        private static void UpdateUserProfile(ApplicationDbContext context, ApplicationUser user, string company)
        {
            UserProfile registeredUser = new UserProfile()
            {
                BaseUser = new BaseUser(user.UserName, context.Companies.First(t => t.Name == company)),
                User = user
            };
            context.UserProfiles.AddOrUpdate(registeredUser);
        }
    }
}
