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
    
    public partial class audioSampleDtl
    {
        public long audioSampleDtlIntno { get; set; }
        public long audioSampleHdrIntno { get; set; }
        public int tagId { get; set; }
        public int TagScore { get; set; }
        public double Match { get; set; }
    
        public virtual audioSampleHdr audioSampleHdr { get; set; }
        public virtual tag tag { get; set; }
    }
}
