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
        private IPOS_SupplierService m_UserService;

        public POS_SupplierController(IPOS_SupplierService pUserService)
        {
            m_UserService = pUserService;
        }


        // GET: POS_Supplier
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var UserProfileList = m_UserService.GetAll().ToList();

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

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }

    }
}
