using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseSystem.Models;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service
{
    public class RFIDErrorMessage : IRFIDErrorMessage
    {
        private IRepository<CST_RFID_ERROR_LOG> _repository;

        public RFIDErrorMessage(IRepository<CST_RFID_ERROR_LOG> repository)
        {
            _repository = repository;
        }

        public IResult Create(RFIDErrorMessageInfo _RFIDErrorMessageInfo)
        {
            IResult pResult = new Result(false);

            CST_RFID_ERROR_LOG _CST_RFID_ERROR_LOG = new CST_RFID_ERROR_LOG();

            try
            {
                _CST_RFID_ERROR_LOG.Lot = _RFIDErrorMessageInfo.Lot;
                _CST_RFID_ERROR_LOG.RFID = _RFIDErrorMessageInfo.RFID;
                _CST_RFID_ERROR_LOG.UpLoadType = _RFIDErrorMessageInfo.UpLoadType;
                _CST_RFID_ERROR_LOG.ErrorMessage = _RFIDErrorMessageInfo.ErrorMessage;
                _CST_RFID_ERROR_LOG.GateReaderNumber = _RFIDErrorMessageInfo.GateReaderNumber;
                _CST_RFID_ERROR_LOG.OrderNo = _RFIDErrorMessageInfo.OrderNo;
                _CST_RFID_ERROR_LOG.PalletRFID = _RFIDErrorMessageInfo.PalletRFID;
                _CST_RFID_ERROR_LOG.StartTime = _RFIDErrorMessageInfo.StartTime;
                _CST_RFID_ERROR_LOG.EndTime = _RFIDErrorMessageInfo.EndTime;
                _CST_RFID_ERROR_LOG.Remark = _RFIDErrorMessageInfo.Remark;
                _repository.Create(_CST_RFID_ERROR_LOG);

                pResult.Success = true;
            }
            catch (Exception ex)
            {
                //pResult.Exception = ex;
                pResult.Message = ex.ToString();
                ////2627 主鍵重複
                ////2601 唯一索引重複
                //var ErrorCode = ((System.Data.SqlClient.SqlException)((ex.InnerException).InnerException)).Number;
                //if (ErrorCode == 2627) pResult.Message = "SID重複";
                //if (ErrorCode == 2601) pResult.Message = "帳號已被使用，請重新申請";
            }

            return pResult;

        }

        public IEnumerable<vw_RFIDLog> Query(string Lot, string OrderNo, string StartTime, string EndTime)
        {
            string sqlString = "";
            string WhereString = "";

            List<string> _List = new List<string>();

            if (!string.IsNullOrEmpty(Lot)) _List.Add(" Lot = '" + Lot + "'");
            if (!string.IsNullOrEmpty(OrderNo)) _List.Add(" OrderNo = '" + OrderNo + "'");
            if (!string.IsNullOrEmpty(StartTime)) _List.Add(" StartTime >= '" + StartTime + " 00:00:00'");
            if (!string.IsNullOrEmpty(EndTime)) _List.Add(" StartTime <= '" + EndTime + " 23:59:59'");


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


            sqlString = "Select * From vw_RFIDLog " + WhereString + " ;";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _Logs = db.Database.SqlQuery<vw_RFIDLog>(sqlString).ToList();

                return _Logs;
            }
        }
        //public IResult Update(RFIDErrorMessageInfo _RFIDErrorMessageInfo)
        //{
        //    Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

        //    var instance = GetByLot(_RFIDErrorMessageInfo.RFID, _RFIDErrorMessageInfo.OrderNo, _RFIDErrorMessageInfo.UpLoadType);

        //    IResult Result = new Result(false);

        //    try
        //    {
        //        if (instance != null)
        //        {
        //            var EndTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        //            DicUpdate.Add("EndTime", EndTime);

        //            _repository.Update(instance, DicUpdate);

        //            Result.Success = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.Exception = ex;
        //    }
        //    return Result;
        //}


        public IResult Update(RFIDErrorMessageInfo _RFIDErrorMessageInfo)
        {
            string sqlString = "";

            IResult result = new Result(false);

            try
            {
                var EndTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                sqlString = string.Format(@"Update CST_RFID_ERROR_LOG 
                    Set EndTime = '{0}' 
                    Where RFID = '{1}' And  OrderNo = '{2}' And UpLoadType = '{3}';", EndTime, _RFIDErrorMessageInfo.RFID, _RFIDErrorMessageInfo.OrderNo, _RFIDErrorMessageInfo.UpLoadType);

                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    db.Database.ExecuteSqlCommand(sqlString);
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }
            return result;
        }
        

        public CST_RFID_ERROR_LOG GetByLot(string RFID, string OrderNo, string UpLoadType)
        {
            return _repository.Get(x => x.RFID == RFID && x.OrderNo == OrderNo && x.UpLoadType == UpLoadType);
        }

        public IEnumerable<CST_RFID_ERROR_LOG> GetAll()
        {
            return _repository.GetAll();
        }
    }
}