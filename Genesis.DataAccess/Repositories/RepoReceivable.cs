using System;
using System.Collections.Generic;
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

        public List<dtoReceivable> GetAllReceivable2(object filter = null, int? skip = null, int? take = null)
        {

            string sQuery = string.Format(@"select top {0} t1.*, t2.clientName, t2.clientCode 
                                            from tbl_receivable t1
                                            left join tbl_client t2
                                            on t1.clientId = t2.clientId
                                            Where (1 = 1) ", take);

            if (filter != null)
            {
                var f = (dtoReceivable)filter;
                sQuery += string.Format("and ('{0}' = '' or t1.referenceNumber like '%{0}%'  )", f.referenceNumber);
                sQuery += string.Format("and ('{0}' = '' or t2.clientName like '%{0}%'  )", f.clientName);
                sQuery += string.Format("and ('{0}' = '' or t2.clientCode like '%{0}%'  )", f.clientCode);
                sQuery += string.Format("and t2.branchId = {0} ", f.branchId);
            }

            sQuery += "order by t1.dateCreated desc";
            

            return DBContext.Database.SqlQuery<dtoReceivable>(sQuery).ToList();
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
            string sQuery = string.Format(@"select * from tbl_receivable where clientId = {0}", clientId);          

            return DBContext.Database.SqlQuery<dtoReceivable>(sQuery).ToList();
        }

            public dtoReceivable GetExistingPaymentDetail(int receivableId)
        {
            string sQuery = string.Format(@"select sum(paymentPrice) as cashAmount, b.referenceNumber,b.chequeNumber,b.chequeDate,b.chequeBank,b.dateCreated
                                            from tbl_receivableDetails a
                                            left join tbl_receivable b
                                            on a.receivableId = b.receivableId
                                            where a.receivableId = {0}
                                            group by b.receivableId,b.referenceNumber,b.chequeNumber,b.chequeDate,b.chequeBank,b.dateCreated", receivableId);

                return DBContext.Database.SqlQuery<dtoReceivable>(sQuery).FirstOrDefault();
        }
    }
}
