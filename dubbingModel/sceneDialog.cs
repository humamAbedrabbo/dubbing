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
    
    public partial class sceneDialog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sceneDialog()
        {
            this.sceneScentences = new HashSet<sceneScentence>();
        }
    
        public int dialogIntno { get; set; }
        public int sceneIntno { get; set; }
        public int dialogNo { get; set; }
    
        public virtual adaptationScene adaptationScene { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sceneScentence> sceneScentences { get; set; }
    }
}
