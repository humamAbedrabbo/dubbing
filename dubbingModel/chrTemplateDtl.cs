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
    
    public partial class chrTemplateDtl
    {
        public long chrTemplateDtlIntno { get; set; }
        public long chrTemplateHdrIntno { get; set; }
        public int tagId { get; set; }
        public int Weight { get; set; }
    
        public virtual chrTemplateHdr chrTemplateHdr { get; set; }
        public virtual tag tag { get; set; }
    }
}