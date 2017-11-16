using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class PalletEditViewModel
    {
        public CST_PALLET Pallet = new CST_PALLET();

        public List<CST_PALLET_TYPE> PalletTypeList = new List<CST_PALLET_TYPE>();

        public List<CST_RFID> RFIDList = new List<CST_RFID>();

        public string PalletTypeName { get; set; }

        public string PalletTypeCode { get; set; }

        public string RFIDNumber { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }

    }
}