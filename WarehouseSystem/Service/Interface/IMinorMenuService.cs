using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IMinorMenuService
    {
        /// <summary>
        /// 新增次選單資料
        /// </summary>
        /// <param name="MinorMenuName"></param>
        /// <param name="SubMenuID"></param>
        /// <param name="MinorMenuID"></param>
        /// <returns></returns>
        IResult Create(string MinorMenuName, string SubMenuID, ref string MinorMenuID);

        /// <summary>
        /// 更新次選單資料
        /// </summary>
        /// <param name="MinorMenuID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string MinorMenuID, string PropertyName, object Value);

        /// <summary>
        /// 刪除次選單資料
        /// </summary>
        /// <param name="MinorMenuID"></param>
        /// <returns></returns>
        IResult Delete(string MinorMenuID);

        /// <summary>
        /// 確認次選單資料是否存在
        /// </summary>
        /// <param name="MinorMenuID"></param>
        /// <returns></returns>
        bool IsExists(string MinorMenuID);

        /// <summary>
        /// 以SID取得次選單資料
        /// </summary>
        /// <param name="MinorMenuID"></param>
        /// <returns></returns>
        CST_MENU_MINOR GetBySID(string MinorMenuID);

        /// <summary>
        /// 取得所有次選單資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_MENU_MINOR> GetAll();

        /// <summary>
        /// 以子選單ID取得子選單資料
        /// </summary>
        /// <param name="SubMenuID"></param>
        /// <returns></returns>
        IEnumerable<CST_MENU_SUB> GetBySubMenuID(string SubMenuID);
    }
}