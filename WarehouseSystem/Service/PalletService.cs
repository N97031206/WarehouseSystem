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
    public class PalletService : IPalletService
    {
        private IRepository<CST_PALLET> _repository;

        public PalletService(IRepository<CST_PALLET> repository)
        {
            _repository = repository;
        }

        public IResult Create(string PalletNumber, string RFIDID, ref string PalletID)
        {
            if (string.IsNullOrEmpty(PalletNumber)) throw new ArgumentNullException();

            IResult pResult = new Result(false);

            CST_PALLET Pallet = new CST_PALLET();

            try
            {
                IdGenerator idg = new IdGenerator();
                PalletID = idg.GetSID();

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                Pallet.PalletID = PalletID;
                Pallet.PalletNumber = PalletNumber;
                Pallet.PalletTypeID = "";
                Pallet.RFIDID = RFIDID;
                Pallet.Status = "Idle";
                Pallet.IsDelete = 0;
                Pallet.Active = 1;
                Pallet.CreateTime = InsertTime;
                Pallet.LastUpdateTime = InsertTime;

                _repository.Create(Pallet);

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

        public IResult Create(string PalletNumber, string RFIDID, string PalletTypeID, ref string PalletID)
        {
            if (string.IsNullOrEmpty(PalletNumber)) throw new ArgumentNullException();

            IResult pResult = new Result(false);

            CST_PALLET Pallet = new CST_PALLET();

            try
            {
                IdGenerator idg = new IdGenerator();
                PalletID = idg.GetSID(1);

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                Pallet.PalletID = PalletID;
                Pallet.PalletNumber = PalletNumber;
                Pallet.PalletTypeID = PalletTypeID;
                Pallet.RFIDID = RFIDID;
                Pallet.Status = "Idle";
                Pallet.IsDelete = 0;
                Pallet.Active = 1;
                Pallet.CreateTime = InsertTime;
                Pallet.LastUpdateTime = InsertTime;

                _repository.Create(Pallet);

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

        public IResult Update(string PalletID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(PalletID);

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

        public IResult Delete(string PalletID)
        {
            IResult result = new Result(false);

            if (!IsExists(PalletID))
            {
                result.Message = "找不到棧板資料";
            }

            try
            {
                var instance = GetBySID(PalletID);
                _repository.Delete(instance);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string PalletID)
        {
            return _repository.GetAll().Any(x => x.PalletID == PalletID);
        }

        public CST_PALLET GetBySID(string PalletID)
        {
            return _repository.Get(x => x.PalletID == PalletID);
        }

        public IEnumerable<CST_PALLET> GetAll()
        {
            return _repository.GetAll();
        }

        public CST_PALLET GetByRFIDID(string RFIDID)
        {
            return _repository.Get(x => x.RFIDID == RFIDID);
        }
    }
}