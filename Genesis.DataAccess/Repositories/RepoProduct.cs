using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DTO;

namespace Genesis.DataAccess.Repositories
{
    public class RepoProduct :  RepoBase, IProduct
    {

        #region IProduct Members

        public IEnumerable<dtoProduct> GetAll(string search, object filter = null, int? skip = null, int? take = null)
        {
            string sQuery = string.Format("select top 10 * from tbl_product where productDescription like '%{0}%'", search);
            return DBContext.Database.SqlQuery<dtoProduct>(sQuery).ToList();
        }

        public IEnumerable<dtoProduct> GetAll(string search, int branchId, int? skip = null, int? take = null)
        {
            string sQuery = string.Format(@"select top 15 * 
                                            from tbl_product 
                                            where productDescription like '%{0}%' 
                                            and branchId = {1}", search,branchId);
            return DBContext.Database.SqlQuery<dtoProduct>(sQuery).ToList();
        }

        public IEnumerable<dtoProduct> GetAll(object filter = null, int? skip = null, int? take = null)
        {
            if (skip == null)
                skip = 0;
            if (take == null)
                take = 10;

            string sQuery = string.Format(@"SELECT TOP {0}
	                                                [Row]
	                                                ,[productId]
                                                    ,[productCode]
                                                    ,[productName]
                                                    ,[productDescription]
                                                    ,[categoryId]
                                                    ,[reorderLevel]
                                                    ,[UOM]
                                                    ,[branchId]
                                                    ,[unitPrice]
                                                    ,[beginning]
                                                    ,[incoming]
                                                    ,[outgoing]
                                                    ,[ending]
                                                    ,[modifiedBy]
                                                    ,[dateLastModified]
                                                    ,[createdBy]
                                                    ,[dateCreated]
                                            FROM (  SELECT
                                                    ROW_NUMBER() OVER(ORDER BY [productId]) AS [Row]
                                                      ,[productId]
                                                      ,[productCode]
                                                      ,[productName]
                                                      ,[productDescription]
                                                      ,[categoryId]
                                                      ,[reorderLevel]
                                                      ,[UOM]
                                                      ,[branchId]
                                                      ,[unitPrice]
                                                      ,[beginning]
                                                      ,[incoming]
                                                      ,[outgoing]
                                                      ,[ending]
                                                      ,[modifiedBy]
                                                      ,[dateLastModified]
                                                      ,[createdBy]
                                                      ,[dateCreated]
                                                    FROM [tbl_product]
                                                    WHERE (1 = 1)
                                            ", take);

            if (filter != null)
            {
                var f = (dtoProduct)filter;
                sQuery += string.Format(" AND ('{0}' = '' OR productCode LIKE '%{0}%')", f.productCode);                
                sQuery += string.Format(" AND ('{0}' = '' OR productDescription LIKE '%{0}%')", f.productDescription);
                sQuery += string.Format(" AND ({0} = -1 OR branchId = {0})", f.branchId);
                sQuery += string.Format(" AND ({0} = -1 OR categoryId = {0})", f.categoryId);
            }

            sQuery += string.Format(") x WHERE x.[Row] > {0}", skip);

            return DBContext.Database.SqlQuery<dtoProduct>(sQuery);
        }

        public int GetRecordCount(object filter = null)
        {
            string sQuery = string.Format("SELECT COUNT(productId) FROM tbl_product WHERE (1 = 1)");

            if (filter != null)
            {
                var f = (dtoProduct)filter;
                sQuery += string.Format(" AND ('{0}' = '' OR productCode LIKE '%{0}%')", f.productCode);
                sQuery += string.Format(" AND ('{0}' = '' OR productDescription LIKE '%{0}%')", f.productDescription);
                sQuery += string.Format(" AND ({0} = -1 OR branchId = {0})", f.branchId);
                sQuery += string.Format(" AND ({0} = -1 OR categoryId = {0})", f.categoryId);
            }

            return DBContext.Database.SqlQuery<int>(sQuery).FirstOrDefault();
        }

        public dtoProduct GetByID(int productId)
        {
            string sQuery = string.Format(@"SELECT [productId]
                                                ,[productCode]
                                                ,[productName]
                                                ,[productDescription]
                                                ,[categoryId]
                                                ,[reorderLevel]
                                                ,[UOM]
                                                ,[branchId]
                                                ,[unitPrice]
                                                ,[beginning]
                                                ,[incoming]
                                                ,[outgoing]
                                                ,[ending]
                                                ,[modifiedBy]
                                                ,[dateLastModified]
                                                ,[createdBy]
                                                ,[dateCreated]
                                            FROM tbl_product WHERE productId = '{0}'", productId);
            return DBContext.Database.SqlQuery<dtoProduct>(sQuery).FirstOrDefault();
        }

        public dtoProduct GetByCode(string productCode)
        {
            throw new NotImplementedException();
        }

        public List<dtoProduct> GetBranchProducts(object filter = null, int? skip = null, int? take = null)
        {
            if (skip == null)
                skip = 0;
            if (take == null)
                take = 10;

            string sQuery = string.Format(@"SELECT TOP {0}
	                                                [Row]
	                                                ,[productId]
                                                    ,[productCode]
                                                    ,[productName]
                                                    ,[productDescription]
                                                    ,[categoryId]
                                                    ,[categoryName]
                                                    ,[reorderLevel]
                                                    ,[UOM]
                                                    ,[branchId]
                                                    ,[unitPrice]
                                                    ,[beginning]
                                                    ,[incoming]
                                                    ,[outgoing]
                                                    ,[ending]
                                                    ,[modifiedBy]
                                                    ,[dateLastModified]
                                                    ,[createdBy]
                                                    ,[dateCreated]
                                            FROM (  SELECT
                                                    ROW_NUMBER() OVER(ORDER BY [productId]) AS [Row]
                                                      ,a.[productId]
                                                      ,a.[productCode]
                                                      ,a.[productName]
                                                      ,a.[productDescription]
                                                      ,a.[categoryId]
                                                      ,a.[reorderLevel]
                                                      ,a.[UOM]
                                                      ,a.[branchId]
                                                      ,a.[unitPrice]
                                                      ,a.[beginning]
                                                      ,a.[incoming]
                                                      ,a.[outgoing]
                                                      ,a.[ending]
                                                      ,a.[modifiedBy]
                                                      ,a.[dateLastModified]
                                                      ,a.[createdBy]
                                                      ,a.[dateCreated]
                                                      ,b.[categoryName] + ' - ' + b.[categoryCode] as categoryName
                                                    FROM [tbl_product] a
                                                    LEFT JOIN [tbl_productCategory] b
                                                    on a.categoryId = b.categoryId
                                                    WHERE (1 = 1)
                                            ", take);

            if (filter != null)
            {
                var f = (dtoProduct)filter;
                sQuery += string.Format(" AND ('{0}' = '' OR productCode LIKE '%{0}%')", f.productCode);
                sQuery += string.Format(" AND ('{0}' = '' OR productDescription LIKE '%{0}%')", f.productDescription);
                sQuery += string.Format(" AND ('{0}' = '' OR branchId LIKE '%{0}%')", f.branchId);
                
                //sQuery += string.Format(" AND ({0} = -1 OR categoryId = {0})", f.categoryId);
            }
            //sQuery += string.Format(" AND ({0} = -1 OR branchId = {0})", id);
            sQuery += string.Format(") x WHERE x.[Row] > {0}", skip);

            return DBContext.Database.SqlQuery<dtoProduct>(sQuery).ToList();
        }

        public List<dtoProductTransactions> GetProductTransactions(int id)
        {
            var sqlString = String.Format("select document.*,b.userName as createdByName,trans.quantity " +
                                        "from tbl_document document "+
                                        "left join tbl_transaction trans "+
                                        "on document.documentId = trans.documentId "+
                                        "left join tbl_users b  "+
                                        "on document.createdBy = b.userId " +
                                        "where trans.productId ={0}", id);
            return DBContext.Database.SqlQuery<dtoProductTransactions>(sqlString).ToList();
        }

        public List<dtoProductTransactions> GetProductSales(int id)
        {
            var sqlString = String.Format("select document.*,b.userName as createdByName,trans.quantity " +
                                        "from tbl_document document " +
                                        "left join tbl_transaction trans " +
                                        "on document.documentId = trans.documentId " +
                                        "left join tbl_users b  " +
                                        "on document.createdBy = b.userId " +
                                        "where trans.productId ={0} " +
                                        "and document.documentType = 1", id);
            return DBContext.Database.SqlQuery<dtoProductTransactions>(sqlString).ToList();
        }

        public List<dtoProductTransactions> GetProductPurchases(int id)
        {
            var sqlString = String.Format("select document.*,b.userName as createdByName,trans.quantity " +
                                        "from tbl_document document " +
                                        "left join tbl_transaction trans " +
                                        "on document.documentId = trans.documentId " +
                                        "left join tbl_users b  " +
                                        "on document.createdBy = b.userId " +
                                        "where trans.productId ={0} " +
                                        "and document.documentType = 2", id);

            return DBContext.Database.SqlQuery<dtoProductTransactions>(sqlString).ToList();
        }

        public List<dtoProductPriceHistory> GetProductPriceHistory(int id)
        {
            var sqlString = String.Format("select * from tbl_productPriceHistory where productId ={0}", id);
            return DBContext.Database.SqlQuery<dtoProductPriceHistory>(sqlString).ToList();
        }

        public dtoProduct GetProductInfo(int id)
        {
            var sqlString = String.Format("select product.*,d.categoryCode,d.categoryName,b.userName as createdByName, c.userName as modifiedByName, " +
                                        "(select TOP(1) ab.dateCreated from tbl_transaction ab left join tbl_document bb on ab.documentId = bb.documentId where ab.productId = product.productId and bb.documentType = 2 order by ab.dateCreated desc) as dateLastOrdered, "+
                                        "(select TOP(1) cast((((ab.unitPrice*(1-ab.discountA/100))*(1-ab.discountB/100))*(1-ab.discountC/100)) as decimal(18,2)) from tbl_transaction ab left join tbl_document bb on ab.documentId = bb.documentId where ab.productId = product.productId and bb.documentType = 2 order by ab.dateCreated desc) as lastOrderedPrice, "+
                                        "(select TOP(1) ab.dateCreated from tbl_transaction ab left join tbl_document bb on ab.documentId = bb.documentId where ab.productId = product.productId and bb.documentType = 1 order by ab.dateCreated desc) as dateLastSold, "+
                                        "(select TOP(1) ab.unitPrice from tbl_transaction ab left join tbl_document bb on ab.documentId = bb.documentId where ab.productId = product.productId and bb.documentType = 1 order by ab.dateCreated desc) as lastSoldPrice "+
                                        "from tbl_product product "+
                                        "left join tbl_users b "+
                                        "on product.createdBy = b.userId "+
                                        "left join tbl_users c "+
                                        "on product.modifiedBy = c.userId "+
                                        "left join tbl_productCategory d " +
                                        "on d.categoryId = product.categoryId "+
                                        "where product.productId = {0}", id);
            return DBContext.Database.SqlQuery<dtoProduct>(sqlString).FirstOrDefault();
        }

        public List<string> CheckProductCodeExists(string productCode)
        {
            string sQuery = string.Format("select top 1 productCode from tbl_product where productCode = '{0}'", productCode);

            return DBContext.Database.SqlQuery<string>(sQuery).ToList();
        }

        #endregion

        #region IBase<dtoProduct> Members

        public dtoProduct Get(dtoProduct t)
        {
            throw new NotImplementedException();
        }



        public List<dtoProduct> GetAll(string search, object filter = null)
        {
            throw new NotImplementedException();
        }

        public dtoResult Insert(dtoProduct t)
        {
            dtoResult objResult = new dtoResult();
            List<object> parameterList = new List<object>();
            object[] parameters1 = null;

            try
            {
                string sQuery = string.Format(@"INSERT INTO tbl_product(categoryId
                                                , productCode
                                                , productDescription
                                                , UOM
                                                , unitPrice
                                                , reorderLevel
                                                , branchId
                                                , createdBy
                                                , dateCreated
                                                , beginning
                                                , incoming
                                                , outgoing
                                                , ending) 
                                                VALUES(@P0, @P1, @P2, @P3, @P4, @P5, @P6, @P7, GETDATE(), @P8, @P9, @P10, @P11)");

                parameterList.Add(t.categoryId);
                parameterList.Add(t.productCode);
                parameterList.Add(t.productDescription);
                parameterList.Add(t.UOM);
                parameterList.Add(t.unitPrice);
                parameterList.Add(t.reorderLevel);
                parameterList.Add(t.branchId);
                parameterList.Add(t.createdBy);
                parameterList.Add(t.beginning);
                parameterList.Add(t.incoming);
                parameterList.Add(t.outgoing);
                parameterList.Add(t.beginning + t.incoming - t.outgoing);
                parameters1 = parameterList.ToArray();

                DBContext.Database.ExecuteSqlCommand(sQuery, parameters1);
                objResult.isSuccessful = true;
            }
            catch (Exception ex)
            {
                objResult.isSuccessful = false;
                objResult.errorMsg = ex.Message;
            }

            parameterList = null;
            parameters1 = null;

            return objResult;
        }

        public dtoResult Update(dtoProduct t)
        {
            dtoResult objResult = new dtoResult();
            List<object> parameterList = new List<object>();
            object[] parameters1 = null;

            try
            {
                string sQuery = string.Format(@"UPDATE tbl_product SET categoryId = @P0
                                                , productCode = @P1
                                                , productDescription=@P2
                                                , UOM=@P3
                                                , unitPrice=@P4
                                                , reorderLevel=@P5
                                                , modifiedBy=@P6
                                                , dateLastModified=GETDATE()
                                                , beginning=@P8
                                                , incoming=@P9
                                                , outgoing=@P10
                                                , ending =@P11
                                                WHERE productId=@P7");

                parameterList.Add(t.categoryId);
                parameterList.Add(t.productCode);
                parameterList.Add(t.productDescription);
                parameterList.Add(t.UOM);
                parameterList.Add(t.unitPrice);
                parameterList.Add(t.reorderLevel);
                parameterList.Add(t.modifiedBy);
                parameterList.Add(t.productId);
                parameterList.Add(t.beginning);
                parameterList.Add(t.incoming);
                parameterList.Add(t.outgoing);
                parameterList.Add(t.beginning + t.incoming - t.outgoing);
                parameters1 = parameterList.ToArray();
                

                DBContext.Database.ExecuteSqlCommand(sQuery, parameters1);
                objResult.isSuccessful = true;
            }
            catch (Exception ex)
            {
                objResult.isSuccessful = false;
                objResult.errorMsg = ex.Message;
            }

            parameterList = null;
            parameters1 = null;

            return objResult;
        }

        public dtoResult SoftDelete(dtoProduct t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Delete(dtoProduct t)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
