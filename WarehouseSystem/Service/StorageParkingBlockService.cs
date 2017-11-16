using WarehouseSystem.Models.Interface;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Service
{
    public class StorageParkingBlockService : IStorageParkingBlockService
    {
        private IRepository<CST_STORAGE_PARKINGBLOCK> _repository;

        public StorageParkingBlockService(IRepository<CST_STORAGE_PARKINGBLOCK> repository)
        {
            _repository = repository;
        }

        public IResult Create(string FieldID, int X, int Y, int Width, int Height, ref string ParkingBlockID)
        {

            if (string.IsNullOrEmpty(FieldID))
            {
                throw new ArgumentNullException();
            }

            IResult result = new Result(false);

            var ParkingBlock = new CST_STORAGE_PARKINGBLOCK();

            try
            {
                IdGenerator idg = new IdGenerator();
                ParkingBlockID = idg.GetSID();

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                ParkingBlock.FieldID = FieldID;
                ParkingBlock.ParkingBlockID = ParkingBlockID;
                ParkingBlock.X = X.ToString();
                ParkingBlock.Y = Y.ToString();
                ParkingBlock.Width = Width;
                ParkingBlock.Height = Height;
                ParkingBlock.CreateTime = InsertTime;
                ParkingBlock.LastUpdateTime = InsertTime;
                
                _repository.Create(ParkingBlock);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
            return result;
        }

        public IResult Update(string ParkingBlockID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            IResult result = new Result(false);

            var instance = GetBySID(ParkingBlockID);

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
                DicUpdate.Add("LastUpdateTime", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                _repository.Update(instance, DicUpdate);

                Result.Success = true;
            }
            catch (Exception ex)
            {
                Result.Exception = ex;
            }

            return Result;
        }

        public IResult UpdateRange(string ParkingBlockID, int X, int Y, int Width, int Height)
        {
            if (string.IsNullOrEmpty(ParkingBlockID)) throw new ArgumentNullException();

            IResult Result = new Result(false);

            try
            {
                var instance = GetBySID(ParkingBlockID);

                if (instance == null)
                {
                    Result.Message = "找不到儲位場域資料";
                    return Result;
                }

                instance.X = X.ToString();
                instance.Y = Y.ToString();
                instance.Width = Width;
                instance.Height = Height;

                _repository.Update(instance);
                Result.Success = true;

            }
            catch (Exception ex)
            {
                Result.Exception = ex;
            }

            return Result;
        }

        public IResult Delete(string ParkingBlockID)
        {
            IResult Result = new Result(false);

            if (!IsExists(ParkingBlockID))
            {
                Result.Message = "找不到儲位場域資料";
            }

            try
            {
                var instance = GetBySID(ParkingBlockID);
                _repository.Delete(instance);
                Result.Success = true;

            }
            catch (Exception ex)
            {
                Result.Exception = ex;
            }

            return Result;
        }

        public bool IsExists(string ParkingBlockID)
        {
            return this._repository.GetAll().Any(x => x.ParkingBlockID == ParkingBlockID);
        }

        public CST_STORAGE_PARKINGBLOCK GetBySID(string ParkingBlockID)
        {
            return _repository.Get(x => x.ParkingBlockID == ParkingBlockID);
        }

        public CST_STORAGE_PARKINGBLOCK GetByStorageID(string StorageID)
        {
            return _repository.Get(x => x.StorageID == StorageID);
        }


        public IEnumerable<CST_STORAGE_PARKINGBLOCK> GetByFieldID(string FieldID)
        {
            return _repository.GetAll().Where(x => x.FieldID == FieldID);
        }

        public IEnumerable<CST_STORAGE_PARKINGBLOCK> GetAll()
        {
            return _repository.GetAll();
        }
    }
}