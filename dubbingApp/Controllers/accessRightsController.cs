using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;
using Microsoft.AspNet.Identity;
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

        public ActionResult usersInRoleList(string role)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            
            List<ViewModels.usersViewModel> usersList = new List<ViewModels.usersViewModel>();
            var model = context.Users.Where(b => b.Roles.Select(y => y.RoleId).Contains(role));
            var z = db.employees.ToList();
            foreach(var x in model)
            {
                ViewModels.usersViewModel usr = new ViewModels.usersViewModel();
                usr.userId = x.Id;
                usr.roleId = role;
                usr.userName = x.UserName;
                if (z.FirstOrDefault(b => b.email == usr.userName && b.status == true) != null)
                    usr.personnelName = z.FirstOrDefault(b => b.email == usr.userName && b.status == true).fullName;
                usersList.Add(usr);
            }
            ViewBag.roleId = role;
            return PartialView("_usersInRoleList", usersList);
        }

        public ActionResult usersNoRoleList()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            List<ViewModels.usersViewModel> usersList = new List<ViewModels.usersViewModel>();
            var model = context.Users.Where(b => !b.Roles.Any());
            var z = db.employees.ToList();
            foreach (var x in model)
            {
                ViewModels.usersViewModel usr = new ViewModels.usersViewModel();
                usr.userId = x.Id;
                usr.roleId = null;
                usr.userName = x.UserName;
                if (z.FirstOrDefault(b => b.email == usr.userName && b.status == true) != null)
                    usr.personnelName = z.FirstOrDefault(b => b.email == usr.userName && b.status == true).fullName;
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

        public ActionResult usersUpdate(string user, string role)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var model = context.Users.Find(user);
            if (!string.IsNullOrEmpty(role))
            {
                string roleName;
                roleName = context.Roles.Find(role).Name;
                ViewBag.roleId = role;
                ViewBag.roleName = roleName;
                List<string> otherRolesList = new List<string>();
                otherRolesList = UserManager.GetRoles(user).ToList();
                otherRolesList.Remove(roleName);
                ViewBag.otherRolesList = otherRolesList;
            }
            else
            {
                ViewBag.roleId = null;
                ViewBag.roleName = "User has no Role Assigned.";
                ViewBag.otherRolesList = null;
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
                }
            }
            if (string.IsNullOrEmpty(roleId))
                return RedirectToAction("usersNoRoleList");
            else
                return RedirectToAction("usersInRoleList", new { role = roleId });
        }
        
    }
}