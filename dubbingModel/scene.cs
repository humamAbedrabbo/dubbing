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
    
    public partial class scene
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public scene()
        {
            this.dialogs = new HashSet<dialog>();
        }
    
        public long sceneIntno { get; set; }
        public long orderTrnHdrIntno { get; set; }
        public short sceneNo { get; set; }
        public string startTimeCode { get; set; }
        public string endTimeCode { get; set; }
        public bool isTaken { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dialog> dialogs { get; set; }
        public virtual orderTrnHdr orderTrnHdr { get; set; }
    }
}