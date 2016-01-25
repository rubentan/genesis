using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoDashboardReorder
    {
        public int productId { get; set; }
        public string productCode { get; set; }
        public string UOM { get; set; }
        public int reorderLevel{ get; set; }
        public string productDescription { get; set; }
        public int ending { get; set; }

    }
}
