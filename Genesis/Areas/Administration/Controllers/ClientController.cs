using System;
using System.Linq;
using System.Web.Mvc;
using Genesis.BusinessLogic;
using Genesis.DTO;

namespace Genesis.Areas.Administration.Controllers
{
    public class ClientController : Controller
    {
        ClientAccount service;
        private ClientAccount serviceClientAccount;

        public ClientController()
        {
            service = new ClientAccount();
            serviceClientAccount = new ClientAccount();
        }

        public ActionResult Clients()
        {
            return View();
        }

        public ActionResult ViewClient(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public JsonResult GetClientInfo(string id)
        {
            //var client = new dtoClient();
            var client = service.GetClientInfo(id);

            return Json(client);
        }


        //not being used?
        public ActionResult GetAllClient(jQueryDataTableParamModel param)
        {

            int filterStatus = 0;
            //int totalRecords = 0;

            if (Request["status"] != null && Request["status"] != string.Empty)
                filterStatus = Convert.ToInt32(Request["status"]);

            var filter = new dtoClient
            {
                clientCode = Request["clientCode"],
                clientName = Request["clientName"],
                clientContactNumber = Request["clientContactNumber"],
                clientContactPerson = Request["clientContactPerson"],
                status = filterStatus
            };

            //var list = serviceClientAccount.GetAllSuppliers(filter, param.iDisplayStart, param.iDisplayLength);
            //totalRecords = serviceClientAccount.GetRecordCount(filter);

            //int count = list.Count();

            return Json(new
            {
                //sEcho = param.sEcho,
                //iTotalRecords = totalRecords,
                //iTotalDisplayRecords = totalRecords,
                //aaData = list
            }
            , JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllClients()
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = int.Parse(Request["page"]);
            var recordPerPage = int.Parse(Request["recordPerPage"]);
            var isExport = false;

            var filter = new dtoClient
            {
                clientCode=Request["clientCodeSearch"],
                clientName=Request["clientNameSearch"],
                clientContactNumber=Request["clientContactNumberSearch"],
                clientContactPerson=Request["clientContactPersonSearch"],
                branchId = currentUser.branchId
            };

           var list = service.GetAllClients(page, recordPerPage, filter,isExport);
           var ret = Json(list);
           return ret;
        }

        public JsonResult DeactivateClient(string id)
        {
            return Json('1');
        }

        public JsonResult ActivateClient(string id)
        {
            return Json('1');
        }
        public JsonResult CheckClientCodeExists(string clientCode)
        {
            if (String.IsNullOrWhiteSpace(Request["id"]) || Request["id"] == "0")
            {
                var result = service.CheckClientCodeExists(clientCode);

                if (!result.Any())
                    return Json(true, JsonRequestBehavior.AllowGet);

                return Json("Client Code already in use.", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddEditClient(dtoClient client)
        {
            dtoResult result;
            var currentUser = (dtoUserAccount)Session["CurrentUser"];

            if (client.clientId == 0)
            {
                client.branchId = currentUser.branchId;
                client.createdBy = currentUser.userId;
                client.dateCreated = DateTime.Now;
                result = service.Insert(client);
            }
            else
            {
                client.modifiedBy = currentUser.userId;
                client.dateLastModified = DateTime.Now;
                result = service.Update(client);
            }

            return Json(result);
        }

        public JsonResult GetClientSalesInvoices(string id)
        {
            var list = service.GetClientSalesInvoices(id);
            return Json(list);
        }

        public JsonResult GetClientSalesInvoicesWithBalance(string id)
        {
            var list = service.GetClientSalesInvoicesWithBalance(id);
            return Json(list);
        }

        public JsonResult GetSalesInvoiceDetails(string id)
        {
            var invoice = service.GetSalesInvoiceDetails(id);
            return Json(invoice);
        }

        public JsonResult GetClientPayments(string id)
        {
            var list = service.GetClientPayments(id);
            return Json(list);
        }

        [HttpPost]
        public JsonResult Save(dtoClient param)
        {
            serviceClientAccount.Save(param);
            return Json(new dtoResult());
        }
    }
}
