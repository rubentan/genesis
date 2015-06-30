using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;

namespace Genesis.DataAccess.Interfaces
{
    public interface IDashboardPayables : IBase<dtoDashboardPayables>
    {
        List<dtoDashboardPayables> GetAllPayables(int branchId, int? skip = null, int? take = null);
    }

    public interface IDashboardReceivables : IBase<dtoDashboardReceivables>
    {
        List<dtoDashboardReceivables> GetAllReceivables(int branchId, int? skip = null, int? take = null);

    }

    public interface IDashboardReorder : IBase<dtoDashboardReorder>
    {
        List<dtoDashboardReorder> GetAllReorders(int branchId, int? skip = null, int? take = null);
    }
}
