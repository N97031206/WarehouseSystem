using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem.Models;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Interface;

namespace WarehouseSystem.Controllers
{
    [AuthorizeUser]
    public class MainMenuController : Controller
    {
        private IMainMenuService m_MainMenuService;

        public MainMenuController(IMainMenuService p_MainMenuService)
        {
            m_MainMenuService = p_MainMenuService;
        }

        public ActionResult Create()
        {
            MainMenuViewModel _MainMenuViewModel = new MainMenuViewModel();

            return View(_MainMenuViewModel);
        }

        [HttpPost]
        public ActionResult CreateMainMenu(string MainMenuName)
        {
            string MainMenuID = "";

            var Result = m_MainMenuService.Create(MainMenuName, ref MainMenuID);

            if (Result.Success)
            {

                return RedirectToAction("Edit", new { MainMenuID = MainMenuID });
            }
            else
            {
                MainMenuViewModel _MainMenuViewModel = new MainMenuViewModel()
                {
                    ErrorMessage = Result.Message
                };

                return View("Create", _MainMenuViewModel);
            }
        }

        public ActionResult Edit(string MainMenuID)
        {
            if (string.IsNullOrEmpty(MainMenuID))
            {
                return RedirectToAction("List");
            }
            else
            {
                var MainMenu = m_MainMenuService.GetBySID(MainMenuID);

                if (MainMenu != null)
                {
                    var SubMenuList = m_MainMenuService.GetByMainMenuID(MainMenuID);

                    MainSubViewModel _MainSubViewModel = new MainSubViewModel()
                    {
                        MainMenu = MainMenu,
                        SubMenuList = SubMenuList.ToList()
                    };
                    return View(_MainSubViewModel);
                }
                else return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_MainMenuService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            if (pResult.Success)
            {
                return RedirectToAction("Edit", new { MainMenuID = _JsonType.pk });
            }
            else return RedirectToAction("List");
        }

        public ActionResult Delete(string MainMenuID)
        {
            if (!string.IsNullOrEmpty(MainMenuID))
            {
                var pResult = m_MainMenuService.Delete(MainMenuID);

                return Json(new { Result = pResult.Success });
            }

            return Json(new { Result = false });
            //return RedirectToAction("List");
        }

        public ActionResult List(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var MainMenuList = m_MainMenuService.GetAll().ToList();

            List<MainMenuViewModel> MainMenuViewModelList = new List<MainMenuViewModel>();

            foreach (var _MainMenu in MainMenuList)
            {
                MainMenuViewModelList.Add(new MainMenuViewModel()
                {
                    MainMenu = _MainMenu
                });
            }

            return View(MainMenuViewModelList);
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }
    }
}