using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class MenuGroupViewModel
    {
        public CST_MENU_MAIN MainMenu = new CST_MENU_MAIN();

        public List<SubMenuItem> SubMenuList = new List<SubMenuItem>();

        public class SubMenuItem
        {
            public CST_MENU_SUB SubMenu = new CST_MENU_SUB();

            public List<CST_MENU_MINOR> MinorMenuList = new List<CST_MENU_MINOR>();
        }
    }
}