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
    
    public partial class voiceActor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public voiceActor()
        {
            this.dubbingAppointments = new HashSet<dubbingAppointment>();
            this.dubbingSheetHdrs = new HashSet<dubbingSheetHdr>();
            this.payments = new HashSet<payment>();
            this.workActors = new HashSet<workActor>();
        }
    
        public long voiceActorIntno { get; set; }
        public string fullName { get; set; }
        public string othFullName { get; set; }
        public string mobileNo { get; set; }
        public string landLineNo { get; set; }
        public string email { get; set; }
        public bool status { get; set; }
        public string accountNo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingAppointment> dubbingAppointments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingSheetHdr> dubbingSheetHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<payment> payments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<workActor> workActors { get; set; }
    }
}
