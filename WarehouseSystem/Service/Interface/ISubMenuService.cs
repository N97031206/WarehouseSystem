using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface ISubMenuService
    {
        /// <summary>
        /// 新增子選單資料
        /// </summary>
        /// <param name="SubMenuName"></param>
        /// <param name="MainMenuID"></param>
        /// <param name="SubMenuID"></param>
        /// <returns></returns>
        IResult Create(string SubMenuName, string MainMenuID, ref string SubMenuID);

        /// <summary>
        /// 更新子選單資料
        /// </summary>
        /// <param name="SubMenuID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string SubMenuID, string PropertyName, object Value);

        /// <summary>
        /// 刪除子選單資料
        /// </summary>
        /// <param name="SubMenuID"></param>
        /// <returns></returns>
        IResult Delete(string SubMenuID);

        /// <summary>
        /// 確認子選單資料是否存在
        /// </summary>
        /// <param name="SubMenuID"></param>
        /// <returns></returns>
        bool IsExists(string SubMenuID);

        /// <summary>
        /// 以SID取得子選單資料
        /// </summary>
        /// <param name="SubMenuID"></param>
        /// <returns></returns>
        CST_MENU_SUB GetBySID(string SubMenuID);

        /// <summary>
        /// 取得所有子選單資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_MENU_SUB> GetAll();

        /// <summary>
        /// 以子選單ID取得次選單資料
        /// </summary>
        /// <param name="SubMenuID"></param>
        /// <returns></returns>
        IEnumerable<CST_MENU_MINOR> GetBySubMenuID(string SubMenuID);

        /// <summary>
        /// 以主選單ID取得主選單資料
        /// </summary>
        /// <param name="MainMenuID"></param>
        /// <returns></returns>
        IEnumerable<CST_MENU_MAIN> GetByMainMenuID(string MainMenuID);
    }
}