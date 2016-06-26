using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Mvc;
//using System.Data.Objects.SqlClient;
using System.Xml;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using dubbingModel;
using dubbingApp.Models;

namespace dubbingApp.Models
{
    public class LookupModels
    {
        static DUBBDBEntities db = new DUBBDBEntities();
        
        public static string getLangCode()
        {
            /*if (System.Web.HttpContext.Current.Session["langCode"] == null)
                System.Web.HttpContext.Current.Session["langCode"] = System.Web.HttpContext.Current.Request.Cookies["_culture"].Value.Substring(0, 2);
            return System.Web.HttpContext.Current.Session["langCode"].ToString();
             */
            return "en";
        }

        public static string getUser()
        {
            return HttpContext.Current.User.Identity.GetUserName();
        }

        public static IEnumerable getDictionary(string domainName)
        {
            short accessLevel = 0;
            return staticCache.getDomainValuesList()
                .Where(b => b.domainName == domainName && b.langCode == getLangCode() && b.minAccessLevel <= accessLevel)
                .ToDictionary(b => b.domainCode, b => b.domainValue);
        }

        public static string decodeDictionaryItem(string domainName, string domainCode)
        {
            return staticCache.getDomainValuesList().FirstOrDefault(b => b.domainName == domainName && b.domainCode == domainCode && b.langCode == getLangCode()).domainValue;
        }
        
        public static string isPartyHasCharges(long party, string partyType)
        {
            if (partyType == "01") //actors
            {
                var x = db.workActors.Include(b => b.agreementWork).Where(b => b.voiceActorIntno == party && b.agreementWork.status == "01" && b.status == true);
                bool isCharged = true;
                foreach (workActor work in x)
                {
                    var y = db.workCharges.Where(b => b.workIntno == work.workIntno && b.workPartyIntno == work.voiceActorIntno
                                        && b.workPartyType == partyType && b.status == true).ToList();
                    if (y.Count() == 0)
                        isCharged = false;
                }
                if (x.Count() != 0 && isCharged)
                    return "01"; //all charged
                else if (x.Count() != 0 && !isCharged)
                    return "02"; //has some uncharged works
                else
                    return "03"; //has no work at hand
            }
            else
            {
                var x = db.workPersonnels.Include(b => b.agreementWork).Where(b => b.empIntno == party && b.agreementWork.status == "01" && b.status == true);
                bool isCharged = true;
                foreach (workPersonnel work in x)
                {
                    var y = db.workCharges.Where(b => b.workIntno == work.workIntno && b.workPartyIntno == work.empIntno
                                        && b.workPartyType == partyType && b.status == true).ToList();
                    if (y.Count() == 0)
                        isCharged = false;
                }
                if (x.Count() != 0 && isCharged)
                    return "01"; //all charged
                else if (x.Count() != 0 && !isCharged)
                    return "02"; //has some uncharged works
                else
                    return "03"; //has no work at hand
            }
        }

    }
}
