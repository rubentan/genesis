using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Genesis.BusinessLogic;
using Genesis.Controllers;
using Genesis.DTO;
using OfficeOpenXml;

namespace Genesis.Areas.Administration.Controllers
{
    public class ProductController : BaseController // BaseController // 
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
            var page = int.Parse(Request["page"]);
            var recordPerPage = int.Parse(Request["recordPerPage"]);
            var isExport = false;

            var filter = new dtoProduct
            {
                productCode = Request["productCodeSearch"],
                productDescription = Request["productDescriptionSearch"],
                //categoryId = int.Parse(Request["categoryId"]),
                branchId = currentUser.branchId
            };

           var list = serviceProduct.GetBranchProducts(page, recordPerPage, filter,isExport);
           var ret = Json(list);
           return ret;
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

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public void ExportBranchProducts(string productCode, string productDesc)
        {
            var currentUser = (dtoUserAccount) Session["CurrentUser"];
            var page = 0;
            var recordPerPage = 0;
            var isExport = true;

            var filter = new dtoProduct
            {
                productCode = productCode,
                productDescription = productDesc,
                branchId = currentUser.branchId
            };

            var list = serviceProduct.GetBranchProducts(page, recordPerPage, filter, isExport);

            DataTable dt = new DataTable();
            dt = ToDataTable(list);


            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Data");

                //Load the datatable into the sheet, starting from cell A1. 
                //Print the column names on row 1
                ws.Cells["A1"].LoadFromDataTable(dt, true);

                //Write it back to the client
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=ProductDetails.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
            }


        }

        [HttpPost]
        public JsonResult InLineUpdate(dtoProduct product)
        {
            dtoResult result;
            var currentUser = (dtoUserAccount)Session["CurrentUser"];

                product.modifiedBy = currentUser.userId;
                result = serviceProduct.InLineUpdate(product);

            return Json(result);
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

        [HttpPost]
        public JsonResult SoftDeleteProduct(dtoProduct product)
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];

            //product.productId = product.productId;
            product.modifiedBy = currentUser.userId;
            product.dateLastModified = DateTime.Now;
            var result = serviceProduct.SoftDeleteProduct(product);

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
