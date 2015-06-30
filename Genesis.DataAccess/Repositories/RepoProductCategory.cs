using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DTO;

namespace Genesis.DataAccess.Repositories
{
    public class RepoProductCategory : RepoBase, IProductCategory
    {
        #region IBase<dtoProductCategory> Members

        public DTO.dtoProductCategory Get(DTO.dtoProductCategory t)
        {
            throw new NotImplementedException();
        }

        public List<DTO.dtoProductCategory> GetAll(string search, object filter = null)
        {
            string sQuery = string.Format(@"SELECT [categoryId]
                                          ,[categoryCode]
                                          ,[categoryName]
                                          ,[dateCreated]
                                          ,[createdBy]
                                      FROM [Genesis].[dbo].[tbl_productCategory] WHERE categoryName LIKE '%{0}%' OR categoryCode LIKE '%{0}%'", search);
            return DBContext.Database.SqlQuery<dtoProductCategory>(sQuery).ToList();
        }

        public DTO.dtoResult Insert(DTO.dtoProductCategory t)
        {
            throw new NotImplementedException();
        }

        public DTO.dtoResult Update(DTO.dtoProductCategory t)
        {
            throw new NotImplementedException();
        }

        public DTO.dtoResult SoftDelete(DTO.dtoProductCategory t)
        {
            throw new NotImplementedException();
        }

        public DTO.dtoResult Delete(DTO.dtoProductCategory t)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
