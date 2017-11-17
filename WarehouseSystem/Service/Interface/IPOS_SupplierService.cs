using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IPOS_SupplierService
    {
        /// <summary>
        /// 建立帳號
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Password"></param>
        /// <param name="SupID"></param>
        /// <returns></returns>
        IResult Create(string CompanyName, string SupID);

        /// <summary>
        /// 更新帳號資訊
        /// </summary>
        /// <param name="SupID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string SupID, string PropertyName, object Value);

        /// <summary>
        /// 刪除帳號
        /// </summary>
        /// <param name="SupID"></param>
        /// <returns></returns>
        IResult Delete(string SupID);

        ///// <summary>
        ///// 確認帳號是否存在
        ///// </summary>
        ///// <param name="SupID"></param>
        ///// <returns></returns>
        bool IsExists(string SupID);

        ///// <summary>
        ///// 以SID取得帳號資料
        ///// </summary>
        ///// <param name="SupID"></param>
        ///// <returns></returns>
        POS_Supplier GetBySID(string SupID);

        /// <summary>
        /// 取得所有帳號資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<POS_Supplier> GetAll();

        /// <summary>
        /// 登入
        /// </summary>
        ///// <param name="UserID"></param>
        ///// <param name="Password"></param>
        /// <returns></returns>
        //UserViewModel Login(string UserID, string Password);

        ///// <summary>
        ///// 取得所有選項清單
        ///// </summary>
        ///// <returns></returns>
        //IEnumerable<MenuGroupViewModel> GetMenuGroup();

        ///// <summary>
        ///// 以權限群組ID取得選項清單
        ///// </summary>
        ///// <param name="UserGroupID"></param>
        ///// <returns></returns>
        //IEnumerable<MenuGroupViewModel> GetMenuGroup(string UserGroupID);

        ///// <summary>
        ///// 修改密碼
        ///// </summary>
        ///// <param name="Password"></param>
        ///// <param name="ModifyPassword"></param>
        ///// <param name="SupID"></param>
        ///// <returns></returns>
        //IResult ChangePassword(string Password, string ModifyPassword, string SupID);
    }
}