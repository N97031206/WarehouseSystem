using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IPalletService
    {
        /// <summary>
        /// 新增棧板編號
        /// </summary>
        /// <param name="PalletNumber"></param>
        /// <param name="PalletID"></param>
        /// <returns></returns>
        IResult Create(string PalletNumber, string RFIDID, ref string PalletID);
        IResult Create(string PalletNumber, string RFIDID, string PalletTypeID, ref string PalletID);

        /// <summary>
        /// 刪除棧板編號
        /// </summary>
        /// <param name="PalletID"></param>
        /// <returns></returns>
        IResult Delete(string PalletID);

        /// <summary>
        /// 更新棧板資料
        /// </summary>
        /// <param name="PalletID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string PalletID, string PropertyName, object Value);

        /// <summary>
        /// 確認棧板是否存在
        /// </summary>
        /// <param name="PalletTypeID"></param>
        /// <returns></returns>
        bool IsExists(string PalletID);

        /// <summary>
        /// 以SID取得棧板資料
        /// </summary>
        /// <param name="PalletTypeID"></param>
        /// <returns></returns>
        CST_PALLET GetBySID(string PalletID);

        /// <summary>
        /// 取得所有棧板資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_PALLET> GetAll();

        /// <summary>
        /// 以棧板編號取得資料
        /// </summary>
        /// <param name="PalletNumber"></param>
        /// <returns></returns>
        CST_PALLET GetByRFIDID(string RFIDID);
    }
}