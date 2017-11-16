using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models
{
    public class RFIDErrorMessageInfo
    {
        /// <summary>
        /// 棧板RFID
        /// </summary>
        public string PalletRFID { get; set; }

        /// <summary>
        /// 閘門編號
        /// </summary>
        public string GateReaderNumber { get; set; }

        /// <summary>
        /// 布批號 (工令 + 疋號)
        /// </summary>
        public string Lot { get; set; }

        /// <summary>
        /// 入庫單/出庫單
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 疋布RFID
        /// </summary>
        public string RFID { get; set; }

        /// <summary>
        /// 上傳類別 (Mapping / UpLoad)
        /// </summary>
        public string UpLoadType { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// 備注
        /// </summary>
        public string Remark { get; set; }
    }
}