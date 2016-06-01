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
    
    public partial class dubbingTrnHdr
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dubbingTrnHdr()
        {
            this.payments = new HashSet<payment>();
            this.dubbingTrnDtls = new HashSet<dubbingTrnDtl>();
            this.studios = new HashSet<studio>();
        }
    
        public long dubbTrnHdrIntno { get; set; }
        public System.DateTime fromDate { get; set; }
        public System.DateTime thruDate { get; set; }
        public bool isPaid { get; set; }
        public bool status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<payment> payments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingTrnDtl> dubbingTrnDtls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<studio> studios { get; set; }
    }
}
