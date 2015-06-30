using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoProduct
    {
        public int productId { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public int? categoryId { get; set; }
        public int? reorderLevel { get; set; }
        public string UOM { get; set; }
        public int? branchId { get; set; }
        public decimal? unitPrice { get; set; }
        public int? beginning { get; set; }
        public int? incoming { get; set; }
        public int? outgoing { get; set; }
        public int? ending { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? dateLastModified { get; set; }
        public int? createdBy { get; set; }
        public DateTime? dateCreated { get; set; }

        //Custom Fields
        public string categoryCode { get; set; }

        public string categoryName { get; set; }

        public string createdByName { get; set; }
        public string modifiedByName { get; set; }
        public DateTime? dateLastOrdered { get; set; }

        public DateTime? dateLastSold { get; set; }

        public decimal? lastOrderedPrice { get; set; }

        public decimal? lastSoldPrice { get; set; }
    }

    public class dtoProductPriceHistory
    {
        public decimal Price { get; set; }

        public DateTime dateCreated { get; set; }
    }

    public class dtoProductTransactions
    {
        public int? documentId { get; set; }

        public string documentNumber { get; set; }

        public string supplier { get; set; }

        public string client { get; set; }

        public int documentType { get; set; }

        public int? quantity { get; set; }

        public decimal? unitPrice { get; set; }

        public DateTime? date { get; set; }

        public DateTime? dateCreated { get; set; }
    }
}
