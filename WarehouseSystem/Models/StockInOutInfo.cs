using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models
{
    public class StockInOutInfo
    {
        /// <summary>
        /// 布種名稱
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 棧板RFID編號
        /// </summary>
        public string PalletRFID { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// 閘門編號
        /// </summary>
        public string GateReaderNumber { get; set; }

        /// <summary>
        /// 工單編號
        /// </summary>
        public string WO { get; set; }

        /// <summary>
        /// 布批號(工令 + 疋號)
        /// </summary>
        public string Lot { get; set; }

        /// <summary>
        /// 入庫單號
        /// </summary>
        public string StockIn { get; set; }

        /// <summary>
        /// 出庫單號
        /// </summary>
        public string StockOut { get; set; }

        /// <summary>
        /// 布疋RFID
        /// </summary>
        public string RFID { get; set; }

        /// <summary>
        /// 工令
        /// </summary>
        public string WKNo { get; set; }

        /// <summary>
        /// 缸號
        /// </summary>
        public string LotNo { get; set; }

        /// <summary>
        /// 訂單
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// 布別
        /// </summary>
        public string Fabric { get; set; }

        /// <summary>
        /// 顏色
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 幅寬
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 碼重
        /// </summary>
        public string YdWt { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// 碼數
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// 疋號
        /// </summary>
        public string PNo { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 內部訂號
        /// </summary>
        public string InternalOrder { get; set; }

        /// <summary>
        /// 顏色代碼
        /// </summary>
        public string ColorCode { get; set; }
    }
}