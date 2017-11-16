using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Models;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IWarehouseService
    {
        /// <summary>
        /// 以棧板編號取得所有對應的FieldMap資料
        /// </summary>
        /// <param name="PalletNumber"></param>
        /// <returns></returns>
        IEnumerable<CST_FIELD_MAP> GetFieldMapByPalletNumber(string PalletNumber);

        /// <summary>
        /// 以工單取得所有對應的FieldMap資料
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <returns></returns>
        IEnumerable<CST_FIELD_MAP> GetFieldMapByWorkOrder(string WorkOrder);

        /// <summary>
        /// 以布號取得所有對應的FieldMap資料
        /// </summary>
        /// <param name="ClothNumber"></param>
        /// <returns></returns>
        IEnumerable<CST_FIELD_MAP> GetFieldMapByClothNumber(string ClothNumber);

        /// <summary>
        /// 以棧板類型取得所有對應的FieldMap資料
        /// </summary>
        /// <param name="PalletTypeID"></param>
        /// <returns></returns>
        IEnumerable<CST_FIELD_MAP> GetFieldMapByPalletType(string PalletTypeID);

        /// <summary>
        /// 以逾時取得所有對應的FieldMap資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_FIELD_MAP> GetFieldMapByOverTime();

        /// <summary>
        /// 取得所有對應的FieldMap資料
        /// </summary>
        /// <returns></returns>
        IEnumerable<CST_FIELD_MAP> GetFieldMapByAll();

        /// <summary>
        /// 以FieldMap取得所有對應的區塊資料
        /// </summary>
        /// <param name="FieldID"></param>
        /// <returns></returns>
        IEnumerable<CST_STORAGE_PARKINGBLOCK> GetParkingBlockByFieldID(string FieldID);

        /// <summary>
        /// 以棧板編號取得所有對應的LotInfo
        /// </summary>
        /// <param name="PalletNumber"></param>
        /// <returns></returns>
        IEnumerable<vw_Lots> GetLotInfoByPalletNumber(string PalletNumber);

        /// <summary>
        /// 以布號取得所有對應的棧板編號
        /// </summary>
        /// <param name="FieldID"></param>
        /// <param name="ClothNumber"></param>
        /// <returns></returns>
        IEnumerable<WarehouseSystem.Service.WarehouseService.Pallets> GetPalletByClothNumber(string FieldID, string ClothNumber);

        /// <summary>
        /// 以工單取得所有對應的棧板編號
        /// </summary>
        /// <param name="FieldID"></param>
        /// <param name="WorkOrder"></param>
        /// <returns></returns>
        IEnumerable<WarehouseSystem.Service.WarehouseService.Pallets> GetPalletByWorkOrder(string FieldID, string WorkOrder);

        /// <summary>
        /// 以棧板類型取得所有對應的棧板編號
        /// </summary>
        /// <param name="FieldID"></param>
        /// <param name="PalletTypeID"></param>
        /// <returns></returns>
        IEnumerable<WarehouseSystem.Service.WarehouseService.Pallets> GetPalletByPalletType(string FieldID, string PalletTypeID);

        /// <summary>
        /// 以逾時取得所有對應的棧板編號
        /// </summary>
        /// <param name="FieldID"></param>
        /// <returns></returns>
        IEnumerable<WarehouseSystem.Service.WarehouseService.Pallets> GetPalletByOverTime(string FieldID);

        /// <summary>
        /// 取得所有對應的棧板編號
        /// </summary>
        /// <param name="FieldID"></param>
        /// <returns></returns>
        IEnumerable<WarehouseSystem.Service.WarehouseService.Pallets> GetPalletByAll(string FieldID);

        IResult Insert(CST_WAREHOUSE Data);

        IEnumerable<CST_WAREHOUSE> GetByRFID(string RFIDID);

        IEnumerable<CST_WAREHOUSE> GetByLot(List<string> lstLot);

        IResult updateByStorage(string ParkingBlockID, string RFIDID);

        IResult updateByPallet(string NewRFID, string RFIDID);

        //IEnumerable<MailBoby> GetLotInfoByWarning();

        IEnumerable<vw_Lots> GetLotsByOverTime();

        IEnumerable<vw_Lots> GetLotsDelayBySetLocation();

        IResult Update(string WareHouseDataID, string PropertyName, object Value);

        CST_WAREHOUSE GetBySID(string WareHouseDataID);

        IEnumerable<CST_WAREHOUSE> GetByLot(string Lot);

        IResult updateByLot(string Lot, string RFIDID);

        IResult stockOut(StockInOutInfo _StockOutInfo);

        IResult ChangeGateReader(StockInOutInfo _StockInOutInfo);

        IEnumerable<vw_Lots> Query(string PLot, string Fabric, string PalletNumber, string ParkingBlock, string InternalOrder, string Status, string StartTime, string EndTime, string LotNo, string ColorCode);

        IEnumerable<CST_WAREHOUSE> GetByRFID(List<string> RFIDList);

        /// <summary>
        /// 依據棧板編號來更新批號的儲位編號
        /// </summary>
        /// <param name="palletNumber"></param>
        /// <param name="locationID"></param>
        void UpdateByLocation(string palletNumber, string locationID);

        /// <summary>
        /// 依據布疋RFID查詢批號資料
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns></returns>
        CST_WAREHOUSE GetDataByRFID(string rfid);

        /// <summary>
        /// 依據批號取得批號資料
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        CST_WAREHOUSE GetDataByLot(string lot);

        /// <summary>
        /// 依據SID刪除資料
        /// </summary>
        /// <param name="data"></param>
        void DeleteBySID(string SID);

        IResult Update(string WareHouseDataID, CST_WAREHOUSE newData);
    }
}