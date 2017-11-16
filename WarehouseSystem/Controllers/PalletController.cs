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
    public class PalletController : Controller
    {
        private IPalletTypeService m_PalletTypeService;
        private IPalletService m_PalletService;
        private IRFIDService m_RFIDService;

        public PalletController(IPalletTypeService pPalletTypeService, IPalletService pPalletService, IRFIDService pRFIDService)
        {
            m_PalletTypeService = pPalletTypeService;
            m_PalletService = pPalletService;
            m_RFIDService = pRFIDService;
        }

        public ActionResult PalletCreate()
        {
            PalletEditViewModel _PalletEditViewModel = new PalletEditViewModel();

            return View(_PalletEditViewModel);
        }

        /// <summary>
        /// ASCII轉16進制
        /// </summary>
        /// <param name="strMsg"></param>
        /// <param name="CodeType"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public string StringToHex(string strMsg, string CodeType = "00", int MaxLength = 24)
        {
            string result = "";

            foreach (var _Char in strMsg)
            {
                var _Int = Convert.ToInt32(_Char);

                result += String.Format("{0:x}", _Int);
            }

            result = CodeType + result;

            int diff = MaxLength - result.Length;

            for (int i = 0; i < diff; i++)
            {
                result += "0";
            }

            return result;
        }

        [HttpPost]
        public ActionResult CreatePallet(string PalletNumber)
        {

            string PalletID = "";
            string RFIDID = "";
            string RFID = "";

            RFID = StringToHex(PalletNumber, "00000001");

            if (string.IsNullOrEmpty(RFID) == false)
            {
                //自動新增RFID
                m_RFIDService.Create(RFID, ref RFIDID);
            }

            var Result = m_PalletService.Create(PalletNumber, RFIDID, ref PalletID);

            if (Result.Success)
            {
                return RedirectToAction("PalletEdit", new { PalletID = PalletID });
            }
            else
            {
                PalletEditViewModel _PalletEditViewModel = new PalletEditViewModel()
                {
                    Pallet = new CST_PALLET() { PalletNumber = PalletNumber },
                    ErrorMessage = Result.Message
                };

                return View("PalletCreate", _PalletEditViewModel);
            }
        }

        public ActionResult PalletEdit(string PalletID)
        {
            if (string.IsNullOrEmpty(PalletID))
            {
                return RedirectToAction("PalletList");
            }
            else
            {
                var Pallet = m_PalletService.GetBySID(PalletID);

                if (Pallet != null)
                {
                    var PalletTypeList = m_PalletTypeService.GetAll().ToList();

                    var PalletType = m_PalletTypeService.GetBySID(Pallet.PalletTypeID);

                    var RFIDList = m_RFIDService.GetAll().ToList();

                    var RFIDNumber = m_RFIDService.GetBySID(Pallet.RFIDID);

                    PalletEditViewModel _PalletEditViewModel = new PalletEditViewModel()
                    {
                        Pallet = Pallet,
                        PalletTypeList = PalletTypeList,
                        PalletTypeCode = (PalletType == null) ? "" : PalletType.TypeCode,
                        PalletTypeName = (PalletType == null) ? "" : PalletType.TypeName,
                        RFIDList = RFIDList,
                        RFIDNumber = (RFIDNumber == null) ? "" : RFIDNumber.RFIDNumber
                    };

                    return View(_PalletEditViewModel);
                }
                else return RedirectToAction("PalletList");
            }
        }

        public ActionResult PalletList(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            bool IsRemoveEnd = false;

            List<PalletListViewModel> _PalletListViewModel = new List<PalletListViewModel>();

            var PalletList = m_PalletService.GetAll().ToList();

            var PalletTypeList = m_PalletTypeService.GetAll().ToList();

            //新增一筆群組為未設定
            PalletTypeList.Add(new CST_PALLET_TYPE()
                {
                    PalletTypeID = "ATEST",
                    TypeName = "未設定",
                    TypeCode = "Null"
                });

            foreach (var item in PalletTypeList)
            {
                _PalletListViewModel.Add(new PalletListViewModel()
                {
                    PalletTypeName = item.TypeName,
                    PalletTypeCode = item.TypeCode,
                    PalletTypeID = item.PalletTypeID,
                });
            }

            foreach (var Pallet in PalletList)
            {
                bool IsSetting = false;

                for (int i = 0; i < _PalletListViewModel.Count; i++)
                {
                    if (_PalletListViewModel[i].PalletTypeID == Pallet.PalletTypeID)
                    {
                        _PalletListViewModel[i].Pallet.Add(Pallet);
                        IsSetting = true;
                    }
                }

                //如果沒有設定棧板類別就分派至未設定
                if (IsSetting == false)
                {
                    _PalletListViewModel[_PalletListViewModel.Count - 1].Pallet.Add(Pallet);
                }
            }


            //移除類別組為零的資料
            while (!IsRemoveEnd)
            {
                int ProcessCount = 0;

                for (int i = 0; i < _PalletListViewModel.Count; i++)
                {
                    if (_PalletListViewModel[i].Pallet.Count == 0)
                    {
                        _PalletListViewModel.RemoveAt(i);
                        break;
                    }
                    ProcessCount++;
                }
                if (ProcessCount == _PalletListViewModel.Count) IsRemoveEnd = true;
            }

            return View(_PalletListViewModel);
        }

        public ActionResult DeletePallet(string PalletID)
        {
            if (!string.IsNullOrEmpty(PalletID))
            {
                var pResult = m_PalletService.Delete(PalletID);

                return Json(new { Result = pResult.Success });
            }
            return Json(new { Result = false });

            //return RedirectToAction("PalletTypeList");
        }

        [HttpPost]
        public ActionResult PalletUpdate(JsonType _JsonType)
        {
            var pResult = m_PalletService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            return Json(new { Result = pResult });
            //if (pResult.Success)
            //{
            //    return RedirectToAction("PalletEdit", new { PalletID = _JsonType.pk });
            //}
            //else return RedirectToAction("PalletList");
        }

        public ActionResult PalletTypeCreate()
        {
            PalletTypeViewModel _PalletTypeViewModel = new PalletTypeViewModel();

            return View(_PalletTypeViewModel);
        }

        [HttpPost]
        public ActionResult CreatePalletType(string TypeName, string TypeCode)
        {
            string PalletTypeID = "";

            var Result = m_PalletTypeService.Create(TypeName, TypeCode, ref PalletTypeID);

            if (Result.Success)
            {
                return RedirectToAction("PalletTypeEdit", new { PalletTypeID = PalletTypeID });
            }
            else
            {
                PalletTypeViewModel _PalletTypeViewModel = new PalletTypeViewModel()
                {
                    PalletType = new CST_PALLET_TYPE() { TypeName = TypeName, TypeCode = TypeCode },
                    ErrorMessage = Result.Message
                };

                return View("PalletTypeCreate", _PalletTypeViewModel);
            }
        }

        public ActionResult PalletTypeEdit(string PalletTypeID)
        {
            if (string.IsNullOrEmpty(PalletTypeID))
            {
                return RedirectToAction("PalletTypeList");
            }
            else
            {
                var PalletType = m_PalletTypeService.GetBySID(PalletTypeID);

                if (PalletType != null)
                {
                    PalletTypeViewModel _PalletTypeViewModel = new PalletTypeViewModel()
                    {
                        PalletType = PalletType
                    };

                    return View(_PalletTypeViewModel);
                }
                else return RedirectToAction("PalletTypeList");
            }
        }

        [HttpPost]
        public ActionResult PalletTypeUpdate(JsonType _JsonType)
        {
            var pResult = m_PalletTypeService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            return Json(new { Result = pResult });

            //if (pResult.Success)
            //{
            //    return RedirectToAction("PalletTypeEdit", new { PalletTypeID = _JsonType.pk });
            //}
            //else return RedirectToAction("PalletTypeList");
        }

        public ActionResult DeletePalletType(string PalletTypeID)
        {
            if (!string.IsNullOrEmpty(PalletTypeID))
            {
                var pResult = m_PalletTypeService.Delete(PalletTypeID);

                return Json(new { Result = pResult.Success });
            }

            return Json(new { Result = false });
            //return RedirectToAction("PalletTypeList");
        }
       
        public ActionResult PalletTypeList(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var PalletTypeList = m_PalletTypeService.GetAll().ToList();

            List<PalletTypeViewModel> PalletTypeViewModelList = new List<PalletTypeViewModel>();

            foreach (var PalletType in PalletTypeList)
            {
                PalletTypeViewModelList.Add(new PalletTypeViewModel()
                {
                    PalletType = PalletType
                });
            }

            return View(PalletTypeViewModelList);
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }
    }
}