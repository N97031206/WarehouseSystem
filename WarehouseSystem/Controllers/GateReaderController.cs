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
    public class GateReaderController : Controller
    {
        private IGateReaderService m_GateReaderService;

        public GateReaderController(IGateReaderService p_GateReaderService)
        {
            m_GateReaderService = p_GateReaderService;
        }

        public ActionResult Create()
        {
            GateReaderViewModel _GateReaderViewModel = new GateReaderViewModel();

            return View(_GateReaderViewModel);
        }

        [HttpPost]
        public ActionResult CreateGateReader(string GateReaderNumber)
        {
            string GateReaderID = "";

            var Result = m_GateReaderService.Create(GateReaderNumber, ref GateReaderID);

            if (Result.Success)
            {
                return RedirectToAction("Edit", new { GateReaderID = GateReaderID });
            }
            else
            {
                GateReaderViewModel _GateReaderViewModel = new GateReaderViewModel()
                {
                    ErrorMessage = Result.Message
                };

                return View("Create", _GateReaderViewModel);
            }
        }

        public ActionResult Edit(string GateReaderID)
        {
            if (string.IsNullOrEmpty(GateReaderID))
            {
                return RedirectToAction("List");
            }
            else
            {
                var GateReader = m_GateReaderService.GetBySID(GateReaderID);

                if (GateReader != null)
                {
                    GateReaderViewModel _GateReaderViewModel = new GateReaderViewModel()
                    {
                        GateReader = GateReader
                    };
                    return View(_GateReaderViewModel);
                }
                else return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_GateReaderService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            return Json(new { Result = pResult });
            //if (pResult.Success)
            //{
            //    return RedirectToAction("Edit", new { GateReaderID = _JsonType.pk });
            //}
            ////else return RedirectToAction("List");
            //return RedirectToAction("Edit", new { GateReaderID = _JsonType.pk });
        }

        public ActionResult Delete(string GateReaderID)
        {
            if (!string.IsNullOrEmpty(GateReaderID))
            {
                var pResult = m_GateReaderService.Delete(GateReaderID);
                return Json(new { Result = pResult.Success });
            }

            return Json(new { Result = false });
        }

        public ActionResult List(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var GateReaderList = m_GateReaderService.GetAll().ToList();

            List<GateReaderViewModel> GateReaderViewModelList = new List<GateReaderViewModel>();

            foreach (var GateReader in GateReaderList)
            {
                GateReaderViewModelList.Add(new GateReaderViewModel()
                {
                    GateReader = GateReader
                });
            }
                    

            return View(GateReaderViewModelList);
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }
    }
}