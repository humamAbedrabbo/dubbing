﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;

namespace dubbingApp.Controllers
{
    //[Authorize(Roles = "ADMIN, GENERAL_MANAGER")]
    public class castingController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: casting
        public ActionResult Index(long work)
        {
            ViewBag.workIntno = work;
            ViewBag.returnUrl = Request.UrlReferrer.ToString();
            return View();
        }

        // characters
        public ActionResult charactersList(long id, string characterType)
        {
            var model = db.workCharacters.Include(b => b.agreementWork)
                        .Where(b => b.workIntno == id && (string.IsNullOrEmpty(characterType) || b.characterType == characterType)).OrderBy(b => b.sortOrder);
            return PartialView("_charactersList", model.ToList());
        }

        public ActionResult characterAddNew(long id)
        {
            ViewBag.workIntno = id;
            ViewBag.characterTypesList = LookupModels.getDictionary("characterType");
            ViewBag.characterGendersList = LookupModels.getDictionary("characterGender");
            return PartialView("_characterAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult characterAddNew(workCharacter item, long id)
        {
            var model = db.workCharacters;
            if (ModelState.IsValid)
            {
                try
                {
                    item.workIntno = id;
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Data. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");
            return Content("Successfully Created.", "text/html");
        }

        public ActionResult characterUpdate(long id)
        {
            var model = db.workCharacters.Find(id);
            ViewBag.characterTypesList = LookupModels.getDictionary("characterType");
            ViewBag.characterGendersList = LookupModels.getDictionary("characterGender");
            return PartialView("_characterUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult characterUpdate(workCharacter item)
        {
            var model = db.workCharacters;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = db.workCharacters.Find(item.workCharacterIntno);
                    this.UpdateModel(modelItem);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Data. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");
            return Content("Successfully Updated.", "text/html");
        }

        public ActionResult castingDetails(long work, long character)
        {
            var model = db.workActors.Where(b => b.workIntno == work && b.workCharacterIntno == character).OrderBy(b => b.fromDate).ToList();
            if (model.Count() != 0)
            {
                ViewBag.workIntno = work;
                ViewBag.workCharacterIntno = character;
                return PartialView("_castingDetails", model);
            }
            else
            {
                workActor actorModel = new workActor();
                actorModel.workIntno = work;
                actorModel.workCharacterIntno = character;
                ViewBag.workIntno = work;
                ViewBag.workCharacterIntno = character;
                ViewBag.actorsList = new SelectList(db.voiceActors.Where(b => b.status == true).Select(b => new { b.voiceActorIntno, b.fullName }), "voiceActorIntno", "fullName");
                return PartialView("_actorAddNew", actorModel);
            }
        }

        // actors
        public ActionResult actorAddNew(long work, long character)
        {
            workActor model = new workActor();
            model.workIntno = work;
            model.workCharacterIntno = character;
            ViewBag.workIntno = work;
            ViewBag.workCharacterIntno = character;
            ViewBag.actorsList = new SelectList(db.voiceActors.Where(b => b.status == true).Select(b => new { b.voiceActorIntno, b.fullName }), "voiceActorIntno", "fullName");
            return PartialView("_actorAddNew", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult actorAddNew(workActor item)
        {
            var model = db.workActors;
            if (ModelState.IsValid)
            {
                var x = model.Where(b => b.workIntno == item.workIntno && b.workCharacterIntno == item.workCharacterIntno && b.status == true).ToList();
                try
                {
                    foreach(workActor actor in x) // applies for the actor replacement
                    {
                        actor.thruDate = item.fromDate.AddDays(-1);
                        actor.status = false;
                    }
                    item.status = true;
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Data. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");
            return Content("Successfully Created.", "text/html");
        }

        public ActionResult actorUpdate(long work, long character)
        {
            var model = db.workActors.FirstOrDefault(b => b.workIntno == work && b.workCharacterIntno == character && b.status == true);
            return PartialView("_actorUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult actorUpdate(workActor item)
        {
            var model = db.workActors;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.Find(item.workActorIntno);
                    this.UpdateModel(modelItem);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Data. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");
            return Content("Successfully Updated.", "text/html");
        }

    }
}