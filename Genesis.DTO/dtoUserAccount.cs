using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DTO
{
    public class dtoUserAccount
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string passWord { get; set; }
        public string passWord2 { get; set; }
        public string emailAddress { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public int branchId { get; set; }
        public int status { get; set; }
        public Nullable<System.DateTime> dateLastLogin { get; set; }
        public System.DateTime dateCreated { get; set; }
        public Nullable<System.DateTime> dateLastPasswordChange { get; set; }

        //Custom Fields
        public string branchName { get; set; }
    }
}
