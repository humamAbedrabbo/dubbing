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
            public orderTrnDtl episode { get; set; }
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
            public long sceneIntno { get; set; }
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
            public string userName { get; set; }
            public string authenticName { get; set; }
            public string roles { get; set; }
        }
        
        public class castingListViewModel
        {
            public long dubbSheetHdrIntno { get; set; }
            public long orderTrnHdrIntno { get; set; }
            public long? workCharacterIntno { get; set; }
            public string characterName { get; set; }
            public long voiceActorIntno { get; set; }
            public string actorName { get; set; }
            public int totalScenes { get; set; }
            public bool isEndorsed { get; set; }
        }

        public class assignmentViewModel
        {
            public long orderTrnDtlIntno { get; set; }
            public long empIntno { get; set; }
            public string empName { get; set; }
            public long workIntno { get; set; }
            public string workName { get; set; }
            public short episodeNo { get; set; }
            public string dueDate { get; set; }
            public long? studioIntno { get; set; }
            public long? studioEpisodeIntno { get; set; }
            public string studioNo { get; set; }
            public long? dubbTrnHdrIntno { get; set; }
            public string schedule { get; set; }
            public string status { get; set; }
        }

    }
}