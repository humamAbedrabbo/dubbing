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
    
    public partial class shipment
    {
        public long shipmentIntno { get; set; }
        public long orderTrnHdrIntno { get; set; }
        public Nullable<long> carrierIntno { get; set; }
        public Nullable<long> carrierScheduleIntno { get; set; }
        public string shipmentMethod { get; set; }
        public System.DateTime shipmentDate { get; set; }
        public string remarks { get; set; }
        public string status { get; set; }
        public long clientIntno { get; set; }
    
        public virtual carrier carrier { get; set; }
        public virtual carrierSchedule carrierSchedule { get; set; }
        public virtual client client { get; set; }
        public virtual orderTrnHdr orderTrnHdr { get; set; }
    }
}
