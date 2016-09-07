using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dubbingApp.Models
{
    public class AdaptationViewModel
    {
        public string Title { get; set; }
        public int SceneMin { get; set; }
        public int SceneMax { get; set; }
        public int SceneMinNo { get; set; }
        public int SceneMaxNo { get; set; }
        public List<ASubtitle> Subtitles { get; set; }
        public List<ACharacter> Characters { get; set; }

        public AdaptationViewModel()
        {
            Title = "Sample Work / 1";
            SceneMin = 300;
            SceneMax = 1300;
            SceneMinNo = 1000;
            SceneMaxNo = 1999;
            Subtitles = new List<Models.ASubtitle>()
            {
                new ASubtitle() { Id = 1, SceneNo = 1, DlgNo = 1, CharacterName = "John", StartTime = "00:07:10", EndTime = "00:07:20", Start = 430, End = 440, Text = "subtitle 430" },
                new ASubtitle() { Id = 2, SceneNo = 1, DlgNo = 1, CharacterName = "Fatime", StartTime = "00:07:30", EndTime = "00:07:40", Start = 450, End = 460, Text = "subtitle 450" },
                new ASubtitle() { Id = 3, SceneNo = 2, DlgNo = 1, CharacterName = "John", StartTime = "00:08:10", EndTime = "00:08:20", Start = 490, End = 500, Text = "subtitle 490" },
                new ASubtitle() { Id = 4, SceneNo = 2, DlgNo = 1, CharacterName = "Fatime", StartTime = "00:08:30", EndTime = "00:08:40", Start = 510, End = 520, Text = "subtitle 510" }

            };
            Characters = new List<ACharacter>()
            {
                new ACharacter() { Id = 1, Name = "John", Gender = "M", Type = "Hero" },
                new ACharacter() { Id = 2, Name = "Fatima", Gender = "F", Type = "Main" },
                new ACharacter() { Id = 3, Name = "Albert", Gender = "M", Type = "Anonymous" },
                new ACharacter() { Id = 4, Name = "Soosoo", Gender = "F", Type = "Anonymous" }
            };
        }
    }

    public class ACharacter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Gender { get; set; }
    }
    public class ASubtitle
    {
        public int Id { get; set; }
        public int No { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Text { get; set; }
        public int SceneId { get; set; }
        public int SceneNo { get; set; }
        public int DlgId { get; set; }
        public int DlgNo { get; set; }
        public int? CharacterId { get; set; }
        public string CharacterName { get; set; }
    }
}