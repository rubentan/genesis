using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Genesis.BusinessLogic;
using Genesis.DataAccess.Repositories;
using Genesis.DTO;

namespace Genesis.Areas.Modules.Controllers
{
    public class SalesController : Controller
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
            var page = int.Parse(Request["page"]);
            var recordPerPage = int.Parse(Request["recordPerPage"]);
            var isExport = false;
            //int totalRecords = 0;

            var filter = new dtoDocument
            {
                documentId = documentId,
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

        //Receivables
        public ActionResult BranchReceivables()
        {
            return View();
        }

        public ActionResult EditBranchReceivable(int? id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public JsonResult GetAllReceivables()
        {

            var currentUser = (dtoUserAccount)Session["CurrentUser"];

            //int totalRecords = 0;


            var filter = new dtoReceivable
            {
                referenceNumber = Request["referenceNumber"],
                clientCode = Request["clientCode"],
                clientName = Request["clientName"],
                //dateFrom = Request["dateFrom"],
                //dateTo = Request["dateTo"],
                branchId = currentUser.branchId,
                //documentType = 1
            };

            //list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);
            var list = repoReceivable.GetAllReceivable2(filter,0,20);
            //int count = list.Count();
            
            return Json(list);

        }

        public ActionResult NewBranchReceivable()
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

        
    }
}
