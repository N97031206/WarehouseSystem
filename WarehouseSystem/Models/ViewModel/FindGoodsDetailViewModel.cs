using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class FindGoodsDetailViewModel
    {
        public string OrderNo { get; set; }
        public string WKNo { get; set; }
        public string LotNo { get; set; }
        public string InternalOrder { get; set; }
        public string Fabric { get; set; }
        public string Color { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string PLot { get; set; }
        public string Status { get; set; }

        public List<vw_Lots> FindGoodsDetail = new List<vw_Lots>();
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}