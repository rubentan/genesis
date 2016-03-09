using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Genesis.BusinessLogic;
using Genesis.Controllers;
using Genesis.DataAccess.Repositories;
using Genesis.DTO;
using OfficeOpenXml;

namespace Genesis.Areas.Modules.Controllers
{
    public class SalesController : BaseController
    {
        BLSales service;
        RepoReceivable repoReceivable;
        BLProduct serviceProduct;
        RepoClientAccount repoClient;

        public SalesController()
        {
            service = new BLSales();
            repoReceivable = new RepoReceivable();
            serviceProduct = new BLProduct();
            repoClient = new RepoClientAccount();
        }

    #region Branch Sales
        public ActionResult BranchSales()
        {
            return View();
        }

        public ActionResult BranchRefunds()
        {
            return View();
        }

        public ActionResult AddBranchSalesInvoice()
        {
            return View();
        }

        public ActionResult EditBranchSalesInvoice(int? id)
        {
            ViewBag.id = id;
            return View();
        }

        public JsonResult GetClientsForDropDown2(string search)
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var results = from result in repoClient.GetClientsForDropDown2(search, currentUser.branchId)
                          select new
                          {
                              id = result.id,
                              text = result.text,
                          };

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckExistingDocument(dtoDocument header, List<dtoTransaction> details)
        {
            var retVal = false;
            var documentNumber = header.documentNumber;
            var documentDate = header.transactionDate;
            retVal = service.CheckExistingDocument(documentNumber, documentDate);

            return Json(retVal);
        }


        [HttpPost]
        public JsonResult GetClientsForDropDown()
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var list = repoClient.GetClientsFroDropDown(currentUser.branchId);
            return Json(list);
        }

        public JsonResult GetAllBranchProducts(string search)
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var results = from result in serviceProduct.GetAllBranchProducts(search, currentUser.branchId)
                          select new
                          {
                              id = result.productId,
                              text = result.productDescription,
                          };

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProduct(int productId)
        {
            return Json(serviceProduct.GetByID(productId));
        }

        public JsonResult GetSaleById(int documentId)
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = 1;
            var recordPerPage = 1;
            var isExport = false;
            //int totalRecords = 0;

            var filter = new dtoDocument
            {
                documentId = documentId,
                dateFrom = "",
                dateTo = "",
                documentNumber = "",
                clientName = "",
                clientCode = "",
                branchId = currentUser.branchId
            };

            //list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);
            var list = service.GetAllSales2(page, recordPerPage, filter, isExport);
            //int count = list.Count();

            return Json(list);
            }

        [HttpPost]
        public JsonResult GetAllSalesItems(int documentId)
        {
            var list = service.GetAllSaleItems(documentId);
            return Json(list);
        }


        [HttpPost]
        public JsonResult GetAllSales()
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = int.Parse(Request["page"]);
            var recordPerPage = int.Parse(Request["recordPerPage"]);
            var isExport = false;

            //int totalRecords = 0;

            var filter = new dtoDocument
            {
                documentNumber = Request["documentNumber"],
                clientCode = Request["clientCode"],
                clientName = Request["clientName"],
                dateFrom = Request["dateFrom"] + " 00:00",
                dateTo = Request["dateTo"] + " 23:59",
                branchId = currentUser.branchId,
                //documentType = 1
            };

            //list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);
            var list = service.GetAllSales2(page, recordPerPage, filter, isExport);
            //int count = list.Count();

            return Json(list);
        }

        public void ExportAllSales()
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = 0;
            var recordPerPage = 0;
            var isExport = true;

            var filter = new dtoDocument
            {
                documentNumber = Request.QueryString["documentNumber"],
                clientCode = Request.QueryString["clientCode"],
                clientName = Request.QueryString["clientName"],
                dateFrom = Request.QueryString["dateFrom"] + " 00:00",
                dateTo = Request.QueryString["dateTo"] + " 23:59",
                branchId = currentUser.branchId,
                //documentType = 1
            };

            //list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);
            var list = service.GetAllSales2(page, recordPerPage, filter, isExport);
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
                Response.AddHeader("content-disposition", "attachment;  filename=BranchSales.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
            }

        }

        //Not going to be used
        public JsonResult GetSalesByFilter(dtoDocument filterParam)
        {
            //int totalRecords = 0;

            var filter = new dtoDocument
            {
                documentNumber = filterParam.documentNumber, 
                supplierCode = filterParam.supplierCode,
                supplierName = filterParam.supplierName,
                dateFrom = filterParam.dateFrom, 
                dateTo = filterParam.dateTo,
                documentType = 1
            };

            var list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);

            //int count = list.Count();

            return Json(list);
        }

        public JsonResult SaveInvoiceTransaction(dtoDocument header, List<dtoTransaction> details)
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            header.branchId = currentUser.branchId;
            //header.createdBy = currentUser.userId;

            if (header.documentId == 0)
            {
                header.createdBy = currentUser.userId;
                header.dateCreated = DateTime.Now;
            }

            var result = service.SaveInvoiceTransaction(header, details);
            //repoReceivable.SaveTransaction(header, details);
            return Json(result);
        }

    #endregion


    #region Branch Receivables (New)

        [HttpPost]
        public JsonResult GetAllReceivables()
        {

            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = int.Parse(Request["page"]);
            var recordPerPage = int.Parse(Request["recordPerPage"]);
            var isExport = false;

            //int totalRecords = 0;


            var filter = new dtoReceivable
            {
                referenceNumber = Request["referenceNumber"],
                clientCode = Request["clientCode"],
                clientName = Request["clientName"],
                dateFrom = Request["dateFrom"] + " 00:00",
                dateTo = Request["dateTo"] + " 23:59",
                branchId = currentUser.branchId,
                //documentType = 1
            };

            //list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);
            var list = repoReceivable.GetAllReceivable2(page, recordPerPage, filter, isExport);
            //int count = list.Count();

            return Json(list);

        }

        [HttpPost]
        public JsonResult GetAllReceivableItems(int receivableId)
        {
            var list = service.GetAllReceivableItems(receivableId);
            return Json(list);
        }

        public void ExportAllReceivables()
        {

            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = 0;
            var recordPerPage = 0;
            var isExport = true;

            //int totalRecords = 0;


            var filter = new dtoReceivable
            {
                referenceNumber = Request.QueryString["referenceNumber"],
                clientCode = Request.QueryString["clientCode"],
                clientName = Request.QueryString["clientName"],
                dateFrom = Request.QueryString["dateFrom"] + " 00:00",
                dateTo = Request.QueryString["dateTo"] + " 23:59",
                branchId = currentUser.branchId,
                //documentType = 1
            };

            //list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);
            var list = repoReceivable.GetAllReceivable2(page, recordPerPage, filter, isExport);
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
                Response.AddHeader("content-disposition", "attachment;  filename=BranchReceivables.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
            }

        }
        
        public ActionResult BranchReceivables()
        {
            return View();
        }

        public ActionResult NewBranchReceivable2()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetAllNotYetReceive(int referenceId)
        {            
            return Json(repoReceivable.GetReceivableOrderByClient(referenceId));
        }

        [HttpPost]
        public void SaveTransaction(dtoReceivable header, List<dtoReceivableDetail> details)
        {
            repoReceivable.SaveTransaction(header, details);
        }

        [HttpPost]
        public JsonResult SaveReceivableTransaction(dtoReceivable header, List<dtoReceivableDetail> details)
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            header.branchId = currentUser.branchId;

            if (header.isNew)
            {
                header.createdBy = currentUser.userId;
                header.dateCreated = DateTime.Now;
            }

            var result = service.SaveReceivableTransaction(header, details);
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetNewBranchRecievable(int id)
        {
            return Json(new { 
                header= repoReceivable.GetReceivable(id),
                details = repoReceivable.GetReceivableDetails(id)
            });
        }

        [HttpPost]
        public JsonResult GetExistingPayments(int clientId)
        {
            var list = repoReceivable.GetExistingPayments(clientId);
            return Json(list);
        }

        public JsonResult GetExistingPaymentDetail(int paymentId)
        {
            var receivable = repoReceivable.GetExistingPaymentDetail(paymentId);
            return Json(receivable);
        }

    #endregion


    #region Branch Receivables (Old)

        public ActionResult NewBranchReceivable()
        {
            return View();
        }

        public ActionResult EditBranchReceivable(int? id)
        {
            ViewBag.id = id;
            return View();
        }

    #endregion

        #region Shared
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
