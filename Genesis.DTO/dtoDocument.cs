using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoDocument
    {
        public int documentId { get; set; }
        public int branchId { get; set; }
        public string documentNumber { get; set; }
        public int documentType { get; set; }
        public DateTime? transactionDate { get; set; }
        public DateTime dateCreated { get; set; }
        public string sDateCreated { get; set; }
        public int createdBy { get; set; }
        public int? referenceId { get; set; }
        public string supplierCode { get; set; }
        public string supplierName { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public decimal? purchasePrice { get; set; }
        public decimal? salesPrice { get; set; }
        public string productDescription { get; set; }
        public string clientName { get; set; }
        public string clientCode { get; set; }
        public decimal? totalPaid { get; set; }
        public int records { get; set; }
    }
}
