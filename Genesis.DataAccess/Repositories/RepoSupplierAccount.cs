using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DTO;

namespace Genesis.DataAccess.Repositories
{
    public class RepoSupplierAccount : RepoBase, ISupplierAccount
    {
        #region IBase<dtoSupplier> Members

        public dtoSupplier Get(dtoSupplier t)
        {
            string sQuery = string.Format(@"SELECT [supplierId]
                                                  ,[supplierCode]
                                                  ,[supplierName]
                                                  ,[supplierAddress]
                                                  ,[supplierContactNumber]
                                                  ,[supplierContactPerson]
                                                  ,[branchId]
                                                  ,[status]
                                                  ,[dateCreated]
                                                  ,[createdBy] 
                                            FROM tbl_supplier 
                                            WHERE (1 = 1)");

            sQuery += string.Format(" AND ('{0}' = '' OR supplierCode = '{0}'  )", t.supplierCode);
            sQuery += string.Format(" AND ('{0}' = '' OR supplierName = '{0}'  )", t.supplierName);
            sQuery += string.Format(" AND ('{0}' = '' OR supplierContactNumber = '{0}'  )", t.supplierContactNumber);
            sQuery += string.Format(" AND ('{0}' = '' OR supplierContactPerson = '{0}'  )", t.supplierContactPerson);
            sQuery += string.Format(" AND ({0} = {0} OR [status] = {0})", t.status);

            return DBContext.Database.SqlQuery<dtoSupplier>(sQuery).FirstOrDefault();
        }

        public List<dtoSupplier> GetAll(string search, object filter = null)
        {
            string sQuery = string.Format(@"SELECT [supplierId]
                                                  ,[supplierCode]
                                                  ,[supplierName]
                                                  ,[supplierAddress]
                                                  ,[supplierContactNumber]
                                                  ,[supplierContactPerson]
                                                  ,[branchId]
                                                  ,[status]
                                                  ,[dateCreated]
                                                  ,[createdBy] 
                                            FROM tbl_supplier  WHERE supplierCode LIKE '%{0}%' OR supplierName LIKE '%{0}%'", search);
            return DBContext.Database.SqlQuery<dtoSupplier>(sQuery).ToList();
        }

        public dtoResult Insert(dtoSupplier t)
        {
            dtoResult objResult = new dtoResult();
            List<object> parameterList = new List<object>();
            object[] parameters1 = null;

            try
            {
                string sQuery = string.Format(@"INSERT INTO tbl_supplier(supplierCode,supplierName,supplierAddress,supplierContactNumber,supplierContactPerson,[status],createdBy,dateCreated,branchId) 
                                                VALUES(@P0,@P1,@P2,@P3,@P4,@P5,@P6,GETDATE(),@P7)");

                parameterList.Add(t.supplierCode);
                parameterList.Add(t.supplierName);
                parameterList.Add(t.supplierAddress);
                parameterList.Add(t.supplierContactNumber);
                parameterList.Add(t.supplierContactPerson);
                parameterList.Add(t.status);
                parameterList.Add(t.createdBy);
                parameterList.Add(t.branchId);
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

        public dtoResult Update(dtoSupplier t)
        {
            dtoResult objResult = new dtoResult();
            List<object> parameterList = new List<object>();
            object[] parameters1 = null;

            try
            {
                string sQuery = string.Format(@"UPDATE tbl_supplier SET supplierCode = @P0,supplierName = @P1,supplierAddress=@P2,supplierContactNumber=@P3,supplierContactPerson=@P4,[status]=@P5,modifiedBy=@P6,dateLastModified=GETDATE() 
                                                WHERE supplierId=@P7");

                parameterList.Add(t.supplierCode);
                parameterList.Add(t.supplierName);
                parameterList.Add(t.supplierAddress);
                parameterList.Add(t.supplierContactNumber);
                parameterList.Add(t.supplierContactPerson);
                parameterList.Add(t.status);
                parameterList.Add(t.modifiedBy);
                parameterList.Add(t.supplierId);
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

        public dtoResult SoftDelete(dtoSupplier t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Delete(dtoSupplier t)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISupplier Members

        public List<dtoSupplier> GetAll(string search, object filter = null, int? skip = null, int? take = null)
        {
            string sQuery = string.Format("select * from tbl_supplier where (1 = 1)");

            if (filter != null)
            {
                var f = (dtoSupplier)filter;
                sQuery += string.Format("and ('{0}' = '' or supplierCode like '%{0}%'  )", f.supplierCode);
                sQuery += string.Format("and ('{0}' = '' or supplierName like '%{0}%'  )", f.supplierName);
                sQuery += string.Format("and ('{0}' = '' or supplierContactNumber like '%{0}%'  )", f.supplierContactNumber);
                sQuery += string.Format("and ('{0}' = '' or supplierContactPerson like '%{0}%'  )", f.supplierContactPerson);
                sQuery += string.Format(" AND ({0} = -1 OR [status] = {0})", f.status);
            }

            return DBContext.Database.SqlQuery<dtoSupplier>(sQuery).ToList();
        }

        public List<dtoSupplier> GetAll(object filter = null, int? skip = null, int? take = null)
        {
            if (skip == null)
                skip = 0;
            if (take == null)
                take = 10;

            string sQuery = string.Format(@"SELECT TOP {0}
	                                              [Row]
	                                              ,[supplierId]
                                                  ,[supplierCode]
                                                  ,[supplierName]
                                                  ,[supplierAddress]
                                                  ,[supplierContactNumber]
                                                  ,[supplierContactPerson]
                                                  ,[branchId]
                                                  ,[status]
                                                  ,[dateCreated]
                                                  ,[createdBy]
                                            FROM (  SELECT
                                                    ROW_NUMBER() OVER(ORDER BY [supplierId]) AS [Row]
                                                      ,[supplierId]
                                                      ,[supplierCode]
                                                      ,[supplierName]
                                                      ,[supplierAddress]
                                                      ,[supplierContactNumber]
                                                      ,[supplierContactPerson]
                                                      ,[branchId]
                                                      ,[status]
                                                      ,[dateCreated]
                                                      ,[createdBy]
                                                    FROM [tbl_supplier]
                                                    WHERE (1 = 1)
                                            ", take);

            if (filter != null)
            {
                var f = (dtoSupplier)filter;
                sQuery += string.Format(" AND ('{0}' = '' OR supplierCode LIKE '%{0}%')", f.supplierCode);
                sQuery += string.Format(" AND ('{0}' = '' OR supplierName LIKE '%{0}%')", f.supplierName);
                sQuery += string.Format(" AND ('{0}' = '' OR supplierContactNumber LIKE '%{0}%')", f.supplierContactNumber);
                sQuery += string.Format(" AND ('{0}' = '' OR supplierContactPerson LIKE '%{0}%')", f.supplierContactPerson);
                sQuery += string.Format(" AND ('{0}' = '' OR branchId LIKE '%{0}%')", f.branchId);
                //sQuery += string.Format(" AND ({0} = 0 OR [status] = {0})", f.status);
            }

            sQuery += string.Format(") x WHERE x.[Row] > {0}", skip);

            return DBContext.Database.SqlQuery<dtoSupplier>(sQuery).ToList();
        }

        public int GetRecordCount(object filter = null)
        {
            string sQuery = string.Format("SELECT COUNT(supplierID) FROM tbl_supplier WHERE (1 = 1)");

            if (filter != null)
            {
                var f = (dtoSupplier)filter;
                sQuery += string.Format("AND ('{0}' = '' OR supplierCode LIKE '%{0}%'  )", f.supplierCode);
                sQuery += string.Format("AND ('{0}' = '' OR supplierName LIKE '%{0}%'  )", f.supplierName);
                sQuery += string.Format("AND ('{0}' = '' OR supplierContactNumber LIKE '%{0}%'  )", f.supplierContactNumber);
                sQuery += string.Format("AND ('{0}' = '' OR supplierContactPerson LIKE '%{0}%'  )", f.supplierContactPerson);
                //sQuery += string.Format(" AND ({0} = -1 OR [status] = {0})", f.status);
            }

            return DBContext.Database.SqlQuery<int>(sQuery).FirstOrDefault();
        }

        public dtoSupplier GetByID(int supplierId)
        {
            string sQuery = string.Format(@"SELECT [supplierId]
                                                  ,[supplierCode]
                                                  ,[supplierName]
                                                  ,[supplierAddress]
                                                  ,[supplierContactNumber]
                                                  ,[supplierContactPerson]
                                                  ,[branchId]
                                                  ,[status]
                                                  ,[dateCreated]
                                                  ,[createdBy] 
                                            FROM tbl_supplier WHERE supplierId = '{0}'", supplierId);
            return DBContext.Database.SqlQuery<dtoSupplier>(sQuery).FirstOrDefault();
        }

        public dtoSupplier GetByCode(string supplierCode)
        {
            string sQuery = string.Format(@"SELECT [supplierId] FROM tbl_supplier WHERE supplierCode = '{0}'", supplierCode);
            return DBContext.Database.SqlQuery<dtoSupplier>(sQuery).FirstOrDefault();
        }

        public dtoSupplier GetByName(string supplierName)
        {
            string sQuery = string.Format(@"SELECT [supplierId] FROM tbl_supplier WHERE supplierName = '{0}'", supplierName);
            return DBContext.Database.SqlQuery<dtoSupplier>(sQuery).FirstOrDefault();
        }

        public dtoSupplier GetSupplierInfo(string id)
        {
            var sqlString = 
                String.Format("select supplier.*,b.userName as createdByName, c.userName as modifiedByName, "+
                            "(select count(*) from tbl_document where referenceId = supplier.supplierId and documentType=2) as transactionCount, "+
                            "(select top(1) transactionDate from tbl_document where referenceId = supplier.supplierId order by transactionDate desc) as lastTransactionDate, "+
                            "(select top(1) paymentDate from tbl_payment where supplierId = supplier.supplierId order by paymentDate desc) as lastPaymentDate, "+
                            "(select ISNULL(sum(a.quantity*a.unitPrice),0) as totalPurchased from tbl_transaction a left join tbl_document b on a.documentId = b.documentId where b.referenceId = supplier.supplierId and b.documentType = 2) - "+
                            "(select ISNULL(sum(paymentPrice),0) as totalPaid from tbl_paymentDetails a left join tbl_payment b on a.paymentId = b.paymentId where b.supplierId = supplier.supplierId) as remainingBalance, "+
                            "(select ISNULL(sum(cashAmount),0) from tbl_payment where supplierId = supplier.supplierId) as totalPayments, "+
                            "(select ISNULL(sum(paymentPrice),0) as totalDocPayment from tbl_paymentDetails a left join tbl_document b on a.documentId = b.documentId where b.referenceId = supplier.supplierId and documentType = 2) as totalDocumentPayments "+
                            " from tbl_supplier supplier "+
                            "left join tbl_users b  "+
                            "on supplier.createdBy = b.userId "+
                            "left join tbl_users c "+
                            "on supplier.modifiedBy = c.userId " +
                            "where supplier.supplierId = {0}",id);

            return DBContext.Database.SqlQuery<dtoSupplier>(sqlString).FirstOrDefault();
        }

        public List<dtoSupplierPurchaseOrder> GetSupplierPurchaseOrders(string id)
        {
            var sqlString =
                String.Format("select document.documentId,document.documentNumber,document.transactionDate, " +
                              "ISNULL(sum(trans.quantity*trans.unitPrice),0) as totalPrice, " +
                              "(select ISNULL(sum(paymentPrice),0) from tbl_paymentDetails n where n.documentId = document.documentId ) as totalPayments " +
                              "from tbl_document document " +
                              "left join tbl_transaction trans " +
                              "on document.documentId = trans.documentId " +
                              "where document.referenceId = {0} and document.documentType = 2 " +
                              "group by document.documentId,document.documentNumber,document.transactionDate", id);
            return DBContext.Database.SqlQuery<dtoSupplierPurchaseOrder>(sqlString).ToList();
        }

        public List<dtoSupplierPayment> GetSupplierPayments(string id)
        {
            var sqlString = String.Format("select * from tbl_payment where supplierId={0}", id);

            return DBContext.Database.SqlQuery<dtoSupplierPayment>(sqlString).ToList();
        }

        public List<string> CheckSupplierCodeExists(string supplierCode)
        {
            string sQuery = string.Format("select top 1 supplierCode from tbl_supplier where supplierCode = '{0}'", supplierCode);

            return DBContext.Database.SqlQuery<string>(sQuery).ToList();
        }

        public List<KeyValue> GetSuppliersFroDropDown()
        {
            string sQuery = "select id=supplierId, text= supplierName  from tbl_supplier";
            return DBContext.Database.SqlQuery<KeyValue>(sQuery).ToList();
        }

        #endregion
    }
}
