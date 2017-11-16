using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class SubMinorViewModel
    {
        public CST_MENU_SUB SubMenu = new CST_MENU_SUB();

        public List<CST_MENU_MINOR> MinorMenuList = new List<CST_MENU_MINOR>();

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } 
    }
}