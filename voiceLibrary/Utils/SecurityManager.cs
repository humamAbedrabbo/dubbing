using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using voiceLibrary.Models;

namespace voiceLibrary.Utils
{
    public class SecurityManager
    {
        private UserManager<ApplicationUser> UserManager { get; set; }
        private RoleManager<IdentityRole> RoleManager { get; set; }
        private ApplicationDbContext context;

        public SecurityManager()
        {
            context = new ApplicationDbContext();

            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        }

        public static void InitializeRolesAndUsers()
        {
            SecurityManager instance = new SecurityManager();
            instance.CreateRolesandUsers();
        }

        // In this method we will create default User roles and Admin user for login
        private void CreateRolesandUsers()
        {

            string Id = null;

            createRole("ADMIN");
            createRole("FINANCE_MANAGER");
            createRole("GENERAL_MANAGER");
            createRole("MIXAGE");
            createRole("MONTAGE");
            createRole("PRODUCTION_ASSISTANT");
            createRole("PRODUCTION_MANAGER");
            createRole("STUDIO_ASSISTANT");
            createRole("STUDIO_SUPERVISOR");
            createRole("EDITOR");
            createRole("CLIENT");

            // Please make user name and email both equal
            Id = CreateUser("admin@fardous.tv", "admin@fardous.tv", "f@Rdous2010");
            if (!string.IsNullOrEmpty(Id))
            {
                AddUserToRole(Id, "ADMIN");
            }
            Id = CreateUser("asad@fardous.tv", "asad@fardous.tv", "asad@123");
            if (!string.IsNullOrEmpty(Id))
            {
                AddUserToRole(Id, "GENERAL_MANAGER");
            }
            Id = CreateUser("yamam@fardous.tv", "yamam@fardous.tv", "yamam@123");
            if (!string.IsNullOrEmpty(Id))
            {
                AddUserToRole(Id, "PRODUCTION_MANAGER");
            }
            Id = CreateUser("bassam.alhindy@fardous.tv", "bassam.alhindy@fardous.tv", "bassam.alhindy@123");
            if (!string.IsNullOrEmpty(Id))
            {
                AddUserToRole(Id, "MONTAGE");
            }
            Id = CreateUser("saif@fardous.tv", "saif@fardous.tv", "saif@123");
            if (!string.IsNullOrEmpty(Id))
            {
                AddUserToRole(Id, "STUDIO_SUPERVISOR");
            }
            Id = CreateUser("samers@fardous.tv", "samers@fardous.tv", "samers@123");
            if (!string.IsNullOrEmpty(Id))
            {
                AddUserToRole(Id, "MIXAGE");
            }
            Id = CreateUser("samera@fardous.tv", "samera@fardous.tv", "samera@123");
            if (!string.IsNullOrEmpty(Id))
            {
                AddUserToRole(Id, "MIXAGE");
            }
            Id = CreateUser("mbustani@fardous.tv", "mbustani@fardous.tv", "mbustani@123");
            if (!string.IsNullOrEmpty(Id))
            {
                AddUserToRole(Id, "FINANCE_MANAGER");
            }
            Id = CreateUser("wael@fardous.tv", "wael@fardous.tv", "wael@123");
            if (!string.IsNullOrEmpty(Id))
            {
                AddUserToRole(Id, "STUDIO_SUPERVISOR");
            }
        }

        public bool createRole(string roleName)
        {
            if (!RoleManager.RoleExists(roleName))
            {

                var role = new IdentityRole();
                role.Name = roleName;
                RoleManager.Create(role);
                return true;
            }

            return false;
        }
        public string CreateUser(string userName, string email, string password)
        {
            var user = new ApplicationUser();
            user.UserName = userName;
            user.Email = email;
            

            string userPWD = password;
            var chkUser = UserManager.Create(user, userPWD);
            if (chkUser.Succeeded)
                return user.Id;
            else
                return null;
        }
        public void AddUserToRole(string userId, string role)
        {
            UserManager.AddToRole(userId, role);           
        }
        public void ToggleUserRole(string userId, string role)
        {
            if (UserManager.IsInRole(userId, role))
            {
                UserManager.RemoveFromRole(userId, role);
            }
            else
            {
                UserManager.AddToRole(userId, role);
            }
        }
    }
}