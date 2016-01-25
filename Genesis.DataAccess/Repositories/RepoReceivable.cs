using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;

namespace Genesis.DataAccess.Repositories
{
    public class RepoReceivable : RepoBase
    {
        public List<dtoReceivable> GetAllReceivable()
        {
            string sQuery = "select t1.*, t2.clientName, t2.clientCode from tbl_receivable t1 left join tbl_client t2 on t1.clientId = t2.clientId";
            return DBContext.Database.SqlQuery<dtoReceivable>(sQuery).ToList();
        }

        public List<dtoReceivable> GetAllReceivable2(int page, int recordPerPage, object filter, bool isExport)
        {

//            string sQuery = string.Format(@"select top {0} t1.*, t2.clientName, t2.clientCode 
//                                            from tbl_receivable t1
//                                            left join tbl_client t2
//                                            on t1.clientId = t2.clientId
//                                            Where (1 = 1) ", take);

//            if (filter != null)
//            {
//                var f = (dtoReceivable)filter;
//                sQuery += string.Format("and ('{0}' = '' or t1.referenceNumber like '%{0}%'  )", f.referenceNumber);
//                sQuery += string.Format("and ('{0}' = '' or t2.clientName like '%{0}%'  )", f.clientName);
//                sQuery += string.Format("and ('{0}' = '' or t2.clientCode like '%{0}%'  )", f.clientCode);
//                sQuery += string.Format("and t2.branchId = {0} ", f.branchId);
//            }

//            sQuery += "order by t1.dateCreated desc";
            

//            return DBContext.Database.SqlQuery<dtoReceivable>(sQuery).ToList();

            var retList = new List<dtoReceivable>();
            try
            {

                var f = (dtoReceivable)filter;

                retList = DBContext.Database.SqlQuery<dtoReceivable>(
                      "EXEC sp_GetAllBranchReceivables @Page,@RecsPerPage,@ClientName,@ClientCode,@ReferenceNumber,@DateFrom,@DateTo,@BranchId,@IsExport"
                      , new SqlParameter("Page", page)
                      , new SqlParameter("RecsPerPage", recordPerPage)
                      , new SqlParameter("ReferenceNumber", f.referenceNumber)
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

        public List<dtoReceivableOrder> GetReceivableOrderByClient(int clientId)
        {
            string sQuery = "exec spGetReceivableOrders " + clientId;
            return DBContext.Database.SqlQuery<dtoReceivableOrder>(sQuery).ToList();
        }

        public void SaveTransaction(dtoReceivable header, List<dtoReceivableDetail> details)
        {

            if (header.receivableId == 0)
            {
                tbl_receivable entiry = new tbl_receivable()
                {
                    receivableId = header.receivableId,
                    clientId = header.clientId,
                    isNewPayment = header.isNewPayment,
                    referenceNumber = header.referenceNumber,
                    isCash = header.isCash,
                    cashAmount = header.cashAmount,
                    chequeNumber = header.chequeNumber,
                    chequeDate = header.chequeDate,
                    chequeBank = header.chequeBank,
                    dateCreated = DateTime.Now
                };

                DBContext.tbl_receivable.Add(entiry);
                DBContext.SaveChanges();
                header.receivableId = entiry.receivableId;
            }
            else
            {

                var entity = DBContext.tbl_receivable.FirstOrDefault(d => d.receivableId == header.receivableId);

                entity.clientId = header.clientId;
                entity.isNewPayment = header.isNewPayment;
                entity.referenceNumber = header.referenceNumber;
                entity.isCash = header.isCash;
                entity.cashAmount = header.cashAmount;
                entity.chequeNumber = header.chequeNumber;
                entity.chequeDate = header.chequeDate;
                entity.chequeBank = header.chequeBank;
                entity.dateLastModified = DateTime.Now;

                var arr = details.Where(c=>c.receivableDetailsId != 0).Select(c => c.receivableDetailsId).ToArray<int>();
                var deleteList = DBContext.tbl_receivableDetails.Where(d => !arr.Contains(d.receivableDetailsId)
                && d.receivableId == header.receivableId);

                foreach (var item in deleteList)
                {
                    DBContext.tbl_receivableDetails.Remove(item);
                }
            }

            foreach (var item in details.Where(d => d.receivableDetailsId == 0))
            {

                DBContext.tbl_receivableDetails.Add(new tbl_receivableDetails { 
                documentId = item.documentId,
                receivableId = header.receivableId,
                paymentPrice = item.paymentPrice,
                dateCreated = DateTime.Now,
                

                
                });
            }
            

            DBContext.SaveChanges();
        }

        public dtoReceivable GetReceivable(int id)
        {
            return DBContext.Database.SqlQuery<dtoReceivable>(string.Format("exec spGetReceivable @id={0}", id)).FirstOrDefault();
        }

        public List<dtoReceivableDetail> GetReceivableDetails(int id)
        {

            return DBContext.Database.SqlQuery<dtoReceivableDetail>(string.Format("GetReceivableDetails @id={0}", id)).ToList();
        }


        public List<dtoReceivable> GetExistingPayments(int clientId)
        {
            string sQuery = string.Format(@"select a.receivableId, a.clientId, a.referenceNumber, a.cashAmount, a.chequeDate, a.chequeBank, a.chequeNumber, a.paymentDate,
                            sum(b.paymentPrice) as totalPayment, (a.cashAmount - sum(b.paymentPrice)) as remainingBalance
                            from tbl_receivable a
                            left join tbl_receivableDetails b
                            on a.receivableId = b.receivableId
                            where a.clientId = {0}
                            group by a.receivableId,a.clientId, a.referenceNumber, a.cashAmount, a.chequeDate, a.chequeBank, a.chequeNumber, a.paymentDate
                            having sum(b.paymentPrice) < a.cashAmount", clientId);          

            return DBContext.Database.SqlQuery<dtoReceivable>(sQuery).ToList();
        }

        public dtoReceivable GetExistingPaymentDetail(int receivableId)
        {
            string sQuery = string.Format(@"select a.receivableId, a.clientId, a.referenceNumber, a.cashAmount, a.chequeDate, a.chequeBank, a.chequeNumber, a.paymentDate,
                                            sum(b.paymentPrice) as totalPayment, (a.cashAmount - sum(b.paymentPrice)) as remainingBalance
                                            from tbl_receivable a
                                            left join tbl_receivableDetails b
                                            on a.receivableId = b.receivableId
                                            where a.receivableId = {0}
                                            group by a.receivableId,a.clientId, a.referenceNumber, a.cashAmount, a.chequeDate, a.chequeBank, a.chequeNumber, a.paymentDate
                                            having sum(b.paymentPrice) < a.cashAmount", receivableId);

                return DBContext.Database.SqlQuery<dtoReceivable>(sQuery).FirstOrDefault();
        }
    }
}
