using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoPaymentDetail
    {
        public Int32? paymentDetailsId { get; set; }
        public Int32? documentId { get; set; }
        public String documentNo { get; set; }
        public Int32? paymentId { get; set; }
        public Decimal? paymentPrice { get; set; }
        public Decimal? totalAmount { get; set; }
        public Decimal? totalPaid { get; set; }
       
    }
}
