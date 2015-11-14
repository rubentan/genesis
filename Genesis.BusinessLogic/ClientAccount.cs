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
    public class ClientAccount
    {
        private IClientAccount repo;

        public ClientAccount()
        {
            repo = new RepoClientAccount();
        }

        public List<dtoClient> GetAllUsers(string search, object filter = null)
        {
            return repo.GetAll(search, filter);
        }

        //public List<dtoClient> GetAllSuppliers(object filter = null, int? skip = null, int? take = null)
        //{
         //   return repo.GetAll(filter, skip, take);
        //}

        public List<dtoClient> GetAllClients(int page, int recordPerPage,object filter,bool isExport)
        {
            return repo.GetAll(page, recordPerPage, filter,isExport);
        }

        public dtoClient GetClientInfo(string id)
        {
            return repo.GetClientInfo(id);
        }

        public List<dtoClientSalesInvoice> GetClientSalesInvoices(string id)
        {
            return repo.GetClientSalesInvoices(id);
        }

        public List<dtoClientSalesInvoice> GetClientSalesInvoicesWithBalance(string id)
        {
            return repo.GetClientSalesInvoicesWithBalance(id);
        }

        public List<dtoClientPayment> GetClientPayments(string id)
        {
            return repo.GetClientPayments(id);
        }
        

        public int GetRecordCount(object filter = null)
        {
            return repo.GetRecordCount(filter);
        }

        public dtoResult Save(dtoClient client)
        {
            if (client.clientId == 0)
            {
                repo.Insert(client);
            }
            else
            {
                repo.Update(client);
            }

            return new dtoResult { };
        }

        //public List<dtoClient> GetAllClients(dtoClient filter, int x, int y)
        //{
        //    return new List<dtoClient>();
        //}

        public List<string> CheckClientCodeExists(string clientCode)
        {
            return repo.CheckClientCodeExists(clientCode);
        }

        public dtoResult Insert(dtoClient client)
        {
            return repo.Insert(client);
        }

        public dtoResult Update(dtoClient supplier)
        {
            return repo.Update(supplier);
        }


        public dtoClientSalesInvoice GetSalesInvoiceDetails(string id)
        {
            return repo.GetSalesInvoiceDetails(id);
        }
    }
}
