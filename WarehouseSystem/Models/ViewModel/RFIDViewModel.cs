using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class RFIDViewModel
    {
        public CST_RFID RFID = new CST_RFID();

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } 
    }
}