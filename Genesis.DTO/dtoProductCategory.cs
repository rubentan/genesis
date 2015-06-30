using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoProductCategory
    {
        public int categoryId { get; set; }
        public string categoryCode { get; set; }
        public string categoryName { get; set; }
        public DateTime dateCreated { get; set; }
        public int createdBy { get; set; }
    }
}
