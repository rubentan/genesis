using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoReceivableOrder
    {
        public int documentId { get; set; }
        public string documentNumber { get; set; }
        public string clientName { get; set; }
        public decimal totalAmount { get; set; }
        public decimal remainingBalance { get; set; }
        public decimal totalPayment { get; set; }
    
    }
}
