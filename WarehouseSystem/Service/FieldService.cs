using WarehouseSystem.Models.Interface;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Service
{
    public class FieldService : IFieldService
    {
        private IRepository<CST_FIELD_MAP> _repository;

        public FieldService(IRepository<CST_FIELD_MAP> repository)
        {
            _repository = repository;
        }

        public IResult Create(string FieldName, ref string FieldID)
        {
            if (string.IsNullOrEmpty(FieldName))
            {
                throw new ArgumentNullException();
            }

            IResult result = new Result(false);

            CST_FIELD_MAP FieldMap = new CST_FIELD_MAP();

            try
            {
                IdGenerator idg = new IdGenerator();
                FieldID = idg.GetSID();
                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                FieldMap.FieldID = FieldID;
                FieldMap.FieldName = FieldName;
                FieldMap.CreateTime = InsertTime;
                FieldMap.LastUpdateTime = InsertTime;

                _repository.Create(FieldMap);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public IResult Update(string FieldID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(FieldID);

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

        public IResult Delete(string FieldID)
        {
            IResult result = new Result(false);

            if (!IsExists(FieldID))
            {
                result.Message = "找不到廠域資料";
            }

            try
            {
                var instance = GetBySID(FieldID);
                this._repository.Delete(instance);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string FieldID)
        {
            return this._repository.GetAll().Any(x => x.FieldID == FieldID);
        }

        public CST_FIELD_MAP GetBySID(string FieldID)
        {
            return this._repository.Get(x => x.FieldID == FieldID);
        }

        public IEnumerable<CST_FIELD_MAP> GetAll()
        {
            return this._repository.GetAll();
        }
    }
}