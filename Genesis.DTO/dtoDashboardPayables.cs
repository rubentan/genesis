using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoDashboardPayables
    {
        public string supplierCode { get; set; }
        public string supplierName { get; set; }
        public decimal due { get; set; }
        public int documentId { get; set; }
        public int supplierId { get; set; }
        public DateTime dateCreated { get; set; }
    }
}
