using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Genesis.BusinessLogic;
using Genesis.Controllers;
using Genesis.DTO;

namespace Genesis.Areas.Administration.Controllers
{
    public class BranchController : BaseController
    {
        BLBranch repo;

        public BranchController()
        {
            repo = new BLBranch();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllBranchesForDropDown()
        {
            var list = repo.GetAllBranches("");
            return Json(list);
        }

        public ActionResult Branches()
        {
            return View();
        }

        public JsonResult GetAllBranches()
        {
            var currentUser = (dtoUserAccount)Session["CurrentUser"];
            var page = int.Parse(Request["page"]);
            var recordPerPage = int.Parse(Request["recordPerPage"]);
            var isExport = false;

            var filter = new dtoBranch()
            {
                branchCode = Request["searchBranchCode"],
                branchName = Request["searchBranchName"]
              };

            var list = repo.GetAllBranches2(page, recordPerPage, filter, isExport);


            return Json(list);
        }

        public JsonResult CheckBranchCodeExists(string branchCode)
        {
            if (String.IsNullOrWhiteSpace(Request["id"]) || Request["id"] == "0")
            {
                var result = repo.CheckBranchCodeExists(branchCode);

                if (!result.Any())
                    return Json(true, JsonRequestBehavior.AllowGet);

                return Json("Branch Code already in use.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public JsonResult AddEditBranch(dtoBranch branch)
        {
            dtoResult result;
            var currentUser = (dtoUserAccount)Session["CurrentUser"];

            if (branch.branchId == 0)
            {
                branch.createdBy = currentUser.userId;
                branch.dateCreated = DateTime.Now;
                result = repo.Insert(branch);
            }
            else
            {
                branch.lastModifiedBy = currentUser.userId;
                branch.dateLastModified = DateTime.Now;
                result = repo.Update(branch);
            }
            
            return Json(result);
        }

    }
}
