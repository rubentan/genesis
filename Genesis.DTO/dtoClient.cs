using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoClient
    {
        public int clientId { get; set; }
        public string clientCode { get; set; }
        public string clientName { get; set; }
        public string clientAddress { get; set; }
        public string clientContactNumber { get; set; }
        public string clientContactPerson { get; set; }
        public int status { get; set; }
        public int? branchId { get; set; }
        public int createdBy { get; set; }
        public DateTime dateCreated { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? dateLastModified { get; set; }

        //Custom Columns

        public string createdByName { get; set; }
        public string modifiedByName { get; set; }

        public int transactionCount { get; set; }

        public DateTime? lastTransactionDate { get; set; }

        public DateTime? lastPaymentDate { get; set; }
        public decimal remainingBalance { get; set; }

        public decimal totalPayments { get; set; }

        public decimal totalDocumentPayments { get; set; }

        //Client Sales Invoice

        //public List<dtoClientSalesInvoice> SalesInvoices { get; set; }
    }

    public class dtoClientSalesInvoice
    {
        public int documentId { get; set; }

        public string documentNumber { get; set; }

        public DateTime transactionDate { get; set; }

        public decimal totalPrice { get; set; }

        public decimal totalPayments { get; set; }

    }

    public class dtoClientPayment
    {
        public int receivableId { get; set; }

        public string referenceNumber { get; set; }

        public decimal cashAmount { get; set; }

        public DateTime dateCreated { get; set; }
    }
}