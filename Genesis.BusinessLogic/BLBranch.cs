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
    
    public class BLBranch
    {
        IBranch repo;

        public BLBranch() {
            repo = new RepoBranch();
        }

        public List<dtoBranch> GetAllBranches(string search, object filter = null)
        {
            return repo.GetAll(search, filter);
        }

        public List<dtoBranch> GetAllBranches2(object filter = null, int? skip = null, int? take = null)
        {
            return repo.GetAll2(filter, skip, take);
        }

        public List<string> CheckBranchCodeExists(string branchCode)
        {
            return repo.CheckBranchCodeExists(branchCode);
        }

        public dtoResult Insert(dtoBranch branch)
        {
            return repo.Insert(branch);
        }

        public dtoResult Update(dtoBranch branch)
        {
            return repo.Update(branch);
        }
    }
}
