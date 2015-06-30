using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DataAccess.Repositories;
using Genesis.DTO;

namespace Genesis.BusinessLogic
{
    public class Dashboard
    {
        private IDashboardReceivables repoReceivables;
        private IDashboardPayables repoPayables;
        private IDashboardReorder repoReorder;
        public Dashboard()
        {
            repoReceivables = new RepoDashboard();
            repoPayables = new RepoDashboard();
            repoReorder = new RepoDashboard();
        }
        public List<dtoDashboardReceivables> GetAllReceivables(int branchId, int? skip = null, int? take = null)
        {
            return repoReceivables.GetAllReceivables(branchId, skip, take);
        }

        public List<dtoDashboardPayables> GetAllPayables(int branchId, int? skip = null, int? take = null)
        {
            return repoPayables.GetAllPayables(branchId, skip, take);
        }

        public List<dtoDashboardReorder> GetAllReorders(int branchId, int? skip = null, int? take = null)
        {
            return repoReorder.GetAllReorders(branchId, skip, take);
        }

    }
}
