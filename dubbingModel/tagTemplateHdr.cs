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
    
    public partial class tagTemplateHdr
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tagTemplateHdr()
        {
            this.audioSampleHdrs = new HashSet<audioSampleHdr>();
            this.chrTemplateHdrs = new HashSet<chrTemplateHdr>();
            this.tagTemplateDtls = new HashSet<tagTemplateDtl>();
        }
    
        public long tagTemplateHdrIntno { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<audioSampleHdr> audioSampleHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<chrTemplateHdr> chrTemplateHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tagTemplateDtl> tagTemplateDtls { get; set; }
    }
}
