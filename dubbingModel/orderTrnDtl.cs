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
    
    public partial class orderTrnDtl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public orderTrnDtl()
        {
            this.studioEpisodes = new HashSet<studioEpisode>();
        }
    
        public long orderTrnDtlIntno { get; set; }
        public long orderTrnHdrIntno { get; set; }
        public string activityType { get; set; }
        public long empIntno { get; set; }
        public string providedService { get; set; }
        public Nullable<int> totalMinutes { get; set; }
        public string timeRating { get; set; }
        public string qualityRating { get; set; }
        public string fromTimeCode { get; set; }
        public string thruTimeCode { get; set; }
        public bool status { get; set; }
        public Nullable<System.DateTime> assignedDate { get; set; }
        public Nullable<System.DateTime> forDueDate { get; set; }
    
        public virtual employee employee { get; set; }
        public virtual orderTrnHdr orderTrnHdr { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<studioEpisode> studioEpisodes { get; set; }
    }
}
