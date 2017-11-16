using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IMainMenuService
    {
        /// <summary>
        /// 新增主選單
        /// </summary>
        /// <param name="MainMenuName"></param>
        /// <param name="MainMenuID"></param>
        /// <returns></returns>
        IResult Create(string MainMenuName, ref string MainMenuID);

        /// <summary>
        /// 更新主選單資料
        /// </summary>
        /// <param name="MainMenuID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string MainMenuID, string PropertyName, object Value);

        /// <summary>
        /// 刪除主選單資料
        /// </summary>
        /// <param name="MainMenuID"></param>
        /// <returns></returns>
        IResult Delete(string MainMenuID);

        /// <summary>
        /// 確認主選單資料是否存在
        /// </summary>
        /// <param name="MainMenuID"></param>
        /// <returns></returns>
        bool IsExists(string MainMenuID);

        /// <summary>
        /// 以SID取得主選單資料
        /// </summary>
        /// <param name="MainMenuID"></param>
        /// <returns></returns>
        CST_MENU_MAIN GetBySID(string MainMenuID);

        /// <summary>
        /// 取得所有主選單資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_MENU_MAIN> GetAll();

        /// <summary>
        /// 以主選單ID取得子選單
        /// </summary>
        /// <param name="MainMenuID"></param>
        /// <returns></returns>
        IEnumerable<CST_MENU_SUB> GetByMainMenuID(string MainMenuID);
    }
}