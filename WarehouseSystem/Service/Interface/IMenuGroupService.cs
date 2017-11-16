using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IMenuGroupService
    {
        /// <summary>
        /// 更新登入選單資料
        /// </summary>
        /// <param name="MenuGroupID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string MenuGroupID, string PropertyName, object Value);

        /// <summary>
        /// 刪除登入選單資料
        /// </summary>
        /// <param name="MenuGroupID"></param>
        /// <returns></returns>
        IResult Delete(string MenuGroupID);

        /// <summary>
        /// 確認登入選單資料是否存在
        /// </summary>
        /// <param name="MenuGroupID"></param>
        /// <returns></returns>
        bool IsExists(string MenuGroupID);

        /// <summary>
        /// 以SID取得登入選單資料
        /// </summary>
        /// <param name="MenuGroupID"></param>
        /// <returns></returns>
        CST_MENU_GRP GetBySID(string MenuGroupID);

        /// <summary>
        /// 取得所有登入選單資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_MENU_GRP> GetAll();
    }
}