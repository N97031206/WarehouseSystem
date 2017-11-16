using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem.Models;
using WarehouseSystem.Service.Interface;
using System.Configuration;
using DesryptLibrary;
using WarehouseSystem.Service.Misc;
using System.Data;
using System.Transactions;
using System.Security.Cryptography.X509Certificates;

namespace WarehouseSystem.Controllers
{
    public class APIController : Controller
    {
        private IPalletTypeService m_PalletTypeService;
        private IPalletService m_PalletService;
        private IRFIDService m_RFIDService;
        private IWarehouseService m_WarehouseService;
        private IWarehouseLogService m_WarehouseLogService;
        private IStorageParkingBlockService m_StorageParkingBlockService;
        private IAlarmService m_AlarmService;
        private IMailDetailService m_MailDetailService;
        private IRFIDErrorMessage m_RFIDErrorMessage;
        private IStockOutService m_StockOutService;
        private IStockOutLogService m_StockOutLogService;

        Lazy<WA27P6MMWebService.WA27P6MMServiceSoapClient> ERPWebService = new Lazy<WA27P6MMWebService.WA27P6MMServiceSoapClient>();
        //Lazy<ERP_WebService.ERPWebServiceSoapClient> ERPWebService = new Lazy<ERP_WebService.ERPWebServiceSoapClient>();


        public APIController(IPalletService p_PalletService, IWarehouseService p_WarehouseService,
            IStorageParkingBlockService p_StorageParkingBlockService, IAlarmService p_AlarmService,
            IMailDetailService p_MailDetailService, IRFIDService p_RFIDService,
            IRFIDErrorMessage p_RFIDErrorMessage, IPalletTypeService p_PalletTypeService,
            IStockOutService p_StockOutService, IWarehouseLogService p_WarehouseLogService,
            IStockOutLogService p_StockOutLogService)
        {
            m_RFIDService = p_RFIDService;
            m_PalletService = p_PalletService;
            m_WarehouseService = p_WarehouseService;
            m_StorageParkingBlockService = p_StorageParkingBlockService;
            m_AlarmService = p_AlarmService;
            m_MailDetailService = p_MailDetailService;
            m_RFIDErrorMessage = p_RFIDErrorMessage;
            m_PalletTypeService = p_PalletTypeService;
            m_StockOutService = p_StockOutService;
            m_WarehouseLogService = p_WarehouseLogService;
            m_StockOutLogService = p_StockOutLogService;
        }


        //[HttpPost]
        public ActionResult AnalysisRFID(AnalysisData analysisData)
        {
            IResult _Result = new Result();

            try
            {
                List<string> rfids = new List<string>();

                foreach (var rfid in analysisData.rfidList)
                {
                    rfids.Add(rfid.RFID);
                }

                //依據傳入的布疋RFID編號，查詢資料庫裡是否都已經建置資料
                var dataList = m_WarehouseService.GetByRFID(rfids);

                if (dataList.Count() == 0)
                {
                    //確認棧板RFID是否為空白
                    if (string.IsNullOrEmpty(analysisData.palletRFID))
                        throw new Exception("棧板編號為空白，不得執行入庫作業");

                    //入庫作業
                    _Result = ExecuteStockIn(analysisData.rfidList, analysisData.palletRFID, analysisData.readerNumber);
                }
                else if (dataList.Count() == analysisData.rfidList.Count)
                {
                    //依據傳入的布疋RFID編號，查詢資料庫裡是否都有屬於該批號的出庫單資訊
                    var stockOutLotDataList = m_StockOutService.GetDataByRFIDList(rfids);

                    if (stockOutLotDataList.Count() == 0)
                    {
                        //確認棧板RFID是否為空白
                        if (string.IsNullOrEmpty(analysisData.palletRFID))
                            throw new Exception("棧板編號為空白，不得執行解綁作業");

                        //解綁作業
                        _Result = ExecuteChangeLocation(analysisData.palletRFID, "");
                    }
                    else
                    {
                        #region 取得出庫單號清單
                        List<string> stockOutNoList = new List<string>();

                        foreach (var stockOutLotData in stockOutLotDataList)
                        {
                            if (stockOutNoList.Contains(stockOutLotData.StockOutNo) == false)
                            {
                                stockOutNoList.Add(stockOutLotData.StockOutNo);
                            }
                        }
                        #endregion

                        //出庫作業
                        _Result = ExecuteStockOut(analysisData.rfidList, stockOutNoList, analysisData.readerNumber, analysisData.palletRFID);
                    }
                }
                else
                {
                    throw new Exception("棧板編號(" + HexToString(analysisData.palletRFID) + ")上的布疋資料建立狀態必須一致");
                }
            }
            catch (Exception ex)
            {
                _Result.ErrorMessage = ex.Message;
            }
            return Json(_Result);
        }

        /// <summary>
        /// 執行出庫作業
        /// </summary>
        /// <param name="rfidList"></param>
        /// <param name="stockOutNoList"></param>
        /// <param name="readerNumber"></param>
        /// <returns></returns>
        private IResult ExecuteStockOut(List<RFIDData> rfidList, List<string> stockOutNoList, string readerNumber, string palletRFID)
        {
            //取得棧板編號
            //var palletNumber = HexToString(palletRFID);

            IResult _Result = new Result();

            #region 依據出庫單號清單查詢符合的所有布疋資訊，並進行資料比對，回傳結果
            var stockOutLotDataList = m_StockOutService.GetDataByStockOutNoList(stockOutNoList);

            List<StockInOutInfo> delLotList = new List<StockInOutInfo>();

            for (int idx = 0; idx < rfidList.Count; idx++)
            {
                StockInOutInfo delLot = new StockInOutInfo();

                foreach (var lotData in stockOutLotDataList)
                {
                    if (string.IsNullOrEmpty(lotData.RFID))
                    {
                        lotData.RFID = StringToHex(lotData.WKNo + lotData.PNo, "");
                    }

                    if (lotData.RFID == rfidList[idx].RFID)
                    {
                        #region 新增一筆批號資料
                        delLot.Lot = lotData.WKNo + lotData.PNo;
                        delLot.StockOut = lotData.StockOutNo;
                        delLot.GateReaderNumber = readerNumber;

                        delLotList.Add(delLot);

                        rfidList[idx].Success = true;
                        #endregion
                    }
                }

                //註記批號己執行比對資料作業
                rfidList[idx].IsProcessed = true;

                #region 記錄對比結果

                if (rfidList[idx].Success == false)
                {
                    rfidList[idx].ErrorMessage = "出庫對比結果失敗";
                    InsertRFIDErrorMessage(rfidList[idx], "Mapping", readerNumber, palletRFID, true);
                }
                else
                {
                    UpdateRFIDErrorMessage(delLot, "Mapping", readerNumber, palletRFID);
                }

                #endregion
            }

            #region 如果比較數量不一致，則顯示失敗 (NG)  數量：xx，反之，成功 (OK)  數量：xx

            #region 確認實物RFID是否都有比對成功 (有料無帳)
            int failCount = 0;
            foreach (var lotData in rfidList)
            {
                if (lotData.Success == false) failCount++;
            }

            if (failCount > 0)
            {
                _Result.Message = "出庫失敗 (NG)\n數量：" + failCount.ToString();
                return _Result;

                //throw new Exception("失敗 (NG)\n數量：" + failCount.ToString());
            }
            #endregion

            #region 確認出庫單的數量是否都有比對成功 (有帳無料)

            if (delLotList.Count != stockOutLotDataList.Count())
            {
                _Result.Message = "出庫失敗 (NG)\n數量：" + (stockOutLotDataList.Count() - delLotList.Count).ToString();
                return _Result;

                //throw new Exception("失敗 (NG)\n數量：" + (stockOutLotDataList.Count() - delLotList.Count).ToString());
            }

            #endregion

            #region 如果比對都成功則更新資料庫及回傳結果給ERP系統

            List<string> lotList = new List<string>();

            using (TransactionScope myScope = new TransactionScope())
            {
                #region 更新資料庫，並將刪除批號的資料搬至LOG
                foreach (var delLot in delLotList)
                {
                    //執行批號出庫
                    StockOut(delLot.Lot, delLot.StockOut);

                    lotList.Add(delLot.Lot);
                }
                #endregion

                #region 更新出庫單資料，並將刪除批號的資料搬至LOG
                foreach (var stockOutLotData in stockOutLotDataList)
                {
                    //刪除資料
                    m_StockOutService.DeleteBySID(stockOutLotData.StockOutID);

                    //新增記錄
                    CST_STOCK_OUT_LOG cstStockOutLog = new CST_STOCK_OUT_LOG()
                    {
                        StockOutNo = stockOutLotData.StockOutNo,
                        StockOutID = stockOutLotData.StockOutID,
                        WKNo = stockOutLotData.WKNo,
                        RFID = stockOutLotData.RFID,
                        PNo = stockOutLotData.PNo,
                        Lot = stockOutLotData.Lot,
                        CreateTime = stockOutLotData.CreateTime,
                        PalletRFID = stockOutLotData.PalletRFID
                    };

                    m_StockOutLogService.Insert(cstStockOutLog);
                }
                #endregion

                //資料回傳至ERP，告知此次布疋已執行出庫作業
                var isSuccess = ERPWebService.Value.SetStockOutData(lotList.ToArray(), "", "");

                if (isSuccess)
                {
                    myScope.Complete();
                    _Result.Success = true;
                    _Result.Message = "出庫成功 (OK)\n數量：" + delLotList.Count.ToString();
                }
                else
                {
                    _Result.Success = false;
                    _Result.Message = "出庫失敗 (NG)\nERP回傳失敗";
                }
            }

            #endregion

            #endregion

            #endregion

            return _Result;
        }

        /// <summary>
        /// 執行入庫作業
        /// </summary>
        /// <param name="rfidList"></param>
        /// <param name="palletRFID"></param>
        /// <param name="readerNumber"></param>
        private IResult ExecuteStockIn(List<RFIDData> rfidList, string palletRFID, string readerNumber)
        {
            IResult _Result = new Result();

            //取得棧板編號
            var palletNumber = HexToString(palletRFID);

            //取得RFID編號
            var rfid = m_RFIDService.GetByNumber(palletRFID);

            #region 確認讀取到的棧板RFID是否存在

            if (rfid == null)
                throw new Exception("棧板RFID[" + palletRFID + "]：不存在此筆資料");

            #endregion

            #region 確認此棧板RFID對應到資料是否存在

            var palletData = m_PalletService.GetByRFIDID(rfid.RFIDID);

            if (palletData == null)
            {
                throw new Exception("棧板編號[" + palletNumber + "]：不存在此筆資料");
            }
            else if (string.IsNullOrEmpty(palletData.PalletTypeID))
            {
                throw new Exception("棧板編號[" + palletData.PalletNumber + "]：無設定類型");
            }
            else if (string.IsNullOrEmpty(palletData.RFIDID))
            {
                throw new Exception("棧板編號[" + palletData.PalletNumber + "]：無設定對應RFID");
            }
            #endregion

            #region 將棧板編號傳至ERP並取回布疋資訊，並進行資料比對，回傳結果

            var lotDataSet = ERPWebService.Value.GetStockInDataByPallet(palletNumber);

            if (lotDataSet.Tables[0].Rows.Count == 0)
            {
                throw new Exception("棧板編號[" + palletNumber + "]：ERP查無任何入庫資料");
            }

            List<StockInOutInfo> addLotList = new List<StockInOutInfo>();
            List<RFIDErrorMessageInfo> addErrorMessageList = new List<RFIDErrorMessageInfo>();

            for (int idx = 0; idx < rfidList.Count; idx++)
            {
                foreach (DataRow lotData in lotDataSet.Tables[0].Rows)
                {
                    var rfidByERP = lotData["RFID"].ToString().Trim();

                    if (string.IsNullOrEmpty(rfidByERP))
                    {
                        rfidByERP = StringToHex(lotData["WKNo"].ToString().Trim() + lotData["PNo"].ToString().Trim(), "");
                    }

                    StockInOutInfo addLot = new StockInOutInfo();

                    if (rfidByERP == rfidList[idx].RFID)
                    {
                        #region 新增一筆批號資料
                        addLot.PalletRFID = palletRFID;
                        addLot.Lot = lotData["WKNo"].ToString().Trim() + lotData["PNo"].ToString().Trim();
                        addLot.WKNo = lotData["WKNo"].ToString().Trim();
                        addLot.LotNo = lotData["LotNo"].ToString().Trim();
                        addLot.Order = lotData["Order"].ToString().Trim();
                        addLot.Fabric = lotData["Fabric"].ToString().Trim();
                        addLot.Color = lotData["Color"].ToString().Trim();
                        addLot.ColorCode = lotData["ColorCode"].ToString().Trim();
                        addLot.Width = lotData["Width"].ToString().Trim();
                        addLot.YdWt = lotData["YdWt"].ToString().Trim();
                        addLot.Weight = lotData["Weight"].ToString().Trim();
                        addLot.Length = lotData["Length"].ToString().Trim();
                        addLot.PNo = lotData["PNo"].ToString().Trim();
                        addLot.Date = lotData["Date"].ToString().Trim();
                        addLot.InternalOrder = lotData["InternalOrder"].ToString().Trim();
                        addLot.RFID = rfidByERP;
                        //addLot.StockIn = lotData["StockIn"].ToString().Trim();
                        addLot.StockOut = "";
                        addLot.GateReaderNumber = readerNumber;

                        addLotList.Add(addLot);

                        rfidList[idx].Success = true;
                        #endregion
                    }

                    //註記批號己執行比對資料作業
                    rfidList[idx].IsProcessed = true;

                    #region 記錄對比結果

                    if (rfidList[idx].Success == false)
                    {
                        rfidList[idx].ErrorMessage = "入庫對比結果失敗";
                        InsertRFIDErrorMessage(rfidList[idx], "Mapping", readerNumber, palletRFID, true);
                    }
                    else
                    {
                        UpdateRFIDErrorMessage(addLot, "Mapping", readerNumber, palletRFID);
                    }

                    #endregion
                }
            }

            #region 如果比較數量不一致，則顯示失敗 (NG)  數量：xx，反之，成功 (OK)  數量：xx

            #region 確認實物RFID是否都有比對成功 (有料無帳)
            int failCount = 0;
            foreach (var lotData in rfidList)
            {
                if (lotData.Success == false) failCount++;
            }


            if (failCount > 0)
            {
                _Result.Message = "入庫失敗 (NG)\n數量：" + failCount.ToString();
                return _Result;
                //throw new Exception("失敗 (NG)\n數量：" + failCount.ToString());
            }
            #endregion

            #region 確認入庫單的數量是否都有比對成功 (有帳無料)

            if (addLotList.Count != lotDataSet.Tables[0].Rows.Count)
            {
                _Result.Message = "入庫失敗 (NG)\n數量：" + (lotDataSet.Tables[0].Rows.Count - addLotList.Count).ToString();
                return _Result;
                //throw new Exception("失敗 (NG)\n數量：" + (lotDataSet.Tables[0].Rows.Count - addLotList.Count).ToString());
            }
            #endregion

            #region 如果比對都成功則寫入資料庫及回傳結果給ERP系統
            List<string> lotList = new List<string>();

            using (TransactionScope myScope = new TransactionScope())
            {
                foreach (var addLot in addLotList)
                {
                    //執行批號入庫
                    var result = StockIn(addLot, palletData);

                    if (result.Success == false)
                        throw new Exception(result.Message);

                    lotList.Add(addLot.Lot);
                }


                //資料回傳至ERP，告知此次布疋已執行入庫作業
                var isSuccess = ERPWebService.Value.SetStockInData(lotList.ToArray(), palletNumber, palletRFID);

                if (isSuccess)
                {
                    myScope.Complete();
                    _Result.Success = true;
                    _Result.Message = "入庫成功 (OK)\n數量：" + addLotList.Count.ToString();
                }
                else
                {
                    _Result.Success = false;
                    _Result.Message = "入庫失敗 (NG)\nERP回傳失敗";
                }

            }
            #endregion

            #endregion

            #endregion

            return _Result;
        }

        /// <summary>
        /// 執行更換儲位作業
        /// </summary>
        /// <param name="palletRFID"></param>
        /// <param name="locationSID"></param>
        /// <returns></returns>
        private IResult ExecuteChangeLocation(string palletRFID, string locationSID)
        {
            IResult _Result = new Result();

            //取得棧板編號
            var palletNumber = HexToString(palletRFID);

            //取得RFID編號
            var rfid = m_RFIDService.GetByNumber(palletRFID);

            #region 確認讀取到的棧板RFID是否存在

            if (rfid == null)
                throw new Exception("棧板RFID[" + palletRFID + "]：不存在此筆資料");

            #endregion

            #region 確認此棧板RFID對應到資料是否存在

            var palletData = m_PalletService.GetByRFIDID(rfid.RFIDID);

            if (palletData == null)
            {
                throw new Exception("棧板編號[" + palletNumber + "]：不存在此筆資料");
            }
            else if (string.IsNullOrEmpty(palletData.PalletTypeID))
            {
                throw new Exception("棧板編號[" + palletData.PalletNumber + "]：無設定類型");
            }
            else if (string.IsNullOrEmpty(palletData.RFIDID))
            {
                throw new Exception("棧板編號[" + palletData.PalletNumber + "]：無設定對應RFID");
            }
            #endregion

            using (TransactionScope myScope = new TransactionScope())
            {
                #region 更新資料庫及回傳結果給ERP系統

                //執行更換儲位
                m_WarehouseService.UpdateByLocation(palletNumber, locationSID);

                List<string> palletList = new List<string>();
                List<string> palletRFIDList = new List<string>();

                palletList.Add(palletNumber);
                palletRFIDList.Add(palletRFID);

                //資料回傳至ERP，告知此次布疋已執行解綁作業
                var isSuccess = ERPWebService.Value.SetStorageByPallet(palletList.ToArray(), palletRFIDList.ToArray(), "", "");

                if (isSuccess)
                {
                    myScope.Complete();
                    _Result.Success = true;
                    _Result.Message = (string.IsNullOrEmpty(locationSID)) ? "解綁儲位成功" : "更換儲位成功";
                }
                else
                {
                    _Result.Success = false;
                    _Result.Message = "解綁儲位 (NG)\nERP回傳失敗";
                }
                #endregion
            }

            return _Result;
        }

        [HttpPost]
        public ActionResult InsertRFIDErrorMsg(RFIDErrorMessageInfo _RFIDErrorMessageInfo)
        {
            IResult _Result = new Result();

            try
            {
                _Result = m_RFIDErrorMessage.Create(_RFIDErrorMessageInfo);
            }
            catch (Exception ex)
            {
                _Result.Message = ex.Message;
            }
            return Json(_Result);
        }

        [HttpPost]
        public ActionResult UpdateRFIDErrorMsg(RFIDErrorMessageInfo _RFIDErrorMessageInfo)
        {
            IResult _Result = new Result();

            try
            {
                _Result = m_RFIDErrorMessage.Update(_RFIDErrorMessageInfo);
            }
            catch (Exception ex)
            {
                _Result.Message = ex.Message;
            }
            return Json(_Result);
        }

        [HttpPost]
        public ActionResult StockInOut(StockInOutInfo _StockInOutInfo)
        {
            IResult _Result = new Result();

            try
            {
                var RFID = m_RFIDService.GetByNumber(_StockInOutInfo.PalletRFID);

                if (RFID == null)
                {
                    _Result.Success = false;
                    _Result.Message = "棧板RFID[" + _StockInOutInfo.PalletRFID + "]：不存在此筆資料";
                    return Json(_Result);
                }

                var Pallet = m_PalletService.GetByRFIDID(RFID.RFIDID);

                if (Pallet == null)
                {
                    _Result.Success = false;
                    _Result.Message = "棧板編號[" + Pallet.PalletNumber + "]：不存在此筆資料";
                }
                else if (Pallet.PalletTypeID == null || Pallet.PalletTypeID == "")
                {
                    _Result.Success = false;
                    _Result.Message = "棧板編號[" + Pallet.PalletNumber + "]：無設定類型";
                }
                else if (Pallet.RFIDID == null || Pallet.RFIDID == "")
                {
                    _Result.Success = false;
                    _Result.Message = "棧板編號[" + Pallet.PalletNumber + "]：無設定對應RFID";
                }
                else
                {
                    var Lots = m_WarehouseService.GetByLot(_StockInOutInfo.Lot).ToList();

                    if (Lots == null || Lots.Count == 0)
                    {
                        if (String.IsNullOrEmpty(_StockInOutInfo.StockIn))
                        {
                            _Result.Success = false;
                            _Result.Message = "Lot[" + _StockInOutInfo.Lot + "]：在系統內無資料且無入庫單號!!";
                        }
                        else
                        {
                            _Result = StockIn(_StockInOutInfo, Pallet);
                        }
                    }
                    else
                    {
                        _Result = CheckLotData(_StockInOutInfo, Lots, Pallet);

                        if (_Result.Success == true)
                        {
                            if (String.IsNullOrEmpty(_StockInOutInfo.StockOut) && String.IsNullOrEmpty(_StockInOutInfo.StockIn))
                            {
                                _Result = ChangeGateReader(_StockInOutInfo);
                            }
                            else if (String.IsNullOrEmpty(_StockInOutInfo.StockOut))
                            {
                                _Result.Success = false;
                                _Result.IsUpLoad = true;
                                _Result.Message = "Lot[" + _StockInOutInfo.Lot + "]：已存在系統內且無出庫單號!!";
                            }
                            else
                            {
                                _Result = StockOut(_StockInOutInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _Result.Success = false;
                _Result.Message = ex.Message;
            }

            return Json(_Result);
        }

        /// <summary>
        /// 補印標籤(入庫)
        /// </summary>
        /// <param name="_StockInOutInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RePrintAndStockIn(StockInOutInfo _StockInOutInfo)
        {
            IResult _Result = new Result();

            try
            {
                #region 確認棧板是否存在
                var RFID = m_RFIDService.GetByNumber(_StockInOutInfo.PalletRFID);

                if (RFID == null)
                {
                    throw new Exception("棧板RFID[" + _StockInOutInfo.PalletRFID + "]：不存在此筆資料");
                }

                var Pallet = m_PalletService.GetByRFIDID(RFID.RFIDID);

                if (Pallet == null)
                {
                    throw new Exception("棧板編號[" + Pallet.PalletNumber + "]：不存在此筆資料");
                }
                else if (Pallet.PalletTypeID == null || Pallet.PalletTypeID == "")
                {
                    throw new Exception("棧板編號[" + Pallet.PalletNumber + "]：無設定類型");
                }
                else if (Pallet.RFIDID == null || Pallet.RFIDID == "")
                {
                    throw new Exception("棧板編號[" + Pallet.PalletNumber + "]：無設定對應RFID");
                }
                #endregion

                #region 確認批號是否存在
                var Lots = m_WarehouseService.GetByLot(_StockInOutInfo.Lot).ToList();

                using (TransactionScope myScope = new TransactionScope())
                {

                    if (Lots == null || Lots.Count == 0)
                    {
                        //執行批號入庫
                        _Result = StockIn(_StockInOutInfo, Pallet);

                        if (_Result.Success == false)
                            throw new Exception(_Result.Message);
                    }
                    else
                    {
                        //如果批號已存在，則更新資料
                        #region 整理資料
                        var AlarmInfo = m_AlarmService.GetByDeviceName(_StockInOutInfo.Fabric);

                        CST_WAREHOUSE Data = new CST_WAREHOUSE()
                        {
                            LastGateReaderNumber = _StockInOutInfo.GateReaderNumber,
                            DeviceName = _StockInOutInfo.DeviceName,
                            PalletNumber = Pallet.PalletNumber,
                            PalletTypeID = Pallet.PalletTypeID,
                            RFIDID = Pallet.RFIDID,
                            Lot = _StockInOutInfo.Lot,
                            RFID = _StockInOutInfo.RFID,
                            WarningDay = (AlarmInfo == null) ? null : AlarmInfo.WarningDay,
                            WO = _StockInOutInfo.WO,
                            Quantity = _StockInOutInfo.Quantity,
                            StockIn = _StockInOutInfo.StockIn,
                            Color = _StockInOutInfo.Color,
                            ColorCode = _StockInOutInfo.ColorCode,
                            Date = _StockInOutInfo.Date,
                            Fabric = _StockInOutInfo.Fabric,
                            Length = _StockInOutInfo.Length,
                            LotNo = _StockInOutInfo.LotNo,
                            Order = _StockInOutInfo.Order,
                            PNo = _StockInOutInfo.PNo,
                            Weight = _StockInOutInfo.Weight,
                            YdWt = _StockInOutInfo.YdWt,
                            Width = _StockInOutInfo.Width,
                            WKNo = _StockInOutInfo.WKNo,
                            InternalOrder = _StockInOutInfo.InternalOrder,

                            LastUpdateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                        };
                        #endregion

                        _Result = m_WarehouseService.Update(Lots[0].WareHouseDataID, Data);

                        if (_Result.Success == false)
                            throw new Exception(_Result.Message);
                    }

                    string palletNumber = HexToString(_StockInOutInfo.PalletRFID);

                    var isSuccess = ERPWebService.Value.SetLotData(_StockInOutInfo.Lot, _StockInOutInfo.RFID, palletNumber, _StockInOutInfo.PalletRFID);

                    if (isSuccess)
                    {
                        myScope.Complete();
                        _Result.Success = true;
                        _Result.Message = _StockInOutInfo.Lot + " ERP資料更新成功";
                    }
                    else
                    {
                        _Result.Success = false;
                        _Result.Message = _StockInOutInfo.Lot + " ERP資料更新失敗";
                    }

                }


                #endregion

            }
            catch (Exception ex)
            {
                _Result.Success = false;
                _Result.Message = ex.Message;
            }

            return Json(_Result);
        }

        /// <summary>
        /// 倉存布疋逾時發送email通知
        /// </summary>
        /// <param name="QueryTime"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AlarmProcess(string QueryTime)
        {
            var result = new WarehouseSystem.Service.Misc.Result();

            try
            {
                //取得逾時的布疋資料
                var LotInfoList = m_WarehouseService.GetLotsByOverTime().ToList();

                if (LotInfoList.Count > 0)
                {
                    #region 重新整理資料(依據DEVICE分類)
                    var _AlarmList = getAlarmList(LotInfoList);

                    if (_AlarmList.Count == 0)
                    {
                        throw new Exception("無設定任何收信人資料");
                    }
                    #endregion

                    #region 發送EMAIL
                    DesryptAPI _DesryptAPI = new DesryptAPI();

                    foreach (var _AlarmInfo in _AlarmList)
                    {
                        int smtpPort = Convert.ToInt16(ConfigurationManager.AppSettings["smtpPort"]);
                        string smtpServer = ConfigurationManager.AppSettings["smtpServer"];
                        string mailPwd = _DesryptAPI.DesDecrypt(ConfigurationManager.AppSettings["mailPwd"]);
                        string mailAccount = ConfigurationManager.AppSettings["mailAccount"];
                        string strAddress = getEmails(_AlarmInfo.MailList);
                        string strCC = "";
                        string mailTitle = "倉管系統-布疋庫存逾時通知[" + _AlarmInfo.Fabric + "]";
                        string mailBoby = getBoby(_AlarmInfo.LotInfo);
                        string[] strMailTos = strAddress == "" ? null : strAddress.Split(';');
                        string[] strcc = strCC == "" ? null : strCC.Split(';');
                        string userName = ConfigurationManager.AppSettings["userName"];

                        bool blnIsSend = SendMail(mailAccount, strMailTos, strcc, mailTitle,
                         mailBoby, false, false, smtpServer, smtpPort, mailPwd, userName);
                    };

                    result.Success = true;
                    result.Message = "處理完畢";
                    #endregion
                }
                else
                {
                    throw new Exception("查詢無任何逾時資料");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(result);

        }

        /// <summary>
        /// 執行未綁定儲位發送email通知
        /// </summary>
        /// <param name="QueryTime"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelayBySetLotcationProcess(string QueryTime)
        {
            var result = new WarehouseSystem.Service.Misc.Result();

            try
            {
                //取得所有儲位欄位為空白的資料
                var LotInfoListTmp = m_WarehouseService.GetLotsDelayBySetLocation().ToList();

                if (LotInfoListTmp.Count > 0)
                {
                    #region 重新整理喻時設定儲位的資料

                    #region 取得設定檔資料
                    CST_ALARM alarmData = new CST_ALARM();

                    var alarmDataList = m_AlarmService.GetDataByType("LOCATION").ToList();

                    if (alarmDataList.Count == 0)
                    {
                        throw new Exception("查無設定檔資料");
                    }
                    else
                    {
                        alarmData = alarmDataList[0];
                    }
                    #endregion

                    #region 取得喻時設定分鐘
                    int iDelayMinutes = Convert.ToInt32(alarmData.WarningDay);

                    if (iDelayMinutes <= 0)
                    {
                        throw new Exception(string.Format("喻時設定分鐘:{0}，必須大於零", iDelayMinutes.ToString()));
                    }
                    #endregion

                    #region 取得收信人清單
                    var mailList = m_MailDetailService.getByMailGroupID(alarmData.MailGroupID).ToList();
                    if (mailList.Count == 0)
                    {
                        throw new Exception("無設定任何收信人資料");
                    }
                    #endregion

                    //取得目前系統時間
                    DateTime dtTime = System.DateTime.Now;

                    List<vw_Lots> lotInfoList = new List<vw_Lots>();

                    foreach (var lot in LotInfoListTmp)
                    {
                        DateTime delayTime = Convert.ToDateTime(lot.LastUpdateTime);

                        delayTime = delayTime.AddMinutes(iDelayMinutes);

                        //如果時間小於目前系統時間，則視為喻時的資料
                        if (dtTime > delayTime)
                        {
                            lotInfoList.Add(lot);
                        }
                    }

                    #endregion

                    if (lotInfoList.Count > 0)
                    {
                        #region 處理EMAIL
                        //取得不重複棧板編號
                        //var palletList = lotInfoList.Select(item => item.PalletNumber).Distinct().ToList();

                        List<vw_Lots> palletList = new List<vw_Lots>();

                        foreach (var item in lotInfoList)
                        {
                            if (palletList.Count != 0)
                            {
                                bool isFind = false;

                                foreach (var pallet in palletList)
                                {
                                    if (item.PalletNumber == pallet.PalletNumber)
                                    {
                                        isFind = true;
                                        break;
                                    }
                                }
                                if (isFind == false)
                                {
                                    palletList.Add(item);
                                }
                            }
                            else
                            {
                                palletList.Add(item);
                            }
                        }

                        #region 設置信件內容
                        string mailMessage = "棧板編號\t過閘門時間\t\n";

                        foreach (var pallet in palletList)
                        {
                            mailMessage += pallet.PalletNumber + "\t" + pallet.LastUpdateTime + "\t\n";
                        }

                        mailMessage += "\n以上棧板編號尚未設定儲位，請相關人員執行設定動作!!\n";

                        #endregion

                        DesryptAPI _DesryptAPI = new DesryptAPI();

                        int smtpPort = Convert.ToInt16(ConfigurationManager.AppSettings["smtpPort"]);
                        string smtpServer = ConfigurationManager.AppSettings["smtpServer"];
                        string mailPwd = _DesryptAPI.DesDecrypt(ConfigurationManager.AppSettings["mailPwd"]);
                        string mailAccount = ConfigurationManager.AppSettings["mailAccount"];
                        string strAddress = getEmails(mailList);
                        string strCC = "";
                        string mailTitle = "倉管系統-入庫棧板逾時未設定儲位通知";
                        string mailBoby = mailMessage;
                        string[] strMailTos = strAddress == "" ? null : strAddress.Split(';');
                        string[] strcc = strCC == "" ? null : strCC.Split(';');
                        string userName = ConfigurationManager.AppSettings["userName"];

                        bool blnIsSend = SendMail(mailAccount, strMailTos, strcc, mailTitle,
                         mailBoby, false, false, smtpServer, smtpPort, mailPwd, userName);

                        result.Success = true;
                        result.Message = "處理完畢";

                        #endregion
                    }
                    else
                    {
                        throw new Exception("查詢無任何逾時資料");
                    }
                }
                else
                {
                    throw new Exception("查詢無任何逾時資料");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(result);
        }

        private IResult CheckLotData(StockInOutInfo _StockInOutInfo, List<CST_WAREHOUSE> Lots, CST_PALLET Pallet)
        {
            IResult _Result = new Result(false);

            try
            {
                string Message = "";

                bool IsFind = false;

                for (int j = 0; j < Lots.Count; j++)
                {
                    if (Lots[j].Lot == _StockInOutInfo.Lot)
                    {
                        IsFind = true;
                        break;
                    }
                }

                if (IsFind == false) Message += _StockInOutInfo.Lot;

                if (String.IsNullOrEmpty(Message))
                {
                    _Result.Success = true;
                }
                else
                {
                    _Result.Success = false;
                    _Result.Message = "Lot[" + Message + "]：在系統上對應棧板編號[" + Pallet.PalletNumber + "] 無此資料";
                }
            }
            catch (Exception ex)
            {
                _Result.Success = false;
                _Result.Message = ex.Message;
            }

            return _Result;
        }

        private IResult StockIn(StockInOutInfo _StockInOutInfo, CST_PALLET Pallet)
        {
            IResult _Result = new Result(false);

            try
            {
                var AlarmInfo = m_AlarmService.GetByDeviceName(_StockInOutInfo.Fabric);

                CST_WAREHOUSE Data = new CST_WAREHOUSE()
                {
                    LastGateReaderNumber = _StockInOutInfo.GateReaderNumber,
                    DeviceName = _StockInOutInfo.DeviceName,
                    PalletNumber = Pallet.PalletNumber,
                    PalletTypeID = Pallet.PalletTypeID,
                    RFIDID = Pallet.RFIDID,
                    Lot = _StockInOutInfo.Lot,
                    RFID = _StockInOutInfo.RFID,
                    WarningDay = (AlarmInfo == null) ? null : AlarmInfo.WarningDay,
                    WO = _StockInOutInfo.WO,
                    Quantity = _StockInOutInfo.Quantity,
                    StockIn = _StockInOutInfo.StockIn,
                    Color = _StockInOutInfo.Color,
                    ColorCode = _StockInOutInfo.ColorCode,
                    Date = _StockInOutInfo.Date,
                    Fabric = _StockInOutInfo.Fabric,
                    Length = _StockInOutInfo.Length,
                    LotNo = _StockInOutInfo.LotNo,
                    Order = _StockInOutInfo.Order,
                    PNo = _StockInOutInfo.PNo,
                    Weight = _StockInOutInfo.Weight,
                    YdWt = _StockInOutInfo.YdWt,
                    Width = _StockInOutInfo.Width,
                    WKNo = _StockInOutInfo.WKNo,
                    InternalOrder = _StockInOutInfo.InternalOrder,

                    LastUpdateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                };

                _Result = m_WarehouseService.Insert(Data);

                //_Result.Success = true;
            }
            catch (Exception ex)
            {
                _Result.Success = false;
                _Result.Message = ex.Message;
            }

            return _Result;
        }

        private void StockOut(string lot, string stockOut)
        {
            //取得批號資料
            var lotData = m_WarehouseService.GetDataByLot(lot);

            #region 刪除資料
            m_WarehouseService.DeleteBySID(lotData.WareHouseDataID);

            CST_WAREHOUSE_LOG cstWarehouseLog = new CST_WAREHOUSE_LOG()
            {
                Color = lotData.Color,
                ColorCode = lotData.ColorCode,
                Date = lotData.Date,
                WarningDay = lotData.WarningDay,
                Weight = lotData.Weight,
                Width = lotData.Width,
                WKNo = lotData.WKNo,
                YdWt = lotData.YdWt,
                StockOutTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                StockIn = lotData.StockIn,
                StockInTime = lotData.StockInTime,
                RFID = lotData.RFID,
                RFIDID = lotData.RFIDID,
                PreGateReaderNumber = lotData.PreGateReaderNumber,
                PNo = lotData.PNo,
                ParkingBlockID = lotData.ParkingBlockID,
                PalletTypeID = lotData.PalletTypeID,
                PalletNumber = lotData.PalletNumber,
                Fabric = lotData.Fabric,
                InternalOrder = lotData.InternalOrder,
                LastGateReaderNumber = lotData.LastGateReaderNumber,
                LastUpdateTime = lotData.LastUpdateTime,
                Length = lotData.Length,
                Lot = lotData.Lot,
                LotNo = lotData.LotNo,
                Order = lotData.Order,
                OverTime = lotData.OverTime,
                StockOut = stockOut,
            };
            #endregion

            //新增記錄
            m_WarehouseLogService.Insert(cstWarehouseLog);

        }

        private IResult StockOut(StockInOutInfo _StockInOutInfo)
        {
            IResult _Result = new Result(false);

            try
            {
                _Result = m_WarehouseService.stockOut(_StockInOutInfo);
            }
            catch (Exception ex)
            {
                _Result.Success = false;
                _Result.Message = ex.Message;
            }

            return _Result;
        }

        private IResult ChangeGateReader(StockInOutInfo _StockInOutInfo)
        {
            IResult _Result = new Result(false);

            try
            {
                _Result = m_WarehouseService.ChangeGateReader(_StockInOutInfo);
            }
            catch (Exception ex)
            {
                _Result.Success = false;
                _Result.Message = ex.Message;
            }

            return _Result;
        }

        private string getBoby(List<vw_Lots> _LotList)
        {
            string Result = "";

            //Result = "儲位\t" + "棧板\t" + "布疋批號\t\n";

            Result = "布疋批號\t" + "布別\t" + "顏色\t" + "缸號\t" + "棧板\t" + "重量\t" + "碼長\t" + "入庫時間\t" + "內部訂單\t\n";

            foreach (var Lot in _LotList)
            {
                Result += Lot.PLot + "\t"
                    + Lot.Fabric + "\t"
                    + Lot.ColorCode + "\t"
                    + Lot.LotNo + "\t"
                    + Lot.PalletNumber + "\t"
                    + Lot.Weight + "\t"
                    + Lot.Length + "\t"
                    + Lot.StockInTime + "\t"
                    + Lot.InternalOrder + "\t\n";
            }

            return Result;
        }

        /// <summary>
        /// 回傳收信人的E-MAIL
        /// </summary>
        /// <param name="_MailList"></param>
        /// <returns></returns>
        private string getEmails(List<WarehouseSystem.Service.MailDetailService.MailList> _MailList)
        {
            string Result = "";

            foreach (var _Mail in _MailList)
            {
                Result += _Mail.Email + ";";
            }

            return Result;
        }

        private List<AlarmInfo> getAlarmList(List<vw_Lots> LotList)
        {
            List<AlarmInfo> _AlarmInfo = new List<AlarmInfo>();

            foreach (var Lot in LotList)
            {
                bool IsFind = false;

                for (int i = 0; i < _AlarmInfo.Count; i++)
                {
                    if (_AlarmInfo[i].Fabric == Lot.Fabric)
                    {
                        _AlarmInfo[i].LotInfo.Add(Lot);
                        IsFind = true;
                    }
                }

                if (IsFind == false)
                {
                    var MailGroup = m_AlarmService.GetByDeviceName(Lot.Fabric);

                    if (MailGroup != null)
                    {
                        _AlarmInfo.Add(new AlarmInfo() { Fabric = Lot.Fabric });

                        _AlarmInfo[_AlarmInfo.Count - 1].LotInfo.Add(Lot);

                        var MailList = m_MailDetailService.getByMailGroupID(MailGroup.MailGroupID).ToList();

                        _AlarmInfo[_AlarmInfo.Count - 1].MailGroupID = MailGroup.MailGroupID;

                        _AlarmInfo[_AlarmInfo.Count - 1].MailList = MailList;
                    }
                }
            }

            return _AlarmInfo;
        }

        public static bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// 完整的寄信功能
        /// </summary>
        /// <param name="MailFrom">寄信人E-mail Address</param>
        /// <param name="MailTos">收信人E-mail Address</param>
        /// <param name="Ccs">副本E-mail Address</param>
        /// <param name="MailSub">主旨</param>
        /// <param name="MailBody">信件內容</param>
        /// <param name="isBodyHtml">是否採用HTML格式</param>
        /// <param name="filePaths">附檔在WebServer檔案總管路徑</param>
        /// <param name="deleteFileAttachment">是否刪除在WebServer上的附件</param>
        /// <returns>是否成功</returns>
        private bool SendMail(string MailFrom, string[] MailTos, string[] Ccs, string MailSub,
            string MailBody, bool isBodyHtml, bool deleteFileAttachment,
            string smtpServer, int smtpPort, string mailPwd, string userName)
        {
            try
            {
                string mailAccount = MailFrom;

                #region
                //防呆
                if (string.IsNullOrEmpty(MailFrom))
                {//※有些公司的Mail Server會規定寄信人的Domain Name要是該Mail Server的Domain Name
                    //MailFrom = mailAccount;
                }

                //建立MailMessage物件
                System.Net.Mail.MailMessage mms = new System.Net.Mail.MailMessage();
                //指定一位寄信人MailAddress
                mms.From = new MailAddress(MailFrom);
                //信件主旨
                mms.Subject = MailSub;

                //信件內容
                mms.Body = MailBody;

                //信件內容 是否採用Html格式
                mms.IsBodyHtml = isBodyHtml;


                if (MailTos != null)//防呆
                {
                    for (int i = 0; i < MailTos.Length; i++)
                    {
                        //加入信件的收信人(們)address
                        if (!string.IsNullOrEmpty(MailTos[i].Trim()))
                        {
                            mms.To.Add(new MailAddress(MailTos[i].Trim()));
                        }
                    }
                }//End if (MailTos !=null)//防呆

                if (Ccs != null) //防呆
                {
                    for (int i = 0; i < Ccs.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(Ccs[i].Trim()))
                        {
                            //加入信件的副本(們)address
                            mms.CC.Add(new MailAddress(Ccs[i].Trim()));
                        }

                    }
                }//End if (Ccs!=null) //防呆

                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))//或公司、客戶的smtp_server
                {
                    if (!string.IsNullOrEmpty(mailAccount) && !string.IsNullOrEmpty(mailPwd))//.config有帳密的話
                    {//寄信要不要帳密？眾說紛紜Orz，分享一下經驗談....

                        //網友阿尼尼:http://www.dotblogs.com.tw/kkc123/archive/2012/06/26/73076.aspx
                        //※公司內部不用認證,寄到外部信箱要特別認證 Account & Password

                        //自家公司MIS:
                        //※要看smtp server的設定呀~

                        //結論...
                        //※程式在客戶那邊執行的話，問客戶，程式在自家公司執行的話，問自家公司MIS，最準確XD

                        // 這段一定要, 要寫這個才可以跳過 "根據驗證程序,遠端憑證是無效的" 的錯誤
                        System.Net.ServicePointManager.ServerCertificateValidationCallback =
                            new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

                        client.Credentials = new NetworkCredential(userName, mailPwd);//寄信帳密
                    }
                    client.UseDefaultCredentials = true;
                    client.EnableSsl = true;
                    client.Send(mms);//寄出一封信
                }//end using

                //釋放每個附件，才不會Lock住
                if (mms.Attachments != null && mms.Attachments.Count > 0)
                {
                    for (int i = 0; i < mms.Attachments.Count; i++)
                    {
                        mms.Attachments[i].Dispose();
                        //mms.Attachments[i] = null;
                    }
                }
                #endregion

                return true;//成功
            }
            catch (Exception ex)
            {
                return false;//寄失敗
            }
        }

        public ActionResult CreatePalletData(string PalletNumber, string PalletTypeName, string PalletTypeCode, string RFID)
        {
            IResult _Result = new Result(false);

            try
            {
                string RFIDID = "";
                string PalletTypeID = "";

                var _RFID = m_RFIDService.GetByNumber(RFID);

                //確認RFID編號是否存在
                if (_RFID == null)
                {
                    var result = m_RFIDService.Create(RFID, ref RFIDID);

                    if (result.Success == false)
                    {
                        _Result.Message = result.Message;
                        return Json(_Result);
                    }
                }
                else
                {
                    RFIDID = _RFID.RFIDID;
                }

                var _PalletType = m_PalletTypeService.GetByTypeName(PalletTypeName);

                //確認棧板型別是否存在
                if (_PalletType == null)
                {
                    var result = m_PalletTypeService.Create(PalletTypeName, PalletTypeCode, ref PalletTypeID);

                    if (result.Success == false)
                    {
                        _Result.Message = result.Message;
                        return Json(_Result);
                    }
                }
                else
                {
                    PalletTypeID = _PalletType.PalletTypeID;
                }

                var _Pallet = m_PalletService.GetByRFIDID(RFIDID);

                //確認RFID編號是否存在
                if (_Pallet == null)
                {
                    string _PalletID = "";

                    var result = m_PalletService.Create(PalletNumber, RFIDID, PalletTypeID, ref _PalletID);

                    if (result.Success == false)
                    {
                        _Result.Message = result.Message;
                        return Json(_Result);
                    }
                    else
                    {
                        _Result = result;
                        _Result.Message = "[" + PalletNumber + "]：資料建立成功";
                    }
                }
                else
                {
                    _Result.Message = "[" + PalletNumber + "]：資料已經存在";
                    return Json(_Result);
                }
            }
            catch (Exception ex)
            {
                _Result.Success = false;
                _Result.Message = ex.Message;
            }

            return Json(_Result);

        }

        public ActionResult GetRFIDNo()
        {
            IResult _Result = new Result(false);

            var _RFID = m_RFIDService.GetRFIDNo().ToList();

            if (_RFID != null)
            {
                _Result.Success = true;
                _Result.Message = _RFID[0].RFIDNumber;
            }
            return Json(_Result);
        }

        /// <summary>
        /// 16進制轉ASCII
        /// </summary>
        /// <param name="strHex"></param>
        /// <param name="codeTypeLength"></param>
        /// <returns></returns>
        private string HexToString(string strHex, int codeTypeLength = 8)
        {
            string result = "";

            try
            {

                int Length = strHex.Length / 2;

                for (int i = 0 + codeTypeLength / 2; i < Length; i++)
                {
                    string SubString = strHex.Substring(i * 2, 2);

                    if (SubString == "00") continue;

                    var _Int = Convert.ToInt32(SubString, 16);

                    result += Convert.ToChar(_Int).ToString();
                }
            }
            catch (Exception ex)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// 新增一筆RFIDErrorMessage
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="readerNumber"></param>
        /// <param name="palletRFID"></param>
        private void InsertRFIDErrorMessage(RFIDData data, string type, string readerNumber, string palletRFID, bool isStockIn)
        {
            string lot = "";
            string orderNo = "";

            if (isStockIn)
            {
                //傳入布疋RFID給ERP系統並取得批號資訊
                var lotDataSet = ERPWebService.Value.GetLotData(data.RFID);
                if (lotDataSet.Tables[0].Rows.Count > 0)
                {
                    lot = lotDataSet.Tables[0].Rows[0]["WKNo"].ToString().Trim() + lotDataSet.Tables[0].Rows[0]["PNo"].ToString().Trim();
                }
                else
                {
                    //如果ERP系統取不到資料，則系統自動轉換
                    lot = HexToString(data.RFID, 0);
                }
            }
            else
            {
                var lotData = m_WarehouseService.GetDataByRFID(data.RFID);

                lot = lotData.Lot;

            }

            #region 建立資料
            RFIDErrorMessageInfo result = new RFIDErrorMessageInfo()
            {
                RFID = data.RFID,
                ErrorMessage = data.ErrorMessage,
                UpLoadType = type,
                GateReaderNumber = readerNumber,
                PalletRFID = palletRFID,
                StartTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                Lot = lot,
                OrderNo = orderNo
            };
            #endregion

            //寫入資料庫
            m_RFIDErrorMessage.Create(result);
        }

        /// <summary>
        /// 更新一筆RFIDErrorMessage
        /// </summary>
        /// <param name="addLot"></param>
        /// <param name="type"></param>
        /// <param name="readerNumber"></param>
        /// <param name="palletRFID"></param>
        private void UpdateRFIDErrorMessage(StockInOutInfo addLot, string type, string readerNumber, string palletRFID)
        {
            #region 建立資料
            RFIDErrorMessageInfo result = new RFIDErrorMessageInfo()
            {
                RFID = addLot.RFID,
                GateReaderNumber = readerNumber,
                PalletRFID = palletRFID,
                Lot = addLot.Lot,
                OrderNo = addLot.Order,
                UpLoadType = type
            };
            #endregion

            //更新資料庫
            m_RFIDErrorMessage.Update(result);
        }

        /// <summary>
        /// ASCII轉16進制
        /// </summary>
        /// <param name="strMsg"></param>
        /// <param name="CodeType"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public string StringToHex(string strMsg, string CodeType = "00", int MaxLength = 24)
        {
            string result = "";

            foreach (var _Char in strMsg)
            {
                var _Int = Convert.ToInt32(_Char);

                result += String.Format("{0:x}", _Int);
            }

            result = CodeType + result;

            int diff = MaxLength - result.Length;

            for (int i = 0; i < diff; i++)
            {
                result += "0";
            }

            return result;
        }

        /// <summary>
        /// 入庫退繳
        /// </summary>
        /// <returns></returns>
        public ActionResult ReturnToStock()
        {
            IResult _Result = new Result();

            try
            {
                string sqlString = "";

                string Message = "";

                #region 取得退繳資料
                var dataSet = ERPWebService.Value.ReturnToStock();

                List<string> lstTmpLot = new List<string>();

                var myTable = dataSet.Tables[0];

                foreach (DataRow dr in myTable.Rows)
                {
                    var sLot = dr["LOT"].ToString().Trim();

                    if (string.IsNullOrEmpty(sLot) == false)
                    {
                        lstTmpLot.Add(sLot);
                    }
                }
                #endregion

                if (lstTmpLot.Count > 0)
                {
                    //查詢資料庫是否有這些退繳布疋資料
                    var dataList = m_WarehouseService.GetByLot(lstTmpLot).ToList();

                    if (dataList.Count > 0)
                    {
                        using (TransactionScope myScope = new TransactionScope())
                        {
                            using (WarehouseServerEntities db = new WarehouseServerEntities())
                            {
                                #region 刪除資料
                                sqlString = string.Format(@"DELETE CST_WAREHOUSE");

                                for (int i = 0; i < dataList.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        sqlString += " WHERE LOT IN (" + "'" + dataList[0].Lot + "'";
                                        Message += dataList[i].Lot;
                                    }
                                    else
                                    {
                                        sqlString += "," + "'" + dataList[i].Lot + "'";
                                        Message += "," + dataList[i].Lot;
                                    }
                                }

                                sqlString += ")";

                                db.Database.ExecuteSqlCommand(sqlString);

                                #endregion

                                #region 新增記錄
                                foreach (var item in dataList)
                                {
                                    IdGenerator idg = new IdGenerator();
                                    var ID = idg.GetSID(1);

                                    sqlString = string.Format(@"INSERT INTO CST_WAREHOUSE_LOG VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}',
                                        '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}','{19}',
                                        '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}','{29}',
                                        '{30}', '{31}', '{32}')",
                                        ID, item.DeviceName, item.PalletNumber, item.PalletTypeID, item.ParkingBlockID, item.RFIDID, item.StockInTime, item.StockOutTime, item.Quantity, item.PreGateReaderNumber,
                                        "Loader", item.WO, item.Lot, item.OverTime, item.MailFlag, item.LastUpdateTime, item.WarningDay, item.StockIn, item.StockOut, item.RFID,
                                        item.WKNo, item.LotNo, item.Order, item.InternalOrder, item.Fabric, item.Color, item.Width, item.YdWt, item.Weight, item.Length,
                                        item.PNo, item.Date, item.ColorCode);
                                    db.Database.ExecuteSqlCommand(sqlString);
                                }
                                #endregion
                            }

                            #region ERP退繳註記
                            List<string> lstLot = new List<string>();
                            List<string> lstPallet = new List<string>();
                            List<string> lstPalletRFID = new List<string>();

                            foreach (var data in dataList)
                            {
                                lstLot.Add(data.Lot);
                                lstPallet.Add(data.PalletNumber);

                                var RFIDData = m_RFIDService.GetBySID(data.RFIDID);

                                lstPalletRFID.Add(RFIDData.RFIDNumber);
                            }


                            var isOK = ERPWebService.Value.SetRemarkByReturnToStock(lstLot.ToArray(), lstPallet.ToArray(), lstPalletRFID.ToArray(), true);

                            if (isOK)
                            {
                                myScope.Complete();
                            }
                            else
                            {
                                throw new Exception(string.Format("ERP回傳退繳註記失敗"));
                            }
                            #endregion
                        }

                        _Result.Success = true;

                        _Result.Message = string.Format("執行成功 {0}筆資料，({1})", dataList.Count(), Message);
                    }
                    else
                    {
                        #region 建置Message資料
                        for (int i = 0; i < lstTmpLot.Count; i++)
                        {
                            if (i == 0)
                            {
                                Message += lstTmpLot[i];
                            }
                            else
                            {
                                Message += "," + lstTmpLot[i];
                            }
                        }
                        #endregion

                        _Result.Success = true;
                        _Result.Message = string.Format("倉管系統查無退繳資料({0})", Message);
                    }
                }
                else
                {
                    _Result.Success = true;
                    _Result.Message = "ERP系統查無退繳資料";
                }
            }
            catch (Exception ex)
            {
                _Result.Message = ex.Message;
            }
            return Json(_Result);
        }

        /// <summary>
        /// 入庫取消退繳
        /// </summary>
        /// <returns></returns>
        public ActionResult UndoReturnToStock()
        {
            IResult _Result = new Result();

            try
            {

                string sqlString = "";

                string Message = "";

                #region 取得取消退繳資料
                var dataSet = ERPWebService.Value.UndoReturnToStock();

                List<string> lstTmpLot = new List<string>();

                var myTable = dataSet.Tables[0];

                foreach (DataRow dr in myTable.Rows)
                {
                    var sLot = dr["LOT"].ToString().Trim();

                    if (string.IsNullOrEmpty(sLot) == false)
                    {
                        lstTmpLot.Add(sLot);
                    }
                }
                #endregion

                if (lstTmpLot.Count > 0)
                {
                    //查詢資料庫是否有這些取消退繳布疋資料
                    var dataList = m_WarehouseLogService.GetByLot(lstTmpLot).ToList();

                    if (dataList.Count > 0)
                    {
                        using (TransactionScope myScope = new TransactionScope())
                        {
                            using (WarehouseServerEntities db = new WarehouseServerEntities())
                            {
                                #region 刪除記錄資料
                                sqlString = string.Format(@"DELETE CST_WAREHOUSE_LOG");

                                for (int i = 0; i < dataList.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        sqlString += " WHERE LOT IN (" + "'" + dataList[0].Lot + "'";
                                        Message += dataList[i].Lot;
                                    }
                                    else
                                    {
                                        sqlString += "," + "'" + dataList[i].Lot + "'";
                                        Message += "," + dataList[i].Lot;
                                    }
                                }

                                sqlString += ")";

                                db.Database.ExecuteSqlCommand(sqlString);

                                #endregion

                                #region 新增資料
                                foreach (var item in dataList)
                                {
                                    IdGenerator idg = new IdGenerator();
                                    var ID = idg.GetSID(1);

                                    sqlString = string.Format(@"INSERT INTO CST_WAREHOUSE VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}',
                                        '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}','{19}',
                                        '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}','{29}',
                                        '{30}', '{31}', '{32}')",
                                        ID, item.DeviceName, item.PalletNumber, item.PalletTypeID, item.ParkingBlockID, item.RFIDID, item.StockInTime, item.StockOutTime, item.Quantity, item.PreGateReaderNumber,
                                        "Loader", item.WO, item.Lot, item.OverTime, item.MailFlag, item.LastUpdateTime, item.WarningDay, item.StockIn, item.StockOut, item.RFID,
                                        item.WKNo, item.LotNo, item.Order, item.InternalOrder, item.Fabric, item.Color, item.Width, item.YdWt, item.Weight, item.Length,
                                        item.PNo, item.Date, item.ColorCode);
                                    db.Database.ExecuteSqlCommand(sqlString);
                                }
                                #endregion

                            }

                            #region ERP取消退繳註記
                            List<string> lstLot = new List<string>();
                            List<string> lstPallet = new List<string>();
                            List<string> lstPalletRFID = new List<string>();

                            foreach (var data in dataList)
                            {
                                lstLot.Add(data.Lot);
                                lstPallet.Add(data.PalletNumber);

                                var RFIDData = m_RFIDService.GetBySID(data.RFIDID);

                                lstPalletRFID.Add(RFIDData.RFIDNumber);
                            }

                            var isOK = ERPWebService.Value.SetRemarkByReturnToStock(lstLot.ToArray(), lstPallet.ToArray(), lstPalletRFID.ToArray(), false);

                            if (isOK)
                            {
                                myScope.Complete();
                            }
                            else
                            {
                                throw new Exception(string.Format("ERP回傳取消退繳註記失敗"));
                            }
                            #endregion
                        }

                        _Result.Success = true;

                        _Result.Message = string.Format("執行成功 {0}筆資料，({1})", dataList.Count(), Message);
                    }
                    else
                    {

                        #region 建置Message資料
                        for (int i = 0; i < lstTmpLot.Count; i++)
                        {
                            if (i == 0)
                            {
                                Message += lstTmpLot[i];
                            }
                            else
                            {
                                Message += "," + lstTmpLot[i];
                            }
                        }
                        #endregion

                        _Result.Success = true;
                        _Result.Message = string.Format("倉管系統查無取消退繳資料({0})", Message);
                    }
                }
                else
                {
                    _Result.Success = true;
                    _Result.Message = "ERP系統查無取消退繳資料";
                }
            }
            catch (Exception ex)
            {
                _Result.Message = ex.Message;
            }
            return Json(_Result);
        }

        /// <summary>
        /// 入庫解除完板
        /// </summary>
        /// <returns></returns>
        public ActionResult ReleasePalletAndReturnToStock()
        {
            IResult _Result = new Result();

            try
            {
                string sqlString = "";

                string Message = "";

                #region 取得退繳資料
                var dataSet = ERPWebService.Value.ReleasePalletAndReturnToStock();

                List<string> lstTmpLot = new List<string>();

                var myTable = dataSet.Tables[0];

                foreach (DataRow dr in myTable.Rows)
                {
                    var sLot = dr["LOT"].ToString().Trim();

                    if (string.IsNullOrEmpty(sLot) == false)
                    {
                        lstTmpLot.Add(sLot);
                    }
                }
                #endregion

                if (lstTmpLot.Count > 0)
                {
                    //查詢資料庫是否有這些退繳布疋資料
                    var dataList = m_WarehouseService.GetByLot(lstTmpLot).ToList();

                    if (dataList.Count > 0)
                    {
                        using (TransactionScope myScope = new TransactionScope())
                        {
                            using (WarehouseServerEntities db = new WarehouseServerEntities())
                            {
                                #region 刪除資料
                                sqlString = string.Format(@"DELETE CST_WAREHOUSE");

                                for (int i = 0; i < dataList.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        sqlString += " WHERE LOT IN (" + "'" + dataList[0].Lot + "'";
                                        Message += dataList[i].Lot;
                                    }
                                    else
                                    {
                                        sqlString += "," + "'" + dataList[i].Lot + "'";
                                        Message += "," + dataList[i].Lot;
                                    }
                                }

                                sqlString += ")";

                                db.Database.ExecuteSqlCommand(sqlString);

                                #endregion

                                #region 新增記錄
                                foreach (var item in dataList)
                                {
                                    IdGenerator idg = new IdGenerator();
                                    var ID = idg.GetSID(1);

                                    sqlString = string.Format(@"INSERT INTO CST_WAREHOUSE_LOG VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}',
                                        '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}','{19}',
                                        '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}','{29}',
                                        '{30}', '{31}', '{32}')",
                                        ID, item.DeviceName, item.PalletNumber, item.PalletTypeID, item.ParkingBlockID, item.RFIDID, item.StockInTime, item.StockOutTime, item.Quantity, item.PreGateReaderNumber,
                                        "Loader", item.WO, item.Lot, item.OverTime, item.MailFlag, item.LastUpdateTime, item.WarningDay, item.StockIn, item.StockOut, item.RFID,
                                        item.WKNo, item.LotNo, item.Order, item.InternalOrder, item.Fabric, item.Color, item.Width, item.YdWt, item.Weight, item.Length,
                                        item.PNo, item.Date, item.ColorCode);
                                    db.Database.ExecuteSqlCommand(sqlString);
                                }
                                #endregion
                            }

                            #region ERP退繳註記
                            List<string> lstLot = new List<string>();
                            List<string> lstPallet = new List<string>();
                            List<string> lstPalletRFID = new List<string>();

                            foreach (var data in dataList)
                            {
                                lstLot.Add(data.Lot);
                                lstPallet.Add(data.PalletNumber);

                                var RFIDData = m_RFIDService.GetBySID(data.RFIDID);

                                lstPalletRFID.Add(RFIDData.RFIDNumber);
                            }

                            var isOK = ERPWebService.Value.SetRemarkByReturnToStock(lstLot.ToArray(), lstPallet.ToArray(), lstPalletRFID.ToArray(), true);

                            if (isOK)
                            {
                                myScope.Complete();
                            }
                            else
                            {
                                throw new Exception(string.Format("ERP回傳退繳註記失敗"));
                            }
                            #endregion
                        }

                        _Result.Success = true;

                        _Result.Message = string.Format("執行成功 {0}筆資料，({1})", dataList.Count(), Message);
                    }
                    else
                    {
                        #region 建置Message資料
                        for (int i = 0; i < lstTmpLot.Count; i++)
                        {
                            if (i == 0)
                            {
                                Message += lstTmpLot[i];
                            }
                            else
                            {
                                Message += "," + lstTmpLot[i];
                            }
                        }
                        #endregion

                        _Result.Success = true;
                        _Result.Message = string.Format("倉管系統查無退繳資料({0})", Message);
                    }
                }
                else
                {
                    _Result.Success = true;
                    _Result.Message = "ERP系統查無退繳資料";
                }
            }
            catch (Exception ex)
            {
                _Result.Message = ex.Message;
            }
            return Json(_Result);
        }

        public ActionResult TestUndoReturnToStock(string lot)
        {
            IResult _Result = new Result();

            try
            {

                string sqlString = "";

                string Message = "";

                if (string.IsNullOrEmpty(lot) == false)
                {
                    //查詢資料庫是否有這些取消退繳布疋資料
                    var dataList = m_WarehouseLogService.GetByLot(lot).ToList();

                    if (dataList.Count > 0)
                    {
                        using (TransactionScope myScope = new TransactionScope())
                        {
                            using (WarehouseServerEntities db = new WarehouseServerEntities())
                            {
                                #region 刪除記錄資料
                                sqlString = string.Format(@"DELETE CST_WAREHOUSE_LOG WHERE LOT = '{0}'", lot);

                                db.Database.ExecuteSqlCommand(sqlString);

                                #endregion

                                #region 新增資料
                                foreach (var item in dataList)
                                {
                                    IdGenerator idg = new IdGenerator();
                                    var ID = idg.GetSID(1);

                                    sqlString = string.Format(@"INSERT INTO CST_WAREHOUSE VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}',
                                        '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}','{19}',
                                        '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}','{29}',
                                        '{30}', '{31}', '{32}')",
                                        ID, item.DeviceName, item.PalletNumber, item.PalletTypeID, item.ParkingBlockID, item.RFIDID, item.StockInTime, item.StockOutTime, item.Quantity, item.PreGateReaderNumber,
                                        "Loader", item.WO, item.Lot, item.OverTime, item.MailFlag, item.LastUpdateTime, item.WarningDay, item.StockIn, item.StockOut, item.RFID,
                                        item.WKNo, item.LotNo, item.Order, item.InternalOrder, item.Fabric, item.Color, item.Width, item.YdWt, item.Weight, item.Length,
                                        item.PNo, item.Date, item.ColorCode);
                                    db.Database.ExecuteSqlCommand(sqlString);
                                }
                                #endregion

                            }

                            myScope.Complete();
                        }

                        _Result.Success = true;

                        _Result.Message = string.Format("執行成功 {0}筆資料，({1})", dataList.Count(), lot);
                    }
                    else
                    {
                        _Result.Success = true;
                        _Result.Message = string.Format("倉管系統查無取消退繳資料({0})", lot);
                    }
                }
                else
                {
                    _Result.Success = true;
                    _Result.Message = "ERP系統查無取消退繳資料";
                }
            }
            catch (Exception ex)
            {
                _Result.Message = ex.Message;
            }
            return Json(_Result);
        }

        public ActionResult TestReturnToStock(string lot)
        {
            IResult _Result = new Result();

            try
            {
                string sqlString = "";

                string Message = "";


                if (string.IsNullOrEmpty(lot) == false)
                {
                    //查詢資料庫是否有這些退繳布疋資料
                    var dataList = m_WarehouseService.GetByLot(lot).ToList();

                    if (dataList.Count > 0)
                    {
                        using (TransactionScope myScope = new TransactionScope())
                        {
                            using (WarehouseServerEntities db = new WarehouseServerEntities())
                            {
                                #region 刪除資料
                                sqlString = string.Format(@"DELETE CST_WAREHOUSE WHERE LOT = '{0}'", lot);

                                db.Database.ExecuteSqlCommand(sqlString);

                                #endregion

                                #region 新增記錄
                                foreach (var item in dataList)
                                {
                                    IdGenerator idg = new IdGenerator();
                                    var ID = idg.GetSID(1);

                                    sqlString = string.Format(@"INSERT INTO CST_WAREHOUSE_LOG VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}',
                                        '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}','{19}',
                                        '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}','{29}',
                                        '{30}', '{31}', '{32}')",
                                        ID, item.DeviceName, item.PalletNumber, item.PalletTypeID, item.ParkingBlockID, item.RFIDID, item.StockInTime, item.StockOutTime, item.Quantity, item.PreGateReaderNumber,
                                        "Loader", item.WO, item.Lot, item.OverTime, item.MailFlag, item.LastUpdateTime, item.WarningDay, item.StockIn, item.StockOut, item.RFID,
                                        item.WKNo, item.LotNo, item.Order, item.InternalOrder, item.Fabric, item.Color, item.Width, item.YdWt, item.Weight, item.Length,
                                        item.PNo, item.Date, item.ColorCode);
                                    db.Database.ExecuteSqlCommand(sqlString);
                                }
                                #endregion
                            }

                            myScope.Complete();
                        }

                        _Result.Success = true;

                        _Result.Message = string.Format("執行成功 {0}筆資料，({1})", dataList.Count(), lot);
                    }
                    else
                    {
                        _Result.Success = true;
                        _Result.Message = string.Format("倉管系統查無退繳資料({0})", lot);
                    }
                }
                else
                {
                    _Result.Success = true;
                    _Result.Message = "ERP系統查無退繳資料";
                }
            }
            catch (Exception ex)
            {
                _Result.Message = ex.Message;
            }
            return Json(_Result);
        }
    }
}