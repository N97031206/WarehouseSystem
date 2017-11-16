using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models
{
    public class TagInfo
    {
        /// <summary>
        /// WebAPI回傳訊息
        /// </summary>
        public string Messae { get; set; }

        /// <summary>
        /// 是否上傳資料庫
        /// </summary>
        public bool IsUpLoad { get; set; }

        /// <summary>
        /// 讀取資料的時間
        /// </summary>
        public DateTime ReadTime { get; set; }

        /// <summary>
        /// ERP比對結果
        /// </summary>
        public bool IsFind { get; set; }

        /// <summary>
        /// 是否上傳異常Log
        /// </summary>
        public bool IsSaveLog { get; set; }

        public StockInOutInfo LotInfo = new StockInOutInfo();
    }
}