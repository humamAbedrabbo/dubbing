//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace dubbingModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class sceneScentence
    {
        public int scentenceIntno { get; set; }
        public int dialogIntno { get; set; }
        public int scentenceNo { get; set; }
        public Nullable<long> workCharacterIntno { get; set; }
        public string characterName { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string scentence { get; set; }
    
        public virtual sceneDialog sceneDialog { get; set; }
        public virtual workCharacter workCharacter { get; set; }
    }
}
