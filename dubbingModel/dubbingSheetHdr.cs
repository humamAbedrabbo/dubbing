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
    
    public partial class dubbingSheetHdr
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dubbingSheetHdr()
        {
            this.dubbingAppointments = new HashSet<dubbingAppointment>();
            this.dubbingSheetDtls = new HashSet<dubbingSheetDtl>();
        }
    
        public long dubbSheetHdrIntno { get; set; }
        public long orderTrnHdrIntno { get; set; }
        public long workCharacterIntno { get; set; }
        public long voiceActorIntno { get; set; }
        public string anonymActorName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingAppointment> dubbingAppointments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingSheetDtl> dubbingSheetDtls { get; set; }
        public virtual orderTrnHdr orderTrnHdr { get; set; }
        public virtual voiceActor voiceActor { get; set; }
        public virtual workCharacter workCharacter { get; set; }
    }
}
