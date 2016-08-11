﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DUBBDBEntities : DbContext
    {
        public DUBBDBEntities()
            : base("name=DUBBDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<agreement> agreements { get; set; }
        public virtual DbSet<agreementSpec> agreementSpecs { get; set; }
        public virtual DbSet<agreementTerm> agreementTerms { get; set; }
        public virtual DbSet<agreementWork> agreementWorks { get; set; }
        public virtual DbSet<carrier> carriers { get; set; }
        public virtual DbSet<client> clients { get; set; }
        public virtual DbSet<contact> contacts { get; set; }
        public virtual DbSet<dubbDomain> dubbDomains { get; set; }
        public virtual DbSet<dubbingSheetDtl> dubbingSheetDtls { get; set; }
        public virtual DbSet<dubbingSheetHdr> dubbingSheetHdrs { get; set; }
        public virtual DbSet<dubbingTrnHdr> dubbingTrnHdrs { get; set; }
        public virtual DbSet<employee> employees { get; set; }
        public virtual DbSet<orderBatchTrnHdr> orderBatchTrnHdrs { get; set; }
        public virtual DbSet<orderCheck> orderChecks { get; set; }
        public virtual DbSet<orderTrnDtl> orderTrnDtls { get; set; }
        public virtual DbSet<orderTrnHdr> orderTrnHdrs { get; set; }
        public virtual DbSet<voiceActor> voiceActors { get; set; }
        public virtual DbSet<workActor> workActors { get; set; }
        public virtual DbSet<workCharacter> workCharacters { get; set; }
        public virtual DbSet<workCharge> workCharges { get; set; }
        public virtual DbSet<workOrder> workOrders { get; set; }
        public virtual DbSet<workPersonnel> workPersonnels { get; set; }
        public virtual DbSet<dubbingTrnDtl> dubbingTrnDtls { get; set; }
        public virtual DbSet<dubbingAppointment> dubbingAppointments { get; set; }
        public virtual DbSet<studio> studios { get; set; }
        public virtual DbSet<studioEpisode> studioEpisodes { get; set; }
        public virtual DbSet<shipmentDetail> shipmentDetails { get; set; }
        public virtual DbSet<shipment> shipments { get; set; }
        public virtual DbSet<clientClaim> clientClaims { get; set; }
        public virtual DbSet<paymentDetail> paymentDetails { get; set; }
        public virtual DbSet<payment> payments { get; set; }
        public virtual DbSet<paymentTemp> paymentTemps { get; set; }
        public virtual DbSet<dialog> dialogs { get; set; }
        public virtual DbSet<scene> scenes { get; set; }
        public virtual DbSet<subtitle> subtitles { get; set; }
    
        public virtual int archiveEndorsedWork(Nullable<long> workIntno, string respMsg)
        {
            var workIntnoParameter = workIntno.HasValue ?
                new ObjectParameter("workIntno", workIntno) :
                new ObjectParameter("workIntno", typeof(long));
    
            var respMsgParameter = respMsg != null ?
                new ObjectParameter("respMsg", respMsg) :
                new ObjectParameter("respMsg", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("archiveEndorsedWork", workIntnoParameter, respMsgParameter);
        }
    }
}
