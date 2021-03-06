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
    
    public partial class dubbingAppointment
    {
        public long dubbAppointIntno { get; set; }
        public long voiceActorIntno { get; set; }
        public string actorName { get; set; }
        public long studioIntno { get; set; }
        public System.DateTime appointmentDate { get; set; }
        public Nullable<System.DateTime> fromTime { get; set; }
        public Nullable<System.DateTime> thruTime { get; set; }
        public long workIntno { get; set; }
        public Nullable<int> totalScenes { get; set; }
        public Nullable<System.DateTime> actualFromTime { get; set; }
        public Nullable<System.DateTime> actualThruTime { get; set; }
        public string remarks { get; set; }
        public int totalMinutes { get; set; }
    
        public virtual agreementWork agreementWork { get; set; }
        public virtual studio studio { get; set; }
        public virtual voiceActor voiceActor { get; set; }
    }
}
