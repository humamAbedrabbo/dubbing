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

        public ActionResult usersList(string filterType)
        {
            List<ViewModels.usersViewModel> model = new List<ViewModels.usersViewModel>();
            var r = context.Roles.ToList();
            var x = context.Users.OrderBy(b => b.UserName).ToList();
            var y = db.employees.Where(b => b.status == true).Select(b => new { email = b.email, name = b.fullName })
                    .Union(db.contacts.Where(b => b.status == true).Select(b => new { email = b.contactEmailAddr, name = b.contactName })).ToList();

            foreach (var x1 in x)
            {
                ViewModels.usersViewModel item = new ViewModels.usersViewModel();
                item.userId = x1.Id;
                item.userName = x1.UserName;
                var y1 = y.FirstOrDefault(b => b.email == x1.UserName);
                item.authenticName = y1 == null ? null : y1.name;
                string roles = null;
                
                foreach (var r1 in x1.Roles)
                {
                    if (string.IsNullOrEmpty(roles))
                        roles = r.FirstOrDefault(b => b.Id == r1.RoleId).Name;
                    else
                        roles = roles + " | " + r.FirstOrDefault(b => b.Id == r1.RoleId).Name;
                }
                item.roles = roles;
                model.Add(item);
            }

            switch (filterType)
            {
                case "01":
                    return PartialView("_usersList", model);
                case "02": // unauthenticated
                    return PartialView("_usersList", model.Where(b => string.IsNullOrEmpty(b.authenticName)));
                case "03": // unauthorized
                    return PartialView("_usersList", model.Where(b => string.IsNullOrEmpty(b.roles)));
                default:
                    return PartialView("_usersList", model);
            }
        }

        public ActionResult usersInRoleList(string role)
        {
            List<ViewModels.usersViewModel> model = new List<ViewModels.usersViewModel>();
            var x = context.Users.Where(b => b.Roles.Any(r => r.RoleId == role));
            var y = db.employees.Where(b => b.status == true).Select(b => new { email = b.email, name = b.fullName })
                    .Union(db.contacts.Where(b => b.status == true).Select(b => new { email = b.contactEmailAddr, name = b.contactName })).ToList();

            foreach (var x1 in x)
            {
                ViewModels.usersViewModel item = new ViewModels.usersViewModel();
                item.userId = x1.Id;
                item.userName = x1.UserName;
                var y1 = y.FirstOrDefault(b => b.email == x1.UserName);
                item.authenticName = y1 == null ? null : y1.name;
                model.Add(item);
            }
            ViewBag.roleId = role;
            return PartialView("_usersInRoleList", model);
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
            var result = UserManager.Create(user, newPassword);
            return Content("User Created with password: " + newPassword, "text/html");
        }

        public ActionResult usersUpdate(string user)
        {
            var model = context.Users.Find(user);
            return PartialView("_usersUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult usersUpdate(IdentityUser item, string submitBtn)
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