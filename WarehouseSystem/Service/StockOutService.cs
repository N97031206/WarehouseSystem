using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseSystem.Models;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service
{
    public class StockOutService : IStockOutService
    {
        private IRepository<CST_STOCK_OUT> _repository;


        public StockOutService(IRepository<CST_STOCK_OUT> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 依據RFID清單取得所有出庫單批號資料
        /// </summary>
        /// <param name="rfidList"></param>
        /// <returns></returns>
        public IEnumerable<CST_STOCK_OUT> GetDataByRFIDList(List<string> rfidList)
        {
            string sqlString = "";
            string addINString = "";

            sqlString = string.Format(@"SELECT * FROM CST_STOCK_OUT ");

            addINString = " WHERE RFID IN ( ";

            for (int i = 0; i < rfidList.Count; i++)
            {
                if (i == 0) addINString += "'" + rfidList[i] + "'";
                else addINString += ", '" + rfidList[i] + "'";
            }

            addINString += " )";

            sqlString += addINString;

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var dataList = db.Database.SqlQuery<CST_STOCK_OUT>(sqlString).ToList();

                return dataList;
            }
        }

        /// <summary>
        /// 依據SID取得出庫單批號資料
        /// </summary>
        /// <param name="stockOutID"></param>
        /// <returns></returns>
        public CST_STOCK_OUT GetBySID(int stockOutID)
        {
            return _repository.Get(x => x.StockOutID == stockOutID);
        }

        /// <summary>
        /// 依據布疋RFID取得出庫單批號資料
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns></returns>
        public CST_STOCK_OUT GetDataByRFID(string rfid)
        {
            return _repository.Get(x => x.RFID == rfid);
        }


        /// <summary>
        /// 依據出庫單號清單取得所有出庫單批號資料
        /// </summary>
        /// <param name="stockOutNoList"></param>
        /// <returns></returns>
        public IEnumerable<CST_STOCK_OUT> GetDataByStockOutNoList(List<string> stockOutNoList)
        {
            string sqlString = "";
            string addINString = "";

            sqlString = string.Format(@"SELECT * FROM CST_STOCK_OUT ");

            addINString = " WHERE STOCKOUTNO IN ( ";

            for (int i = 0; i < stockOutNoList.Count; i++)
            {
                if (i == 0) addINString += "'" + stockOutNoList[i] + "'";
                else addINString += ", '" + stockOutNoList[i] + "'";
            }

            addINString += " )";

            sqlString += addINString;

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var dataList = db.Database.SqlQuery<CST_STOCK_OUT>(sqlString).ToList();

                return dataList;
            }
        }

        /// <summary>
        /// 依據SID刪除資料
        /// </summary>
        /// <param name="data"></param>
        public void DeleteBySID(int SID)
        {
            var data = GetBySID(SID);

            _repository.Delete(data);
        }
    }
}