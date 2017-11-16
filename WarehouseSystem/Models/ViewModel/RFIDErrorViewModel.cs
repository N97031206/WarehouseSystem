using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class RFIDErrorViewModel
    {
        public List<vw_RFIDLog> Logs = new List<vw_RFIDLog>();
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}