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
    
    public partial class workPersonnel
    {
        public long workPersonnelIntno { get; set; }
        public long workIntno { get; set; }
        public long empIntno { get; set; }
        public string titleType { get; set; }
        public System.DateTime fromDate { get; set; }
        public Nullable<System.DateTime> thruDate { get; set; }
        public bool status { get; set; }
    
        public virtual agreementWork agreementWork { get; set; }
        public virtual employee employee { get; set; }
    }
}
