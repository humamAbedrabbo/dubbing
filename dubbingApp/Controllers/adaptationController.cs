using dubbingModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dubbingApp.Controllers
{
    public class adaptationController : Controller
    {
        DUBBDBEntities ctx = new DUBBDBEntities();

        // GET: adaptations
        public ActionResult Index()
        {
            // Get list of episodes where adaptation is in progress
            var model = ctx.orderTrnHdrs.Include(x => x.agreementWork).Where(x => x.startAdaptation.HasValue && !x.endAdaptation.HasValue).ToList();
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            var model = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderIntno == id);
            
            /*
             * 1- select scenes of the selected episode ordered by seqNo
             * 2- 
             */ 

            return View(model);
        }
    }
}