using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DTO;

namespace Genesis.DataAccess.Repositories
{
    public class RepoBranch : RepoBase, IBranch
    {

        public dtoBranch Get(dtoBranch t)
        {
            throw new NotImplementedException();
        }

        public List<dtoBranch> GetAll(string search, object filter = null)
        {
            return DBContext.Database.SqlQuery<dtoBranch>("select * from tbl_branch").ToList();
        }

        public List<dtoBranch> GetAll2(object filter = null, int? skip = null, int? take = null)
        {
            string sQuery = string.Format(@"SELECT TOP {0} * FROM tbl_branch 
                                            WHERE (1 = 1) ", take);

            if (filter != null)
            {
                var f = (dtoBranch)filter;
                sQuery += string.Format("and ('{0}' = '' or branchName like '%{0}%'  )", f.branchName);
                sQuery += string.Format("and ('{0}' = '' or branchCode like '%{0}%' )", f.branchCode);

            }

            return DBContext.Database.SqlQuery<dtoBranch>(sQuery).ToList();
        }


        public dtoResult Insert(dtoBranch t)
        {
            var result = new dtoResult();
            try
            {
                var obj = new tbl_branch
                {
                    branchCode = t.branchCode,
                    branchName = t.branchName,
                    branchAddress = t.branchAddress,
                    dateCreated = t.dateCreated,
                    createdBy = t.createdBy
                };

                DBContext.tbl_branch.Add(obj);

                DBContext.SaveChanges();

                t.branchName = obj.branchName;
                result.isSuccessful = true;
                result.returnObj = t;
            }
            catch (Exception ex)
            {
                result.isSuccessful = false;
                result.errorMsg = ex.ToString();
            }

            return result;
        }

        public dtoResult Update(dtoBranch t)
        {
            var result = new dtoResult();
            try
            {

                var branch = DBContext.tbl_branch.FirstOrDefault(d => d.branchId == t.branchId);

                if (branch != null)
                {
                    branch.branchCode = t.branchCode;
                    branch.branchName = t.branchName;
                    branch.branchAddress = t.branchAddress;
                    branch.dateLastModified = t.dateLastModified;
                    branch.lastModifiedBy = t.lastModifiedBy;
                }

                DBContext.SaveChanges();

                //t.userName = obj.userName;
                result.isSuccessful = true;
                result.returnObj = t;
            }
            catch (Exception ex)
            {
                result.isSuccessful = false;
                result.errorMsg = ex.ToString();
            }

            return result;
        }

        public dtoResult SoftDelete(dtoBranch t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Delete(dtoBranch t)
        {
            throw new NotImplementedException();
        }

        public List<string> CheckBranchCodeExists(string branchCode)
        {
            string sQuery = string.Format("select top 1 branchCode from tbl_branch where branchCode = '{0}'", branchCode);

            return DBContext.Database.SqlQuery<string>(sQuery).ToList();
        }
    }
}
