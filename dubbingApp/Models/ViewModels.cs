using System;
using System.Collections.Generic;
using dubbingModel;

namespace dubbingApp.Models
{
    public class ViewModels
    {
        public struct customPair
        {
            public long Key;
            public string Value;
        }
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

        public class dubbingSceneViewModel
        {
            public long dubbSheetDtlIntno { get; set; }
            public long dubbSheetHdrIntno { get; set; }
            public long orderTrnHdrIntno { get; set; }
            public long workIntno { get; set; }
            public string workName { get; set; }
            public int episodeNo { get; set; }
            public int sceneNo { get; set; }
            public long actor { get; set; }
            public string actorName { get; set; }
            public string startTimeCode { get; set; }
            public bool isTaken { get; set; }
        }

        public class usersViewModel
        {
            public string userId { get; set; }
            public string roleId { get; set; }
            public string userName { get; set; }
            public string personnelName { get; set; }
        }
        
    }
}