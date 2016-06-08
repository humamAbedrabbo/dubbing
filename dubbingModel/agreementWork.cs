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
    
    public partial class agreementWork
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public agreementWork()
        {
            this.orderBatchTrnHdrs = new HashSet<orderBatchTrnHdr>();
            this.orderTrnHdrs = new HashSet<orderTrnHdr>();
            this.workActors = new HashSet<workActor>();
            this.workCharacters = new HashSet<workCharacter>();
            this.workOrders = new HashSet<workOrder>();
            this.workPersonnels = new HashSet<workPersonnel>();
            this.dubbingTrnDtls = new HashSet<dubbingTrnDtl>();
            this.dubbingAppointments = new HashSet<dubbingAppointment>();
            this.studios = new HashSet<studio>();
            this.payments = new HashSet<payment>();
        }
    
        public long workIntno { get; set; }
        public long agreementIntno { get; set; }
        public string workType { get; set; }
        public string workName { get; set; }
        public string othWorkName { get; set; }
        public string workNationality { get; set; }
        public string workOriginalLanguage { get; set; }
        public Nullable<System.DateTime> firstEpisodeShowDate { get; set; }
        public short totalNbrEpisodes { get; set; }
        public short totalWeekNbrEpisodes { get; set; }
        public Nullable<long> contactIntno { get; set; }
        public string status { get; set; }
    
        public virtual agreement agreement { get; set; }
        public virtual contact contact { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orderBatchTrnHdr> orderBatchTrnHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orderTrnHdr> orderTrnHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<workActor> workActors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<workCharacter> workCharacters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<workOrder> workOrders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<workPersonnel> workPersonnels { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingTrnDtl> dubbingTrnDtls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingAppointment> dubbingAppointments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<studio> studios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<payment> payments { get; set; }
    }
}
