using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Genesis.BusinessLogic;
using Genesis.Controllers;
using Genesis.DataAccess.Interfaces;
using Genesis.DataAccess.Repositories;
using Genesis.DTO;
using OfficeOpenXml;

namespace Genesis.Areas.Modules.Controllers
{
    public class PurchaseController : BaseController
    {
        BLPurchase service;
        SupplierAccount serviceSupplier;
        BLProduct serviceProduct;

        IPurchase repo;
        IPayment repPayment;
        RepoSupplierAccount repoSupplier;
       
        
        public PurchaseController()
        {
            service = new BLPurchase();
            serviceSupplier = new SupplierAccount();
            serviceProduct = new BLProduct();

            repo = new RepoPurchase();
            repPayment = new RepoPayment();
            repoSupplier = new RepoSupplierAccount();
        }

        #region Branch Purchases
        public ActionResult BranchPurchases()
        {
            return View();
        }

        public ActionResult BranchReturns()
        {
            return View();
        }

        public ActionResult AddBranchPurchaseOrder()
        {
            return View();
        }

        public ActionResult EditBranchPurchaseOrder(int id)
        {
            ViewBag.id = id;
            return View();
        }

        public JsonResult GetPurchaseById(int documentId)
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = 1;
            var recordPerPage = 1;
            var isExport = false;

            //int totalRecords = 0;


            var filter = new dtoDocument
            {
                documentId = documentId,
                documentNumber = "",
                supplierCode = "",
                supplierName = "",
                dateFrom = "",
                dateTo = "",
                branchId = currentUser.branchId

            };

            //list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);
            var list = service.GetAllPurchases2(page, recordPerPage, filter, isExport);
            //int count = list.Count();

            return Json(list);
        }

        [HttpPost]
        public JsonResult GetAllPurchaseItems(int documentId)
        {
            var list = service.GetAllPurchaseItems(documentId);
            return Json(list);
        }


        [HttpPost]
        public JsonResult GetAllPurchases()
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = int.Parse(Request.QueryString["page"]);
            var recordPerPage = int.Parse(Request.QueryString["recordPerPage"]);
            var isExport = Request.QueryString["export"];
            var exportBool = isExport != null;

            var filter = new dtoDocument
            {
                documentNumber = Request.QueryString["documentNumber"],
                supplierCode = Request.QueryString["supplierCode"],
                supplierName = Request.QueryString["supplierName"],
                dateFrom = Request.QueryString["dateFrom"] + " 00:00",
                dateTo = Request.QueryString["dateTo"] + " 23:59",
                branchId = currentUser.branchId,
                documentType = 2
            };

            var list = service.GetAllPurchases2(page, recordPerPage, filter, exportBool);

            return Json(list);
        }

        
        public void ExportAllPurchases()
        {
            var currentUser = (dtoUserAccount) Session["CurrentUser"];
            var page = 0;
            var recordPerPage = 0;
            var isExport = Request.QueryString["export"];
            var exportBool = isExport != null;

            var filter = new dtoDocument
            {
                documentNumber = Request.QueryString["documentNumber"],
                supplierCode = Request.QueryString["supplierCode"],
                supplierName = Request.QueryString["supplierName"],
                dateFrom = Request.QueryString["dateFrom"] + " 00:00",
                dateTo = Request.QueryString["dateTo"] + " 23:59",
                branchId = currentUser.branchId,
                documentType = 2
            };

            var list = service.GetAllPurchases2(page, recordPerPage, filter, exportBool);

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
                Response.AddHeader("content-disposition", "attachment;  filename=BranchPurchases.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
            }
        }

        [HttpGet]
        public JsonResult GetAllSupplier(string search)
        {
        

            var results = from result in  serviceSupplier.GetAllSuppliers(search, null, 0, 10)
                          select new
                          {
                              id = result.supplierId,
                              text = result.supplierName
                          };
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetALLProducts(string search)
        {
            var _result = from result in serviceProduct.GetAllProducts(search)
                          select new
                          {
                              id = result.productId,
                              text = result.productDescription
                          };

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProduct(int productId)
        {
            
            return Json( serviceProduct.GetByID(productId));
        }


        public JsonResult SavePurchaseTransaction(dtoDocument header, List<dtoTransaction> details)
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            header.branchId = currentUser.branchId;
            //header.createdBy = currentUser.userId;

            if (header.documentId == 0)
            {
                header.createdBy = currentUser.userId;
                header.dateCreated = DateTime.Now;
            }

            var result = service.SavePurchaseTransaction(header, details);
            
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetAllOrderItems2(int documentId)
        {
            return Json(service.GetAllOrderItems(documentId.ToString(), null, 0, 100));
        }

        [HttpPost]
        public JsonResult GetDocument(int documentId)
        {

            return Json(repo.GetDocument(documentId));
        }

        [HttpPost]
        public void SaveTransaction(dtoDocument document, List<dtoTransaction> products)
        {
            service.SaveTransaction(document, products);
       
        }

        #endregion

        #region Branch Payables (New)

        [HttpPost]
        public JsonResult GetAllPayments2()
        {

            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = int.Parse(Request["page"]);
            var recordPerPage = int.Parse(Request["recordPerPage"]);
            var isExport = false;

            //int totalRecords = 0;


            var filter = new dtoPayment
            {
                referenceNumber = Request["referenceNumber"],
                supplierCode = Request["supplierCode"],
                supplierName = Request["supplierName"],
                dateFrom = Request["dateFrom"] + " 00:00",
                dateTo = Request["dateTo"] + " 23:59",
                branchId = currentUser.branchId,
                //documentType = 1
            };

            //list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);
            var list = repPayment.GetAllPayments(page, recordPerPage, filter, isExport);
            //int count = list.Count();

            return Json(list);

        }

        public void ExportAllPayments()
        {

            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = 0;
            var recordPerPage = 0;
            var isExport = true;

            //int totalRecords = 0;


            var filter = new dtoPayment
            {
                referenceNumber = Request.QueryString["referenceNumber"],
                supplierCode = Request.QueryString["supplierCode"],
                supplierName = Request.QueryString["supplierName"],
                dateFrom = Request.QueryString["dateFrom"] + " 00:00",
                dateTo = Request.QueryString["dateTo"] + " 23:59",
                branchId = currentUser.branchId,
                //documentType = 1
            };

            //list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);
            var list = repPayment.GetAllPayments(page, recordPerPage, filter, isExport);
            //int count = list.Count();

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
                Response.AddHeader("content-disposition", "attachment;  filename=BranchPayables.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
            }

        }


        public ActionResult NewBranchPayment2()
        {
            return View();
        }

        public ActionResult BranchPayables()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetExistingPayments(int clientId)
        {
            var list = repPayment.GetExistingPayments(clientId);
            return Json(list);
        }

        public JsonResult GetExistingPaymentDetail(int paymentId)
        {
            var receivable = repPayment.GetExistingPaymentDetail(paymentId);
            return Json(receivable);
        }

        [HttpPost]
        public JsonResult SavePaymentTransaction2(dtoPayment header, List<dtoPaymentDetail> details)
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            header.branchId = currentUser.branchId;

            if (header.isNew)
            {
                header.createdBy = currentUser.userId;
                header.dateCreated = DateTime.Now;
            }

            var result = repPayment.SavePaymentTransaction2(header, details);
            return Json(result);
        }

        #endregion


        #region Branch Payables (Old)
        

        public ActionResult NewBranchPayment()
        {
            return View();
        }

        public JsonResult GetAllPayments()
        {
            
            var filter = new dtoPayment
            {
                referenceNumber = String.IsNullOrWhiteSpace(Request["documentNumber"]) ? null : Request["documentNumber"],
                supplierCode = String.IsNullOrWhiteSpace(Request["supplierCode"]) ? null : Request["supplierCode"],
                supplierName = String.IsNullOrWhiteSpace(Request["supplierName"]) ? null : Request["supplierName"],
                //DateFrom = String.IsNullOrWhiteSpace(Request["dateFrom"]) ? (DateTime?)null : Convert.ToDateTime(Request["dateFrom"]),
                //DateTo = String.IsNullOrWhiteSpace(Request["dateTo"]) ? (DateTime?)null : Convert.ToDateTime(Request["dateTo"])
            };

            var payments = repPayment.GetPaymentsByFilters(filter);

            return Json(payments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentsByFilter(dtoPayment filter)
        {
            var payments = repPayment.GetPaymentsByFilters(filter);
            return Json(payments, JsonRequestBehavior.AllowGet);
        }

        


        public JsonResult GetPurchaseOrdersByFilter(dtoDocument filter)
        {
            var list = new List<dtoDocument>();
            try
            {
               list = repo.GetPurchaseOrderByFilter(filter);
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavePaymentTransaction(dtoPayment header, List<dtoPaymentDetail> details)
        {
            var respose = repPayment.SavePaymentTransaction(header, details);

            return Json(respose, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentDetails(dtoPaymentDetail filter)
        {
            var details = repPayment.GetPaymentDetails(filter);
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region shared
        public JsonResult GetSupplierForDropDown(string search)
        {
            //var list = repoSupplier.GetSuppliersFroDropDown();
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var results = from result in repoSupplier.GetSuppliersFroDropDown(search, currentUser.branchId)
                          select new
                          {
                              id = result.id,
                              text = result.text,
                          };

            return Json(results, JsonRequestBehavior.AllowGet);
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


        #endregion

    }
}
;