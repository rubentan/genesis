﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;

namespace Genesis.DataAccess.Interfaces
{
    public interface ISales : IBase<dtoDocument>
    {
        IEnumerable<dtoDocument> GetAll(string search, object filter = null, int? skip = null, int? take = null);
        IEnumerable<dtoDocument> GetAll(object filter = null, int? skip = null, int? take = null);

        List<dtoDocument> GetAll2(object filter = null, int? skip = null, int? take = null);
        int GetRecordCount(object filter = null);

        dtoResult SaveInvoiceTransaction(dtoDocument header, List<dtoTransaction> details);

        List<dtoTransaction> GetAllSaleItems(int documentId);

        List<dtoReceivable> GetAllReceivables(object filter = null, int? skip = null, int? take = null);
    }
}