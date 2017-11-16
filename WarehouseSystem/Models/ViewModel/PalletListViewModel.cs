using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class PalletListViewModel
    {
        public List<CST_PALLET> Pallet = new List<CST_PALLET>();

        public string PalletTypeName { get; set; }

        public string PalletTypeCode { get; set; }

        public string PalletTypeID { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}