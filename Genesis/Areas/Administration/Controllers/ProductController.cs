using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Genesis.BusinessLogic;
using Genesis.DTO;

namespace Genesis.Areas.Administration.Controllers
{
    public class ProductController : Controller // BaseController // 
    {
        private BLProduct serviceProduct;
        private BLProductCategory serviceProductCategory;

        public ProductController()
        {
            serviceProduct = new BLProduct();
            serviceProductCategory = new BLProductCategory();
        }


        public ActionResult Products()
        {
            return View();
        }

        public JsonResult GetAllBranchProducts()
        {
            var currentUser = (dtoUserAccount) Session["CurrentUser"];
            var filter = new dtoProduct
            {
                productCode = Request["productCodeSearch"],
                productDescription = Request["productDescriptionSearch"],
                //categoryId = int.Parse(Request["categoryId"]),
                branchId = currentUser.branchId
            };

            var list = serviceProduct.GetBranchProducts(filter, 0, 20);

           

            return Json(list);
            
        }

        public JsonResult CheckProductCodeExists(string productCode)
        {
            if (String.IsNullOrWhiteSpace(Request["id"]) || Request["id"] == "0")
            {
                var result = serviceProduct.CheckProductCodeExists(productCode);

                if (!result.Any())
                    return Json(true, JsonRequestBehavior.AllowGet);

                return Json("Product Code already in use.", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddEditProduct(dtoProduct product)
        {
            dtoResult result;
            var currentUser = (dtoUserAccount)Session["CurrentUser"];

            if (product.productId == 0)
            {
                product.branchId = currentUser.branchId;
                product.createdBy = currentUser.userId;
                product.dateCreated = DateTime.Now;
                result = serviceProduct.Insert(product);
            }
            else
            {
                product.modifiedBy = currentUser.userId;
                product.dateLastModified = DateTime.Now;
                result = serviceProduct.Update(product);
            }

            return Json(result);
        }

        public ActionResult BranchProducts()
        {
            return View();
        }

        public ActionResult BranchProductsV2()
        {
            return View();
        }


        public ActionResult ViewProductdetails(int id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public JsonResult Save(dtoProduct objProduct)
        {
            /* Live */
            //objUserAccount = (dtoUserAccount)Session["CurrentUser"];
            //if (objProduct.supplierId < 1)
            //    objProduct.createdBy = objUserAccount.userId;
            //else if (objProduct.supplierId > 0)
            //    objProduct.modifiedBy = objUserAccount.userId;
            //objProduct.branchId = objUserAccount.branchId;

            /* Test */
            if (objProduct.productId < 1)
                objProduct.createdBy = 1;
            else if (objProduct.productId > 0)
                objProduct.modifiedBy = 2;
            objProduct.branchId = 1;

            return Json(serviceProduct.Save(objProduct));
        }

        [HttpPost]
        public JsonResult GetProductByID(int productId)
        {
            return Json(serviceProduct.GetByID(productId));
        }

        [HttpGet]
        public JsonResult GetAllProductCategories(string search)
        {
            var results = from result in serviceProductCategory.GetAll(search)
                          select new
                          {
                            id = result.categoryId,
                            text = result.categoryName + " - " + result.categoryCode
                          };
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductInfo(int id)
        {
            //var client = new dtoClient();
            var product = serviceProduct.GetProductInfo(id);
            return Json(product);
        }


        public JsonResult GetProductTransactions(int id)
        {
            var list = serviceProduct.GetProductTransactions(id);
            return Json(list);
        }

        public JsonResult GetProductSales(int id)
        {
            var list = serviceProduct.GetProductSales(id);
            return Json(list);
        }

        public JsonResult GetProductPurchases(int id)
        {
            var list = serviceProduct.GetProductPurchases(id);
            return Json(list);
        }


        public JsonResult GetProductPriceHistory(int id)
        {
            var list = serviceProduct.GetProductPriceHistory(id);
            return Json(list);
        }

    }
}
