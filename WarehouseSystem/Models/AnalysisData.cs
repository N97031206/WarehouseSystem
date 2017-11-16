using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models
{
    public class AnalysisData
    {
        /// <summary>
        /// RFID清單
        /// </summary>
        public List<RFIDData> rfidList { get; set; }
        /// <summary>
        /// 棧板RFID
        /// </summary>
        public string palletRFID { get; set; }

        /// <summary>
        /// 閘門ID
        /// </summary>
        public string readerNumber { get; set; }
    }
}