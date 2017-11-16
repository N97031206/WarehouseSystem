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
    public class RFIDController : Controller
    {
        private IRFIDService m_RFIDService;

        public RFIDController(IRFIDService p_RFIDService)
        {
            m_RFIDService = p_RFIDService;
        }

        public ActionResult Create()
        {
            RFIDViewModel _RFIDViewModel = new RFIDViewModel();

            return View(_RFIDViewModel);
        }

        [HttpPost]
        public ActionResult CreateRFID(string RFIDNumber)
        {
            string RFIDID = "";

            var Result = m_RFIDService.Create(RFIDNumber, ref RFIDID);

            if (Result.Success)
            {
                return RedirectToAction("Edit", new { RFIDID = RFIDID });
            }
            else
            {
                RFIDViewModel _RFIDViewModel = new RFIDViewModel()
                {
                    ErrorMessage = Result.Message
                };

                return View("Create", _RFIDViewModel);
            }
        }

        public ActionResult Edit(string RFIDID)
        {
            if (string.IsNullOrEmpty(RFIDID))
            {
                return RedirectToAction("List");
            }
            else
            {
                var RFID = m_RFIDService.GetBySID(RFIDID);

                if (RFID != null)
                {
                    RFIDViewModel _RFIDViewModel = new RFIDViewModel()
                    {
                        RFID = RFID
                    };
                    return View(_RFIDViewModel);
                }
                else return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_RFIDService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            return Json(new { Result = pResult });
            //if (pResult.Success)
            //{
                
            //}
            //else return RedirectToAction("List");
        }

        public ActionResult Delete(string RFIDID)
        {
            if (!string.IsNullOrEmpty(RFIDID))
            {
                var pResult = m_RFIDService.Delete(RFIDID);
                return Json(new { Result = pResult.Success });
            }

            return Json(new { Result = false });
            //return RedirectToAction("List");
        }

        public ActionResult List(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var RFIDList = m_RFIDService.GetAll().ToList();

            List<RFIDViewModel> RFIDViewModelList = new List<RFIDViewModel>();

            foreach (var RFID in RFIDList)
            {
                RFIDViewModelList.Add(new RFIDViewModel()
                {
                    RFID = RFID
                });
            }

            return View(RFIDViewModelList);
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }
    }
}