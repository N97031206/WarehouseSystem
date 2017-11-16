using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class MainMenuViewModel
    {
        public CST_MENU_MAIN MainMenu = new CST_MENU_MAIN();

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } 
    }
}