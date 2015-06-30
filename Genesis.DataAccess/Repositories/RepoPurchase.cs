using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DTO;

namespace Genesis.DataAccess.Repositories
{
    public class RepoPurchase : RepoBase, IPurchase
    {

        public List<dtoDocument> GetAll(string search, object filter = null, int? skip = null, int? take = null)
        {
            throw new NotImplementedException();
        }

        public List<dtoDocument> GetAll(object filter = null, int? skip = null, int? take = null)
        {
            if (skip == null)
                skip = 0;
            if (take == null)
                take = 10;

            var f = (dtoDocument)filter;
            
            string sQuery = string.Format("exec GetAllDocument '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}'", skip, take, f.documentNumber, f.documentType, f.supplierCode, f.supplierName, f.dateFrom, f.dateTo);

            return DBContext.Database.SqlQuery<dtoDocument>(sQuery).ToList();
        }

        public List<dtoDocument> GetAll2(object filter = null, int? skip = null, int? take = null)
        {
            string sQuery = string.Format(@"select a.documentId,a.referenceId,a.documentNumber,a.dateCreated,b.supplierName,b.supplierCode, sum(c.unitPrice*c.quantity) as salesPrice
                                            from tbl_document a
                                            left join tbl_supplier b
                                            on a.referenceId = b.supplierId
                                            left join tbl_transaction c
                                            on a.documentId = c.documentId 
                                            Where (1 = 1)");

            if (filter != null)
            {
                var f = (dtoDocument)filter;
                sQuery += string.Format("and ('{0}' = '0' or a.documentId = {0}  )", f.documentId);
                sQuery += string.Format("and ('{0}' = '' or a.documentNumber like '%{0}%'  )", f.documentNumber);
                sQuery += string.Format("and ('{0}' = '' or b.supplierName like '%{0}%'  )", f.supplierName);
                sQuery += string.Format("and ('{0}' = '' or b.supplierCode like '%{0}%'  )", f.supplierCode);
                sQuery += string.Format("and a.branchId = {0} ", f.branchId);
            }

            sQuery += "and a.documentType =2 ";
            sQuery += "group by a.documentId,a.documentNumber,a.referenceId,a.dateCreated,b.supplierName,b.supplierCode";

            return DBContext.Database.SqlQuery<dtoDocument>(sQuery).ToList();
        }

        public int GetRecordCount(object filter = null)
        {
            string sQuery = string.Format("SELECT COUNT(documentId) FROM tbl_document WHERE (1 = 1)");

            if (filter != null)
            {
                //var f = (dtoClient)filter;
                //sQuery += string.Format(" AND ('{0}' = '' OR clientCode LIKE '%{0}%')", f.clientCode);
                //sQuery += string.Format(" AND ('{0}' = '' OR clientName LIKE '%{0}%')", f.clientName);
                //sQuery += string.Format(" AND ('{0}' = '' OR clientContactNumber LIKE '%{0}%')", f.clientContactNumber);
                //sQuery += string.Format(" AND ('{0}' = '' OR clientContactPerson LIKE '%{0}%')", f.clientContactPerson);
                //sQuery += string.Format(" AND ({0} = 0 OR [status] = {0})", f.status);
            }

            return DBContext.Database.SqlQuery<int>(sQuery).First();
        }

        public dtoDocument Get(dtoDocument t)
        {
            throw new NotImplementedException();
        }

        public List<dtoDocument> GetAll(string search, object filter = null)
        {
            throw new NotImplementedException();
        }

        public dtoResult Insert(dtoDocument t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Update(dtoDocument t)
        {
            throw new NotImplementedException();
        }

        public dtoResult SoftDelete(dtoDocument t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Delete(dtoDocument t)
        {
            throw new NotImplementedException();
        }


        public List<dtoTransaction> GetAllOrderItems(int documentId)
        {
            string sQuery = string.Format("exec GetAllOrderItems {0}", documentId);

            return DBContext.Database.SqlQuery<dtoTransaction>(sQuery).ToList();
        }

        public void SaveTransaction(dtoDocument document, List<dtoTransaction> products)
        {
            if (document.documentId == 0)
            {
                AddDocument(ref document);
            }else
            {
                EditDocument(document);

            }

            foreach (var item in DBContext.tbl_transaction.ToList().Where(d => d.documentId == document.documentId &&  !products.Select(t => t.transactionId).Contains(d.transactionId)))
            {
                DBContext.tbl_transaction.Remove(item);
            }
            

            foreach (var item in products)
            {
                
                if (item.transactionId == 0)
                {
                    item.documentId = document.documentId;
                    AddTransaction(item);
                }
            }

            DBContext.SaveChanges();
        }

        

        public void AddTransaction(dtoTransaction t)
        {
            var transaction = new tbl_transaction { 
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


            if (t.transactionType == 1)
            {
                var product = DBContext.tbl_product.FirstOrDefault(d => d.productId == t.productId);
                product.unitPrice = t.unitPrice;
            }

            DBContext.tbl_transaction.Add(transaction);
            t.transactionId = transaction.transactionId;

            
            
        }

        public dtoDocument GetDocument(int documentId)
        {
            string sQuery = string.Format("select t1.*, t2.supplierName, t3.clientName from tbl_document t1 left join tbl_supplier t2 on t1.referenceid = t2.supplierid left join tbl_client t3 on t1.referenceid = t3.clientId where t1.documentid = {0}", documentId);
            return DBContext.Database.SqlQuery<dtoDocument>(sQuery).FirstOrDefault();
        }


        public List<dtoDocument> GetPurchaseOrderByFilter(dtoDocument filter)
        {
            var poList = (from i in DBContext.tbl_document.Where(i => i.documentType == 2 && i.referenceId == filter.referenceId)
                        join b in DBContext.tbl_supplier on i.referenceId equals b.supplierId
                        select new dtoDocument {    
                        documentId = i.documentId,
                        branchId = i.branchId,
                        referenceId = i.referenceId,
                        documentNumber = i.documentNumber,
                        documentType = i.documentType,
                        dateCreated = i.dateCreated,
                        purchasePrice = 0,
                        transactionDate = i.transactionDate,
                        supplierCode = b.supplierCode,
                        supplierName = b.supplierName,
                        totalPaid = DBContext.tbl_paymentDetails.Where(d => d.documentId == i.documentId).Sum(e => e.paymentPrice) ?? 0.00M
                        }).ToList();

            

            foreach (var item in poList)
            {
                //DBContext.tbl_transaction.Where(x => x.documentId == x.documentId).Sum(y => (y.discountA != 0 || y.discountB != 0 || y.discountC != 0) ? ((((y.unitPrice * ((100M - (decimal)y.discountA) * 0.01M)) * ((100M - (decimal)y.discountB) * 0.01M)) * ((100M - (decimal)y.discountC) * 0.01M)) * y.quantity) : (y.unitPrice * y.quantity)),
                var details = DBContext.tbl_transaction.Where(x => x.documentId == item.documentId).ToList();
                item.purchasePrice = details.Sum(y => (y.discountA != 0 || y.discountB != 0 || y.discountC != 0) ? ((((y.unitPrice * ((100M - (decimal)y.discountA) * 0.01M)) * ((100M - (decimal)y.discountB) * 0.01M)) * ((100M - (decimal)y.discountC) * 0.01M)) * y.quantity) : (y.unitPrice * y.quantity));
                
            }

            poList = poList.Where(i => i.totalPaid < i.purchasePrice).ToList();
            return poList;
        }

        public dtoResult SavePurchaseTransaction(dtoDocument document, List<dtoTransaction> products)
        {
            var result = new dtoResult();
            try
            {


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
                            .Where( d => d.documentId == document.documentId ))
                {
                    var product = DBContext.tbl_product.FirstOrDefault(d => d.productId == item.productId);
                    if (product != null)
                    {
                        product.unitPrice = item.unitPrice;
                        product.incoming = product.incoming - item.quantity;
                        product.ending = (product.beginning + product.incoming) - product.outgoing;
                    }
                    DBContext.tbl_transaction.Remove(item);
                }

                if (products != null)
                foreach (var item in products)
                {

                    if (item.transactionId == 0)
                    {
                        item.documentId = document.documentId;
                        item.transactionType = 1;
                        AddTransaction(item, document.createdBy);
                    }
                }

                DBContext.SaveChanges();
                result.isSuccessful = true;
                //result.returnObj = t;
            }
            catch (Exception ex)
            {
                result.isSuccessful = false;
                result.errorMsg = ex.ToString();
            }
            return result;
        }

        public List<dtoTransaction> GetAllPurchaseItems(int documentId)
        {
            string sQuery = string.Format("exec GetAllOrderItems {0}", documentId);

            return DBContext.Database.SqlQuery<dtoTransaction>(sQuery).ToList();
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

        public void AddTransaction(dtoTransaction t, int userId)
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
            var priceHistory = new tbl_productPriceHistory()
            {
                productId = product.productId,
                dateCreated = DateTime.Now,
                createdBy = userId,
                price = product.unitPrice
            };

            DBContext.tbl_productPriceHistory.Add(priceHistory);


            

            if (product != null)
            {
                product.unitPrice = t.unitPrice;
                product.incoming = product.incoming + t.quantity;
                product.ending = (product.beginning + product.incoming) - product.outgoing;
            }


            DBContext.tbl_transaction.Add(transaction);
            t.transactionId = transaction.transactionId;



        }
    }
}
