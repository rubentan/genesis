using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;

namespace Genesis.DataAccess.Interfaces
{
    public interface IPurchase : IBase<dtoDocument>
    {
        List<dtoDocument> GetAll(string search, object filter = null, int? skip = null, int? take = null);
        List<dtoDocument> GetAll(object filter = null, int? skip = null, int? take = null);
        List<dtoDocument> GetAll2(int page, int recordPerPage, object filter, bool isExport);
        int GetRecordCount(object filter = null);

        List<dtoTransaction> GetAllOrderItems(int documentId);
        void SaveTransaction(dtoDocument document, List<dtoTransaction> products);
        void AddDocument(ref dtoDocument t);
        void AddTransaction(dtoTransaction t, int docType);
        dtoDocument GetDocument(int documentId);
        List<dtoDocument> GetPurchaseOrderByFilter(dtoDocument filter);
        dtoResult SavePurchaseTransaction(dtoDocument header, List<dtoTransaction> details);

        List<dtoTransaction> GetAllPurchaseItems(int documentId);
    }
}
