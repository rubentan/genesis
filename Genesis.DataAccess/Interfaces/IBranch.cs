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
        List<dtoBranch> GetAll2(int page, int recordPerPage, object filter, bool isExport);

        List<string> CheckBranchCodeExists(string branchCode);
    }
}
