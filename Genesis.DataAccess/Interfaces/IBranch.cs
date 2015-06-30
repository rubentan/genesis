using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;


namespace Genesis.DataAccess.Interfaces
{
    public interface IBranch : IBase<dtoBranch>
    {
        List<dtoBranch> GetAll2(object filter = null, int? skip = null, int? take = null);

        List<string> CheckBranchCodeExists(string branchCode);
    }
}
