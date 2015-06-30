using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoTransaction
    {
        public int transactionId { get; set; }
        public int inventoryId { get; set; }
        public int documentId { get; set; }
        public int transactionType { get; set; }
        public int quantity { get; set; }
        public decimal? unitPrice { get; set; }
        public DateTime dateCreated { get; set; }
        public double? discountA { get; set; }
        public double? discountB { get; set; }
        public double? discountC { get; set; }
        public decimal? originalPrice { get; set; }
        public double? discountPrice { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public double total { get; set; }
    }
}
