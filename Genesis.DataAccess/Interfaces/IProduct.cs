using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;

namespace Genesis.DataAccess.Interfaces
{
    public interface IProduct : IBase<dtoProduct>
    {
        IEnumerable<dtoProduct> GetAll(string search, object filter = null, int? skip = null, int? take = null);

        IEnumerable<dtoProduct> GetAll(string search, int branchId, int? skip = null, int? take = null);

        IEnumerable<dtoProduct> GetAll(object filter = null, int? skip = null, int? take = null);
        int GetRecordCount(object filter = null);
        dtoProduct GetByID(int productId);
        dtoProduct GetByCode(string productCode);

        List<dtoProduct> GetBranchProducts(int page, int recordPerPage, object filter,bool isExport);

        List<dtoProductTransactions> GetProductTransactions(int id);

        List<dtoProductTransactions> GetProductSales(int id);

        List<dtoProductTransactions> GetProductPurchases(int id);

        List<dtoProductPriceHistory> GetProductPriceHistory(int id);

        dtoProduct GetProductInfo(int id);

        List<string> CheckProductCodeExists(string productCode);
        dtoResult InLineUpdate(dtoProduct product);
    }
}
