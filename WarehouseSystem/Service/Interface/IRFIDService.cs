using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IRFIDService
    {
        /// <summary>
        /// 新增RFID編號
        /// </summary>
        /// <param name="RFIDNumber"></param>
        /// <param name="RFIDID"></param>
        /// <returns></returns>
        IResult Create(string RFIDNumber, ref string RFIDID);

        /// <summary>
        /// 更新RFID資料
        /// </summary>
        /// <param name="RFIDID"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        IResult Update(string RFIDID, string PropertyName, object Value);

        /// <summary>
        /// 刪除RFID資料
        /// </summary>
        /// <param name="RFIDID"></param>
        /// <returns></returns>
        IResult Delete(string RFIDID);

        /// <summary>
        /// 確認RFID資料是否存在
        /// </summary>
        /// <param name="RFIDID"></param>
        /// <returns></returns>
        bool IsExists(string RFIDID);

        /// <summary>
        /// 以SID取得RFID資料
        /// </summary>
        /// <param name="RFIDID"></param>
        /// <returns></returns>
        CST_RFID GetBySID(string RFIDID);

        /// <summary>
        /// 取得所有RFID資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_RFID> GetAll();

        CST_RFID GetByNumber(string RFIDNumber);

        IEnumerable<CST_RFID> GetRFIDNo();
    }
}