using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dubbingApp.Models
{
    public class AdaptationViewModel
    {
        public long OrderTrnHdrIntno { get; set; }
        public string Title { get; set; }
        public int SceneMin { get; set; }
        public int SceneMax { get; set; }
        public int SceneMinNo { get; set; }
        public int SceneMaxNo { get; set; }
        public List<ASubtitle> Subtitles { get; set; }
        public List<ACharacter> Characters { get; set; }

        public AdaptationViewModel()
        {
            Subtitles = new List<ASubtitle>();
            Characters = new List<ACharacter>();
        }
    }

    public class ACharacter
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Gender { get; set; }
    }
    public class ASubtitle
    {
        public long Id { get; set; }
        public int No { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Text { get; set; }
        public long SceneId { get; set; }
        public int SceneNo { get; set; }
        public long DlgId { get; set; }
        public int DlgNo { get; set; }
        public long? CharacterId { get; set; }
        public string CharacterName { get; set; }
    }
}