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
    public class BLPurchase
    {
        IPurchase repo;
        public BLPurchase()
        {
            repo = new RepoPurchase();
        }

        public List<dtoDocument> GetAllPurchases(object filter = null, int? skip = null, int? take = null)
        {
            return repo.GetAll(filter, skip, take);
        }

        public List<dtoDocument> GetAllPurchases2(int page, int recordPerPage, object filter, bool isExport)
        {
            return repo.GetAll2(page, recordPerPage, filter, isExport);
        }

        public List<dtoTransaction> GetAllOrderItems(string search, object filter = null, int? skip = null, int? take = null)
        {
            return repo.GetAllOrderItems(Convert.ToInt32(search));
        }

        public int GetRecordCount(object filter = null)
        {
            return repo.GetRecordCount(filter);
        }

        public void SaveTransaction(dtoDocument document, List<dtoTransaction> products)
        {
            repo.SaveTransaction(document,products);
        }


        public dtoResult SavePurchaseTransaction(dtoDocument header, List<dtoTransaction> details)
        {
            return repo.SavePurchaseTransaction(header, details);
        }

        public List<dtoTransaction> GetAllPurchaseItems(int documentId)
        {
            return repo.GetAllPurchaseItems(documentId);
        }

    }
}
