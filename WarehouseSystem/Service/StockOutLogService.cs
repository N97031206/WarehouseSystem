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
    public class StockOutLogService : IStockOutLogService
    {
        private IRepository<CST_STOCK_OUT_LOG> _repository;


        public StockOutLogService(IRepository<CST_STOCK_OUT_LOG> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 新增一筆資料
        /// </summary>
        /// <param name="data"></param>
        public void Insert(CST_STOCK_OUT_LOG data)
        {
            IdGenerator idg = new IdGenerator();
            var ID = idg.GetSID(1);

            data.StockOutLogID = ID;

            _repository.Create(data);

        }

        /// <summary>
        /// 依據RFID清單取得所有出庫單批號資料
        /// </summary>
        /// <param name="rfidList"></param>
        /// <returns></returns>
        public IEnumerable<CST_STOCK_OUT_LOG> GetDataByRFIDList(List<string> rfidList)
        {
            string sqlString = "";
            string addINString = "";

            sqlString = string.Format(@"SELECT * FROM CST_STOCK_OUT_LOG ");

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
                var dataList = db.Database.SqlQuery<CST_STOCK_OUT_LOG>(sqlString).ToList();

                return dataList;
            }
        }

        /// <summary>
        /// 依據SID取得出庫單批號資料
        /// </summary>
        /// <param name="stockOutID"></param>
        /// <returns></returns>
        public CST_STOCK_OUT_LOG GetBySID(string stockOutLogID)
        {
            return _repository.Get(x => x.StockOutLogID == stockOutLogID);
        }

        /// <summary>
        /// 依據布疋RFID取得出庫單批號資料
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns></returns>
        public CST_STOCK_OUT_LOG GetDataByRFID(string rfid)
        {
            return _repository.Get(x => x.RFID == rfid);
        }


        /// <summary>
        /// 依據出庫單號清單取得所有出庫單批號資料
        /// </summary>
        /// <param name="stockOutNoList"></param>
        /// <returns></returns>
        public IEnumerable<CST_STOCK_OUT_LOG> GetDataByStockOutNoList(List<string> stockOutNoList)
        {
            string sqlString = "";
            string addINString = "";

            sqlString = string.Format(@"SELECT * FROM CST_STOCK_OUT_LOG ");

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
                var dataList = db.Database.SqlQuery<CST_STOCK_OUT_LOG>(sqlString).ToList();

                return dataList;
            }
        }
    }
}