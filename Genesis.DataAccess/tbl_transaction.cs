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
    
    public partial class tbl_transaction
    {
        public int transactionId { get; set; }
        public int productId { get; set; }
        public int documentId { get; set; }
        public int transactionType { get; set; }
        public int quantity { get; set; }
        public Nullable<decimal> unitPrice { get; set; }
        public System.DateTime dateCreated { get; set; }
        public Nullable<double> discountA { get; set; }
        public Nullable<double> discountB { get; set; }
        public Nullable<double> discountC { get; set; }
    }
}
