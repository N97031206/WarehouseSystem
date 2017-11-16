using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class UserGroupViewModel
    {
        public CST_USER_GRP UserGroup = new CST_USER_GRP();

        public List<CST_MENU_GRP> MenuGroup = new List<CST_MENU_GRP>();


        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}