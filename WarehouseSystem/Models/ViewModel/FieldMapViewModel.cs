using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class FieldMapViewModel
    {
        public CST_FIELD_MAP FieldMap = new CST_FIELD_MAP();

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } 
    }
}