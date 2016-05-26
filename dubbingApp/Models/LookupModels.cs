using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
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
            return "user";
            //return WebMatrix.WebData.WebSecurity.CurrentUserName;
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

        //public static List<ViewModels.domainLov> getDomainValuesList(string domainName)
        //{
        //    short accessLevel = 0;
        //    string langCode = getLangCode();
        //    List<ViewModels.domainLov> list = new List<ViewModels.domainLov>();
        //    var x = db.dubbDomains.Where(b => b.domainName == domainName && b.valueUsage == "USR" && b.langCode == langCode && b.minAccessLevel <= accessLevel).OrderBy(b => b.sortOrder).ToList();
        //    for (int i = 0; i < x.Count(); i++)
        //    {
        //        ViewModels.domainLov lov = new ViewModels.domainLov();
        //        lov.domainCode = x[i].domainCode;
        //        lov.domainValue = x[i].status == true ? x[i].domainValue : x[i].domainValue + " (" + x[i].userMessage + ")";
        //        lov.status = x[i].status;
        //        list.Add(lov);
        //    }
        //    return list;
        //}

        //public static string getDomainDecodedValue(string domainName, string domainCode)
        //{
        //    string langCode = getLangCode();
        //    return db.dubbDomains.SingleOrDefault(b => b.domainName == domainName && b.domainCode == domainCode && b.langCode == langCode).domainValue;
        //}
        public static bool isPartyHasCharges(long party, string partyType)
        {
            var x = db.workActors.Where(b => b.voiceActorIntno == party && b.status == true).ToList();
            
            bool isCharged = true;
            foreach (workActor work in x)
            {
                var y = db.workCharges.Where(b => b.workIntno == work.workIntno && b.workPartyIntno == work.voiceActorIntno
                                    && b.workPartyType == partyType && b.status == true).ToList();
                if (y.Count() == 0)
                    isCharged = false;
            }
            if (x.Count() != 0 && !isCharged)
                return false;
            else
                return true;
        }

        public static IEnumerable getClientsList()
        {
            string langCode = getLangCode();
            if (langCode == "en")
                return (from A in db.clients
                        select new { A.clientIntno, A.clientName, A.status }).ToList();
            else
                return (from A in db.clients
                        select new { A.clientIntno, clientName = A.othClientName, A.status }).ToList();
        }

        public static IEnumerable getRefAgreementsList(long? agreementIntno)
        {
            string langCode = getLangCode();
            var x = (from A in db.agreements
                     select new { A.agreementIntno, A.agreementName }).ToList();
            if (agreementIntno.HasValue)
                return x.Where(b => b.agreementIntno != agreementIntno);
            else
                return x;

        }

        public static IEnumerable getAgreementsList(long? clientIntno)
        {
            string langCode = getLangCode();
            var x = (from A in db.agreements
                     select new { A.clientIntno, A.agreementIntno, A.agreementName }).ToList();
            if (clientIntno.HasValue)
                return x.Where(b => b.clientIntno == clientIntno);
            else
                return x;
        }

        public static IEnumerable getClientContactsList(long? clientIntno)
        {
            string langCode = getLangCode();
            var x = (from A in db.contacts
                     where A.contactParty == "01"
                     select new { A.partyIntno, A.contactIntno, A.contactName }).ToList();
            if (clientIntno.HasValue)
                return x.Where(b => b.partyIntno == clientIntno);
            else
                return x;
        }

        public static IEnumerable getWorkCharactersList(long workIntno)
        {
            string langCode = getLangCode();
            return (from A in db.workCharacters
                    where A.workIntno == workIntno
                    select new { A.workCharacterIntno, workCharacterName = A.characterName }).ToList();
        }

        public static IEnumerable getVoiceActorsList()
        {
            string langCode = getLangCode();
            return (from A in db.voiceActors
                    select new { A.voiceActorIntno, voiceActorName = A.fullName }).ToList();
        }

        public static IEnumerable getPersonnelList(string empType)
        {
            string langCode = getLangCode();
            return (from A in db.employees
                     where A.empType == empType
                    select new { personnelIntno = A.empIntno, personnelName = A.fullName, A.empType }).ToList();
        }

        public static IEnumerable getWorkPersonnelList(long workIntno, string workPartyType)
        {
            string langCode = getLangCode();
            if (workPartyType == "01") // actors
                return (from C in db.workActors
                        join D in db.voiceActors on C.voiceActorIntno equals D.voiceActorIntno
                        where C.workIntno == workIntno
                        select new { personnelIntno = C.voiceActorIntno, personnelName = D.fullName }).ToList();
            else if (workPartyType == "02") // technicians
                return (from A in db.workPersonnels
                        join B in db.employees on A.empIntno equals B.empIntno
                        where A.workIntno == workIntno && B.empType != "03"
                        select new { personnelIntno = A.empIntno, personnelName = B.fullName }).ToList();
            else if (workPartyType == "03") // contractors
                return (from A in db.workPersonnels
                        join B in db.employees on A.empIntno equals B.empIntno
                        where A.workIntno == workIntno && B.empType == workPartyType
                        select new { personnelIntno = A.empIntno, personnelName = B.fullName }).ToList();
            else // when it is null
                return (from C in db.workActors
                        join D in db.voiceActors on C.voiceActorIntno equals D.voiceActorIntno
                        where C.workIntno == workIntno
                        select new { personnelIntno = C.voiceActorIntno, personnelName = D.fullName }).ToList()
                        .Union
                        (from A in db.workPersonnels
                         join B in db.employees on A.empIntno equals B.empIntno
                         where A.workIntno == workIntno
                         select new { personnelIntno = A.empIntno, personnelName = B.fullName }).ToList();
        }

        public static IEnumerable getWorksList(long? agreementIntno)
        {
            string langCode = getLangCode();
            var x = (from A in db.agreementWorks
                     join B in db.agreements on A.agreementIntno equals B.agreementIntno
                     join C in db.clients on B.clientIntno equals C.clientIntno
                     select new { A.agreementIntno, A.workIntno, clientName = C.clientShortName, A.workName }).ToList();
            if (agreementIntno.HasValue)
                return x.Where(b => b.agreementIntno == agreementIntno);
            else
                return x;
        }

        public static IEnumerable getOrderItems(long? workIntno)
        {
            var x = (from A in db.orderTrnHdrs
                    join B in db.agreementWorks on A.workIntno equals B.workIntno
                    where A.status == "04"
                    select new { A.orderTrnHdrIntno, B.workIntno, B.workName, A.episodeNo });
            if (workIntno.HasValue)
                return x.Where(b => b.workIntno == workIntno).ToList();
            else
                return x.ToList();
        }

        

        public static int getCharacterEpisodeTotalScenes(long dubbSheetHdrIntno)
        {
            System.Nullable<int> x = (from A in db.dubbingSheetHdrs
                                     join B in db.dubbingSheetDtls on A.dubbSheetHdrIntno equals B.dubbSheetHdrIntno
                                     where A.dubbSheetHdrIntno == dubbSheetHdrIntno
                                     select new { B.dubbSheetDtlIntno }).Count();
            return (int)x;
        }

        //public static IEnumerable getActiveSchedulesList()
        //{
        //    List<ViewModels.dateFilter> list = new List<ViewModels.dateFilter>();
        //    var x = (from A in db.dubbingTrnHdrs
        //            where A.status == true
        //            select new { A.fromDate }).Distinct().ToList();
        //    for (int i = 0; i < x.Count(); i++)
        //    {
        //        ViewModels.dateFilter f = new ViewModels.dateFilter();
        //        f.fromDate = x[i].fromDate.ToShortDateString();
        //        list.Add(f);
        //    }
        //    return list;
        //}

        
        public static int getTotalAssignedScripts(long orderIntno)
        {
            var x = db.orderBatchTrnHdrs.Where(b => b.orderIntno == orderIntno && b.trnType == "01" && b.status == true).ToList();
            if (x.Count() == 0)
                return 0;
            else
                return x.Sum(b => b.thruEpisode - b.fromEpisode + 1);
        }

        public static int getTotalTranslatedScripts(long orderIntno)
        {
            var x = db.orderBatchTrnHdrs.Where(b => b.orderIntno == orderIntno && b.trnType == "02" && b.status == true).ToList();
            if (x.Count() == 0)
                return 0;
            else
                return x.Sum(b => b.thruEpisode - b.fromEpisode + 1);
        }

        public static List<ViewModels.dayDatePair> getDaysList(DateTime? startDate)
        {
            List<ViewModels.dayDatePair> daysList = new List<ViewModels.dayDatePair>();
            if (startDate.HasValue)
            {
                DateTime calendarWeekFirstDay = startDate.Value;
                string definedFirstDayOfTheWeek = decodeDictionaryItem("settings", "fdw");
                while (calendarWeekFirstDay.DayOfWeek.ToString().ToLower() != definedFirstDayOfTheWeek.ToLower())
                    calendarWeekFirstDay = calendarWeekFirstDay.AddDays(-1);
                for (int i = 0; i < 7; i++)
                {
                    ViewModels.dayDatePair d = new ViewModels.dayDatePair();
                    d.dayDate = calendarWeekFirstDay.AddDays(i);
                    d.weekDay = d.dayDate.Value.DayOfWeek.ToString();
                    daysList.Add(d);
                }
            }
            else
            {
                foreach (DayOfWeek dw in Enum.GetValues(typeof(DayOfWeek)))
                {
                    ViewModels.dayDatePair d = new ViewModels.dayDatePair();
                    d.weekDay = dw.ToString();
                    daysList.Add(d);
                }
            }
            return daysList;
        }

        public static List<ViewModels.scheduleHdr> getDubbingSchedulesList()
        {
            List<ViewModels.scheduleHdr> scheduleList = new List<ViewModels.scheduleHdr>();
            var x = db.dubbingTrnHdrs.Where(b => b.status == true).OrderBy(b => b.fromDate).ToList();
            for (int i = 0; i < x.Count(); i++)
            {
                ViewModels.scheduleHdr item = new ViewModels.scheduleHdr();
                item.dubbTrnHdrIntno = x[i].dubbTrnHdrIntno;
                item.week = x[i].fromDate.ToShortDateString() + " ->  " + x[i].thruDate.ToShortDateString();
                scheduleList.Add(item);
            }
            return scheduleList;
        }

        public static List<ViewModels.carrierScheduleViewModel> getCarrierScheduleViewModel()
        {
            List<ViewModels.carrierScheduleViewModel> csList = new List<ViewModels.carrierScheduleViewModel>();
            var x = (from A in db.carriers
                     join B in db.carrierSchedules on A.carrierIntno equals B.carrierIntno
                     where A.status == true && B.status == true
                     select new { B.carrierScheduleIntno, A.carrierName, B.weekDay, B.flightTime, B.departure, B.destination, A.remarks }).ToList();
            for (int i = 0; i < x.Count(); i++)
            {
                ViewModels.carrierScheduleViewModel cs = new ViewModels.carrierScheduleViewModel();
                cs.carrierScheduleIntno = x[i].carrierScheduleIntno.ToString();
                cs.weekDay = x[i].weekDay;
                cs.summaryText = x[i].carrierName + " - Flight Time: " + x[i].flightTime
                                + " (" + decodeDictionaryItem("departure", x[i].departure)
                                + " -> " + decodeDictionaryItem("destination", x[i].destination) + ")";
                cs.remarks = x[i].remarks;
                csList.Add(cs);
            }
            return csList;
        }

        public static IEnumerable getCarriersList(string carrierType)
        {
            var x = db.carriers.Where(b => b.status == true);
            if (carrierType != null)
                return x.Where(b => b.carrierType == carrierType).ToList();
            else
                return x.ToList();
        }

        public static IEnumerable getCarrierSchedulesList(long? carrierIntno)
        {
            var x = (from A in db.carriers
                     join B in db.carrierSchedules on A.carrierIntno equals B.carrierIntno
                     join C in db.dubbDomains on B.departure equals C.domainCode
                     join D in db.dubbDomains on B.destination equals D.domainCode
                     where C.domainName == "departure" && D.domainName == "destination"
                     select new { B.carrierScheduleIntno, A.carrierIntno, B.flightTime, departure = C.domainValue, destination = D.domainValue });
            if (carrierIntno.HasValue)
                return x.Where(b => b.carrierIntno == carrierIntno).ToList();
            else
                return x.ToList();
        }
        
    }
}

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = dt.DayOfWeek - startOfWeek;
        if (diff < 0)
        {
            diff += 7;
        }

        return dt.AddDays(-1 * diff).Date;
    }
}