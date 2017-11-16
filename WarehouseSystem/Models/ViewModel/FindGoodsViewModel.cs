using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class FindGoodsViewModel
    {
        public CST_SER_INV Order = new CST_SER_INV();

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } 
    }
}