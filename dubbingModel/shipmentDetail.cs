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
    
    public partial class shipmentDetail
    {
        public long shipmentDtlIntno { get; set; }
        public long shipmentIntno { get; set; }
        public long orderTrnHdrIntno { get; set; }
    
        public virtual orderTrnHdr orderTrnHdr { get; set; }
        public virtual shipment shipment { get; set; }
    }
}
