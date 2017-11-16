using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class MainSubViewModel
    {
        public CST_MENU_MAIN MainMenu = new CST_MENU_MAIN();

        public List<CST_MENU_SUB> SubMenuList = new List<CST_MENU_SUB>();

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } 
    }
}