using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class InventoryDetailViewModel
    {
        public List<string> List { get; set; }
        public string OrderNo { get; set; }

        public string OrderID { get; set; }

        public List<CST_INV_DTL> InventoryDetail = new List<CST_INV_DTL>();
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}