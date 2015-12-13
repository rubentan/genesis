using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Genesis.BusinessLogic;
using Genesis.DTO;

namespace Genesis.Areas.Administration.Controllers
{
    public class SupplierController : Controller
    {
        private SupplierAccount serviceSupplier;

        public SupplierController()
        {
            serviceSupplier = new SupplierAccount();
        }

        public ActionResult Suppliers()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Save(dtoSupplier objSupplier)
        {
            /* Live */
            //objUserAccount = (dtoUserAccount)Session["CurrentUser"];
            //if (objSupplier.supplierId < 1)
            //    objSupplier.createdBy = objUserAccount.userId;
            //else if (objSupplier.supplierId > 0)
            //    objSupplier.modifiedBy = objUserAccount.userId;

            /* Test */
            if(objSupplier.supplierId < 1)
                objSupplier.createdBy = 1;
            else if (objSupplier.supplierId > 0)
                objSupplier.modifiedBy = 2;
            
            return Json(serviceSupplier.Save(objSupplier));
        }

        [HttpPost]
        public JsonResult GetSupplierByID(int supplierId)
        {
            return Json(serviceSupplier.GetByID(supplierId));
        }

        public JsonResult GetAllSuppliers()
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = int.Parse(Request["page"]);
            var recordPerPage = int.Parse(Request["recordPerPage"]);
            var isExport = false;
            

            var filter = new dtoSupplier
            {
                supplierCode = Request["supplierCode"],
                supplierName = Request["supplierName"],
                supplierContactNumber = Request["supplierContactNumber"],
                supplierContactPerson = Request["supplierContactPerson"],
                branchId = currentUser.branchId
            };

            var list = serviceSupplier.GetAllSuppliers(page, recordPerPage, filter, isExport);
            var ret = Json(list);
            return ret;
        }

        public ActionResult ViewSupplier(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public JsonResult CheckSupplierCodeExists(string supplierCode)
        {
            if (String.IsNullOrWhiteSpace(Request["id"]) || Request["id"] == "0")
            {
                var result = serviceSupplier.CheckSupplierCodeExists(supplierCode);

                if (!result.Any())
                    return Json(true, JsonRequestBehavior.AllowGet);

                return Json("Supplier Code already in use.", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddEditSupplier(dtoSupplier supplier)
        {
            dtoResult result;
            var currentUser = (dtoUserAccount)Session["CurrentUser"];

            if (supplier.supplierId == 0)
            {
                supplier.branchId = currentUser.branchId;
                supplier.createdBy = currentUser.userId;
                supplier.dateCreated = DateTime.Now;
                result = serviceSupplier.Insert(supplier);
            }
            else
            {
                supplier.modifiedBy = currentUser.userId;
                supplier.dateLastModified = DateTime.Now;
                result = serviceSupplier.Update(supplier);
            }

            return Json(result);
        }

#region View Supplier
        
        public JsonResult GetSupplierInfo(string id)
        {
            //var client = new dtoClient();
            var supplier = serviceSupplier.GetSupplierInfo(id);

            return Json(supplier);
        }

        public JsonResult GetSupplierPurchaseOrders(string id)
        {
            var list = serviceSupplier.GetSupplierPurchaseOrders(id);
            return Json(list);
        }

        public JsonResult GetSupplierPayments(string id)
        {
            var list = serviceSupplier.GetSupplierPayments(id);
            return Json(list);
        }

#endregion

        public JsonResult GetSupplierPurchaseOrdersWithBalance(string id)
        {
            var list = serviceSupplier.GetSupplierPurchaseOrdersWithBalance(id);
            return Json(list);
        }

        public JsonResult GetPurchaseOrderDetails(string id)
        {
            var invoice = serviceSupplier.GetPurchaseOrderDetails(id);
            return Json(invoice);
        }

    }
}
