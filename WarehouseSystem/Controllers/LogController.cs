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
    public class LogController : Controller
    {
        private IWarehouseService m_WarehouseService;
        private IRFIDErrorMessage m_RFIDErrorMessage;

        public LogController(IWarehouseService p_WarehouseService, IRFIDErrorMessage p_RFIDErrorMessage)
        {
            m_WarehouseService = p_WarehouseService;
            m_RFIDErrorMessage = p_RFIDErrorMessage;
        }

        public ActionResult LogView(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            LogViewModel _LogViewModel = new LogViewModel();
            
            return View(_LogViewModel);
        }

        
        public ActionResult Query(string PLot, string Fabric, string PalletNumber, string ParkingBlock, string InternalOrder, string Status, string StartTime, string EndTime, string LotNo, string ColorCode)
        {
            LogViewModel _LogViewModel = new LogViewModel();

            var Lots = m_WarehouseService.Query(PLot, Fabric, PalletNumber, ParkingBlock, InternalOrder, Status, StartTime, EndTime, LotNo, ColorCode).ToList();

            if (Lots.Count > 0)
            {
                _LogViewModel.Lots.AddRange(Lots);
                _LogViewModel.ErrorMessage = "查詢結果：[ 共" + Lots.Count.ToString() + " 筆資料 ]";
            }
            else
            {
                _LogViewModel.ErrorMessage = "查詢結果：[ 無資料 ]";
            }

            //return View("LogView", _LogViewModel);
            return PartialView("_LogViewPartial", _LogViewModel);
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }

        public ActionResult RFIDLogView(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            RFIDErrorViewModel _RFIDErrorViewModel = new RFIDErrorViewModel();

            return View(_RFIDErrorViewModel);
        }

        
        public ActionResult RFIDLogQuery(string Lot, string OrderNo, string StartTime, string EndTime)
        {
            RFIDErrorViewModel _RFIDErrorViewModel = new RFIDErrorViewModel();

            var Logs = m_RFIDErrorMessage.Query(Lot, OrderNo, StartTime, EndTime).ToList();

            if (Logs.Count > 0)
            {
                _RFIDErrorViewModel.Logs.AddRange(Logs);
                _RFIDErrorViewModel.ErrorMessage = "查詢結果：[ 共" + Logs.Count.ToString() + " 筆資料 ]";
            }
            else
            {
                _RFIDErrorViewModel.ErrorMessage = "查詢結果：[ 無資料 ]";
            }

            //return View("RFIDLogView", _RFIDErrorViewModel);
            return PartialView("_RFIDLogPartial", _RFIDErrorViewModel);
        }
    }
}