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
    public interface IUserGroupService
    {
        /// <summary>
        /// 新增權限群組
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="UserGroupID"></param>
        /// <returns></returns>
        IResult Create(string GroupName, ref string UserGroupID);

        /// <summary>
        /// 更新權限群組資料
        /// </summary>
        /// <param name="UserGroupID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string UserGroupID, string PropertyName, object Value);

        /// <summary>
        /// 刪除權限群組資料
        /// </summary>
        /// <param name="UserGroupID"></param>
        /// <returns></returns>
        IResult Delete(string UserGroupID);

        /// <summary>
        /// 確認權限群組資料是否存在
        /// </summary>
        /// <param name="UserGroupID"></param>
        /// <returns></returns>
        bool IsExists(string UserGroupID);

        /// <summary>
        /// 以SID取得權限群組資料
        /// </summary>
        /// <param name="UserGroupID"></param>
        /// <returns></returns>
        CST_USER_GRP GetBySID(string UserGroupID);

        /// <summary>
        /// 取得所有權限群組資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_USER_GRP> GetAll();

        /// <summary>
        /// 以權限群組ID取得選單清單
        /// </summary>
        /// <param name="UserGroupID"></param>
        /// <returns></returns>
        IEnumerable<CST_MENU_GRP> GetByUserGroupID(string UserGroupID);

        /// <summary>
        /// 寫入選單清單
        /// </summary>
        /// <param name="_MenuGroupViewModel"></param>
        /// <param name="UserGroupID"></param>
        /// <returns></returns>
        IResult Insert(List<MenuGroupViewModel> _MenuGroupViewModel, string UserGroupID);

        /// <summary>
        /// 取得選單樹狀節點
        /// </summary>
        /// <param name="UserGroupID"></param>
        /// <returns></returns>
        IEnumerable<JObject> GetTreeNode(string UserGroupID);
    }
}