using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoPaymentDetail
    {
        public int paymentDetailsId { get; set; }
        public int? documentId { get; set; }
        public string documentNo { get; set; }
        public int? paymentId { get; set; }
        public decimal? paymentPrice { get; set; }
        public decimal? totalAmount { get; set; }
        public decimal? totalPaid { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? createdBy { get; set; }
        public DateTime? dateLastModified { get; set; }
        public int? lastModifiedBy { get; set; }
        public string documentNumber { get; set; }
        public decimal totalPayAmount { get; set; }
        public decimal remainingBalance { get; set; }
        public decimal totalPayment { get; set; }
       
    }
}
