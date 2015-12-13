using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoSupplier
    {
        public int supplierId { get; set; }
        public string supplierCode { get; set; }
        public string supplierName { get; set; }
        public string supplierAddress { get; set; }
        public string supplierContactNumber { get; set; }
        public string supplierContactPerson { get; set; }
        public int status { get; set; }
        public int? branchId { get; set; }
        public string branchName { get; set; }
        public DateTime dateCreated { get; set; }
        public int createdBy { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? dateLastModified { get; set; }
        public int records { get; set; }

        //Custom Columns

        public string createdByName { get; set; }
        public string modifiedByName { get; set; }

        public int transactionCount { get; set; }

        public DateTime? lastTransactionDate { get; set; }

        public DateTime? lastPaymentDate { get; set; }
        public decimal remainingBalance { get; set; }

        public decimal totalPayments { get; set; }

        public decimal totalDocumentPayments { get; set; }
    }

    public class dtoSupplierPurchaseOrder
    {
        public int documentId { get; set; }

        public string documentNumber { get; set; }

        public DateTime transactionDate { get; set; }

        public decimal totalPrice { get; set; }

        public decimal totalPayments { get; set; }

    }

    public class dtoSupplierPayment
    {
        public int receivableId { get; set; }

        public string referenceNumber { get; set; }

        public decimal? cashAmount { get; set; }

        public DateTime dateCreated { get; set; }
    }
}
