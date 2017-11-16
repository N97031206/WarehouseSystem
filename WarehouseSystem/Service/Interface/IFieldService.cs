using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IFieldService
    {
        /// <summary>
        /// 新增域區
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="FieldID"></param>
        /// <returns></returns>
        IResult Create(string FieldName, ref string FieldID);

        /// <summary>
        /// 更新域區
        /// </summary>
        /// <param name="FieldID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string FieldID, string PropertyName, object Value);

        /// <summary>
        /// 刪除域區
        /// </summary>
        /// <param name="FieldID"></param>
        /// <returns></returns>
        IResult Delete(string FieldID);

        /// <summary>
        /// 確認域區是否存在
        /// </summary>
        /// <param name="FieldID"></param>
        /// <returns></returns>
        bool IsExists(string FieldID);

        /// <summary>
        ///  以SID取得域區資料
        /// </summary>
        /// <param name="FieldID"></param>
        /// <returns></returns>
        CST_FIELD_MAP GetBySID(string FieldID);

        /// <summary>
        /// 取得所有域區資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_FIELD_MAP> GetAll();
    }
}