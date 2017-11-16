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
    public class MinorMenuController : Controller
    {
        private IMinorMenuService m_MinorMenuService;

        public MinorMenuController(IMinorMenuService p_MinorMenuService)
        {
            m_MinorMenuService = p_MinorMenuService;
        }

        public ActionResult Create(string SubMenuID)
        {
            if (string.IsNullOrEmpty(SubMenuID)) return RedirectToAction("List", "MainMenu");

            var SubMenuList = m_MinorMenuService.GetBySubMenuID(SubMenuID);

            if (SubMenuList.Count() > 0)
            {
                MinorMenuViewModel _MinorMenuViewModel = new MinorMenuViewModel()
                {
                    MinorMenu = new CST_MENU_MINOR() { SubMenuID = SubMenuID }
                };
                return View(_MinorMenuViewModel);
            }
            else return RedirectToAction("List", "MainMenu");
        }

        [HttpPost]
        public ActionResult CreateMinorMenu(string MinorMenuName, string SubMenuID)
        {
            string MinorMenuID = "";

            var Result = m_MinorMenuService.Create(MinorMenuName, SubMenuID, ref MinorMenuID);

            if (Result.Success)
            {
                return RedirectToAction("Edit", new { MinorMenuID = MinorMenuID });
            }
            else
            {
                MinorMenuViewModel _MinorMenuViewModel = new MinorMenuViewModel()
                {
                    MinorMenu = new CST_MENU_MINOR() { SubMenuID = SubMenuID },
                    ErrorMessage = Result.Message
                };

                return View("Create", _MinorMenuViewModel);
            }
        }

        public ActionResult Edit(string MinorMenuID)
        {
            if (string.IsNullOrEmpty(MinorMenuID))
            {
                return RedirectToAction("List", "MainMenu");
            }
            else
            {
                var MinorMenu = m_MinorMenuService.GetBySID(MinorMenuID);

                if (MinorMenu != null)
                {
                    MinorMenuViewModel _MinorMenuViewModel = new MinorMenuViewModel()
                    {
                        MinorMenu = MinorMenu
                    };
                    return View(_MinorMenuViewModel);
                }
                else return RedirectToAction("List", "MainMenu");
            }
        }

        [HttpPost]
        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_MinorMenuService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            if (pResult.Success)
            {
                return RedirectToAction("Edit", new { MinorMenuID = _JsonType.pk });
            }
            else return RedirectToAction("List");
        }

        public ActionResult Delete(string MinorMenuID)
        {
            if (!string.IsNullOrEmpty(MinorMenuID))
            {
                var pResult = m_MinorMenuService.Delete(MinorMenuID);

                return Json(new { Result = pResult.Success });
            }

            return Json(new { Result = false });
            //return RedirectToAction("List");
        }
    }
}