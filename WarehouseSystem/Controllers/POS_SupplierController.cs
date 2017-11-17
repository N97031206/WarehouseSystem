using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem;
using WarehouseSystem.Models;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Misc;
using System.Web.Security;

namespace WarehouseSystem.Controllers
{
    [AuthorizeUser]
    public class POS_SupplierController : Controller
    {
        //private IPOS_SupplierService m_UserService;
        private IPOS_SupplierService m_SupplyService;
        private IUserGroupService m_UserGroupService;

        public POS_SupplierController(IPOS_SupplierService pSupplierService, IUserGroupService pUserGroupService)
        {
            m_SupplyService = pSupplierService;
            m_UserGroupService = pUserGroupService;
        }


        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }

        public ActionResult List(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var UserProfileList = m_SupplyService.GetAll().ToList();

            List<POS_SupplierViewModel> UserViewModelList = new List<POS_SupplierViewModel>();

            foreach (var UserProfile in UserProfileList)
            {
                UserViewModelList.Add(new POS_SupplierViewModel()
                {
                    SupplierProfile = UserProfile
                });
            }
            return View(UserViewModelList);
        }


        # region Create
        public ActionResult Create()
        {
            POS_SupplierViewModel  pSupplierViewModel = new POS_SupplierViewModel();
            return View(pSupplierViewModel);
        }
        #endregion

        public ActionResult Edit(string SupID)
        {
            if (string.IsNullOrEmpty(SupID))
            {
                return RedirectToAction("List");
            }
            else
            {
                var SupplierProfile = m_SupplyService.GetBySID(SupID);
                var UserGroup = m_UserGroupService.GetAll().ToList();
                var UserGroupName = UserGroup.Where(e => e.UserGroupID == SupplierProfile.UserGroupID).ToList();

                POS_SupplierViewModel pUserViewModel = new POS_SupplierViewModel()
                {
                    SupplierProfile = SupplierProfile,
                    UserGroup = UserGroup,
                    UserGroupName = (UserGroupName.Count > 0) ? UserGroupName[0].GroupName : "",
                    ErrorMessage = ""
                };

                return View(pUserViewModel);
            }
        }

        [HttpPost]
        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_SupplyService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            return Json(new { Result = pResult });
        }

        #region
        public ActionResult CreateUserProfile(string CompanyName ,string SupID)
        {
            POS_SupplierViewModel POS_SupplierViewModel = new POS_SupplierViewModel();

            if (!string.IsNullOrEmpty(SupID) && 
                !string.IsNullOrEmpty(CompanyName) && 
                ModelState.IsValid)

            {
                //string UserProfileID = "";

                var pResult = m_SupplyService.Create(CompanyName, SupID);

                if (pResult.Success == false)
                {
                    POS_SupplierViewModel.SupplierProfile.SupID = SupID;
                    POS_SupplierViewModel.SupplierProfile.CompanyName = CompanyName;
                    POS_SupplierViewModel.ErrorMessage = pResult.Message;
                    return View("Create", POS_SupplierViewModel);
                }
                else
                {
                    return RedirectToAction("Edit");
                }
            }
            else
            {
                POS_SupplierViewModel.SupplierProfile.CompanyName = CompanyName;
                POS_SupplierViewModel.ErrorMessage = "統編空白或供應商名空白";
            }

            return View("Create", POS_SupplierViewModel);
        }
        #endregion


        #region
        public ActionResult Delete(string SupID)
        {
            if (!string.IsNullOrEmpty(SupID))
            {
                if (m_SupplyService.IsExists(SupID))
                {
                    var pResult = m_SupplyService.Delete(SupID);
                    return Json(new { Result = pResult.Success });
                }
            }
            return Json(new { Result = false });
        }
        #endregion


    }
}
