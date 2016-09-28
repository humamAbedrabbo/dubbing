using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER")]
    public class accessRightsController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private UserManager<ApplicationUser> UserManager { get; set; }
        private RoleManager<IdentityRole> RoleManager { get; set; }
        private ApplicationDbContext context = new ApplicationDbContext();

        // GET: accessRights
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult rolesList()
        {
            var model = context.Roles;
            return PartialView("_rolesList", model.ToList());
        }

        public ActionResult usersInRoleList(string filterType, string role)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            List<ViewModels.usersViewModel> usersList = new List<ViewModels.usersViewModel>();
            var model1 = context.Users.AsQueryable();
            var model = model1;

            switch(filterType)
            {
                case "03": // un authorized
                    model = model1.Where(b => !b.Roles.Any());
                    break;
                case "04": // users in role
                    model = model1.Where(b => b.Roles.Select(r => r.RoleId).Contains(role));
                    break;
            }
            var x = db.employees.Where(b => b.status == true);
            var y = db.contacts;
            foreach (var z in model)
            {
                ViewModels.usersViewModel usr = new ViewModels.usersViewModel();
                usr.userId = z.Id;
                usr.roleId = role;
                usr.userName = z.UserName;
                var x1 = x.FirstOrDefault(b => b.email == usr.userName);
                var y1 = y.FirstOrDefault(b => b.contactEmailAddr == usr.userName);
                if (filterType == "02")
                {
                    if (x1 == null && y1 == null)
                    {
                        usr.personnelName = null;
                        usersList.Add(usr);
                    }
                }
                else if (filterType != "02")
                {
                    if (x1 != null)
                        usr.personnelName = x1.fullName;
                    else if (y1 != null)
                        usr.personnelName = y1.contactName;
                    else
                        usr.personnelName = null;
                    usersList.Add(usr);
                }
            }
            ViewBag.roleId = role;
            return PartialView("_usersInRoleList", usersList);
        }

        public ActionResult usersNoRoleList()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            List<ViewModels.usersViewModel> usersList = new List<ViewModels.usersViewModel>();
            var model = context.Users.Where(b => !b.Roles.Any());
            var z1 = db.employees;
            foreach (var x in model)
            {
                ViewModels.usersViewModel usr = new ViewModels.usersViewModel();
                usr.userId = x.Id;
                usr.roleId = null;
                usr.userName = x.UserName;
                if (z1.FirstOrDefault(b => b.email == usr.userName && b.status == true) != null)
                    usr.personnelName = z1.FirstOrDefault(b => b.email == usr.userName && b.status == true).fullName;
                else
                    usr.personnelName = null;
                usersList.Add(usr);
            }
            return PartialView("_usersNoRoleList", usersList);
        }

        public ActionResult grantRoleToUser(string role)
        {
            ViewBag.usersList = new SelectList(context.Users.ToList(), "Id", "UserName");
            ViewBag.roleId = role;
            return PartialView("_grantRoleToUser");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult grantRoleToUser(IdentityUser user, string roleId)
        {
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            UserManager.AddToRole(user.Id, RoleManager.FindById(roleId).Name);
            context.SaveChanges();
            return RedirectToAction("usersInRoleList", new { role = roleId });
        }

        public ActionResult revokeRoleFromUser(string user, string role)
        {
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            UserManager.RemoveFromRole(user, RoleManager.FindById(role).Name);
            context.SaveChanges();
            string roleId = role;
            return RedirectToAction("usersInRoleList", new { role = roleId });
        }

        public ActionResult usersAddNew()
        {
            return PartialView("_usersAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult usersAddNew(RegisterViewModel item)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string newPassword = item.Email.Split('@')[0] + "@123";
            var user = new ApplicationUser { UserName = item.Email, Email = item.Email };
            var result = UserManager.CreateAsync(user, newPassword);
            return Content("User Created with password: " + newPassword, "text/html");
        }

        public ActionResult usersUpdate(string user, string role)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var model = context.Users.Find(user);
            List<string> otherRolesList = new List<string>();
            if (!string.IsNullOrEmpty(role))
            {
                string roleName;
                roleName = context.Roles.Find(role).Name;
                ViewBag.roleId = role;
                ViewBag.roleName = roleName;
                otherRolesList = UserManager.GetRoles(user).ToList();
                otherRolesList.Remove(roleName);
                ViewBag.otherRolesList = otherRolesList;
            }
            else
            {
                ViewBag.roleId = null;
                ViewBag.roleName = "User has no Role Assigned.";
                ViewBag.otherRolesList = otherRolesList;
            }
            return PartialView("_usersUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult usersUpdate(IdentityUser item, string roleId, string submitBtn)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            ApplicationUser usr = UserManager.FindById(item.Id);

            if (ModelState.IsValid)
            {
                switch(submitBtn)
                {
                    case "01":
                        usr.UserName = item.UserName;
                        UserManager.Update(usr);
                        context.SaveChanges();
                        break;
                    case "02":
                        UserManager.Delete(usr);
                        context.SaveChanges();
                        break;
                    case "03":
                        UserManager.RemovePassword(usr.Id);
                        UserManager.AddPassword(usr.Id, usr.UserName.Split('@')[0] + "@123");
                        break;
                }
            }
            if (submitBtn == "03")
                return Content("User Password Successfully Reset To: " + usr.UserName.Split('@')[0] + "@123", "text/html");
            else
                return Content("User Successfully Updated/Deleted", "text/html");
        }
        
    }
}