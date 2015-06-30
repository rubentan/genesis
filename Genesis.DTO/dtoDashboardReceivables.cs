using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoDashboardReceivables
    {
        public string clientCode { get; set; }
        public string clientName { get; set; }
        public decimal due { get; set; }
        public int documentId { get; set; }
        public DateTime dateCreated { get; set; }
    }
}
