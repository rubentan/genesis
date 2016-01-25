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
    public class BLProduct
    {
        IProduct repo;

        public BLProduct()
        {
            repo = new RepoProduct();
        }

        public IEnumerable<dtoProduct> GetAllProducts(object filter = null, int? skip = null, int? take = null)
        {
            return repo.GetAll(filter, skip, take);
        }



        public IEnumerable<dtoProduct> GetAllProducts(string search, object filter = null, int? skip = null, int? take = null)
        {
            return repo.GetAll(search);
        }
        public IEnumerable<dtoProduct> GetAllBranchProducts(string search, int branchId, object filter = null, int? skip = null, int? take = null)
        {
            return repo.GetAll(search,branchId);
        }

        public int GetRecordCount(object filter = null)
        {
            return repo.GetRecordCount(filter);
        }

        public List<dtoProduct> GetBranchProducts(int page, int recordPerPage, object filter,bool isExport)
        {
            return repo.GetBranchProducts(page, recordPerPage, filter,isExport);
        }

        public List<dtoProductTransactions> GetProductTransactions(int id)
        {
            return repo.GetProductTransactions(id);
        }

        public List<dtoProductTransactions> GetProductSales(int id)
        {
            return repo.GetProductSales(id);
        }

        public List<dtoProductTransactions> GetProductPurchases(int id)
        {
            return repo.GetProductPurchases(id);
        }

        public List<dtoProductPriceHistory> GetProductPriceHistory(int id)
        {
            return repo.GetProductPriceHistory(id);
        }

        public dtoProduct GetProductInfo(int id)
        {
            return repo.GetProductInfo(id);
        }


        public dtoResult Save(dtoProduct objProduct)
        {
            dtoResult objResult = new dtoResult();

            try
            {
                if (objProduct == null)
                    throw new Exception("Please provide valid Product details.");

                objProduct.productCode = ("" + objProduct.productCode).Trim();

                if (objProduct.productCode == string.Empty)
                    throw new Exception("Please provide valid Product Code.");                

                if (objProduct.productId == 0)
                {
                    if (objProduct.createdBy < 1)
                        throw new Exception("Please provide valid User ID.");
                    objResult = repo.Insert(objProduct);
                }
                else
                {
                    if (objProduct.modifiedBy < 1)
                        throw new Exception("Please provide valid User ID.");
                    objResult = repo.Update(objProduct);
                }
            }
            catch (Exception ex)
            {
                objResult.isSuccessful = false;
                objResult.errorMsg = ex.Message;
            }

            return objResult;
        }

        public dtoProduct GetByID(int id)
        {
            dtoProduct objProduct = null;

            try
            {
                if (id < 1)
                    throw new Exception("Please provide valid Product ID.");

                objProduct = repo.GetByID(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objProduct;
        }

        public List<string> CheckProductCodeExists(string productCode)
        {
            return repo.CheckProductCodeExists(productCode);
        }

        public dtoResult Insert(dtoProduct product)
        {
            return repo.Insert(product);
        }

        public dtoResult Update(dtoProduct product)
        {
            return repo.Update(product);
        }

        public dtoResult InLineUpdate(dtoProduct product)
        {
            return repo.InLineUpdate(product);
        }
    }
}
