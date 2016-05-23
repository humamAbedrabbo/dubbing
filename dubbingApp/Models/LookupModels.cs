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

        public static IEnumerable getScheduledStudiosList(long schedule)
        {
            List<ViewModels.keyValuePair> studiosList = new List<ViewModels.keyValuePair>();
            var x = db.dubbingTrnDtls.Where(b => b.dubbTrnHdrIntno == schedule).Select(b => new {b.dubbTrnDtlIntno, b.studioNo}).ToList();
            for (int i = 0; i < x.Count(); i++)
            {
                ViewModels.keyValuePair std = new ViewModels.keyValuePair();
                std.key = x[i].dubbTrnDtlIntno;
                std.value = decodeDictionaryItem("studio", x[i].studioNo);
                studiosList.Add(std);
            }
            return studiosList;
        }

        public static IEnumerable getRelocationStudiosList(long apt)
        {
            var y = (from A in db.dubbingAppointments
                     join B in db.dubbingTrnDtls on A.dubbTrnDtlIntno equals B.dubbTrnDtlIntno
                     where A.dubbAppointIntno == apt
                     select new { B.orderTrnHdrIntno, B.dubbTrnHdrIntno, B.dubbTrnDtlIntno }).FirstOrDefault();
            long order = y.orderTrnHdrIntno;
            long hdr = y.dubbTrnHdrIntno;
            long dtl = y.dubbTrnDtlIntno;
            List<ViewModels.keyValuePair> studiosList = new List<ViewModels.keyValuePair>();
            var x = (from A in db.dubbingTrnDtls
                     where A.dubbTrnHdrIntno == hdr && A.orderTrnHdrIntno == order && A.dubbTrnDtlIntno != dtl
                     select new { A.dubbTrnDtlIntno, A.studioNo }).ToList();
            for (int i = 0; i < x.Count(); i++)
            {
                ViewModels.keyValuePair std = new ViewModels.keyValuePair();
                std.key = x[i].dubbTrnDtlIntno;
                std.value = decodeDictionaryItem("studio", x[i].studioNo);
                studiosList.Add(std);
            }
            return studiosList;
        }

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

        public static IEnumerable getAppointmentMatrixModel(DateTime calendarWeekFirstDay, DateTime calendarWeekLastDay, long? dubbAppointIntno)
        {
            var x = (from A in db.dubbingTrnDtls
                     join B in db.orderTrnHdrs on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                     where A.status == "02" && A.dubbingDate >= calendarWeekFirstDay && A.dubbingDate <= calendarWeekLastDay
                     select new { A.dubbTrnDtlIntno, A.studioNo, A.dubbingDate, B.workIntno, B.episodeNo }).Distinct().ToList();
            List<ViewModels.appointmentMatrixViewModel> aptList = new List<ViewModels.appointmentMatrixViewModel>();
            for (int i = 0; i < x.Count(); i++)
            {
                ViewModels.appointmentMatrixViewModel apt = new ViewModels.appointmentMatrixViewModel();
                apt.dubbTrnDtlIntno = x[i].dubbTrnDtlIntno;
                apt.appointmentDate = x[i].dubbingDate.Value;
                apt.studioNo = x[i].studioNo;
                long workIntno = x[i].workIntno;
                apt.workName = db.agreementWorks.FirstOrDefault(b => b.workIntno == workIntno).workName + " (" + x[i].episodeNo + ")";
                int cnt = db.dubbingAppointments.Where(b => b.dubbTrnDtlIntno == apt.dubbTrnDtlIntno && b.fromTime == null).Count();
                if (cnt != 0)
                    apt.workName = apt.workName + "*";
                // check if it is locked for a selected appointment
                apt.isLocked = false;
                if (dubbAppointIntno.HasValue)
                {
                    long voiceActorIntno = db.dubbingAppointments.FirstOrDefault(b => b.dubbAppointIntno == dubbAppointIntno).dubbingSheetHdr.voiceActorIntno;
                    var y = (from A in db.dubbingSheetHdrs
                             join B in db.dubbingAppointments on A.dubbSheetHdrIntno equals B.dubbSheetHdrIntno
                             join C in db.dubbingTrnDtls on B.dubbTrnDtlIntno equals C.dubbTrnDtlIntno
                             join D in db.dubbingTrnHdrs on C.dubbTrnHdrIntno equals D.dubbTrnHdrIntno
                             where A.voiceActorIntno == voiceActorIntno && B.appointmentDate >= calendarWeekFirstDay && B.appointmentDate <= calendarWeekLastDay && B.fromTime == null
                             select new { A.dubbSheetHdrIntno }).ToList();
                    if (y.Count() != 0)
                        apt.isLocked = true;
                }
                aptList.Add(apt);
            }
            return aptList;
        }

        public static IEnumerable getAppointmentActorsList(DateTime? appointmentDate)
        {
            if (appointmentDate.HasValue)
                return (from A in db.dubbingAppointments
                        join B in db.dubbingSheetHdrs on A.dubbSheetHdrIntno equals B.dubbSheetHdrIntno
                        where A.appointmentDate == appointmentDate
                        select new { B.dubbSheetHdrIntno, voiceActorName = B.actorName }).ToList();
            else
                return null;
        }

        public static IEnumerable getScheduleCharactersList(long? dubbTrnDtlIntno)
        {
            if (dubbTrnDtlIntno.HasValue)
                return (from A in db.dubbingAppointments
                        join B in db.dubbingSheetHdrs on A.dubbSheetHdrIntno equals B.dubbSheetHdrIntno
                        where A.dubbTrnDtlIntno == dubbTrnDtlIntno
                        select new { B.dubbSheetHdrIntno, voiceActorName = B.characterName + " / " + B.actorName }).ToList();
            else
                return null;
        }

        public static IEnumerable getScheduleActorsList(long dubbTrnHdrIntno)
        {
                return (from A in db.dubbingAppointments
                        join B in db.dubbingSheetHdrs on A.dubbSheetHdrIntno equals B.dubbSheetHdrIntno
                        join C in db.dubbingTrnDtls on A.dubbTrnDtlIntno equals C.dubbTrnDtlIntno
                        where C.dubbTrnHdrIntno == dubbTrnHdrIntno
                        select new { B.dubbSheetHdrIntno, voiceActorName = B.actorName + " (" + B.characterName + ")" }).ToList();
        }

        public static IEnumerable getScheduleMatrixModel(DateTime calendarWeekFirstDay, DateTime calendarWeekLastDay)
        {
            var x = (from A in db.dubbingTrnDtls
                     join B in db.orderTrnHdrs on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                     where (A.status == "01" || A.status == "02") && A.dubbingDate >= calendarWeekFirstDay && A.dubbingDate <= calendarWeekLastDay
                     select new { A.dubbTrnDtlIntno, B.orderTrnHdrIntno, A.dubbingDate, A.studioNo, B.workIntno, B.episodeNo, A.supervisor, A.soundTechnician, A.assistant }).Distinct().ToList();
            List<ViewModels.orderScheduleViewModel> schList = new List<ViewModels.orderScheduleViewModel>();
            for (int i = 0; i < x.Count(); i++)
            {
                ViewModels.orderScheduleViewModel sch = new ViewModels.orderScheduleViewModel();
                long dubbTrnDtlIntno = x[i].dubbTrnDtlIntno;
                sch.dubbTrnDtlIntno = dubbTrnDtlIntno;
                sch.orderTrnHdrIntno = x[i].orderTrnHdrIntno;
                sch.planFromDate = x[i].dubbingDate;
                sch.studioNo = x[i].studioNo;
                sch.supervisor = x[i].supervisor;
                sch.soundTechnician = x[i].soundTechnician;
                sch.assistant = x[i].assistant;
                long workIntno = x[i].workIntno;
                int cnt = db.dubbingAppointments.Where(b => b.dubbTrnDtlIntno == dubbTrnDtlIntno).Count();
                if (cnt == 0 || x[i].studioNo == null || !x[i].supervisor.HasValue || !x[i].soundTechnician.HasValue)
                    sch.workName = db.agreementWorks.FirstOrDefault(b => b.workIntno == workIntno).workName + " (" + x[i].episodeNo + "*)";
                else
                    sch.workName = db.agreementWorks.FirstOrDefault(b => b.workIntno == workIntno).workName + " (" + x[i].episodeNo + ")";
                sch.episodeNo = x[i].episodeNo;
                schList.Add(sch);
            }
            return schList;
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