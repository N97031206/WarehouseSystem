using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class FieldMapEditViewModel
    {
        public CST_FIELD_MAP FieldMap = new CST_FIELD_MAP();

        public List<CST_STORAGE_PARKINGBLOCK> ParkingBlockList = new List<CST_STORAGE_PARKINGBLOCK>();

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } 
    }
}