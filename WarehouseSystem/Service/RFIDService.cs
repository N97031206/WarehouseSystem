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
    public class RFIDService : IRFIDService
    {
        private IRepository<CST_RFID> _repository;

        public RFIDService(IRepository<CST_RFID> repository)
        {
            _repository = repository;
        }

        public IResult Create(string RFIDNumber, ref string RFIDID)
        {
            if (string.IsNullOrEmpty(RFIDNumber)) throw new ArgumentNullException();

            IResult pResult = new Result(false);

            CST_RFID RFID = new CST_RFID();

            try
            {
                IdGenerator idg = new IdGenerator();
                RFIDID = idg.GetSID(1);

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                RFID.RFIDID = RFIDID;
                RFID.RFIDNumber = RFIDNumber;
                RFID.IsDelete = 0;
                RFID.Active = 1;
                RFID.CreateTime = InsertTime;
                RFID.LastUpdateTime = InsertTime;

                _repository.Create(RFID);

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

        public IResult Update(string RFIDID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(RFIDID);

            if (instance == null) throw new ArgumentNullException();

            IResult Result = new Result(false);

            try
            {
                if (PropertyName == "Active")
                {
                    DicUpdate.Add(PropertyName, Convert.ToInt32(Value));
                }
                else
                {
                    DicUpdate.Add(PropertyName, Value);
                }
                var LastUpdateTime =  System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
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

        public IResult Delete(string RFIDID)
        {
            IResult result = new Result(false);

            if (!IsExists(RFIDID))
            {
                result.Message = "找不到RFID資料";
            }

            try
            {
                var instance = GetBySID(RFIDID);
                _repository.Delete(instance);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string RFIDID)
        {
            return _repository.GetAll().Any(x => x.RFIDID == RFIDID);
        }

        public CST_RFID GetBySID(string RFIDID)
        {
            return _repository.Get(x => x.RFIDID == RFIDID);
        }

        public IEnumerable<CST_RFID> GetAll()
        {
            return _repository.GetAll();
        }

        public CST_RFID GetByNumber(string RFIDNumber)
        {
            return _repository.Get(x => x.RFIDNumber == RFIDNumber);
        }

        public IEnumerable<CST_RFID> GetRFIDNo()
        {
            string sqlString = "";

            sqlString = "Select TOP 1 * From CST_RFID Where Active ='1' Order by RFIDNumber DESC";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _List = db.Database.SqlQuery<CST_RFID>(sqlString).ToList();

                return _List;
            }
        }
    }
}