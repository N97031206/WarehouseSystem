using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem.Models;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Controllers
{
    [AuthorizeUser]
    public class FieldController : Controller
    {
        private IFieldService m_FieldService;
        private IStorageParkingBlockService m_StorageParkingBlockService;

        public FieldController(IFieldService p_FieldService, IStorageParkingBlockService p_BeaconParkingBlockService)
        {
            m_FieldService = p_FieldService;
            m_StorageParkingBlockService = p_BeaconParkingBlockService;
        }

        public ActionResult Create()
        {
            FieldMapViewModel _FiedViewModel = new FieldMapViewModel();

            return View(_FiedViewModel);
        }

        [HttpPost]
        public ActionResult UploadMap(HttpPostedFileBase upload, string FieldID)
        {
            if (upload.ContentLength > 0)
            {
                try
                {
                    var fileName = System.IO.Path.GetFileName(upload.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Media/Map"), fileName);
                    upload.SaveAs(path);

                    var pResult = m_FieldService.Update(FieldID, "MapName", fileName);

                    if (pResult.Success) return Json(new { mapName = fileName });
                }
                catch (Exception ex)
                { }
                return Json(null);
            }

            else
            {
                return Json(null);
            }
        }

        public ActionResult CreateField(string FieldName)
        {
            string FieldID = "";

            IResult Result = m_FieldService.Create(FieldName, ref FieldID);

            if (Result.Success)
            {
                //return RedirectToAction("Edit", new { FieldID = FieldID });
                return RedirectToAction("List");
            }
            else
            {
                FieldMapViewModel _FiedViewModel = new FieldMapViewModel()
                {
                    ErrorMessage = Result.Message
                };

                return View("Create", _FiedViewModel);
            }
        }

        public ActionResult Edit(string FieldID)
        {

            if (string.IsNullOrEmpty(FieldID))
            {
                return RedirectToAction("List");
            }
            else
            {
                var FieldMap = m_FieldService.GetBySID(FieldID);

                var ParkingBlockList = m_StorageParkingBlockService.GetByFieldID(FieldID).ToList();

                FieldMapEditViewModel _FieldMapEditViewModel = new FieldMapEditViewModel()
                {
                    FieldMap = FieldMap,
                    ParkingBlockList = ParkingBlockList
                };

                return View(_FieldMapEditViewModel);
            }
        }


        public ActionResult List(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var FieldMapList = m_FieldService.GetAll().ToList();

            List<FieldMapViewModel> FieldMapViewModelList = new List<FieldMapViewModel>();

            foreach (var FieldMap in FieldMapList)
            {
                FieldMapViewModelList.Add(new FieldMapViewModel()
                {
                    FieldMap = FieldMap
                });
            }

            return View(FieldMapViewModelList);
        }

        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_FieldService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            return Json(new { Result = pResult });
            //if (pResult.Success)
            //{

            //    var FieldMap = m_FieldService.GetBySID(_JsonType.pk);

            //    FieldMapViewModel _FiedViewModel = new FieldMapViewModel()
            //    {
            //        FieldMap = FieldMap
            //    };

            //    return Json(_FiedViewModel);
            //}
            //return RedirectToAction("Edit", new { FieldID = _JsonType.pk });
        }


        public ActionResult Delete(string FieldID)
        {
            if (string.IsNullOrEmpty(FieldID))
            {
                return RedirectToAction("List");
            }

            var FieldMap = m_FieldService.GetBySID(FieldID);

            if (FieldMap == null) return Json(new { Result = false });


            var pResult = m_FieldService.Delete(FieldID);
            return Json(new { Result = pResult.Success });
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }
    }
}