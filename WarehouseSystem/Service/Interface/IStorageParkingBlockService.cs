using WarehouseSystem.Service.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseSystem.Service.Interface
{
    public interface IStorageParkingBlockService
    {
        IResult Create(string FieldID, int X, int Y, int Width, int Height, ref string ParkingBlockID);
        IResult Update(string ParkingBlockID, string PropertyName, object Value);
        IResult UpdateRange(string ParkingBlockID, int X, int Y, int Width, int Height);
        IResult Delete(string ParkingBlockID);
        bool IsExists(string ParkingBlockID);
        CST_STORAGE_PARKINGBLOCK GetBySID(string ParkingBlockID);
        IEnumerable<CST_STORAGE_PARKINGBLOCK> GetByFieldID(string FieldID);
        IEnumerable<CST_STORAGE_PARKINGBLOCK> GetAll();

        CST_STORAGE_PARKINGBLOCK GetByStorageID(string StorageID);
    }
}