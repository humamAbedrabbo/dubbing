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
    
    public partial class paymentDetail
    {
        public long paymentDtlIntno { get; set; }
        public long paymentIntno { get; set; }
        public System.DateTime dubbingDate { get; set; }
        public int totalUnits { get; set; }
    
        public virtual payment payment { get; set; }
    }
}
