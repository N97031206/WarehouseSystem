using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Interface;

namespace WarehouseSystem.Controllers
{
    public class RecordController : Controller
    {
        private IRecord m_Record;

        public RecordController(IRecord p_Record)
        {
            m_Record = p_Record;
        }

        public ActionResult RecordView(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            RecordViewModel _RecordViewModel = new RecordViewModel();

            return View(_RecordViewModel);
        }

        public ActionResult Query(string Code, string Name, string StartTime, string EndTime)
        {

            RecordViewModel _RecordViewModel = new RecordViewModel();

            _RecordViewModel.Lots.AddRange(m_Record.Query(Code, Name, StartTime, EndTime).ToList());

           // return View("View", _RecordViewModel);
            return PartialView("_QueryPartial", _RecordViewModel);
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }

	}
}