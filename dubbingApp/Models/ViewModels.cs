using System;
using System.Collections.Generic;
using dubbingModel;

namespace dubbingApp.Models
{
    public class ViewModels
    {
        public class episodeItem
        {
            public dubbingTrnDtl episode { get; set; }
            public string status { get; set; }
        }
        public class studioEpisodeItem
        {
            public long studioEpisodeIntno { get; set; }
            public int episodeNo { get; set; }
            public string studioNo { get; set; }
            public bool status { get; set; }
        }
        public class scheduleViewModel
        {
            public long workIntno { get; set; }
            public string workName { get; set; }
            public int episodesPerWeek { get; set; }
            public long dubbTrnHdrIntno { get; set; }
            public DateTime fromDate { get; set; }
            public DateTime thruDate { get; set; }
            public bool isScheduled { get; set; }
            public List<episodeItem> episodesList { get; set; }
            public List<studioEpisodeItem> studioEpisodesList { get; set; }
        }

        
        //old
        public class domainLov
        {
            public string domainCode { get; set; }
            public string domainValue { get; set; }
            public bool status { get; set; }
        }

        public class keyValuePair
        {
            public long key { get; set; }
            public string value { get; set; }
        }

        public class dayDatePair
        {
            public string weekDay { get; set; }
            public DateTime? dayDate { get; set; }
        }

        public class preDubbingViewModel
        {
            public string receivedFrom { get; set; }
            public DateTime receivedDate { get; set; }
            public long workIntno { get; set; }
            public short fromEpisode { get; set; }
            public short thruEpisode { get; set; }
        }

        public class receivedFilesViewModel
        {
            public short episodeNo { get; set; }
            public string fileName { get; set; }
        }

        public class scheduleHdr
        {
            public long dubbTrnHdrIntno { get; set; }
            public string week { get; set; }
        }

        public class orderScheduleViewModel
        {
            public long dubbTrnDtlIntno { get; set; }
            public long orderTrnHdrIntno { get; set; }
            public long workIntno { get; set; }
            public string workName { get; set; }
            public short episodeNo { get; set; }
            public string studioNo { get; set; }
            public System.Nullable<DateTime> planFromDate { get; set; }
            public System.Nullable<DateTime> planThruDate { get; set; }
            public System.Nullable<long> supervisor { get; set; }
            public System.Nullable<long> soundTechnician { get; set; }
            public System.Nullable<long> assistant { get; set; }
        }

        public class dubbTrnViewModel
        {
            public long dubbSheetHdrIntno { get; set; }
            public long dubbTrnDtlIntno { get; set; }
            public string studioNo { get; set; }
            public string studioTeam { get; set; }
            public long supervisor { get; set; }
            public long? soundTechnician { get; set; }
            public string workName { get; set; }
            public string episodeNo { get; set; }
            public string characterName { get; set; }
            public string voiceActorName { get; set; }
            public string characterType { get; set; }
            public string totalScenes { get; set; }
            public string takenScenes { get; set; }
            public string remainingScenes { get; set; }
            public string currentScene { get; set; }
        }

        public class dubbScheduleDtlViewModel
        {
            public long dubbTrnDtlIntno { get; set; }
            public long orderTrnHdrIntno { get; set; }
            public long dubbTrnHdrIntno { get; set; }
            public string studioNo { get; set; }
            public long? supervisor { get; set; }
            public long? soundTechnician { get; set; }
            public long? assistant { get; set; }
            public string workName { get; set; }
            public short episodeNo { get; set; }
        }

        public class dubbAppointmentViewModel
        {
            public long? dubbAppointIntno { get; set; }
            public long dubbSheetHdrIntno { get; set; }
            public long dubbTrnDtlIntno { get; set; }
            public string characterName { get; set; }
            public string voiceActorName { get; set; }
            public string totalScenes { get; set; }
            public System.Nullable<DateTime> appointmentDate { get; set; }
            public System.Nullable<DateTime> fromTime { get; set; }
            public System.Nullable<DateTime> thruTime { get; set; }
            public string remarks { get; set; }
            public bool isIncluded { get; set; }
        }

        public class makeAppointmentViewModel
        {
            public long dubbAppointIntno { get; set; }
            public string voiceActorName { get; set; }
            public string totalScenes { get; set; }
            public string totalMinutes { get; set; }
            public System.Nullable<DateTime> appointmentDate { get; set; }
            public System.Nullable<DateTime> fromTime { get; set; }
            public System.Nullable<DateTime> thruTime { get; set; }
            public System.Nullable<DateTime> actualFromTime { get; set; }
            public System.Nullable<DateTime> actualThruTime { get; set; }
            public string mobiles { get; set; }
            public string landlines { get; set; }
            public string emails { get; set; }
            public string remarks { get; set; }
            public string workName { get; set; }
            public int? episodeNo { get; set; }
            public string characterName { get; set; }
            public string studioName { get; set; }
            public long? srl { get; set; }
            public string supervisorName { get; set; }
            public string soundTechnicianName { get; set; }
        }

        public class appointmentMatrixViewModel
        {
            public long dubbTrnDtlIntno { get; set; }
            public string studioNo { get; set; }
            public DateTime appointmentDate { get; set; }
            public string workName { get; set; }
            public bool? isLocked { get; set; }
        }

        public class appointmentExportViewModel
        {
            public int? rowSrl { get; set; }
            public int? colSrl { get; set; }
            public string workName { get; set; }
            public string voiceActorName { get; set; }
            public string studio { get; set; }
            public string supervisor { get; set; }
            public string dubbingDate { get; set; }
            public string bookedTime { get; set; }
        }

        public class dubbSheetUploadData
        {
            public string characterCode { get; set; }
            public short sceneNo { get; set; }
        }

        public class dateFilter
        {
            public string fromDate { get; set; }
            public string thruDate { get; set; }
        }

        public class studiosActivitiesViewModel
        {
            public string ID { get; set; }
            public string parentID { get; set; }
            public string dubbTrnDtlIntno { get; set; }
            public string studioName { get; set; }
            public string studioTeam { get; set; }
            public string workName { get; set; }
            public string episodeNo { get; set; }
            public string dubbingDate { get; set; }
            public string dubbingStarted { get; set; }
            public string totalScenes { get; set; }
            public string registeredScenes { get; set; }
            public string remainingScenes { get; set; }
            public string dubbingFinished { get; set; }
        }

        public class carrierScheduleViewModel
        {
            public string carrierScheduleIntno { get; set; }
            public string summaryText { get; set; }
            public string weekDay { get; set; }
            public string remarks { get; set; }
        }

        public class userRolesViewModel
        {
            public int userId { get; set; }
            public int roleId { get; set; }
            public bool isGranted { get; set; }
        }

        // for upload excel
        public class Record
        {
            public string characterCode;
            public string characterName;
            public string actor;
            public List<short> scenes;

            public Record()
            {
                scenes = new List<short>();
            }
        }

        
    }
}