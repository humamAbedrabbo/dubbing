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
    
    public partial class studio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public studio()
        {
            this.dubbingAppointments = new HashSet<dubbingAppointment>();
            this.studioEpisodes = new HashSet<studioEpisode>();
        }
    
        public long studioIntno { get; set; }
        public long dubbTrnHdrIntno { get; set; }
        public long workIntno { get; set; }
        public string studioNo { get; set; }
        public long supervisor { get; set; }
        public long sound { get; set; }
        public bool isDefaultTeam { get; set; }
        public bool status { get; set; }
    
        public virtual agreementWork agreementWork { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingAppointment> dubbingAppointments { get; set; }
        public virtual dubbingTrnHdr dubbingTrnHdr { get; set; }
        public virtual employee employee { get; set; }
        public virtual employee employee1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<studioEpisode> studioEpisodes { get; set; }
    }
}