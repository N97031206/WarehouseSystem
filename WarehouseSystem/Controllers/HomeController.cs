using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Message = "<h1>歡迎使用導覽管理系統</h1>";
            Session["MainMenu"] = "";
            Session["SubMenu"] = "";
            Session["MinorMenu"] = "";

            return View();
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "<h1>歡迎使用導覽管理系統</h1>";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}