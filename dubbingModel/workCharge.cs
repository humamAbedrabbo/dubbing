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
    
    public partial class workCharge
    {
        public long chargeIntno { get; set; }
        public long workIntno { get; set; }
        public string workPartyType { get; set; }
        public long workPartyIntno { get; set; }
        public long chargeAmount { get; set; }
        public string chargeUom { get; set; }
        public string currencyCode { get; set; }
        public System.DateTime fromDate { get; set; }
        public string remarks { get; set; }
        public bool status { get; set; }
    }
}
