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
    public class InventoryService : IInventoryService
    {
        private IRepository<CST_INV> _repository;

        public InventoryService(IRepository<CST_INV> repository)
        {
            _repository = repository;
        }

        public IResult Create(ref string OrderID)
        {
            IResult pResult = new Result(false);

            CST_INV Order = new CST_INV();

            var OrderNo = GetNo();

            try
            {
                IdGenerator idg = new IdGenerator();
                OrderID = idg.GetSID();

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

        public IResult Update(string OrderID)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(OrderID);

            if (instance == null) throw new ArgumentNullException();

            IResult Result = new Result(false);

            try
            {
                var LastUpdateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                DicUpdate.Add("LastUpdateTime", LastUpdateTime);

                _repository.Update(instance, DicUpdate);

                Result.Success = true;
                Result.LastUpdateTime = LastUpdateTime;
            }
            catch (Exception ex)
            {
                Result.Exception = ex;
            }

            return Result;
        }

        public IResult Delete(string OrderID)
        {
            IResult result = new Result(false);

            if (!IsExists(OrderID)) result.Message = "找不到單據資料";
            else
            {
                try
                {
                    var instance = GetBySID(OrderID);
                    _repository.Delete(instance);
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                }
            }
            return result;
        }

        public string GetNo()
        {
            string sqlString = "";

            var _Date = System.DateTime.Now.ToString("yyyy/MM/dd");

            string OrderNo = System.DateTime.Now.ToString("yyyyMMdd");

            sqlString = string.Format(@"Select TOP 1 * From CST_INV Where CreateTime >= '{0}' AND CreateTime <= '{1}' Order by CreateTime desc;", _Date + " 00:00:00", _Date + " 23:59:59");

            try
            {
                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    var _List = db.Database.SqlQuery<CST_INV>(sqlString).ToList();

                    if (_List.Count > 0)
                    {
                        var No = Convert.ToInt32(_List[0].OrderNo.Substring(10, 4));
                        OrderNo = "I" + OrderNo + "-" + String.Format("{0:0000}", (No + 1));
                    }
                    else
                    {
                        OrderNo = "I" + OrderNo + "-0001";
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

        public CST_INV GetBySID(string OrderID)
        {
            return _repository.Get(x => x.OrderID == OrderID);
        }

        public IEnumerable<CST_INV> GetAll()
        {
            return _repository.GetAll();
        }

        public CST_INV GetByOrderNo(string OrderNo)
        {
            return _repository.Get(x => x.OrderNo == OrderNo);
        }
    }
}