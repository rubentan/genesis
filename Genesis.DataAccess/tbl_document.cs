//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class tbl_document
    {
        public int documentId { get; set; }
        public int branchId { get; set; }
        public Nullable<int> referenceId { get; set; }
        public string documentNumber { get; set; }
        public int documentType { get; set; }
        public System.DateTime dateCreated { get; set; }
        public int createdBy { get; set; }
        public Nullable<System.DateTime> transactionDate { get; set; }
        public Nullable<int> payment { get; set; }
    }
}
