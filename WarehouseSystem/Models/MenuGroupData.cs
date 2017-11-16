using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models
{
    public class MenuGroupData
    {
        public string MenuGroupID { get; set; }

        public string UserGroupID { get; set; }

        public string MainMenuID { get; set; }

        public string MainMenuName { get; set; }

        public string SubMenuID { get; set; }

        public string SubMenuName { get; set; }

        public string MinorMenuID { get; set; }

        public string MinorMenuName { get; set; }

        public int Active { get; set; }

        public string MainAction { get; set; }

        public string MainController { get; set; }

        public string MainOrder { get; set; }

        public string SubAction { get; set; }

        public string SubController { get; set; }

        public string SubOrder { get; set; }

        public string MinorAction { get; set; }

        public string MinorController { get; set; }

        public string MinorOrder { get; set; }
    }
}