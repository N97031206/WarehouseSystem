using Newtonsoft.Json.Linq;
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
    public class AlarmService : IAlarmService
    {
        private IRepository<CST_ALARM> _repository;

        public AlarmService(IRepository<CST_ALARM> repository)
        {
            _repository = repository;
        }

        public IResult Create(string DeviceName, string WarningDay, ref string AlarmID)
        {
            if (string.IsNullOrEmpty(DeviceName) || string.IsNullOrEmpty(WarningDay))
            {
                throw new ArgumentNullException();
            }

            IResult pResult = new Result(false);

            CST_ALARM _CST_ALARM = new CST_ALARM();

            try
            {
                IdGenerator idg = new IdGenerator();
                AlarmID = idg.GetSID();

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                _CST_ALARM.AlarmID = AlarmID;
                _CST_ALARM.DeviceName = DeviceName;
                _CST_ALARM.WarningDay = WarningDay;
                _CST_ALARM.Active = 1;
                _CST_ALARM.IsDelete = 0;
                _CST_ALARM.LastUpdateTime = InsertTime;
                _CST_ALARM.CreateTime = InsertTime;
                _CST_ALARM.TypeName = "DEVICE";

                _repository.Create(_CST_ALARM);

                pResult.Success = true;
            }
            catch (Exception ex)
            {
                pResult.Exception = ex;

                //2627 主鍵重複
                //2601 唯一索引重複
                var ErrorCode = ((System.Data.SqlClient.SqlException)((ex.InnerException).InnerException)).Number;
                if (ErrorCode == 2627) pResult.Message = "SID重複";
                if (ErrorCode == 2601) pResult.Message = "名稱已被使用，請重新申請";
            }

            return pResult;

        }

        public IResult Update(string AlarmID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(AlarmID);

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

        public IResult Delete(string AlarmID)
        {
            IResult result = new Result(false);

            if (!IsExists(AlarmID))
            {
                result.Message = "找不到資料";
            }

            try
            {
                var instance = GetBySID(AlarmID);
                _repository.Delete(instance);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string AlarmID)
        {
            return _repository.GetAll().Any(x => x.AlarmID == AlarmID);
        }

        public CST_ALARM GetBySID(string AlarmID)
        {
            return _repository.Get(x => x.AlarmID == AlarmID);
        }

        public IEnumerable<CST_ALARM> GetAll()
        {
            return _repository.GetAll();
        }

        public CST_ALARM GetByDeviceName(string DeviceName)
        {
            return _repository.Get(x => x.DeviceName == DeviceName);
        }

        /// <summary>
        /// 依據傳入TYPE，回傳資料
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<CST_ALARM> GetDataByType(string type)
        {
            string sqlString = "";

            sqlString = string.Format(@" SELECT * FROM CST_ALARM WHERE TYPENAME IS NOT NULL AND TYPENAME != '' AND TYPENAME = '{0}';", type);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var dataList = db.Database.SqlQuery<CST_ALARM>(sqlString).ToList();

                return dataList;
            }
        }
    }
}