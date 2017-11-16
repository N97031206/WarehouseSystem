using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem.Models;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Controllers
{
    [AuthorizeUser]
    public class ParkingBlockController : Controller
    {
        private IStorageParkingBlockService m_IBeaconParkingBlockService;

        public ParkingBlockController(IStorageParkingBlockService p_IBeaconParkingBlockService)
        {
            m_IBeaconParkingBlockService = p_IBeaconParkingBlockService;
        }

        //[HttpPost]
        public ActionResult Create(string FieldID, string X, string Y, string Width, string Height)
        {
            if (!string.IsNullOrEmpty(FieldID) && ModelState.IsValid)
            {
                string ParkingBlockID = "";

                var dX = Convert.ToDouble(X);
                var dY = Convert.ToDouble(Y);
                var dWidth = Convert.ToDouble(Width);
                var dHeight = Convert.ToDouble(Height);

                var iX = Convert.ToInt16(dX);
                var iY = Convert.ToInt16(dY);
                var iWidth = Convert.ToInt16(dWidth);
                var iHeight = Convert.ToInt16(dHeight);

                IResult Result = m_IBeaconParkingBlockService.Create(FieldID, iX, iY, iWidth, iHeight, ref ParkingBlockID);

                if (Result.Success == false) return Json(null);

                if (string.IsNullOrEmpty(ParkingBlockID)) return Json(null);
                else
                {
                    //var ParkingBlockList = m_IBeaconParkingBlockService.GetAll().ToList();

                    //ViewData["ParkingBlock"] = ParkingBlockList;
                    var ParkingBlock = m_IBeaconParkingBlockService.GetBySID(ParkingBlockID);

                    return Json(ParkingBlock);
                }
            }

            return Json(null);
        }

        public ActionResult Update(JsonType _JsonType)
        {
            var Result = m_IBeaconParkingBlockService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            return Json(Result);
        }

        public ActionResult UpdateRange(string ParkingBlockID, string X, string Y, string Width, string Height)
        {
            if (!string.IsNullOrEmpty(ParkingBlockID) && ModelState.IsValid)
            {
                var dX = Convert.ToDouble(X);
                var dY = Convert.ToDouble(Y);
                var dWidth = Convert.ToDouble(Width);
                var dHeight = Convert.ToDouble(Height);

                var iX = Convert.ToInt16(dX);
                var iY = Convert.ToInt16(dY);
                var iWidth = Convert.ToInt16(dWidth);
                var iHeight = Convert.ToInt16(dHeight);

                var Result = m_IBeaconParkingBlockService.UpdateRange(ParkingBlockID, iX, iY, iWidth, iHeight);
                    
                return Json(Result);
            }
            else
            {
                return Json(null);
            }
        }

        public ActionResult Delete(string ParkingBlockID)
        {
            IResult Result = new Result(false);

            try
            {
                if (!string.IsNullOrEmpty(ParkingBlockID) && ModelState.IsValid)
                {
                    var IsExists = m_IBeaconParkingBlockService.IsExists(ParkingBlockID);

                    if (!IsExists)
                    {
                        Result.Message = "找不到儲位場域資料";
                        return Json(Result);
                    }
                    else
                    {
                        Result = m_IBeaconParkingBlockService.Delete(ParkingBlockID);
                    }

                }
            }
            catch (Exception ex)
            {
                Result.Exception = ex;
            }

            return Json(Result);
        }
    }
}