using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Providers.Entities;
using Genesis.DataAccess.Interfaces;
using Genesis.DTO;

namespace Genesis.DataAccess.Repositories
{
    public class RepoSales : RepoBase, ISales
    {
        #region ISales Members

        public IEnumerable<DTO.dtoDocument> GetAll(string search, object filter = null, int? skip = null, int? take = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DTO.dtoDocument> GetAll(object filter = null, int? skip = null, int? take = null)
        {
            if (skip == null)
                skip = 0;
            if (take == null)
                take = 10;

            var f = (dtoDocument)filter;

            string sQuery = string.Format("exec GetAllDocumentByType '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}", skip, take, f.documentNumber, f.supplierCode, f.supplierName, f.dateFrom, f.dateTo, f.documentType);

            return DBContext.Database.SqlQuery<dtoDocument>(sQuery);
        }

        public int GetRecordCount(object filter = null)
        {
            string sQuery = string.Format("SELECT COUNT(documentId) FROM tbl_document WHERE (1 = 1) AND documentType = 1");

            return DBContext.Database.SqlQuery<int>(sQuery).FirstOrDefault();
        }

        //used to get all branch sales
        public List<dtoDocument> GetAll2(int page, int recordPerPage, object filter, bool isExport)
        {
//            string sQuery = string.Format(@"select a.documentId,a.referenceId,a.documentNumber,a.dateCreated,b.clientName,b.clientCode, sum(c.unitPrice*c.quantity) as salesPrice
//                                            from tbl_document a
//                                            left join tbl_client b
//                                            on a.referenceId = b.clientId
//                                            left join tbl_transaction c
//                                            on a.documentId = c.documentId 
//                                            Where (1 = 1)");

//            if (filter != null)
//            {
//                var f = (dtoDocument)filter;
//                sQuery += string.Format("and ('{0}' = '0' or a.documentId = {0}  )", f.documentId);
//                sQuery += string.Format("and ('{0}' = '' or a.documentNumber like '%{0}%'  )", f.documentNumber);
//                sQuery += string.Format("and ('{0}' = '' or b.clientName like '%{0}%'  )", f.clientName);
//                sQuery += string.Format("and ('{0}' = '' or b.clientCode like '%{0}%'  )", f.clientCode);
//                sQuery += string.Format("and ('{0}' = '' or a.transactionDate BETWEEN '{0}' AND '{1}'   )", f.dateFrom, f.dateTo);
//                sQuery += string.Format("and a.branchId = {0} ", f.branchId);
//            }
            
//            sQuery += "and a.documentType in ('1','5') ";
//            sQuery += "group by a.documentId,a.documentNumber,a.referenceId,a.dateCreated,b.clientName,b.clientCode";

//            return DBContext.Database.SqlQuery<dtoDocument>(sQuery).ToList();
            var retList = new List<dtoDocument>();
            try
            {

                var f = (dtoDocument) filter;
                
                  retList=  DBContext.Database.SqlQuery<dtoDocument>(
                        "EXEC sp_GetAllBranchSales @Page,@RecsPerPage,@DocumentId,@ClientName,@ClientCode,@DocumentNumber,@DateFrom,@DateTo,@BranchId,@IsExport"
                        , new SqlParameter("Page", page)
                        , new SqlParameter("RecsPerPage", recordPerPage)
                        , new SqlParameter("DocumentId", f.documentId)
                        , new SqlParameter("DocumentNumber", f.documentNumber)
                        , new SqlParameter("ClientName", f.clientName)
                        , new SqlParameter("ClientCode", f.clientCode)
                        , new SqlParameter("DateTo", f.dateTo)
                        , new SqlParameter("DateFrom", f.dateFrom)
                        , new SqlParameter("BranchId", f.branchId)
                        , new SqlParameter("IsExport", isExport)
                        ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " : " + ex.InnerException);
            }

            return retList;
        }

        public dtoResult SaveInvoiceTransaction(dtoDocument document, List<dtoTransaction> products)
        {
            var result = new dtoResult();
            try
            {
                int docType = 0;
                switch (document.documentNumber.Substring(0, 2))
                {
                    //sales invoice
                    case "SI":
                        docType = 1;
                        break;
                    //sales refund
                    case "RF":
                        docType = 5;
                        break;
                }

                document.documentType = docType;

                if (document.documentId == 0)
                {
                    AddDocument(ref document);
                }
                else
                {
                    EditDocument(document);
                }

                foreach (
                    var item in
                        DBContext.tbl_transaction.ToList()
                            .Where( d => d.documentId == document.documentId))
                {
                    
                    var product = DBContext.tbl_product.FirstOrDefault(d => d.productId == item.productId);
                    if (product != null)
                    {
                        if (document.documentType == 1)
                        {
                            //item.transactionType = 3;
                            product.outgoing = product.outgoing - item.quantity;
                            product.ending = (product.beginning + product.incoming) - product.outgoing;
                        }
                        else if (document.documentType == 5)
                        {
                            //item.transactionType = 7;
                            product.incoming = product.incoming - item.quantity;
                            product.ending = (product.beginning + product.incoming) - product.outgoing;
                        }
                    }

                    DBContext.tbl_transaction.Remove(item);
                }

                if(products != null)
                foreach (var item in products)
                {

                    if (item.transactionId == 0)
                    {
                        item.documentId = document.documentId;

                        if (document.documentType == 1)
                        {
                            item.transactionType = 3;
                        }
                        else if (document.documentType == 5)
                        {
                            item.transactionType = 7;
                        }
                        AddTransaction(item, document.createdBy);
                    }
                }

                DBContext.SaveChanges();
                result.isSuccessful = true;
                //result.returnObj = t;
            }
            catch(Exception ex)
            {
                result.isSuccessful = false;
                result.errorMsg = ex.ToString();
            }
            return result;
        }

        public dtoResult SaveReceivableTransaction(dtoReceivable header, List<dtoReceivableDetail> details)
        {
            var result = new dtoResult();
            try
            {
                if (header.isNew)
                {
                    AddReceivable(ref header);
                }

                if (details != null)
                    foreach (var item in details)
                    {
                       item.receivableId = header.receivableId;
                       item.paymentPrice = item.totalPayAmount;
                       item.createdBy = header.createdBy;
                       item.dateCreated = header.dateCreated;
                       AddReceivableDetail(item);
                    }

                DBContext.SaveChanges();
                result.isSuccessful = true;

            }
            catch (Exception ex)
            {
                result.isSuccessful = false;
                result.errorMsg = ex.ToString();
            }
            return result;
        }

        public void AddReceivable(ref dtoReceivable t)
        {

            var receivable = new tbl_receivable()
            {
                referenceNumber = t.referenceNumber,
                cashAmount = t.chequeAmount,
                chequeNumber = t.chequeNumber,
                chequeDate = t.chequeDate,
                chequeBank = t.chequeBank,
                dateCreated = DateTime.Now,
                createdBy = t.createdBy,
                clientId = t.clientId
            };

            DBContext.tbl_receivable.Add(receivable);
            DBContext.SaveChanges();

            t.receivableId = receivable.receivableId;
        }

        public void AddReceivableDetail(dtoReceivableDetail t)
        {

            var receivableDetail = new tbl_receivableDetails()
            {
                receivableId = t.receivableId,
                paymentPrice = t.paymentPrice,
                documentId = t.documentId,
                dateCreated = DateTime.Now,
                createdBy = t.createdBy,
            };

            DBContext.tbl_receivableDetails.Add(receivableDetail);
            DBContext.SaveChanges();

        }

        public void AddDocument(ref dtoDocument t)
        {

            var document = new tbl_document
            {
                documentNumber = t.documentNumber,
                transactionDate = t.transactionDate,
                referenceId = t.referenceId,
                dateCreated = DateTime.Now,
                createdBy = t.createdBy,
                documentType = t.documentType,
                //documentType = docType,
                documentId = t.documentId,
                branchId = t.branchId
            };

            DBContext.tbl_document.Add(document);
            DBContext.SaveChanges();

            t.documentId = document.documentId;
            


        }

        public void EditDocument(dtoDocument t)
        {

            var item = DBContext.tbl_document.FirstOrDefault(d => d.documentId == t.documentId);
            if (item != null)
            {
                item.documentNumber = t.documentNumber;
                item.referenceId = t.referenceId;
                item.transactionDate = t.transactionDate;
                DBContext.SaveChanges();
            }


        }

        public void AddTransaction(dtoTransaction t,int userId)
        {
            var transaction = new tbl_transaction
            {
                documentId = t.documentId,
                productId = t.productId,
                transactionType = t.transactionType,
                quantity = t.quantity,
                unitPrice = t.unitPrice,
                dateCreated = DateTime.Now,
                discountA = t.discountA ?? 0,
                discountB = t.discountB ?? 0,
                discountC = t.discountC ?? 0,

            };



            var product = DBContext.tbl_product.FirstOrDefault(d => d.productId == t.productId);
            if (product != null)
            {
                
                if (t.transactionType == 3)
                {
                    //item.transactionType = 3;
                    product.outgoing = product.outgoing + t.quantity;
                    product.ending = (product.beginning + product.incoming) - product.outgoing;
                }
                else if (t.transactionType == 7)
                {
                    //item.transactionType = 7;
                    product.incoming = product.incoming+ t.quantity;
                    product.ending = (product.beginning + product.incoming) - product.outgoing;
                }
            }
            

            DBContext.tbl_transaction.Add(transaction);
            t.transactionId = transaction.transactionId;



        }

        public Boolean CheckExistingDocument(String documentNumber, DateTime? documentDate)
        {
            string sQuery = string.Format(@"IF EXISTS (SELECT * 
                                                       FROM tbl_document 
                                                       WHERE documentNumber = '{0}'
                                                       AND transactionDate = '{1}')
                                            BEGIN
                                                select 'true';
                                            END
                                            ELSE
                                            BEGIN
                                                select 'false';
                                            END", documentNumber,documentDate);
            var result = DBContext.Database.SqlQuery<String>(sQuery).FirstOrDefault();

            return Boolean.Parse(result);
        }

        public List<dtoTransaction> GetAllSaleItems(int documentId)
        {
            string sQuery = string.Format("exec GetAllOrderItems {0}", documentId);

            return DBContext.Database.SqlQuery<dtoTransaction>(sQuery).ToList();
        }

        public List<dtoReceivable> GetAllReceivables(object filter = null, int? skip = null, int? take = null)
        {
            string sQuery = string.Format(@"select a.documentId,a.referenceId,a.documentNumber,a.dateCreated,b.clientName,b.clientCode, sum(c.unitPrice*c.quantity) as salesPrice
                                            from tbl_document a
                                            left join tbl_client b
                                            on a.referenceId = b.clientId
                                            left join tbl_transaction c
                                            on a.documentId = c.documentId 
                                            Where (1 = 1)");

            if (filter != null)
            {
                var f = (dtoReceivable)filter;
                sQuery += string.Format("and ('{0}' = '0' or a.referenceNumber = {0}  )", f.referenceNumber);
                sQuery += string.Format("and ('{0}' = '' or b.clientName like '%{0}%'  )", f.clientName);
                sQuery += string.Format("and ('{0}' = '' or b.clientCode like '%{0}%'  )", f.clientCode);
                sQuery += string.Format("and a.branchId = {0} ", f.branchId);
            }

            sQuery += "and a.documentType =1 ";
            sQuery += "group by a.documentId,a.documentNumber,a.referenceId,a.dateCreated,b.clientName,b.clientCode";

            return DBContext.Database.SqlQuery<dtoReceivable>(sQuery).ToList();
        }

        #endregion

        #region IBase<dtoDocument> Members

        public DTO.dtoDocument Get(DTO.dtoDocument t)
        {
            throw new NotImplementedException();
        }

        public List<DTO.dtoDocument> GetAll(string search, object filter = null)
        {
            throw new NotImplementedException();
        }

        public DTO.dtoResult Insert(DTO.dtoDocument t)
        {
            throw new NotImplementedException();
        }

        public DTO.dtoResult Update(DTO.dtoDocument t)
        {
            throw new NotImplementedException();
        }

        public DTO.dtoResult SoftDelete(DTO.dtoDocument t)
        {
            throw new NotImplementedException();
        }

        public DTO.dtoResult Delete(DTO.dtoDocument t)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
