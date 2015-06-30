using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;

namespace Genesis.DataAccess.Interfaces
{
    public interface IClientAccount : IBase<dtoClient>
    {
        List<dtoClient> GetAll(string search, object filter = null, int? skip = null, int? take = null);
        List<dtoClient> GetAll(object filter = null, int? skip = null, int? take = null);
        dtoClient GetClientInfo(string id);
        List<dtoClientSalesInvoice> GetClientSalesInvoices(string id);
        List<dtoClientPayment> GetClientPayments(string id);
        int GetRecordCount(object filter = null);

        List<string> CheckClientCodeExists(string clientCode);
        dtoClientSalesInvoice GetSalesInvoiceDetails(string id);
    }
}
