using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Genesis.BusinessLogic;
using Genesis.DataAccess.Interfaces;
using Genesis.DataAccess.Repositories;
using Genesis.DTO;

namespace Genesis.Areas.Modules.Controllers
{
    public class PurchaseController : Controller
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


        public ActionResult BranchPurchases()
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

            //int totalRecords = 0;


            var filter = new dtoDocument
            {
                documentId = documentId,
                branchId = currentUser.branchId

            };

            //list = (new BLPurchase()).GetAllPurchases(filter, 0, 100);
            //totalRecords = service.GetRecordCount(filter);
            var list = service.GetAllPurchases2(filter, 0, 20);
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
            var currentUser = (dtoUserAccount) Session["CurrentUser"];
            
            //int totalRecords = 0;

     
            var filter = new dtoDocument
            {
                documentNumber = Request["documentNumber"],
                supplierCode = Request["supplierCode"],
                supplierName = Request["supplierName"],
                //dateFrom = Request["dateFrom"] ,
                //dateTo = Request["dateTo"],
                branchId = currentUser.branchId,
                documentType = 2
            };

            var list = service.GetAllPurchases2(filter, 0, 100);

            return Json(list);
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

        public JsonResult GetSupplierForDropDown()
        {
            var list = repoSupplier.GetSuppliersFroDropDown();
            return Json(list);
        }

        #region Payment

        public ActionResult BranchPayables()
        {
            return View();
        }

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
                DateFrom = String.IsNullOrWhiteSpace(Request["dateFrom"]) ? (DateTime?)null : Convert.ToDateTime(Request["dateFrom"]),
                DateTo = String.IsNullOrWhiteSpace(Request["dateTo"]) ? (DateTime?)null : Convert.ToDateTime(Request["dateTo"])
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
        public JsonResult SavePaymentTransaction(dtoPayment Header, List<dtoPaymentDetail> Details)
        {
            var respose = repPayment.SavePaymentTransaction(Header, Details);

            return Json(respose, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentDetails(dtoPaymentDetail filter)
        {
            var details = repPayment.GetPaymentDetails(filter);
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
;