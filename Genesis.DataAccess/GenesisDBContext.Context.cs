﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Genesis.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class GenesisEntities : DbContext
    {
        public GenesisEntities()
            : base("name=GenesisEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<tbl_branch> tbl_branch { get; set; }
        public DbSet<tbl_client> tbl_client { get; set; }
        public DbSet<tbl_document> tbl_document { get; set; }
        public DbSet<tbl_documentType> tbl_documentType { get; set; }
        public DbSet<tbl_payment> tbl_payment { get; set; }
        public DbSet<tbl_paymentDetails> tbl_paymentDetails { get; set; }
        public DbSet<tbl_product> tbl_product { get; set; }
        public DbSet<tbl_productCategory> tbl_productCategory { get; set; }
        public DbSet<tbl_productPriceHistory> tbl_productPriceHistory { get; set; }
        public DbSet<tbl_receivable> tbl_receivable { get; set; }
        public DbSet<tbl_receivableDetails> tbl_receivableDetails { get; set; }
        public DbSet<tbl_supplier> tbl_supplier { get; set; }
        public DbSet<tbl_transaction> tbl_transaction { get; set; }
        public DbSet<tbl_transactionType> tbl_transactionType { get; set; }
        public DbSet<tbl_users> tbl_users { get; set; }
    
        public virtual int GetAllDocument(Nullable<int> skip, Nullable<int> take, string documentNumber, string documentType, string supplierCode, string supplierName, string dateFrom, string dateTo)
        {
            var skipParameter = skip.HasValue ?
                new ObjectParameter("skip", skip) :
                new ObjectParameter("skip", typeof(int));
    
            var takeParameter = take.HasValue ?
                new ObjectParameter("take", take) :
                new ObjectParameter("take", typeof(int));
    
            var documentNumberParameter = documentNumber != null ?
                new ObjectParameter("documentNumber", documentNumber) :
                new ObjectParameter("documentNumber", typeof(string));
    
            var documentTypeParameter = documentType != null ?
                new ObjectParameter("documentType", documentType) :
                new ObjectParameter("documentType", typeof(string));
    
            var supplierCodeParameter = supplierCode != null ?
                new ObjectParameter("supplierCode", supplierCode) :
                new ObjectParameter("supplierCode", typeof(string));
    
            var supplierNameParameter = supplierName != null ?
                new ObjectParameter("supplierName", supplierName) :
                new ObjectParameter("supplierName", typeof(string));
    
            var dateFromParameter = dateFrom != null ?
                new ObjectParameter("dateFrom", dateFrom) :
                new ObjectParameter("dateFrom", typeof(string));
    
            var dateToParameter = dateTo != null ?
                new ObjectParameter("dateTo", dateTo) :
                new ObjectParameter("dateTo", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetAllDocument", skipParameter, takeParameter, documentNumberParameter, documentTypeParameter, supplierCodeParameter, supplierNameParameter, dateFromParameter, dateToParameter);
        }
    
        public virtual int GetAllDocumentByType(Nullable<int> skip, Nullable<int> take, string documentNumber, string supplierCode, string supplierName, string dateFrom, string dateTo, Nullable<int> documentType)
        {
            var skipParameter = skip.HasValue ?
                new ObjectParameter("skip", skip) :
                new ObjectParameter("skip", typeof(int));
    
            var takeParameter = take.HasValue ?
                new ObjectParameter("take", take) :
                new ObjectParameter("take", typeof(int));
    
            var documentNumberParameter = documentNumber != null ?
                new ObjectParameter("documentNumber", documentNumber) :
                new ObjectParameter("documentNumber", typeof(string));
    
            var supplierCodeParameter = supplierCode != null ?
                new ObjectParameter("supplierCode", supplierCode) :
                new ObjectParameter("supplierCode", typeof(string));
    
            var supplierNameParameter = supplierName != null ?
                new ObjectParameter("supplierName", supplierName) :
                new ObjectParameter("supplierName", typeof(string));
    
            var dateFromParameter = dateFrom != null ?
                new ObjectParameter("dateFrom", dateFrom) :
                new ObjectParameter("dateFrom", typeof(string));
    
            var dateToParameter = dateTo != null ?
                new ObjectParameter("dateTo", dateTo) :
                new ObjectParameter("dateTo", typeof(string));
    
            var documentTypeParameter = documentType.HasValue ?
                new ObjectParameter("documentType", documentType) :
                new ObjectParameter("documentType", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetAllDocumentByType", skipParameter, takeParameter, documentNumberParameter, supplierCodeParameter, supplierNameParameter, dateFromParameter, dateToParameter, documentTypeParameter);
        }
    
        public virtual ObjectResult<GetAllOrderItems_Result> GetAllOrderItems(Nullable<int> decumentId)
        {
            var decumentIdParameter = decumentId.HasValue ?
                new ObjectParameter("decumentId", decumentId) :
                new ObjectParameter("decumentId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetAllOrderItems_Result>("GetAllOrderItems", decumentIdParameter);
        }
    
        public virtual int GetAllSuppliers(Nullable<int> skip, Nullable<int> take, string documentNumber, string supplierCode, string supplierName, string dateFrom, string dateTo)
        {
            var skipParameter = skip.HasValue ?
                new ObjectParameter("skip", skip) :
                new ObjectParameter("skip", typeof(int));
    
            var takeParameter = take.HasValue ?
                new ObjectParameter("take", take) :
                new ObjectParameter("take", typeof(int));
    
            var documentNumberParameter = documentNumber != null ?
                new ObjectParameter("documentNumber", documentNumber) :
                new ObjectParameter("documentNumber", typeof(string));
    
            var supplierCodeParameter = supplierCode != null ?
                new ObjectParameter("supplierCode", supplierCode) :
                new ObjectParameter("supplierCode", typeof(string));
    
            var supplierNameParameter = supplierName != null ?
                new ObjectParameter("supplierName", supplierName) :
                new ObjectParameter("supplierName", typeof(string));
    
            var dateFromParameter = dateFrom != null ?
                new ObjectParameter("dateFrom", dateFrom) :
                new ObjectParameter("dateFrom", typeof(string));
    
            var dateToParameter = dateTo != null ?
                new ObjectParameter("dateTo", dateTo) :
                new ObjectParameter("dateTo", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetAllSuppliers", skipParameter, takeParameter, documentNumberParameter, supplierCodeParameter, supplierNameParameter, dateFromParameter, dateToParameter);
        }
    
        public virtual int insert_client(string clientCode, string clientName, string clientAddress, string clientContactNumber, string clientContactPerson, Nullable<int> status, Nullable<int> createdBy)
        {
            var clientCodeParameter = clientCode != null ?
                new ObjectParameter("clientCode", clientCode) :
                new ObjectParameter("clientCode", typeof(string));
    
            var clientNameParameter = clientName != null ?
                new ObjectParameter("clientName", clientName) :
                new ObjectParameter("clientName", typeof(string));
    
            var clientAddressParameter = clientAddress != null ?
                new ObjectParameter("clientAddress", clientAddress) :
                new ObjectParameter("clientAddress", typeof(string));
    
            var clientContactNumberParameter = clientContactNumber != null ?
                new ObjectParameter("clientContactNumber", clientContactNumber) :
                new ObjectParameter("clientContactNumber", typeof(string));
    
            var clientContactPersonParameter = clientContactPerson != null ?
                new ObjectParameter("clientContactPerson", clientContactPerson) :
                new ObjectParameter("clientContactPerson", typeof(string));
    
            var statusParameter = status.HasValue ?
                new ObjectParameter("status", status) :
                new ObjectParameter("status", typeof(int));
    
            var createdByParameter = createdBy.HasValue ?
                new ObjectParameter("createdBy", createdBy) :
                new ObjectParameter("createdBy", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("insert_client", clientCodeParameter, clientNameParameter, clientAddressParameter, clientContactNumberParameter, clientContactPersonParameter, statusParameter, createdByParameter);
        }
    }
}
