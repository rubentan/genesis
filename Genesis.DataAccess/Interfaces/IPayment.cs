using Genesis.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.DataAccess.Interfaces
{
    public interface IPayment : IBase<dtoPayment>
    {
        List<dtoPayment> GetPaymentsByFilters(dtoPayment filter);
        //New (used for branch payments list)
        List<dtoPayment> GetAllPayments(int page, int recordPerPage, object filter, bool isExport);
        dtoResult SavePaymentTransaction(dtoPayment Header, List<dtoPaymentDetail> Details);
        List<dtoPaymentDetail> GetPaymentDetails(dtoPaymentDetail filter);
    }
}
