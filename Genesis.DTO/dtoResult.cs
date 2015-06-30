using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoResult
    {
        public bool isSuccessful { get; set; }
        public object returnObj { get; set; }        
        public string errorMsg { get; set; }

    }
}
