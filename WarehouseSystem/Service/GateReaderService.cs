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
    public class GateReaderService : IGateReaderService
    {
        private IRepository<CST_GATE_READER> _repository;

        public GateReaderService(IRepository<CST_GATE_READER> repository)
        {
            _repository = repository;
        }

        public IResult Create(string GateReaderNumber, ref string GateReaderID)
        {
            if (string.IsNullOrEmpty(GateReaderNumber)) throw new ArgumentNullException();

            IResult pResult = new Result(false);

            CST_GATE_READER GateReader = new CST_GATE_READER();

            try
            {
                IdGenerator idg = new IdGenerator();
                GateReaderID = idg.GetSID();

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                GateReader.GateReaderID = GateReaderID;
                GateReader.GateReaderNumber = GateReaderNumber;
                GateReader.IsDelete = 0;
                GateReader.Active = 1;
                GateReader.CreateTime = InsertTime;
                GateReader.LastUpdateTime = InsertTime;

                _repository.Create(GateReader);

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

        public IResult Update(string GateReaderID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(GateReaderID);

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

        public IResult Delete(string GateReaderID)
        {
            IResult result = new Result(false);

            if (!IsExists(GateReaderID))
            {
                result.Message = "找不到接收器資料";
            }

            try
            {
                var instance = GetBySID(GateReaderID);
                _repository.Delete(instance);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string GateReaderID)
        {
            return _repository.GetAll().Any(x => x.GateReaderID == GateReaderID);
        }

        public CST_GATE_READER GetBySID(string GateReaderID)
        {
            return _repository.Get(x => x.GateReaderID == GateReaderID);
        }

        public IEnumerable<CST_GATE_READER> GetAll()
        {
            return _repository.GetAll();
        }
    }
}