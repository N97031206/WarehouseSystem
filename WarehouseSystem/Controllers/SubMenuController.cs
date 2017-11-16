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
    public class SubMenuController : Controller
    {
        private ISubMenuService m_SubMenuService;

        public SubMenuController(ISubMenuService p_SubMenuService)
        {
            m_SubMenuService = p_SubMenuService;
        }

        public ActionResult Create(string MainMenuID)
        {
            if (string.IsNullOrEmpty(MainMenuID)) return RedirectToAction("List", "MainMenu");

            var MainMenuList = m_SubMenuService.GetByMainMenuID(MainMenuID);

            if (MainMenuList.Count() > 0)
            {
                SubMenuViewModel _SubMenuViewModel = new SubMenuViewModel()
                {
                    SubMenu = new CST_MENU_SUB() { MainMenuID = MainMenuID }
                };
                return View(_SubMenuViewModel);
            }
            else return RedirectToAction("List", "MainMenu");
        }

        [HttpPost]
        public ActionResult CreateSubMenu(string SubMenuName, string MainMenuID)
        {
            string SubMenuID = "";

            var Result = m_SubMenuService.Create(SubMenuName, MainMenuID, ref SubMenuID);

            if (Result.Success)
            {
                return RedirectToAction("Edit", new { SubMenuID = SubMenuID });
            }
            else
            {
                SubMenuViewModel _SubMenuViewModel = new SubMenuViewModel()
                {
                    SubMenu = new CST_MENU_SUB() { MainMenuID = MainMenuID },
                    ErrorMessage = Result.Message
                };

                return View("Create", _SubMenuViewModel);
            }
        }

        public ActionResult Edit(string SubMenuID)
        {
            if (string.IsNullOrEmpty(SubMenuID))
            {
                return RedirectToAction("List", "MainMenu");
            }
            else
            {
                var SubMenu = m_SubMenuService.GetBySID(SubMenuID);

                if (SubMenu != null)
                {
                    var MinorMenuList = m_SubMenuService.GetBySubMenuID(SubMenuID);

                    SubMinorViewModel _SubMinorViewModel = new SubMinorViewModel()
                    {
                        MinorMenuList = MinorMenuList.ToList(),
                        SubMenu = SubMenu
                    };
                    return View(_SubMinorViewModel);
                }
                else return RedirectToAction("List", "MainMenu");
            }
        }

        [HttpPost]
        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_SubMenuService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            if (pResult.Success)
            {
                return RedirectToAction("Edit", new { SubMenuID = _JsonType.pk });
            }
            else return RedirectToAction("List");
        }

        public ActionResult Delete(string SubMenuID)
        {
            if (!string.IsNullOrEmpty(SubMenuID))
            {
                var pResult = m_SubMenuService.Delete(SubMenuID);

                return Json(new { Result = pResult.Success });
            }

            return Json(new { Result = false });
            //return RedirectToAction("List");
        }
    }
}