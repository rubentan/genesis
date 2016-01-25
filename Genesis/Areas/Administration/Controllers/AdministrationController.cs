using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Genesis.BusinessLogic;
using Genesis.Controllers;
using Genesis.DTO;
using Microsoft.Ajax.Utilities;

namespace Genesis.Areas.Administration.Controllers
{
    public class AdministrationController : Controller
    {
        UserAccount serviceUserAccount;
        Dashboard serviceDashboard;

        public AdministrationController()
        {
            serviceUserAccount = new UserAccount();
            serviceDashboard = new Dashboard();
        }


        #region Dashboard

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetReceivables()
        {
            //var list = new List<dtoDashboardReceivables>();
            var currentUser = (dtoUserAccount)Session["CurrentUser"];

            //int totalRecords = 0;

            var list = serviceDashboard.GetAllReceivables(currentUser.branchId, 0, 50);

            return Json(list);
        }

        public JsonResult GetPayables()
        {
            //var list = new List<dtoDashboardPayables>();
            var currentUser = (dtoUserAccount)Session["CurrentUser"];

            //int totalRecords = 0;


            var list = serviceDashboard.GetAllPayables(currentUser.branchId, 0, 50);

            return Json(list);
        }

        public JsonResult GetReorders()
        {
            //var list = new List<dtoDashboardReorder>();
            var currentUser = (dtoUserAccount)Session["CurrentUser"];

            //int totalRecords = 0;

            var list = serviceDashboard.GetAllReorders(currentUser.branchId, 0, 50);

            return Json(list);
        }


        #endregion


        #region Users

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult LoginUser(dtoUserAccount user)
        {
            var result = serviceUserAccount.ValidateCredentials(user);

            var resultObject = (dtoUserAccount) result.returnObj;

            if (result.isSuccessful)
            {
                FormsAuthentication.SetAuthCookie(resultObject.userName, false);
                Session["CurrentUser"] =  resultObject;
            }

            return Json(result);
        }

        public ActionResult LogoutUser()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Administration");
        }

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult GetAllUsers_old(jQueryDataTableParamModel param)
        {

            var filter = new dtoUserAccount
            {
                userId = Request["userId"] == "" ? 0 : Convert.ToInt32( Request["userId"]),
                firstName = Request["firstname"],
                lastName = Request["lastname"],
                emailAddress = Request["emailAddress"],
                userName = Request["userName"],
                branchId =  Request["branchId"] == "" ? 0 : Convert.ToInt32(Request["branchId"])                
            };

            var list = serviceUserAccount.GetAllUsers(param.sSearch ?? "", filter);

            int count = list.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = count,
                iTotalDisplayRecords = 10,
                aaData = list
            }
            , JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllUsers()
        {

            var currentUser = (dtoUserAccount) Session["CurrentUser"];
            var page = int.Parse(Request["page"]);
            var recordPerPage = int.Parse(Request["recordPerPage"]);
            var isExport = false;

            var filter = new dtoUserAccount
            {
                userName = Request["searchUserName"],
                firstName = Request["searchFirstName"],
                lastName = Request["searchLastName"],
                emailAddress = Request["searchEmailAddress"],
                branchId = (Request["searchBranchId"] == "null" || Request["searchBranchId"] == "") ? 0 : Convert.ToInt32(Request["searchBranchId"])                
            };

            var list = serviceUserAccount.GetAllUsers2(page, recordPerPage, filter, isExport);
            //totalRecords = serviceUserAccount.GetRecordCount(filter);

            //int count = list.Count();


            return Json(list);
        }

        public JsonResult CheckUserNameExists(string username)
        {
            if (String.IsNullOrWhiteSpace(Request["id"]) || Request["id"] == "0")
            {
                var result = serviceUserAccount.CheckUserNameExists(username);

                if (!result.Any())
                    return Json(true, JsonRequestBehavior.AllowGet);

                return Json("Username already in use.", JsonRequestBehavior.AllowGet);    
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddEditUser(dtoUserAccount user)
        {
            var result = user.userId == 0 ? serviceUserAccount.Register(user) : serviceUserAccount.Update(user);
            return Json(result);
        }


        public JsonResult ActivateUser(int id)
        {
            var result = serviceUserAccount.ActivateUser(id);    
            return Json(result);
        }

        public JsonResult DeactivateUser(int id)
        {
            var result = serviceUserAccount.DeactivateUser(id);
            return Json(result);
        }
        #endregion


        

    }
}
