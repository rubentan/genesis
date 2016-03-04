using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DTO;

namespace Genesis.DataAccess.Repositories
{
    public class RepoDashboard : RepoBase, IDashboardReceivables,IDashboardPayables,IDashboardReorder
    {

        public List<dtoDashboardReceivables> GetAllReceivables(int branchId, int? skip = null, int? take = null)
        {
            if (skip == null)
                skip = 0;
            if (take == null)
                take = 10;

            string sQuery = string.Format(@" select price.documentId,price.dateCreated,price.clientName,price.clientCode,price.clientId,price.documentTotal, isnull(payment.paymentTotal,0) as paymentReceived,price.documentTotal-isnull(payment.paymentTotal,0) as due  from
                                                      (select a.documentId, sum(b.quantity*b.unitPrice) as documentTotal, a.dateCreated, c.clientName,c.clientCode,c.clientId 
                                                      from tbl_document a
                                                      left join tbl_transaction b
                                                      on a.documentId = b.documentId
                                                      left join tbl_client c
                                                      on a.referenceId = c.clientId 
                                                      where a.documentType = 1
                                                      and c.branchId = {0}
                                                      and c.clientName <> 'CASH'
                                                      group by a.documentId,a.dateCreated,c.clientName,c.clientCode,c.clientId) price
                                                      inner join 
                                                      (select a.documentId, sum(paymentPrice) as paymentTotal
                                                      from tbl_document a
                                                      left join tbl_receivableDetails b
                                                      on a.documentId = b.documentId
                                                      left join tbl_client c
                                                      on a.referenceId = c.clientId 
                                                      where a.documentType = 1
                                                      and c.branchId = {0}
                                                      and c.clientName <> 'CASH'
                                                      group by a.documentId) payment
                                                      on price.documentId = payment.documentId
                                                      where price.documentTotal > isnull(payment.paymentTotal,0)
                                                      order by price.dateCreated asc;", branchId);

            //sQuery += string.Format(") x WHERE x.[Row] > {0}", skip);

            return DBContext.Database.SqlQuery<dtoDashboardReceivables>(sQuery).ToList();
        }

        public List<dtoDashboardPayables> GetAllPayables(int branchId, int? skip = null, int? take = null)
        {
            if (skip == null)
                skip = 0;
            if (take == null)
                take = 10;

            string sQuery = string.Format(@"  select price.documentId,price.dateCreated,price.supplierName,price.supplierCode,price.supplierId,price.documentTotal, isnull(payment.paymentTotal,0) as paymentSent,price.documentTotal-isnull(payment.paymentTotal,0) as due  from
                                                      (select a.documentId, sum(b.quantity*b.unitPrice) as documentTotal, a.dateCreated, c.supplierName,c.supplierCode,c.supplierId 
                                                      from tbl_document a
                                                      left join tbl_transaction b
                                                      on a.documentId = b.documentId
                                                      left join tbl_supplier c
                                                      on a.referenceId = c.supplierId 
                                                      where a.documentType = 2
                                                      and c.branchId = {0}
                                                      group by a.documentId,a.dateCreated,c.supplierName,c.supplierCode,c.supplierId) price
                                                      inner join 
                                                      (select a.documentId, sum(paymentPrice) as paymentTotal
                                                      from tbl_document a
                                                      left join tbl_paymentDetails b
                                                      on a.documentId = b.documentId
                                                      left join tbl_supplier c
                                                      on a.referenceId = c.supplierId 
                                                      where a.documentType = 2
                                                      and c.branchId = {0}
                                                      group by a.documentId) payment
                                                      on price.documentId = payment.documentId
                                                      where price.documentTotal > isnull(payment.paymentTotal,0)
                                                      order by price.dateCreated asc;", branchId);

            //sQuery += string.Format(") x WHERE x.[Row] > {0}", skip);

            return DBContext.Database.SqlQuery<dtoDashboardPayables>(sQuery).ToList();
        }

        public List<dtoDashboardReorder> GetAllReorders(int branchId, int? skip = null, int? take = null)
        {
            if (skip == null)
                skip = 0;
            if (take == null)
                take = 10;

            string sQuery = string.Format(@" select top 50 * from tbl_product 
                                              where ending <= reorderLevel
                                              and branchId = {0}
                                              order by productDescription asc;", branchId);

            return DBContext.Database.SqlQuery<dtoDashboardReorder>(sQuery).ToList();
        }


        public dtoDashboardReceivables Get(dtoDashboardReceivables t)
        {
            throw new NotImplementedException();
        }

        public dtoDashboardPayables Get(dtoDashboardPayables t)
        {
            throw new NotImplementedException();
        }

        public dtoDashboardReorder Get(dtoDashboardReorder t)
        {
            throw new NotImplementedException();
        }

        List<dtoDashboardReorder> IBase<dtoDashboardReorder>.GetAll(string search, object filter)
        {
            throw new NotImplementedException();
        }

        public dtoResult Insert(dtoDashboardReorder t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Update(dtoDashboardReorder t)
        {
            throw new NotImplementedException();
        }

        public dtoResult SoftDelete(dtoDashboardReorder t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Delete(dtoDashboardReorder t)
        {
            throw new NotImplementedException();
        }


        List<dtoDashboardPayables> IBase<dtoDashboardPayables>.GetAll(string search, object filter)
        {
            throw new NotImplementedException();
        }

        public dtoResult Insert(dtoDashboardPayables t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Update(dtoDashboardPayables t)
        {
            throw new NotImplementedException();
        }

        public dtoResult SoftDelete(dtoDashboardPayables t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Delete(dtoDashboardPayables t)
        {
            throw new NotImplementedException();
        }

        List<dtoDashboardReceivables> IBase<dtoDashboardReceivables>.GetAll(string search, object filter)
        {
            throw new NotImplementedException();
        }

        public dtoResult Insert(dtoDashboardReceivables t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Update(dtoDashboardReceivables t)
        {
            throw new NotImplementedException();
        }

        public dtoResult SoftDelete(dtoDashboardReceivables t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Delete(dtoDashboardReceivables t)
        {
            throw new NotImplementedException();
        }
    }
}
