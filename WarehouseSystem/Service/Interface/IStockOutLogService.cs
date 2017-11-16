using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Models;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IStockOutLogService
    {
        /// <summary>
        /// 依據SID取得出庫單批號資料
        /// </summary>
        /// <param name="stockOutID"></param>
        /// <returns></returns>
        CST_STOCK_OUT_LOG GetBySID(string stockOutLogID);

        /// <summary>
        /// 依據布疋RFID取得出庫單批號資料
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns></returns>
        CST_STOCK_OUT_LOG GetDataByRFID(string rfid);

        /// <summary>
        /// 依據RFID清單取得所有出庫單批號資料
        /// </summary>
        /// <param name="rfidList"></param>
        /// <returns></returns>
        IEnumerable<CST_STOCK_OUT_LOG> GetDataByRFIDList(List<string> rfidList);

        /// <summary>
        /// 依據出庫單號清單取得所有出庫單批號資料
        /// </summary>
        /// <param name="stockOutNoList"></param>
        /// <returns></returns>
        IEnumerable<CST_STOCK_OUT_LOG> GetDataByStockOutNoList(List<string> stockOutNoList);

        /// <summary>
        /// 新增一筆資料
        /// </summary>
        /// <param name="data"></param>
        void Insert(CST_STOCK_OUT_LOG data);

    }
}