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
    
    public partial class clientClaim
    {
        public long claimIntno { get; set; }
        public long orderTrnHdrIntno { get; set; }
        public long clientIntno { get; set; }
        public string claimType { get; set; }
        public System.DateTime receivedDate { get; set; }
        public string claimRefNo { get; set; }
        public string claimDesc { get; set; }
        public string refLocation { get; set; }
        public string requiredAction { get; set; }
        public Nullable<System.DateTime> actionDate { get; set; }
        public bool status { get; set; }
    
        public virtual client client { get; set; }
        public virtual orderTrnHdr orderTrnHdr { get; set; }
    }
}
