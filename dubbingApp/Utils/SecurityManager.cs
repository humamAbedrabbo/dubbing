using dubbingApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace dubbingApp.Utils
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

            createRole("Admin");
            // Please make user name and email both equal
            Id = CreateUser("admin@fardous.tv", "admin@fardous.tv", "Admin@123");
            if (!string.IsNullOrEmpty(Id))
            {
                AddUserToRole(Id, "Admin");
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