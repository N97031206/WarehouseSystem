using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service
{
    public class FindGoodsService : IFindGoodsService
    {
        private IRepository<CST_SER_INV> _repository;

        public FindGoodsService(IRepository<CST_SER_INV> repository)
        {
            _repository = repository;
        }

        public IResult Create( ref string OrderID)
        {
            IResult pResult = new Result(false);

            CST_SER_INV Order = new CST_SER_INV();

            try
            {
                IdGenerator idg = new IdGenerator();
                OrderID = idg.GetSID();

                string OrderNo = GetNo();

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                Order.OrderID = OrderID;
                Order.OrderNo = OrderNo;
                Order.CreateTime = InsertTime;
                Order.LastUpdateTime = InsertTime;

                _repository.Create(Order);

                pResult.Success = true;
            }
            catch (Exception ex)
            {
                pResult.Exception = ex;

                //2627 主鍵重複
                //2601 唯一索引重複
                var ErrorCode = ((System.Data.SqlClient.SqlException)((ex.InnerException).InnerException)).Number;
                if (ErrorCode == 2627) pResult.Message = "SID重複";
                if (ErrorCode == 2601) pResult.Message = "RFID編號已被使用，請重新申請";
            }

            return pResult;

        }

        public string GetNo()
        {
            string sqlString = "";

            var _Date = System.DateTime.Now.ToString("yyyy/MM/dd");

            string OrderNo = System.DateTime.Now.ToString("yyyyMMdd");

            sqlString = string.Format(@"Select TOP 1 * From CST_SER_INV Where CreateTime >= '{0}' AND CreateTime <= '{1}' Order by CreateTime desc;", _Date + " 00:00:00", _Date + " 23:59:59");

            try
            {
                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    var _List = db.Database.SqlQuery<CST_SER_INV>(sqlString).ToList();

                    if (_List.Count > 0)
                    {
                        var No = Convert.ToInt32(_List[0].OrderNo.Substring(10, 4));
                        OrderNo = "F" + OrderNo + "-" + String.Format("{0:0000}", (No + 1));
                    }
                    else
                    {
                        OrderNo = "F" + OrderNo + "-0001";
                    }
                }
            }
            catch (Exception ex)
            {
                _Date = "";
            }

            return OrderNo;
        }

        public bool IsExists(string OrderID)
        {
            return _repository.GetAll().Any(x => x.OrderID == OrderID);
        }

        public CST_SER_INV GetBySID(string OrderID)
        {
            return _repository.Get(x => x.OrderID == OrderID);
        }

        public IEnumerable<CST_SER_INV> GetAll()
        {
            return _repository.GetAll();
        }

        public CST_SER_INV GetByOrderNo(string OrderNo)
        {
            return _repository.Get(x => x.OrderNo == OrderNo);
        }

        public IEnumerable<vw_Lots> Query(string WKNo, string LotNo, string InternalOrder, string Fabric, string Color, string StartTime, string EndTime, string PLot, string Status)
        {
            string sqlString = "";
            string WhereString = "";

            List<string> _List = new List<string>();


            if (!string.IsNullOrEmpty(PLot)) _List.Add(" PLot = '" + PLot + "'");
            if (!string.IsNullOrEmpty(WKNo)) _List.Add(" WKNo = '" + WKNo + "'");
            if (!string.IsNullOrEmpty(LotNo)) _List.Add(" LotNo = '" + LotNo + "'");
            if (!string.IsNullOrEmpty(InternalOrder)) _List.Add(" InternalOrder = '" + InternalOrder + "'");
            if (!string.IsNullOrEmpty(Fabric)) _List.Add(" Fabric = '" + Fabric + "'");
            if (!string.IsNullOrEmpty(Color)) _List.Add(" Color = '" + Color + "'");
            if (!string.IsNullOrEmpty(Status)) _List.Add(" Status = '" + Status + "'");
            if (!string.IsNullOrEmpty(StartTime)) _List.Add(" StockInTime >= '" + StartTime + " 00:00:00'");
            if (!string.IsNullOrEmpty(EndTime)) _List.Add(" StockInTime <= '" + EndTime + " 23:59:59'");

            for (int i = 0; i < _List.Count; i++)
            {
                if (i != 0)
                {
                    WhereString += " AND " + _List[i];
                }
                else
                {
                    WhereString += "Where " + _List[i];
                }
            }

            sqlString = "Select * From vw_Lots CW " + WhereString + " ;";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _Lots = db.Database.SqlQuery<vw_Lots>(sqlString).ToList();

                return _Lots;
            }
        }
    }
}