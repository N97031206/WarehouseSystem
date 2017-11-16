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
    public class PalletTypeService : IPalletTypeService
    {
        private IRepository<CST_PALLET_TYPE> _repository;

        public PalletTypeService(IRepository<CST_PALLET_TYPE> repository)
        {
            _repository = repository;
        }

        public IResult Create(string TypeName, string TypeCode, ref string PalletTypeID)
        {
            if (string.IsNullOrEmpty(TypeName) || string.IsNullOrEmpty(TypeCode))
            {
                throw new ArgumentNullException();
            }

            IResult pResult = new Result(false);

            CST_PALLET_TYPE PalletType = new CST_PALLET_TYPE();

            try
            {
                IdGenerator idg = new IdGenerator();
                PalletTypeID = idg.GetSID(1);

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                PalletType.PalletTypeID = PalletTypeID;
                PalletType.TypeName = TypeName;
                PalletType.TypeCode = TypeCode;
                PalletType.IsDelete = 0;
                PalletType.Active = 1;
                PalletType.CreateTime = InsertTime;
                PalletType.LastUpdateTime = InsertTime;

                _repository.Create(PalletType);

                pResult.Success = true;
            }
            catch (Exception ex)
            {
                pResult.Exception = ex;

                //2627 主鍵重複
                //2601 唯一索引重複
                var ErrorCode = ((System.Data.SqlClient.SqlException)((ex.InnerException).InnerException)).Number;
                if (ErrorCode == 2627) pResult.Message = "SID重複";
                if (ErrorCode == 2601) pResult.Message = "帳號已被使用，請重新申請";
            }

            return pResult;

        }

        public IResult Update(string PalletTypeID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(PalletTypeID);

            if (instance == null) throw new ArgumentNullException();

            IResult Result = new Result(false);

            try
            {
                DicUpdate.Add(PropertyName, Value);

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

        public IResult Delete(string PalletTypeID)
        {
            IResult result = new Result(false);

            if (!IsExists(PalletTypeID))
            {
                result.Message = "找不到棧板類別資料";
            }

            try
            {
                var instance = GetBySID(PalletTypeID);
                _repository.Delete(instance);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string PalletTypeID)
        {
            return _repository.GetAll().Any(x => x.PalletTypeID == PalletTypeID);
        }

        public CST_PALLET_TYPE GetBySID(string PalletTypeID)
        {
            return _repository.Get(x => x.PalletTypeID == PalletTypeID);
        }

        public CST_PALLET_TYPE GetByTypeName(string TypeName)
        {
            return _repository.Get(x => x.TypeName == TypeName);
        }


        public IEnumerable<CST_PALLET_TYPE> GetAll()
        {
            return _repository.GetAll();
        }
    }
}