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
    public class SupplierAccount
    {
        ISupplierAccount repo;

        public SupplierAccount()
        {
            repo = new RepoSupplierAccount();
        }

        public List<dtoSupplier> GetAll(string search)
        {
            return repo.GetAll(search);
        }

        public List<dtoSupplier> GetAllUsers(string search, object filter = null)
        {
            return repo.GetAll(search, filter);
        }

        public List<dtoSupplier> GetAllSuppliers(int page, int recordPerPage, object filter,bool isExport)
        {
            return repo.GetAll(page, recordPerPage, filter,isExport);
        }

        public List<dtoSupplier> GetAllSuppliers(string search, object filter = null, int? skip = null, int? take = null)
        {
            return repo.GetAll(search, filter, skip, take);
        }

        public int GetRecordCount(object filter = null)
        {
            return repo.GetRecordCount(filter);
        }

        public dtoResult Save(dtoSupplier objSupplier)
        {
            dtoResult objResult = new dtoResult();
            dtoSupplier objValidation;

            try
            {
                if (objSupplier == null)
                    throw new Exception("Please provide valid Supplier details.");

                objSupplier.supplierCode = ("" + objSupplier.supplierCode).Trim();
                objSupplier.supplierName = ("" + objSupplier.supplierName).Trim();

                if (objSupplier.supplierCode == string.Empty)
                    throw new Exception("Please provide valid Supplier Code.");
                if (objSupplier.supplierName == string.Empty)
                    throw new Exception("Please provide valid Supplier Name.");                                

                if (objSupplier.supplierId == 0)
                {
                    if (objSupplier.createdBy < 1)
                        throw new Exception("Please provide valid User ID.");

                    objValidation = repo.GetByCode(objSupplier.supplierCode);
                    if (objValidation != null)
                        throw new Exception("Cannot be saved because Supplier Code already exists.");

                    objValidation = null;
                    objValidation = repo.GetByName(objSupplier.supplierName);
                    if (objValidation != null)
                        throw new Exception("Cannot be saved because Suppier Name already exists.");

                    objResult = repo.Insert(objSupplier);
                }
                else
                {
                    if (objSupplier.modifiedBy < 1)
                        throw new Exception("Please provide valid User ID.");

                    objResult = repo.Update(objSupplier);
                }
            }
            catch (Exception ex)
            {
                objResult.isSuccessful = false;
                objResult.errorMsg = ex.Message;
            }

            objValidation = null;

            return objResult;
        }

        public dtoSupplier GetByID(int id)
        {
            dtoSupplier objSupplier = null;

            try
            {
                if (id < 1)
                    throw new Exception("Please provide valid Supplier ID.");

                objSupplier = repo.GetByID(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objSupplier;
        }

        public dtoSupplier GetSupplierInfo(string id)
        {
            return repo.GetSupplierInfo(id);
        }

        public List<dtoSupplierPurchaseOrder> GetSupplierPurchaseOrders(string id)
        {
            return repo.GetSupplierPurchaseOrders(id);
        }

        public List<dtoSupplierPayment> GetSupplierPayments(string id)
        {
            return repo.GetSupplierPayments(id);
        }

        public List<string> CheckSupplierCodeExists(string supplierCode)
        {
            return repo.CheckSupplierCodeExists(supplierCode);
        }

        public dtoResult Insert(dtoSupplier supplier)
        {
            return repo.Insert(supplier);
        }

        public dtoResult Update(dtoSupplier supplier)
        {
            return repo.Update(supplier);
        }

        public List<dtoSupplierPurchaseOrder> GetSupplierPurchaseOrdersWithBalance(string id)
        {
            return repo.GetSupplierPurchaseOrdersWithBalance(id);
        }

        public dtoSupplierPurchaseOrder GetPurchaseOrderDetails(string id)
        {
            return repo.GetPurchaseOrderDetails(id);
        }
    }
}
