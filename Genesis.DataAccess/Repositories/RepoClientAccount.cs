using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DTO;

namespace Genesis.DataAccess.Repositories
{
    public class RepoClientAccount : RepoBase, IClientAccount
    {
        public dtoClient Get(dtoClient t)
        {
            throw new NotImplementedException();
        }

        public List<dtoClient> GetAll(string search, object filter = null)
        {
            string sQuery = string.Format("select * from tbl_client where ('{0}' = '' or clientCode like '%{0}%' or clientName like '%{0}%' or clientContactPerson like '%{0}%') ", search);

            if (filter != null)
            {
                var f = (dtoClient)filter;
                sQuery += string.Format("and ('{0}' = '' or clientCode like '%{0}%'  )", f.clientCode);
                sQuery += string.Format("and ('{0}' = '' or clientName like '%{0}%'  )", f.clientName);
                sQuery += string.Format("and ('{0}' = '' or clientContactNumber like '%{0}%'  )", f.clientContactNumber);
                sQuery += string.Format("and ('{0}' = '' or clientContactPerson like '%{0}%'  )", f.clientContactPerson);
                sQuery += string.Format("and ('{0}' = '' or branchId like '%{0}%'  )", f.branchId);
                sQuery += string.Format("and ({0} = {0} or status = {0}  )", f.status);
            }

            return DBContext.Database.SqlQuery<dtoClient>(sQuery).ToList();
        }        

        public dtoResult Insert(dtoClient t)
        {
            var result = new dtoResult();
            result.isSuccessful = true;
            try
            {
                
                var entiry = new tbl_client { 
                    clientCode = t.clientCode,
                    clientName = t.clientName,
                    clientAddress = t.clientAddress,
                    clientContactNumber = t.clientContactNumber,
                    clientContactPerson = t.clientContactPerson,
                    createdBy = t.createdBy,
                    dateCreated = t.dateCreated,
                    status = t.status,
                    branchId = t.branchId
                };

                DBContext.tbl_client.Add(entiry);
                DBContext.SaveChanges();

                t.clientId = entiry.clientId;
                result.returnObj = t;
            }
            catch (Exception ex)
            {
                result.isSuccessful = false;
                result.errorMsg = ex.ToString();
                
            }

            return result;
            
        }

        public dtoResult Update(dtoClient t)
        {
            var result = new dtoResult();
            result.isSuccessful = true;
            try
            {
                var entity = DBContext.tbl_client.FirstOrDefault(d => d.clientId == t.clientId);
               
                entity.clientCode = t.clientCode;
                entity.clientName = t.clientName;
                entity.clientAddress = t.clientAddress;
                entity.clientContactNumber = t.clientContactNumber;
                entity.clientContactPerson = t.clientContactPerson;
                entity.status = t.status;
                entity.modifiedBy = t.modifiedBy;
                entity.dateLastModified = t.dateLastModified;
                

                
                DBContext.SaveChanges();

                
                
            }
            catch (Exception ex)
            {
                result.isSuccessful = false;
                result.errorMsg = ex.ToString();

            }

            return result;
        }

        public dtoResult SoftDelete(dtoClient t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Delete(dtoClient t)
        {
            throw new NotImplementedException();
        }

        #region IClientAccount Members

        public List<dtoClient> GetAll(string search, object filter = null, int? skip = null, int? take = null)
        {
            string sQuery = string.Format("select * from tbl_client where ('{0}' = '' or clientCode like '%{0}%' or clientName like '%{0}%' or clientContactPerson like '%{0}%') ", search);

            if (filter != null)
            {
                var f = (dtoClient)filter;
                sQuery += string.Format("and ('{0}' = '' or clientCode like '%{0}%'  )", f.clientCode);
                sQuery += string.Format("and ('{0}' = '' or clientName like '%{0}%'  )", f.clientName);
                sQuery += string.Format("and ('{0}' = '' or clientContactNumber like '%{0}%'  )", f.clientContactNumber);
                sQuery += string.Format("and ('{0}' = '' or clientContactPerson like '%{0}%'  )", f.clientContactPerson);
                sQuery += string.Format(" AND ({0} = 0 OR [status] = {0})", f.status);
            }

            return DBContext.Database.SqlQuery<dtoClient>(sQuery).ToList();
        }

        public List<dtoClient> GetAll(object filter = null, int? skip = null, int? take = null)
        {
            if (skip == null)
                skip = 0;
            if (take == null)
                take = 10;

            string sQuery = string.Format(@"SELECT TOP {0}
	                                              [Row]
                                                  ,[clientId]
	                                              ,[clientCode]
                                                  ,[clientName]
                                                  ,[clientAddress]
                                                  ,[clientContactNumber]
                                                  ,[clientContactPerson]
                                                  ,[branchId]
                                                  ,[status]
                                                  ,[dateCreated]
                                                  ,[createdBy]
                                            FROM (  SELECT
                                                    ROW_NUMBER() OVER(ORDER BY [clientId]) AS [Row]
                                                      ,[clientId]
                                                      ,[clientCode]
                                                      ,[clientName]
                                                      ,[clientAddress]
                                                      ,[clientContactNumber]
                                                      ,[clientContactPerson]
                                                      ,[branchId]
                                                      ,[status]
                                                      ,[dateCreated]
                                                      ,[createdBy]
                                                    FROM [Genesis].[dbo].[tbl_client]
                                                    WHERE (1 = 1)
                                            ", take);

            if (filter != null)
            {
                var f = (dtoClient)filter;
                sQuery += string.Format(" AND ('{0}' = '' OR clientCode LIKE '%{0}%')", f.clientCode);
                sQuery += string.Format(" AND ('{0}' = '' OR clientName LIKE '%{0}%')", f.clientName);
                sQuery += string.Format(" AND ('{0}' = '' OR clientContactNumber LIKE '%{0}%')", f.clientContactNumber);
                sQuery += string.Format(" AND ('{0}' = '' OR clientContactPerson LIKE '%{0}%')", f.clientContactPerson);
                sQuery += string.Format(" AND ('{0}' = '' OR branchId LIKE '%{0}%')", f.branchId);
                sQuery += string.Format(" AND ({0} = 0 OR [status] = {0})", f.status);
            }

            sQuery += string.Format(") x WHERE x.[Row] > {0}", skip);

            return DBContext.Database.SqlQuery<dtoClient>(sQuery).ToList();
        }

        public dtoClient GetClientInfo(string id)
        {
            var sqlString =
                String.Format("select client.*,b.userName as createdByName, c.userName as modifiedByName, " +
                              "(select count(*) from tbl_document where referenceId = client.clientId and documentType=1) as transactionCount, " +
                              "(select top(1) transactionDate from tbl_document where referenceId = client.clientId order by transactionDate desc) as lastTransactionDate, " +
                              "(select top(1) paymentDate from tbl_receivable where clientId = client.clientId order by paymentDate desc) as lastPaymentDate, " +
                              "(select ISNULL(cast(sum(((a.quantity*a.unitPrice*(1-a.discountA/100))*(1-a.discountB/100))*(1-a.discountC/100)) as decimal(18,2)),0) as totalPurchased from tbl_transaction a left join tbl_document b on a.documentId = b.documentId where b.referenceId = client.clientId and b.documentType = 1) -" +
                              "(select ISNULL(sum(paymentPrice),0) as totalPaid from tbl_receivableDetails a left join tbl_receivable b on a.receivableId = b.receivableId where b.clientId = client.clientId) as remainingBalance, " +
                              "(select ISNULL(sum(cashAmount),0) from tbl_receivable where clientId = client.clientId) as totalPayments, "+
                              "(select ISNULL(sum(paymentPrice),0) as totalDocPayment from tbl_receivableDetails a left join tbl_document b on a.documentId = b.documentId where b.referenceId = client.clientId and documentType = 1) as totalDocumentPayments "+
                              "from tbl_client client " +
                              "left join tbl_users b " +
                              "on client.createdBy = b.userId " +
                              "left join tbl_users c " +
                              "on client.modifiedBy = c.userId where client.clientId = {0}" +
                              "", id);

            return DBContext.Database.SqlQuery<dtoClient>(sqlString).FirstOrDefault();
        }

        public List<dtoClientSalesInvoice> GetClientSalesInvoices(string id)
        {
            var sqlString =
                String.Format("select document.documentId,document.documentNumber,document.transactionDate, "+
                            "cast(sum(((trans.quantity*trans.unitPrice*(1-trans.discountA/100))*(1-trans.discountB/100))*(1-trans.discountC/100)) as decimal(18,2)) as totalPrice, "+
                            "(select ISNULL(sum(paymentPrice),0) from tbl_receivableDetails n where n.documentId = document.documentId ) as totalPayments "+
                            "from tbl_document document "+
                            "left join tbl_transaction trans "+
                            "on document.documentId = trans.documentId "+
                            "where document.referenceId = {0} and document.documentType = 1 " +
                            "group by document.documentId,document.documentNumber,document.transactionDate" +
                              "", id);

            return DBContext.Database.SqlQuery<dtoClientSalesInvoice>(sqlString).ToList();
        }

        public List<dtoClientSalesInvoice> GetClientSalesInvoicesWithBalance(string id)
        {
            var sqlString =
                String.Format("select document.documentId,document.documentNumber,document.transactionDate, " +
                            "cast(sum(((trans.quantity*trans.unitPrice*(1-trans.discountA/100))*(1-trans.discountB/100))*(1-trans.discountC/100)) as decimal(18,2)) as totalPrice, " +
                            "(select ISNULL(sum(paymentPrice),0) from tbl_receivableDetails n where n.documentId = document.documentId ) as totalPayments " +
                            "from tbl_document document " +
                            "left join tbl_transaction trans " +
                            "on document.documentId = trans.documentId " +
                            "where document.referenceId = {0} and document.documentType = 1 " +
                            "group by document.documentId,document.documentNumber,document.transactionDate " +
                             "having (select ISNULL(sum(paymentPrice),0) from tbl_receivableDetails n where n.documentId = document.documentId ) < cast(sum(((trans.quantity*trans.unitPrice*(1-trans.discountA/100))*(1-trans.discountB/100))*(1-trans.discountC/100)) as decimal(18,2))", id);

            return DBContext.Database.SqlQuery<dtoClientSalesInvoice>(sqlString).ToList();
        }

        public List<dtoClientPayment> GetClientPayments(string id)
        {
            var sqlString = String.Format("select * from tbl_receivable where clientId={0}", id);

            return DBContext.Database.SqlQuery<dtoClientPayment>(sqlString).ToList();
        }


        public int GetRecordCount(object filter = null)
        {
            string sQuery = string.Format("SELECT COUNT(clientId) FROM tbl_client WHERE (1 = 1)");

            if (filter != null)
            {
                var f = (dtoClient)filter;
                sQuery += string.Format(" AND ('{0}' = '' OR clientCode LIKE '%{0}%')", f.clientCode);
                sQuery += string.Format(" AND ('{0}' = '' OR clientName LIKE '%{0}%')", f.clientName);
                sQuery += string.Format(" AND ('{0}' = '' OR clientContactNumber LIKE '%{0}%')", f.clientContactNumber);
                sQuery += string.Format(" AND ('{0}' = '' OR clientContactPerson LIKE '%{0}%')", f.clientContactPerson);
                sQuery += string.Format(" AND ({0} = 0 OR [status] = {0})", f.status);
            }

            return DBContext.Database.SqlQuery<int>(sQuery).First();
        }

        public List<string> CheckClientCodeExists(string clientCode)
        {
            string sQuery = string.Format("select top 1 clientCode from tbl_client where clientCode = '{0}'", clientCode);

            return DBContext.Database.SqlQuery<string>(sQuery).ToList();
        }

        public dtoClientSalesInvoice GetSalesInvoiceDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
                id = "0";

            var sqlString =
                String.Format("select document.documentId,document.documentNumber,document.transactionDate, " +
                            "cast(sum(((trans.quantity*trans.unitPrice*(1-trans.discountA/100))*(1-trans.discountB/100))*(1-trans.discountC/100)) as decimal(18,4)) as totalPrice, " +
                            "(select ISNULL(sum(paymentPrice),0) from tbl_receivableDetails n where n.documentId = document.documentId ) as totalPayments " +
                            "from tbl_document document " +
                            "left join tbl_transaction trans " +
                            "on document.documentId = trans.documentId " +
                            "where document.documentType = 1 and document.documentId = {0} " +
                            "group by document.documentId,document.documentNumber,document.transactionDate" +
                              "", id);

            return DBContext.Database.SqlQuery<dtoClientSalesInvoice>(sqlString).FirstOrDefault();
        }

        public List<KeyValue> GetClientsFroDropDown()
        {
            string sQuery = "select id=clientId, text= clientName  from tbl_client";
            return DBContext.Database.SqlQuery<KeyValue>(sQuery).ToList();
        }

        public List<KeyValue> GetClientsForDropDown2(string search,int branchId)
        {
            //string sQuery = "select id=clientId, text= clientName  from tbl_client";

            string sQuery = string.Format(@"select id=clientId,text= clientName from tbl_client 
                                            where ('{0}' = '' or clientCode like '%{0}%' 
                                            or clientName like '%{0}%' 
                                            or clientContactPerson like '%{0}%')
                                            and branchId = {1}", search,branchId);
            return DBContext.Database.SqlQuery<KeyValue>(sQuery).ToList();
        }

        
        #endregion
    }
}
