using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IPalletTypeService
    {
        /// <summary>
        /// 新增棧板類別
        /// </summary>
        /// <param name="TypeName"></param>
        /// <param name="TypeCode"></param>
        /// <param name="PalletTypeID"></param>
        /// <returns></returns>
        IResult Create(string TypeName, string TypeCode, ref string PalletTypeID);

        /// <summary>
        /// 刪除棧板類別
        /// </summary>
        /// <param name="PalletTypeID"></param>
        /// <returns></returns>
        IResult Delete(string PalletTypeID);

        /// <summary>
        /// 更新棧板類別
        /// </summary>
        /// <param name="PalletTypeID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string PalletTypeID, string PropertyName, object Value);

        /// <summary>
        /// 確認棧板類別是否存在
        /// </summary>
        /// <param name="PalletTypeID"></param>
        /// <returns></returns>
        bool IsExists(string PalletTypeID);

        /// <summary>
        /// 以SID取得棧板類別資料
        /// </summary>
        /// <param name="PalletTypeID"></param>
        /// <returns></returns>
        CST_PALLET_TYPE GetBySID(string PalletTypeID);

        /// <summary>
        /// 取得所有棧板類別資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_PALLET_TYPE> GetAll();

        CST_PALLET_TYPE GetByTypeName(string TypeName);
    }
}