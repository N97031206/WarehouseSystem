using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IGateReaderService
    {
        /// <summary>
        /// 新增接收器編號
        /// </summary>
        /// <param name="GateReaderNumber"></param>
        /// <param name="GateReaderID"></param>
        /// <returns></returns>
        IResult Create(string GateReaderNumber, ref string GateReaderID);

        /// <summary>
        /// 更新接收器資料
        /// </summary>
        /// <param name="GateReaderID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string GateReaderID, string PropertyName, object Value);

        /// <summary>
        /// 刪除接收器資料
        /// </summary>
        /// <param name="GateReaderID"></param>
        /// <returns></returns>
        IResult Delete(string GateReaderID);

        /// <summary>
        /// 確認接收器資料是否存在
        /// </summary>
        /// <param name="GateReaderID"></param>
        /// <returns></returns>
        bool IsExists(string GateReaderID);

        /// <summary>
        /// 以SID取得接收器資料
        /// </summary>
        /// <param name="GateReaderID"></param>
        /// <returns></returns>
        CST_GATE_READER GetBySID(string GateReaderID);

        /// <summary>
        /// 取得所有接收器資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_GATE_READER> GetAll();
    }
}