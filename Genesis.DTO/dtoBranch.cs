using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoBranch
    {
        public int branchId { get; set; }
        public string branchCode { get; set; }
        public string branchName { get; set; }
        public string branchAddress { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? createdBy { get; set; }
        public DateTime? dateLastModified { get; set; }
        public int? lastModifiedBy { get; set; }
    }
}
