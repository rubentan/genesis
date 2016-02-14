using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DataAccess.Repositories;
using Genesis.DTO;

namespace Genesis.BusinessLogic
{
    public class BLSales
    {
        ISales repo;
        public BLSales()
        {
            repo = new RepoSales();
        }

        public IEnumerable<dtoDocument> GetAllSales(object filter = null, int? skip = null, int? take = null)
        {
            return repo.GetAll(filter, skip, take);
        }

        public List<dtoDocument> GetAllSales2(int page, int recordPerPage, object filter, bool isExport)
        {
            return repo.GetAll2(page, recordPerPage, filter, isExport);
        }

        public int GetRecordCount(object filter = null)
        {
            return repo.GetRecordCount(filter);
        }

        public dtoResult SaveInvoiceTransaction(dtoDocument header, List<dtoTransaction> details)
        {
            return repo.SaveInvoiceTransaction(header,details);
        }

        public dtoResult SaveReceivableTransaction(dtoReceivable header, List<dtoReceivableDetail> details)
        {
            return repo.SaveReceivableTransaction(header, details);
        }

        public List<dtoTransaction> GetAllSaleItems(int documentId)
        {
            return repo.GetAllSaleItems(documentId);
        }

        public List<dtoReceivableDetail> GetAllReceivableItems(int documentId)
        {
            return repo.GetAllReceivableItems(documentId);
        }

        public List<dtoReceivable> GetAllReceivables(object filter = null, int? skip = null, int? take = null)
        {
            return repo.GetAllReceivables(filter, skip, take);
        }

        public Boolean CheckExistingDocument(String documentNumber, DateTime? documentDate)
        {
            return repo.CheckExistingDocument(documentNumber, documentDate);
        }
    }
}
