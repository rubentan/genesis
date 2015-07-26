using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoReceivable
    {
        public int receivableId { get; set; }
        public int? clientId { get; set; }
        public string clientName { get; set; }
        public string clientCode { get; set; }
        public string referenceNumber { get; set; }
        public DateTime? paymentDate { get; set; }
        public decimal? cashAmount { get; set; }
        public decimal? chequeAmount { get; set; }
        public string chequeNumber { get; set; }
        public DateTime? chequeDate { get; set; }
        public string chequeBank { get; set; }
        public decimal? totalPayment { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? createdBy { get; set; }
        public DateTime? dateLastModified { get; set; }
        public int? lastModifiedBy { get; set; }
        public decimal? runningBalance { get; set; }
        public decimal? remainingBalance { get; set; }

        public bool isNew { get; set; }
        public bool? isNewPayment { get; set; }
        public bool? isCash { get; set; }
        public decimal? excessPayment { get; set; }

        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public int branchId { get; set; }

        
        
    }
}
