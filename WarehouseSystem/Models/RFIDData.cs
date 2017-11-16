using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models
{
    public class RFIDData
    {
        /// <summary>
        /// RFID編號
        /// </summary>
        public string RFID { get; set; }

        /// <summary>
        /// 是否對比資料處理過
        /// </summary>
        public bool IsProcessed { get; set; }

        /// <summary>
        /// 對比資料成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }
        
    }
}