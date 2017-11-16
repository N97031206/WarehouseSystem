using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem.Models;
using WarehouseSystem.Service.Interface;

namespace WarehouseSystem.Controllers
{
    [AuthorizeUser]
    public class TrackingController : Controller
    {
        private IPalletTypeService m_PalletTypeService;
        private IPalletService m_PalletService;
        private IWarehouseService m_WarehouseService;
        private IFieldService m_FieldService;
        private IStorageParkingBlockService m_IBeaconParkingBlockService;

        public TrackingController(IPalletTypeService p_PalletTypeService, IPalletService p_PalletService, IWarehouseService p_WarehouseService, IFieldService p_FieldService, IStorageParkingBlockService p_IBeaconParkingBlockService)
        {
            m_PalletTypeService = p_PalletTypeService;
            m_PalletService = p_PalletService;
            m_WarehouseService = p_WarehouseService;
            m_FieldService = p_FieldService;
            m_IBeaconParkingBlockService = p_IBeaconParkingBlockService;

        }

        public ActionResult TrackingPallet(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            ViewData["PalletType"] = m_PalletTypeService.GetAll().ToList();

            //ViewData["FieldMap"] = m_FieldService.GetAll().ToList();

            //ViewData["Pallet"] = m_PalletService.GetAll().ToList();

            return View(ViewData);
        }

        [HttpPost]
        public ActionResult GetFieldMapAndParkingBlock(string PalletNumber)
        {
            if (string.IsNullOrEmpty(PalletNumber)) { return HttpNotFound(); }

            //取得該車號對應的MAP編號
            var FieldMapList = m_WarehouseService.GetFieldMapByPalletNumber(PalletNumber).ToList();

            if (FieldMapList.Count > 0)
            {
                //取得MAP編號內的區域資料
                var ParkingBlockList = m_WarehouseService.GetParkingBlockByFieldID(FieldMapList[0].FieldID);

                var JsonData = new { FieldData = FieldMapList[0], ParkingBlockData = ParkingBlockList };

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JsonData), "application/json");
            }
            else
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { Result = false }), "application/json");
            }
        }

        [HttpPost]
        public ActionResult GetFieldMapByPalletNumber(string PalletNumber)
        {
            if (string.IsNullOrEmpty(PalletNumber)) { return HttpNotFound(); }

            //取得該棧板編號對應的MAP編號
            var FieldMapList = m_WarehouseService.GetFieldMapByPalletNumber(PalletNumber).ToList();

            if (FieldMapList.Count > 0)
            {
                //取得MAP編號內的區域資料
                var ParkingBlockList = m_WarehouseService.GetParkingBlockByFieldID(FieldMapList[0].FieldID).ToList();

                var JsonData = new { FieldData = FieldMapList[0], ParkingBlockData = ParkingBlockList };

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JsonData), "application/json");
            }
            else
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { Result = false }), "application/json");
            }
        }

        [HttpPost]
        public ActionResult GetFieldMapByClothNumber(string ClothNumber)
        {
            if (string.IsNullOrEmpty(ClothNumber)) { return HttpNotFound(); }

            //取得該布號對應的MAP編號
            var FieldMapList = m_WarehouseService.GetFieldMapByClothNumber(ClothNumber).ToList();

            if (FieldMapList.Count > 0)
            {
                List<FieldMapInfo> JsonData = new List<FieldMapInfo>();

                foreach (var FieldMap in FieldMapList)
                {
                    //取得MAP編號內的區域資料
                    var ParkingBlockList = m_WarehouseService.GetParkingBlockByFieldID(FieldMap.FieldID).ToList();

                    JsonData.Add(new FieldMapInfo()
                    {
                        FieldData = FieldMap,
                        ParkingBlockData = ParkingBlockList
                    });
                }

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JsonData), "application/json");
            }
            else
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { Result = false }), "application/json");
            }
        }

        [HttpPost]
        public ActionResult GetFieldMapByWorkOrder(string WorkOrder)
        {
            if (string.IsNullOrEmpty(WorkOrder)) { return HttpNotFound(); }

            //取得該布號對應的MAP編號
            var FieldMapList = m_WarehouseService.GetFieldMapByWorkOrder(WorkOrder).ToList();

            if (FieldMapList.Count > 0)
            {
                List<FieldMapInfo> JsonData = new List<FieldMapInfo>();

                foreach (var FieldMap in FieldMapList)
                {
                    //取得MAP編號內的區域資料
                    var ParkingBlockList = m_WarehouseService.GetParkingBlockByFieldID(FieldMap.FieldID).ToList();

                    JsonData.Add(new FieldMapInfo()
                    {
                        FieldData = FieldMap,
                        ParkingBlockData = ParkingBlockList
                    });
                }

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JsonData), "application/json");
            }
            else
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { Result = false }), "application/json");
            }
        }

        [HttpPost]
        public ActionResult GetFieldMapByPalletType(string PalletTypeID)
        {
            if (string.IsNullOrEmpty(PalletTypeID)) { return HttpNotFound(); }

            //取得該布號對應的MAP編號
            var FieldMapList = m_WarehouseService.GetFieldMapByPalletType(PalletTypeID).ToList();

            if (FieldMapList.Count > 0)
            {
                List<FieldMapInfo> JsonData = new List<FieldMapInfo>();

                foreach (var FieldMap in FieldMapList)
                {
                    //取得MAP編號內的區域資料
                    var ParkingBlockList = m_WarehouseService.GetParkingBlockByFieldID(FieldMap.FieldID).ToList();

                    JsonData.Add(new FieldMapInfo()
                    {
                        FieldData = FieldMap,
                        ParkingBlockData = ParkingBlockList
                    });
                }

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JsonData), "application/json");
            }
            else
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { Result = false }), "application/json");
            }
        }

        [HttpPost]
        public ActionResult GetFieldMapByOverTime()
        {
            //取得該布號對應的MAP編號
            var FieldMapList = m_WarehouseService.GetFieldMapByOverTime().ToList();

            if (FieldMapList.Count > 0)
            {
                List<FieldMapInfo> JsonData = new List<FieldMapInfo>();

                foreach (var FieldMap in FieldMapList)
                {
                    //取得MAP編號內的區域資料
                    var ParkingBlockList = m_WarehouseService.GetParkingBlockByFieldID(FieldMap.FieldID).ToList();

                    JsonData.Add(new FieldMapInfo()
                    {
                        FieldData = FieldMap,
                        ParkingBlockData = ParkingBlockList
                    });
                }

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JsonData), "application/json");
            }
            else
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { Result = false }), "application/json");
            }
        }

        [HttpPost]
        public ActionResult GetFieldMapByAll()
        {
            //取得該布號對應的MAP編號
            var FieldMapList = m_WarehouseService.GetFieldMapByAll().ToList();

            if (FieldMapList.Count > 0)
            {
                List<FieldMapInfo> JsonData = new List<FieldMapInfo>();

                foreach (var FieldMap in FieldMapList)
                {
                    //取得MAP編號內的區域資料
                    var ParkingBlockList = m_WarehouseService.GetParkingBlockByFieldID(FieldMap.FieldID).ToList();

                    JsonData.Add(new FieldMapInfo()
                    {
                        FieldData = FieldMap,
                        ParkingBlockData = ParkingBlockList
                    });
                }

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JsonData), "application/json");
            }
            else
            {
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { Result = false }), "application/json");
            }
        }

        [HttpPost]
        public ActionResult GetLotInfoByPalletNumber(string PalletNumber)
        {
            var PalletInfoList = m_WarehouseService.GetLotInfoByPalletNumber(PalletNumber);

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(PalletInfoList), "application/json");
        }

        [HttpPost]
        public ActionResult GetPalletByClothNumber(string FieldID, string ClothNumber)
        {
            var PalletInfoList = m_WarehouseService.GetPalletByClothNumber(FieldID, ClothNumber);

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(PalletInfoList), "application/json");
        }

        [HttpPost]
        public ActionResult GetPalletByWorkOrder(string FieldID, string WorkOrder)
        {
            var PalletInfoList = m_WarehouseService.GetPalletByWorkOrder(FieldID, WorkOrder);

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(PalletInfoList), "application/json");
        }

        [HttpPost]
        public ActionResult GetPalletByPalletType(string FieldID, string PalletTypeID)
        {
            var PalletInfoList = m_WarehouseService.GetPalletByPalletType(FieldID, PalletTypeID);

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(PalletInfoList), "application/json");
        }

        [HttpPost]
        public ActionResult GetPalletByOverTime(string FieldID)
        {
            var PalletInfoList = m_WarehouseService.GetPalletByOverTime(FieldID);

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(PalletInfoList), "application/json");
        }

        [HttpPost]
        public ActionResult GetPalletByAll(string FieldID)
        {
            var PalletInfoList = m_WarehouseService.GetPalletByAll(FieldID);

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(PalletInfoList), "application/json");
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }

        public ActionResult InsertView()
        {
            ViewData["ParkingBlock"] = m_IBeaconParkingBlockService.GetAll().ToList();
            ViewData["Pallet"] = m_PalletService.GetAll().ToList();
            return View(ViewData);
        }
        [HttpPost]
        public ActionResult InsertWarehouseData(string WO, string DeviceName, string PalletID, string ParkingBlockID, string Lot)
        {
            var Pallet = m_PalletService.GetBySID(PalletID);

            if (Pallet.PalletTypeID == null || Pallet.PalletTypeID == "")
            {
                return Json(new WarehouseSystem.Service.Misc.Result()
                {
                    Success = false,
                    Message = "棧板編號[" + Pallet.PalletNumber + "]：無設定類型"
                }
                );
            }

            if (Pallet.RFIDID == null || Pallet.RFIDID == "")
            {
                return Json(new WarehouseSystem.Service.Misc.Result()
                {
                    Success = false,
                    Message = "棧板編號[" + Pallet.PalletNumber + "]：無設定對應RFID"
                }
                );
            }

            CST_WAREHOUSE Data = new CST_WAREHOUSE()
            {
                DeviceName = DeviceName,
                PalletNumber = Pallet.PalletNumber,
               // ParkingBlockID = ParkingBlockID,
                PalletTypeID = Pallet.PalletTypeID,
                RFIDID = Pallet.RFIDID,
                Lot = Lot,
                WO = WO
            };

            var Result = m_WarehouseService.Insert(Data);

            return Json(Result);
        }

        public ActionResult TestPanel()
        {
            return View();
        }
    }
}