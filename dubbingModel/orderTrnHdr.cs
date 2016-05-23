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
    
    public partial class orderTrnHdr
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public orderTrnHdr()
        {
            this.deliveryFeedbacks = new HashSet<deliveryFeedback>();
            this.dubbingSheetDtls = new HashSet<dubbingSheetDtl>();
            this.dubbingSheetHdrs = new HashSet<dubbingSheetHdr>();
            this.dubbingTrnDtls = new HashSet<dubbingTrnDtl>();
            this.orderChecks = new HashSet<orderCheck>();
            this.orderTrnDtls = new HashSet<orderTrnDtl>();
            this.shipments = new HashSet<shipment>();
        }
    
        public long orderTrnHdrIntno { get; set; }
        public long workIntno { get; set; }
        public long orderIntno { get; set; }
        public short episodeNo { get; set; }
        public string priority { get; set; }
        public Nullable<System.DateTime> orderReceivedDate { get; set; }
        public Nullable<System.DateTime> expectedDeliveryDate { get; set; }
        public Nullable<bool> allowFirstDubbing { get; set; }
        public Nullable<System.DateTime> startTranslation { get; set; }
        public Nullable<System.DateTime> endTranslation { get; set; }
        public Nullable<System.DateTime> startAdaptation { get; set; }
        public Nullable<System.DateTime> endAdaptation { get; set; }
        public Nullable<System.DateTime> startDischarge { get; set; }
        public Nullable<System.DateTime> endDischarge { get; set; }
        public Nullable<System.DateTime> plannedDubbing { get; set; }
        public Nullable<System.DateTime> startDubbing { get; set; }
        public Nullable<System.DateTime> endDubbing { get; set; }
        public Nullable<System.DateTime> startMixage { get; set; }
        public Nullable<System.DateTime> endMixage { get; set; }
        public Nullable<System.DateTime> startMontage { get; set; }
        public Nullable<System.DateTime> endMontage { get; set; }
        public Nullable<System.DateTime> plannedUpload { get; set; }
        public Nullable<System.DateTime> shipmentLowRes { get; set; }
        public Nullable<System.DateTime> plannedShipment { get; set; }
        public Nullable<System.DateTime> shipmentFinal { get; set; }
        public string status { get; set; }
    
        public virtual agreementWork agreementWork { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<deliveryFeedback> deliveryFeedbacks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingSheetDtl> dubbingSheetDtls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingSheetHdr> dubbingSheetHdrs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dubbingTrnDtl> dubbingTrnDtls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orderCheck> orderChecks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orderTrnDtl> orderTrnDtls { get; set; }
        public virtual workOrder workOrder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<shipment> shipments { get; set; }
    }
}
