using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class POS_SupplierViewModel
    {
        /// <summary>
        /// 帳號資料
        /// </summary>
        public POS_Supplier SupplierProfile = new POS_Supplier();

        /// <summary>
        /// 權限群組資料
        /// </summary>
        public List<CST_USER_GRP> UserGroup = new List<CST_USER_GRP>();

        public string UserGroupName { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}