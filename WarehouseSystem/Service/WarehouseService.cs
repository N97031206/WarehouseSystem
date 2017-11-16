using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using WarehouseSystem.Models;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service
{
    public class WarehouseService : IWarehouseService
    {
        private IRepository<CST_WAREHOUSE> _repository;

        public class Pallets
        {
            public string PalletNumber { get; set; }
        }

        public WarehouseService(IRepository<CST_WAREHOUSE> repository)
        {
            _repository = repository;
        }

        public IEnumerable<CST_FIELD_MAP> GetFieldMapByPalletNumber(string PalletNumber)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select * 
                            From CST_FIELD_MAP 
                            where FieldID in (
                                Select CSP.FieldID 
                                From CST_WAREHOUSE CW 
                                    Left Join CST_STORAGE_PARKINGBLOCK CSP On CSP.ParkingBlockID = CW.ParkingBlockID
                                Where CW.PalletNumber = '{0}' 
                                AND (CW.StockOutTime Is null OR CW.StockOutTime = ''));", PalletNumber);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var FieldMapList = db.Database.SqlQuery<CST_FIELD_MAP>(sqlString).ToList();

                return FieldMapList;
            }
        }

        public IEnumerable<CST_FIELD_MAP> GetFieldMapByClothNumber(string ClothNumber)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select * From CST_FIELD_MAP CFM 
                            Where CFM.FieldID In ( 
                                Select Distinct CSP.FieldID 
                                From CST_WAREHOUSE CW 
                                    Left Join CST_STORAGE_PARKINGBLOCK CSP 
                                    On CSP.ParkingBlockID = CW.ParkingBlockID 
                                Where CW.Fabric = '{0}' 
                                And (CW.StockOutTime Is null OR CW.StockOutTime = ''));", ClothNumber);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var WarehouseList = db.Database.SqlQuery<CST_FIELD_MAP>(sqlString).ToList();

                return WarehouseList;
            }
        }

        public IEnumerable<CST_FIELD_MAP> GetFieldMapByWorkOrder(string WorkOrder)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select * From CST_FIELD_MAP CFM 
                            Where CFM.FieldID In ( 
                                Select Distinct CSP.FieldID 
                                From CST_WAREHOUSE CW 
                                    Left Join CST_STORAGE_PARKINGBLOCK CSP 
                                    On CSP.ParkingBlockID = CW.ParkingBlockID 
                                Where CW.InternalOrder = '{0}' 
                                And (CW.StockOutTime Is null OR CW.StockOutTime = ''));", WorkOrder);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var WarehouseList = db.Database.SqlQuery<CST_FIELD_MAP>(sqlString).ToList();

                return WarehouseList;
            }
        }

        public IEnumerable<CST_FIELD_MAP> GetFieldMapByPalletType(string PalletTypeID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select * From CST_FIELD_MAP CFM 
                            Where CFM.FieldID In ( 
                                Select Distinct CSP.FieldID 
                                From CST_WAREHOUSE CW 
                                    Left Join CST_STORAGE_PARKINGBLOCK CSP 
                                    On CSP.ParkingBlockID = CW.ParkingBlockID 
                                Where CW.PalletTypeID = '{0}' 
                                And (CW.StockOutTime Is null OR CW.StockOutTime = ''));", PalletTypeID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var WarehouseList = db.Database.SqlQuery<CST_FIELD_MAP>(sqlString).ToList();

                return WarehouseList;
            }
        }

        public IEnumerable<CST_FIELD_MAP> GetFieldMapByOverTime()
        {
            string sqlString = "";

            sqlString = string.Format(@"Select * From CST_FIELD_MAP CFM 
                            Where CFM.FieldID In ( 
                                Select Distinct CSP.FieldID 
                                From CST_WAREHOUSE CW 
                                    Left Join CST_STORAGE_PARKINGBLOCK CSP 
                                    On CSP.ParkingBlockID = CW.ParkingBlockID 
                                Where CW.OverTime <= '{0}' 
                                And (CW.StockOutTime Is null OR CW.StockOutTime = ''));", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var WarehouseList = db.Database.SqlQuery<CST_FIELD_MAP>(sqlString).ToList();

                return WarehouseList;
            }
        }

        public IEnumerable<CST_FIELD_MAP> GetFieldMapByAll()
        {
            string sqlString = "";

            sqlString = @"Select * From CST_FIELD_MAP CFM 
                            Where CFM.FieldID In ( 
                                Select Distinct CSP.FieldID 
                                From CST_WAREHOUSE CW 
                                    Left Join CST_STORAGE_PARKINGBLOCK CSP 
                                    On CSP.ParkingBlockID = CW.ParkingBlockID 
                                Where CW.StockOutTime Is null OR CW.StockOutTime = '');";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var WarehouseList = db.Database.SqlQuery<CST_FIELD_MAP>(sqlString).ToList();

                return WarehouseList;
            }
        }

        public IEnumerable<CST_STORAGE_PARKINGBLOCK> GetParkingBlockByFieldID(string FieldID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select * From CST_STORAGE_PARKINGBLOCK Where FieldID = '{0}'", FieldID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var ParkingBlockList = db.Database.SqlQuery<CST_STORAGE_PARKINGBLOCK>(sqlString).ToList();

                return ParkingBlockList;
            }
        }

        public IEnumerable<vw_Lots> GetLotInfoByPalletNumber(string PalletNumber)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select * From vw_Lots Where PalletNumber = '{0}' Order by PLot ", PalletNumber);
            //            sqlString = string.Format(@"Select CW.*, case when CW.OverTime <='{1}' Then '2' else '0' End As Status  
            //                            From CST_WAREHOUSE CW 
            //                            Where CW.PalletNumber = '{0}'
            //                            And (CW.StockOutTime Is null OR CW.StockOutTime = '');", PalletNumber, System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var PalletInfoList = db.Database.SqlQuery<vw_Lots>(sqlString).ToList();

                return PalletInfoList;
            }
        }

        //        public IEnumerable<MailBoby> GetLotInfoByWarning()
        //        {
        //            string sqlString = "";

        //            sqlString = string.Format(@"Select CW.*, CSP.ParkingBlockName 
        //                            From CST_WAREHOUSE CW 
        //                                Left Join CST_STORAGE_PARKINGBLOCK CSP
        //                                On CSP.ParkingBlockID = CW.ParkingBlockID 
        //                            Where (CW.StockOutTime Is null OR CW.StockOutTime = '') 
        //                            AND CW.OverTime <='{0}' 
        //                            AND (CW.MailFlag Is null OR CW.MailFlag = '') 
        //                            AND (CW.OverTime Is not null AND CW.OverTime != '') 
        //                            Order by CW.DeviceName;", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        //            using (WarehouseServerEntities db = new WarehouseServerEntities())
        //            {
        //                var PalletInfoList = db.Database.SqlQuery<MailBoby>(sqlString).ToList();

        //                return PalletInfoList;
        //            }
        //        }
        public IEnumerable<vw_Lots> GetLotsByOverTime()
        {
            string sqlString = "";

            sqlString = "Select * From vw_Lots Where Status = '2' And (MailFlag Is null OR MailFlag = '');";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _List = db.Database.SqlQuery<vw_Lots>(sqlString).ToList();

                return _List;
            }
        }

        public IEnumerable<vw_Lots> GetLotsDelayBySetLocation()
        {
            string sqlString = "";

            sqlString = string.Format("SELECT * FROM VW_LOTS WHERE PARKINGBLOCKNAME IS NULL OR PARKINGBLOCKNAME = '' ORDER BY PALLETNUMBER ;");

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _List = db.Database.SqlQuery<vw_Lots>(sqlString).ToList();

                return _List;
            }
        }

        public IEnumerable<Pallets> GetPalletByClothNumber(string FieldID, string ClothNumber)
        {
            string sqlString = "";

            //            sqlString = string.Format(@"Select CW.* 
            //                            From CST_WAREHOUSE CW 
            //                                Left Join CST_STORAGE_PARKINGBLOCK CSP 
            //                                On CSP.ParkingBlockID = CW.ParkingBlockID
            //                            Where CW.DeviceName = '{0}' 
            //                            And CSP.FieldID = '{1}' 
            //                            And (CW.StockOutTime Is null OR CW.StockOutTime = '');", ClothNumber, FieldID);

            sqlString = string.Format(@"Select Distinct CW.PalletNumber 
                            From CST_WAREHOUSE CW 
                                Left Join CST_STORAGE_PARKINGBLOCK CSP 
                                On CSP.ParkingBlockID = CW.ParkingBlockID
                            Where CW.Fabric = '{0}' 
                            And CSP.FieldID = '{1}' 
                            And (CW.StockOutTime Is null OR CW.StockOutTime = '');", ClothNumber, FieldID);



            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var PalletInfoList = db.Database.SqlQuery<Pallets>(sqlString).ToList();

                return PalletInfoList;
            }
        }

        public IEnumerable<Pallets> GetPalletByWorkOrder(string FieldID, string WorkOrder)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select Distinct CW.PalletNumber 
                            From CST_WAREHOUSE CW 
                                Left Join CST_STORAGE_PARKINGBLOCK CSP 
                                On CSP.ParkingBlockID = CW.ParkingBlockID
                            Where CW.InternalOrder = '{0}' 
                            And CSP.FieldID = '{1}' 
                            And (CW.StockOutTime Is null OR CW.StockOutTime = '');", WorkOrder, FieldID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var PalletInfoList = db.Database.SqlQuery<Pallets>(sqlString).ToList();

                return PalletInfoList;
            }
        }

        public IEnumerable<Pallets> GetPalletByPalletType(string FieldID, string PalletTypeID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select Distinct CW.PalletNumber 
                            From CST_WAREHOUSE CW 
                                Left Join CST_STORAGE_PARKINGBLOCK CSP 
                                On CSP.ParkingBlockID = CW.ParkingBlockID
                            Where CW.PalletTypeID = '{0}' 
                            And CSP.FieldID = '{1}' 
                            And (CW.StockOutTime Is null OR CW.StockOutTime = '');", PalletTypeID, FieldID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var PalletInfoList = db.Database.SqlQuery<Pallets>(sqlString).ToList();

                return PalletInfoList;
            }
        }

        public IEnumerable<Pallets> GetPalletByOverTime(string FieldID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select Distinct CW.PalletNumber 
                            From CST_WAREHOUSE CW 
                                Left Join CST_STORAGE_PARKINGBLOCK CSP 
                                On CSP.ParkingBlockID = CW.ParkingBlockID
                            Where CW.OverTime <= '{0}' 
                            And CSP.FieldID = '{1}' 
                            And (CW.StockOutTime Is null OR CW.StockOutTime = '');", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), FieldID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var PalletInfoList = db.Database.SqlQuery<Pallets>(sqlString).ToList();

                return PalletInfoList;
            }
        }

        public IEnumerable<Pallets> GetPalletByAll(string FieldID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select Distinct CW.PalletNumber 
                            From CST_WAREHOUSE CW 
                                Left Join CST_STORAGE_PARKINGBLOCK CSP 
                                On CSP.ParkingBlockID = CW.ParkingBlockID
                            Where CSP.FieldID = '{0}' 
                            And (CW.StockOutTime Is null OR CW.StockOutTime = '');", FieldID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var PalletInfoList = db.Database.SqlQuery<Pallets>(sqlString).ToList();

                return PalletInfoList;
            }
        }

        public IEnumerable<CST_WAREHOUSE> GetByRFID(string RFIDID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select *
                            From CST_WAREHOUSE CW 
                            Where CW.RFIDID = '{0}' 
                            And (CW.StockOutTime Is null OR CW.StockOutTime = '');", RFIDID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _InfoList = db.Database.SqlQuery<CST_WAREHOUSE>(sqlString).ToList();

                return _InfoList;
            }
        }

        public IEnumerable<CST_WAREHOUSE> GetByLot(string Lot)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select *
                            From CST_WAREHOUSE CW 
                            Where CW.Lot = '{0}' 
                            And (CW.StockOutTime Is null OR CW.StockOutTime = '');", Lot);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _InfoList = db.Database.SqlQuery<CST_WAREHOUSE>(sqlString).ToList();

                return _InfoList;
            }
        }

        public IEnumerable<CST_WAREHOUSE> GetByRFID(List<string> RFIDList)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select *
                                        From CST_WAREHOUSE CW 
                                        Where (CW.StockOutTime Is null OR CW.StockOutTime = '')");

            for (int i = 0; i < RFIDList.Count; i++)
            {
                if (i == 0) sqlString += "AND RFID IN (" + "'" + RFIDList[0] + "'";
                else sqlString += "," + "'" + RFIDList[i] + "'";
            }

            sqlString += ")";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _InfoList = db.Database.SqlQuery<CST_WAREHOUSE>(sqlString).ToList();

                return _InfoList;
            }
        }

        public IEnumerable<CST_WAREHOUSE> GetByLot(List<string> lstLot)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select *
                                        From CST_WAREHOUSE CW 
                                        Where (CW.StockOutTime Is null OR CW.StockOutTime = '')");

            for (int i = 0; i < lstLot.Count; i++)
            {
                if (i == 0) sqlString += "AND LOT IN (" + "'" + lstLot[0] + "'";
                else sqlString += "," + "'" + lstLot[i] + "'";
            }

            sqlString += ")";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _InfoList = db.Database.SqlQuery<CST_WAREHOUSE>(sqlString).ToList();

                return _InfoList;
            }
        }

        public IEnumerable<vw_Lots> Query(string PLot, string Fabric, string PalletNumber, string ParkingBlock, string InternalOrder, string Status, string StartTime, string EndTime, string LotNo, string ColorCode)
        {
            string sqlString = "";
            string WhereString = "";

            List<string> _List = new List<string>();

            if (!string.IsNullOrEmpty(PLot)) _List.Add(" PLot = '" + PLot + "'");
            if (!string.IsNullOrEmpty(Fabric)) _List.Add(" Fabric = '" + Fabric + "'");
            if (!string.IsNullOrEmpty(PalletNumber)) _List.Add(" PalletNumber = '" + PalletNumber + "'");
            if (!string.IsNullOrEmpty(ParkingBlock)) _List.Add(" ParkingBlockName = '" + ParkingBlock + "'");
            if (!string.IsNullOrEmpty(InternalOrder)) _List.Add(" InternalOrder = '" + InternalOrder + "'");
            if (!string.IsNullOrEmpty(Status)) _List.Add(" Status = '" + Status + "'");
            if (!string.IsNullOrEmpty(StartTime)) _List.Add(" StockInTime >= '" + StartTime + " 00:00:00'");
            if (!string.IsNullOrEmpty(EndTime)) _List.Add(" StockInTime <= '" + EndTime + " 23:59:59'");
            if (!string.IsNullOrEmpty(LotNo)) _List.Add(" LotNo = '" + LotNo + "'");
            if (!string.IsNullOrEmpty(ColorCode)) _List.Add(" ColorCode = '" + ColorCode + "'");

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

            sqlString = "Select * From vw_Lots CW " + WhereString + " ;";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _Lots = db.Database.SqlQuery<vw_Lots>(sqlString).ToList();

                return _Lots;
            }
        }

        public IResult updateByStorage(string ParkingBlockID, string RFIDID)
        {
            string sqlString = "";

            IResult result = new Result(false);

            try
            {
                var LastUpdateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                sqlString = string.Format(@"Update CST_WAREHOUSE 
                    Set ParkingBlockID = '{0}', LastUpdateTime = '{1}' 
                    Where RFIDID = '{2} 
                    And (StockOutTime Is null OR StockOutTime = '');", ParkingBlockID, LastUpdateTime, RFIDID);

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

        public IResult updateByPallet(string NewRFID, string RFIDID)
        {
            string sqlString = "";

            IResult result = new Result(false);

            try
            {
                var LastUpdateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                sqlString = string.Format(@"Update CST_WAREHOUSE 
                    Set RFIDID = '{0}', LastUpdateTime = '{1}' 
                    Where RFIDID = '{2} 
                    And (StockOutTime Is null OR StockOutTime = '');", NewRFID, LastUpdateTime, RFIDID);

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

        public IResult updateByLot(string Lot, string RFIDID)
        {
            string sqlString = "";

            IResult result = new Result(false);

            try
            {
                var LastUpdateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                sqlString = string.Format(@"Update CST_WAREHOUSE 
                    Set RFIDID = '{0}', LastUpdateTime = '{1}'
                    Where Lot = '{2} 
                    And (StockOutTime Is null OR StockOutTime = '');", RFIDID, LastUpdateTime, Lot);

                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    var tran = db.Database.BeginTransaction();

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

        public IResult stockOut(StockInOutInfo _StockInOutInfo)
        {
            string sqlString = "";

            IResult result = new Result(false);

            try
            {

                var LastUpdateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                sqlString = string.Format(@"Update CST_WAREHOUSE 
                    Set StockOut = '{0}', LastUpdateTime = '{1}', StockOutTime = '{2}', LastGateReaderNumber = '{3}', PreGateReaderNumber = LastGateReaderNumber
                    Where Lot = '{4}'
                    And (StockOutTime Is null OR StockOutTime = ''); ", _StockInOutInfo.StockOut, LastUpdateTime, LastUpdateTime, _StockInOutInfo.GateReaderNumber, _StockInOutInfo.Lot);

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

        public IResult ChangeGateReader(StockInOutInfo _StockInOutInfo)
        {
            string sqlString = "";

            IResult result = new Result(false);

            try
            {

                var LastUpdateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                sqlString = string.Format(@"Update CST_WAREHOUSE 
                    Set LastUpdateTime = '{0}', LastGateReaderNumber = '{1}', PreGateReaderNumber = LastGateReaderNumber 
                    Where Lot = '{2}'
                    And (StockOutTime Is null OR StockOutTime = ''); ", LastUpdateTime, _StockInOutInfo.GateReaderNumber, _StockInOutInfo.Lot);

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

        public IResult Insert(CST_WAREHOUSE Data)
        {
            IResult result = new Result(false);

            try
            {
                IdGenerator idg = new IdGenerator();
                var ID = idg.GetSID(1);
                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                string OverTime = "";

                if (Data.WarningDay != "" && Data.WarningDay != null)
                {
                    OverTime = DateTime.Now.AddDays(Convert.ToDouble(Data.WarningDay)).ToString("yyyy/MM/dd HH:mm:ss");
                }

                Data.WareHouseDataID = ID;
                Data.StockInTime = InsertTime;
                Data.OverTime = OverTime;

                _repository.Create(Data);

                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
            }

            return result;
        }

        public IResult Update(string WareHouseDataID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(WareHouseDataID);

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

        public IResult Update(string WareHouseDataID, CST_WAREHOUSE newData)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(WareHouseDataID);

            if (instance == null) throw new ArgumentNullException();

            IResult Result = new Result(false);

            try
            {
                var LastUpdateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                DicUpdate.Add("LastUpdateTime", LastUpdateTime);
                DicUpdate.Add("DeviceName", newData.DeviceName);
                DicUpdate.Add("PalletNumber", newData.PalletNumber);
                DicUpdate.Add("PalletTypeID", newData.PalletTypeID);
                DicUpdate.Add("ParkingBlockID", newData.ParkingBlockID);
                DicUpdate.Add("RFIDID", newData.RFIDID);
                //DicUpdate.Add("StockInTime", newData.StockInTime);
                //DicUpdate.Add("StockOutTime", newData.StockOutTime);
                DicUpdate.Add("Quantity", newData.Quantity);
                //DicUpdate.Add("PreGateReaderNumber", newData.PreGateReaderNumber);
                //DicUpdate.Add("LastGateReaderNumber", newData.LastGateReaderNumber);
                DicUpdate.Add("WO", newData.WO);
                DicUpdate.Add("OverTime", newData.OverTime);
                DicUpdate.Add("WarningDay", newData.WarningDay);
                //DicUpdate.Add("StockIn", newData.StockIn);
                //DicUpdate.Add("StockOut", newData.StockOut);
                //DicUpdate.Add("RFID", newData.RFID);
                DicUpdate.Add("WKNo", newData.WKNo);
                DicUpdate.Add("LotNo", newData.LotNo);
                DicUpdate.Add("Order", newData.Order);
                DicUpdate.Add("InternalOrder", newData.InternalOrder);
                DicUpdate.Add("Fabric", newData.Fabric);
                DicUpdate.Add("Color", newData.Color);
                DicUpdate.Add("Width", newData.Width);
                DicUpdate.Add("YdWt", newData.YdWt);
                DicUpdate.Add("Weight", newData.Weight);
                DicUpdate.Add("Length", newData.Length);
                DicUpdate.Add("PNo", newData.PNo);
                DicUpdate.Add("Date", newData.Date);
                DicUpdate.Add("ColorCode", newData.ColorCode);

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

        //public CST_WAREHOUSE GetBySID(string WareHouseDataID)
        //{
        //    return _repository.Get(x => x.WareHouseDataID == WareHouseDataID);
        //}

        /// <summary>
        /// 依據布疋RFID查詢批號資料
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns></returns>
        public CST_WAREHOUSE GetDataByRFID(string rfid)
        {
            return _repository.Get(x => x.RFID == rfid);
        }

        /// <summary>
        /// 依據棧板編號來更新批號的儲位編號
        /// </summary>
        /// <param name="palletNumber"></param>
        /// <param name="locationID"></param>
        public void UpdateByLocation(string palletNumber, string locationID)
        {
            string OverTime = "";
            string sqlString = "";
            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                #region 依據棧板編號查詢符合的所有批號資訊，並將找到的所有批號進行更新資料

                sqlString = string.Format(@"SELECT * 
                                            FROM CST_WAREHOUSE 
                                            WHERE PALLETNUMBER = '{0}' AND (STOCKOUTTIME IS NULL OR STOCKOUTTIME = '');", palletNumber);

                var lotDataList = db.Database.SqlQuery<CST_WAREHOUSE>(sqlString).ToList();

                //如果系統上沒有符合的批號資料，則回傳錯誤訊息
                if (lotDataList.Count == 0)
                {
                    throw new Exception("棧板編號[" + palletNumber + "]：系統上無任何資料");
                }
                else
                {

                    #region 批號進行更新儲位及逾時時間
                    foreach (var lotData in lotDataList)
                    {
                        OverTime = "";

                        #region 重新計算批號逾時時間
                        if (!string.IsNullOrEmpty(lotData.WarningDay))
                        {
                            OverTime = DateTime.Now.AddDays(Convert.ToDouble(lotData.WarningDay)).ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        #endregion

                        #region 更新批號資料
                        sqlString = string.Format(@"UPDATE CST_WAREHOUSE
                                                    SET PARKINGBLOCKID = '{0}', LASTUPDATETIME = '{1}', OVERTIME = '{2}' 
                                                    WHERE PALLETNUMBER = '{3}' AND LOT = '{4}'
                                                    AND (STOCKOUTTIME IS NULL OR STOCKOUTTIME = '');", locationID, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), OverTime, palletNumber, lotData.Lot);

                        db.Database.ExecuteSqlCommand(sqlString);
                        #endregion
                    }
                    #endregion
                }

                #endregion
            }
        }
    

        /// <summary>
        /// 依據批號取得批號資料
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        public CST_WAREHOUSE GetDataByLot(string lot)
        {
            //return _repository.Get(p => p.Lot == lot && p.StockOutTime == string.Empty);
            return _repository.Get(p => p.Lot == lot && (p.StockOutTime == "" || p.StockOutTime == null));
        }

        /// <summary>
        /// 依據SID刪除資料
        /// </summary>
        /// <param name="data"></param>
        public void DeleteBySID(string SID)
        {
            var data = GetBySID(SID);

            _repository.Delete(data);
        }

        public CST_WAREHOUSE GetBySID(string SID)
        {
            return _repository.Get(p => p.WareHouseDataID == SID);
        }
    }
}