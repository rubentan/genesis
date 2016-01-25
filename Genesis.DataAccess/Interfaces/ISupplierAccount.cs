using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;

namespace Genesis.DataAccess.Interfaces
{
    public interface ISupplierAccount : IBase<dtoSupplier>
    {
        List<dtoSupplier> GetAll(string search, object filter = null, int? skip = null, int? take = null);
        List<dtoSupplier> GetAll(int page, int recordPerPage, object filter,bool isExport);
        int GetRecordCount(object filter = null);
        dtoSupplier GetByID(int supplierId);
        dtoSupplier GetByCode(string supplierCode);
        dtoSupplier GetByName(string supplierName);
        dtoSupplier GetSupplierInfo(string id);
        List<dtoSupplierPurchaseOrder> GetSupplierPurchaseOrders(string id);
        List<dtoSupplierPayment> GetSupplierPayments(string id);

        List<string> CheckSupplierCodeExists(string supplierCode);

        List<dtoSupplierPurchaseOrder> GetSupplierPurchaseOrdersWithBalance(string id);
        dtoSupplierPurchaseOrder GetPurchaseOrderDetails(string id);

    }
}
