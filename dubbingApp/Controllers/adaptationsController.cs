using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dubbingApp.Controllers
{
    public class adaptationsController : Controller
    {
        // GET: adaptations
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {

            return View();
        }
    }
}