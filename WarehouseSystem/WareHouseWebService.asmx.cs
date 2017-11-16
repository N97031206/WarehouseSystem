using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using WarehouseSystem.Models;
using WarehouseSystem.Service;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem
{
    /// <summary>
    ///WareHouseWebService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]

    public class WareHouseWebService : System.Web.Services.WebService
    {
        private WA27P6MMWebService.WA27P6MMServiceSoapClient m_ERPWebService = new WA27P6MMWebService.WA27P6MMServiceSoapClient();
        //private ERP_WebService.ERPWebServiceSoapClient m_ERPWebService = new ERP_WebService.ERPWebServiceSoapClient();
        string SqlConnString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

        public class Result
        {
            /// <summary>
            /// 是否處理成功
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// 訊息
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// 錯誤訊息
            /// </summary>
            public string ErrorMessage { get; set; }

        }

        public class RecordInfo
        {
            public string UserProfileID;
            public string FunctionName;
            public string Parameter;
            public string NewValue;
            public string OldValue;
        }

        public class CSTStockOutInfo
        {
            public string StockOutNo;
            public string Lot;
            public string RFID;
            public string PNo;
            public string WKNo;
            public string PalletRFID;
        }

        /// <summary>
        /// 查詢RFID並取得批號資料
        /// </summary>
        /// <param name="RFID"></param>
        /// <returns></returns>
        [WebMethod]
        public DataSet GetLotData(string RFID)
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("SELECT * FROM VW_LOTS WHERE RFID ='" + RFID + "'", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        /// <summary>
        /// 傳入板號是否存在
        /// </summary>
        /// <param name="palletNumber"></param>
        /// <returns></returns>
        [WebMethod]
        public bool PalletIsExists(string palletNumber)
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("SELECT * FROM VW_LOTS WHERE PalletNumber ='" + palletNumber + "'", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();

            if (CustDataSet.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// 取得閘門清單
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public DataSet GetGateReader()
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("SELECT GATEREADERNUMBER, HOSTADDRESS FROM CST_GATE_READER WHERE ACTIVE = '1';", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        /// <summary>
        /// 登入(倉管系統)
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [WebMethod]
        public string Login(string UserID, string Password)
        {
            string UserProfileID = "";

            Encrypt encoder = new Encrypt();

            string encodePassword = encoder.EncryptSHA(Password);

            SqlConnection connection = new SqlConnection(SqlConnString);
            string strSql = string.Format("SELECT * FROM CST_USER_PROFILE WHERE ACTIVE = '1' AND USERID = '{0}' AND PASSWORD = '{1}';", UserID, encodePassword);

            SqlDataAdapter CustDataAdapter = new SqlDataAdapter(strSql, connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();

            if (CustDataSet.Tables[0].Rows.Count > 0) UserProfileID = CustDataSet.Tables[0].Rows[0]["UserProfileID"].ToString();

            return UserProfileID;
        }

        /// <summary>
        /// 取得棧板清單
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public DataSet GetPallet()
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("SELECT CP.PALLETNUMBER, CR.RFIDNUMBER FROM CST_PALLET CP LEFT JOIN CST_RFID CR ON CR.RFIDID = CP.RFIDID WHERE CP.ACTIVE = '1';", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        /// <summary>
        /// 取得儲位清單
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public DataSet GetStorage()
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("SELECT PARKINGBLOCKNAME, STORAGEID FROM CST_STORAGE_PARKINGBLOCK ORDER BY PARKINGBLOCKNAME;", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        /// <summary>
        /// 更新儲位資訊
        /// </summary>
        /// <param name="StorageName"></param>
        /// <param name="RFID"></param>
        /// <returns></returns>
        [WebMethod]
        public bool SetStorage(List<string> StorageName, List<string> RFID)
        {
            bool isSuccess = false;

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var ts = db.Database.BeginTransaction();

                try
                {
                    for (int i = 0; i < RFID.Count; i++)
                    {
                        string sqlString = string.Format(@"UPDATE CST_STORAGE_PARKINGBLOCK
                                                            SET STORAGEID = '{0}', LASTUPDATETIME = '{1}'
                                                            WHERE PARKINGBLOCKNAME = '{2}'",
                                                                 RFID[i], DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), StorageName[i]);

                        db.Database.ExecuteSqlCommand(sqlString);
                    }
                    ts.Commit();
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                }

            }
            return isSuccess;
        }

        /// <summary>
        /// 更新棧板對應的儲位編號
        /// </summary>
        /// <param name="palletNumberList"></param>
        /// <param name="palletRFIDList"></param>
        /// <param name="parkingBlockName"></param>
        /// <param name="storageRFID"></param>
        /// <returns></returns>
        [WebMethod]
        public string UpdateByStorage(List<string> palletNumberList, List<string> palletRFIDList, string parkingBlockName, string storageRFID)
        {
            //回傳訊息
            string Message = "";

            //儲位ID
            string parkingBlockID = "";

            string sqlString = "";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var ts = db.Database.BeginTransaction();

                try
                {
                    #region 確認儲位編號是否存在，如果輸入值為空白，則不檢查
                    if (!(string.IsNullOrEmpty(parkingBlockName) && string.IsNullOrEmpty(storageRFID)))
                    {
                        sqlString = string.Format(@"SELECT * FROM CST_STORAGE_PARKINGBLOCK WHERE PARKINGBLOCKNAME = '{0}';", parkingBlockName);

                        var _StorageList = db.Database.SqlQuery<CST_STORAGE_PARKINGBLOCK>(sqlString).ToList();

                        //如果儲位不存在，則顯示錯誤訊息
                        if (_StorageList.Count == 0)
                            throw new Exception("儲位[" + parkingBlockName + "]：未建置資料");

                        parkingBlockID = _StorageList[0].ParkingBlockID;
                    }
                    #endregion


                    foreach (var palletNumber in palletNumberList)
                    {
                        #region 確認此棧板上是否有批號資料
                        sqlString = string.Format(@"SELECT * 
                                            FROM CST_WAREHOUSE 
                                            WHERE PALLETNUMBER = '{0}' AND (STOCKOUTTIME IS NULL OR STOCKOUTTIME = '');", palletNumber);

                        var lotList = db.Database.SqlQuery<CST_WAREHOUSE>(sqlString).ToList();

                        //如果沒有批號資料，則顯示錯誤訊息
                        if (lotList.Count == 0)
                            throw new Exception("棧板編號[" + palletNumber + "]：系統上無任何資料");
                        #endregion

                        #region 更新批號資訊
                        foreach (var lot in lotList)
                        {
                            //逾時時間
                            string overTime = "";

                            #region 如果此批號先前的逾時時間不是空白，則重新計算時間
                            if (lot.WarningDay != "" && lot.WarningDay != null)
                            {
                                overTime = DateTime.Now.AddDays(Convert.ToDouble(lot.WarningDay)).ToString("yyyy/MM/dd HH:mm:ss");
                            }
                            #endregion

                            #region 更新批號資料
                            sqlString = string.Format(@"UPDATE CST_WAREHOUSE
                                                            SET PARKINGBLOCKID = '{0}', LASTUPDATETIME = '{1}', OVERTIME = '{2}' 
                                                            WHERE PALLETNUMBER = '{3}' AND LOT = '{4}'
                                                            AND (STOCKOUTTIME IS NULL OR STOCKOUTTIME = '');",
                                                            parkingBlockID, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                                            overTime, palletNumber, lot.Lot);

                            db.Database.ExecuteSqlCommand(sqlString);
                            #endregion
                        }
                        #endregion
                    }

                    #region 回傳至ERP更新儲位資料，如果更新成功，則資料庫COMMIT，反之，資料庫ROLLBACK
                    var isSuccess = m_ERPWebService.SetStorageByPallet(palletNumberList.ToArray(), palletRFIDList.ToArray(), parkingBlockName, storageRFID);

                    if (isSuccess)
                    {
                        ts.Commit();
                        Message = "更新資料成功";
                    }
                    else
                    {
                        ts.Rollback();
                        Message = "ERP更新資料失敗";
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    Message = ex.Message;
                }
            }
            return Message;
        }

        /// <summary>
        /// 更新棧板
        /// </summary>
        /// <param name="lotList"></param>
        /// <param name="palletNumber"></param>
        /// <param name="palletRFID"></param>
        /// <returns></returns>
        [WebMethod]
        public string UpdateByLot(string[] lotList, string palletNumber, string palletRFID, string userID)
        {
            string Message = "";

            string sqlString = "";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var ts = db.Database.BeginTransaction();

                try
                {
                    #region 確認棧板編號是否存在
                    sqlString = string.Format(@"SELECT * 
                                    FROM CST_PALLET 
                                    WHERE PALLETNUMBER = '{0}' AND ACTIVE = '1';", palletNumber);

                    var palletList = db.Database.SqlQuery<CST_PALLET>(sqlString).ToList();

                    if (palletList.Count == 0)
                        throw new Exception("棧板編號[" + palletNumber + "]：未建置資料");

                    var palletData = palletList[0];

                    #endregion

                    #region 查詢此棧板編號先前的儲位編號為何
                    sqlString = string.Format(@"SELECT * 
                                FROM CST_WAREHOUSE 
                                WHERE PALLETNUMBER = '{0}'  
                                AND (STOCKOUTTIME IS NULL OR STOCKOUTTIME = '');", palletData.PalletNumber);

                    string parkingBlockID = "";

                    var dataList = db.Database.SqlQuery<CST_WAREHOUSE>(sqlString).ToList();

                    //如果有資料，則設置此儲位編號
                    if (dataList.Count != 0)
                        parkingBlockID = dataList[0].ParkingBlockID;
                    #endregion

                    #region 更新批號資料
                    foreach (var lot in lotList)
                    {
                        #region 確認批號是否存在
                        sqlString = string.Format(@"SELECT * 
                                                    FROM CST_WAREHOUSE 
                                                    WHERE LOT = '{0}' 
                                                    AND (STOCKOUTTIME IS NULL OR STOCKOUTTIME = '');", lot);

                        var lotDataList = db.Database.SqlQuery<CST_WAREHOUSE>(sqlString).ToList();

                        //如果批號不存在，則顯示錯誤訊息
                        if (lotDataList.Count == 0)
                            throw new Exception("布疋批號[" + lot + "]：系統上無任何資料");

                        var lotData = lotDataList[0];
                        #endregion

                        #region 更新批號資料

                        string overTime = "";

                        #region 如果此批號先前的逾時時間不是空白，則重新計算時間
                        if (lotData.WarningDay != "" && lotData.WarningDay != null)
                        {
                            overTime = DateTime.Now.AddDays(Convert.ToDouble(lotData.WarningDay)).ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        #endregion

                        sqlString = string.Format(@"UPDATE CST_WAREHOUSE
                                                                SET PALLETNUMBER = '{0}', PALLETTYPEID = '{1}', RFIDID = '{2}', PARKINGBLOCKID = '{3}', LASTUPDATETIME = '{4}', OVERTIME = '{6}' 
                                                                WHERE LOT = '{5}' 
                                                                AND (STOCKOUTTIME IS NULL OR STOCKOUTTIME = '');",
                                                        palletData.PalletNumber, palletData.PalletTypeID, palletData.RFIDID, parkingBlockID, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), lot, overTime);

                        db.Database.ExecuteSqlCommand(sqlString);
                        #endregion
                    }

                    #region 回傳至ERP更新儲位資料，如果更新成功，則資料庫COMMIT，反之，資料庫ROLLBACK
                    var isSuccess = m_ERPWebService.SetPalletByLot(lotList, palletNumber, palletRFID, userID);

                    if (isSuccess)
                    {
                        ts.Commit();
                        Message = "更新資料成功";
                    }
                    else
                    {
                        ts.Rollback();
                        Message = "ERP更新資料失敗";
                    }
                    #endregion

                    #endregion
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    Message = ex.Message;
                }
            }
            return Message;
        }

        /// <summary>
        /// 解除棧板(ALL)
        /// </summary>
        /// <param name="palletNumber"></param>
        /// <param name="palletRFID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [WebMethod]
        public string ClearPalletDataByPallet(string palletNumber, string palletRFID, string userID)
        {
            string Message = "";

            string sqlString = "";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var ts = db.Database.BeginTransaction();

                try
                {
                    #region 確認棧板編號是否存在
                    sqlString = string.Format(@"SELECT * 
                                    FROM CST_PALLET 
                                    WHERE PALLETNUMBER = '{0}' AND ACTIVE = '1';", palletNumber);

                    var palletList = db.Database.SqlQuery<CST_PALLET>(sqlString).ToList();

                    if (palletList.Count == 0)
                        throw new Exception("棧板編號[" + palletNumber + "]：未建置資料");

                    var palletData = palletList[0];

                    #endregion

                    #region 查詢此棧板編號先前的儲位編號為何
                    sqlString = string.Format(@"SELECT * 
                                FROM CST_WAREHOUSE 
                                WHERE PALLETNUMBER = '{0}'  
                                AND (STOCKOUTTIME IS NULL OR STOCKOUTTIME = '');", palletData.PalletNumber);

                    var dataList = db.Database.SqlQuery<CST_WAREHOUSE>(sqlString).ToList();

                    //如果有資料，則設置此儲位編號
                    if (dataList.Count == 0)
                        throw new Exception("棧板編號[" + palletData.PalletNumber + "]：於倉管系統無疋布資料");
                    #endregion

                    #region 更新資料
                    sqlString = string.Format(@"UPDATE CST_WAREHOUSE
                                                                SET PALLETNUMBER = '', PALLETTYPEID = '', RFIDID = '', PARKINGBLOCKID = '', LASTUPDATETIME = '{0}' 
                                                                WHERE PALLETNUMBER = '{1}' 
                                                                AND (STOCKOUTTIME IS NULL OR STOCKOUTTIME = '');"
                                                        , DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), palletData.PalletNumber);

                    db.Database.ExecuteSqlCommand(sqlString);

                    #region 回傳至ERP更新儲位資料，如果更新成功，則資料庫COMMIT，反之，資料庫ROLLBACK
                    var isSuccess = m_ERPWebService.ClearPalletDataByPallet(palletNumber, palletRFID, userID);

                    if (isSuccess)
                    {
                        ts.Commit();
                        Message = "更新資料成功";
                    }
                    else
                    {
                        ts.Rollback();
                        Message = "ERP更新資料失敗";
                    }
                    #endregion

                    #endregion
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    Message = ex.Message;
                }
            }
            return Message;
        }


        [WebMethod]
        public string ReturnToStock(string palletNumber, string userID)
        {
            string Message = "";

            string sqlString = "";

            try
            {
                using (TransactionScope myScope = new TransactionScope())
                {
                    using (WarehouseServerEntities db = new WarehouseServerEntities())
                    {
                        #region 確認棧板編號是否存在
                        sqlString = string.Format(@"SELECT * 
                                    FROM CST_PALLET 
                                    WHERE PALLETNUMBER = '{0}' AND ACTIVE = '1';", palletNumber);

                        var palletList = db.Database.SqlQuery<CST_PALLET>(sqlString).ToList();

                        if (palletList.Count == 0)
                            throw new Exception("棧板編號[" + palletNumber + "]：未建置資料");

                        var palletData = palletList[0];

                        #endregion

                        #region 查詢此棧板編號是否有註定布疋資料
                        sqlString = string.Format(@"SELECT * 
                                FROM CST_WAREHOUSE 
                                WHERE PALLETNUMBER = '{0}'  
                                AND (STOCKOUTTIME IS NULL OR STOCKOUTTIME = '');", palletData.PalletNumber);

                        var dataList = db.Database.SqlQuery<CST_WAREHOUSE>(sqlString).ToList();

                        //如果有資料，則設置此儲位編號
                        if (dataList.Count == 0)
                            throw new Exception("棧板編號[" + palletData.PalletNumber + "]：於倉管系統無疋布資料");
                        #endregion

                        #region 刪除資料
                        sqlString = string.Format(@"DELETE CST_WAREHOUSE WHERE PALLETNUMBER = '{0}'", palletData.PalletNumber);
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
                                userID, item.WO, item.Lot, item.OverTime, item.MailFlag, item.LastUpdateTime, item.WarningDay, item.StockIn, item.StockOut, item.RFID,
                                item.WKNo, item.LotNo, item.Order, item.InternalOrder, item.Fabric, item.Color, item.Width, item.YdWt, item.Weight, item.Length,
                                item.PNo, item.Date, item.ColorCode);
                            db.Database.ExecuteSqlCommand(sqlString);
                        }
                        #endregion

                    }

                    Message = "退繳成功";
                    myScope.Complete();
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            return Message;
        }

        /// <summary>
        /// 記錄操作Log
        /// </summary>
        /// <param name="_List"></param>
        /// <returns></returns>
        [WebMethod]
        public bool OperatingRecord(List<RecordInfo> _List)
        {
            bool result = false;
            string SQLTable = "CST_USER_HIST";
            string g_MysqlConn = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("SID", typeof(int));
                dt.Columns.Add("UserProfileID", typeof(string));
                dt.Columns.Add("FunctionName", typeof(string));
                dt.Columns.Add("Parameter", typeof(string));
                dt.Columns.Add("NewValue", typeof(string));
                dt.Columns.Add("OldValue", typeof(string));
                dt.Columns.Add("CreateTime", typeof(string));
                var dateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                foreach (var item in _List)
                {
                    DataRow dr = dt.NewRow();

                    dr["UserProfileID"] = item.UserProfileID;
                    dr["FunctionName"] = item.FunctionName;
                    dr["Parameter"] = item.Parameter;
                    dr["NewValue"] = item.NewValue;
                    dr["OldValue"] = item.OldValue;
                    dr["CreateTime"] = dateTime;

                    dt.Rows.Add(dr);
                }

                //宣告連結字串
                SqlConnection conn = new SqlConnection(g_MysqlConn);

                conn.Open();
                //宣告SqlBulkCopy
                using (SqlBulkCopy sqlBC = new SqlBulkCopy(conn))
                {
                    //設定一個批次量寫入多少筆資料
                    sqlBC.BatchSize = 1000;
                    //設定逾時的秒數
                    sqlBC.BulkCopyTimeout = 60;

                    //設定 NotifyAfter 屬性，以便在每複製 10000 個資料列至資料表後，呼叫事件處理常式。
                    sqlBC.NotifyAfter = 10000;
                    //sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);

                    //設定要寫入的資料庫
                    sqlBC.DestinationTableName = "dbo." + SQLTable;

                    //對應資料行
                    sqlBC.ColumnMappings.Add("UserProfileID", "UserProfileID");
                    sqlBC.ColumnMappings.Add("FunctionName", "FunctionName");
                    sqlBC.ColumnMappings.Add("Parameter", "Parameter");
                    sqlBC.ColumnMappings.Add("NewValue", "NewValue");
                    sqlBC.ColumnMappings.Add("OldValue", "OldValue");
                    sqlBC.ColumnMappings.Add("CreateTime", "CreateTime");

                    //開始寫入
                    sqlBC.WriteToServer(dt);
                }
                conn.Dispose();

                result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 閘門出庫(手持式)
        /// </summary>
        /// <param name="stockOutNo"></param>
        /// <returns></returns>
        [WebMethod]
        public string StockOutByGate(string stockOutNo)
        {
            string Message = "";

            try
            {
                //取得資料
                var _ErpDataSet = m_ERPWebService.GetStockOutData(stockOutNo);

                #region 確認該出庫單號是否有資料，如果有，則將資料寫入資料庫，反之，則顯示錯誤訊息

                //顯示錯誤訊息
                if (_ErpDataSet.Tables[0].Rows.Count == 0)
                    throw new Exception("單號[" + stockOutNo + "]：無任何資料，請確認是否正確");

                #region 確認此出庫單是否建置過
                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    string sqlString = string.Format(@"SELECT * 
                                    FROM CST_STOCK_OUT 
                                    WHERE STOCKOUTNO = '{0}';", stockOutNo);

                    var stockList = db.Database.SqlQuery<CST_STOCK_OUT>(sqlString).ToList();

                    if (stockList.Count > 0)
                        throw new Exception("出庫單[" + stockOutNo + "]：已建置資料");
                }
                #endregion

                #region 寫入資料庫
                List<CSTStockOutInfo> stockOutList = new List<CSTStockOutInfo>();

                foreach (DataRow dr in _ErpDataSet.Tables[0].Rows)
                {
                    CSTStockOutInfo stockOut = new CSTStockOutInfo()
                    {
                        Lot = dr["WKNo"].ToString().Trim() + dr["PNo"].ToString().Trim(),
                        PalletRFID = dr["PalletRFID"].ToString().Trim(),
                        PNo = dr["PNo"].ToString().Trim(),
                        RFID = dr["RFID"].ToString().Trim(),
                        StockOutNo = stockOutNo,
                        WKNo = dr["WKNo"].ToString().Trim()
                    };
                    stockOutList.Add(stockOut);
                }

                CreateCSTStockOutInfo(stockOutList);
                #endregion

                Message = "出庫單設置成功";

                #endregion

            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            return Message;
        }

        /// <summary>
        /// 寫入CST_STOCK_OUT
        /// </summary>
        /// <param name="datalist"></param>
        /// <returns></returns>
        private void CreateCSTStockOutInfo(List<CSTStockOutInfo> datalist)
        {
            string SQLTable = "CST_STOCK_OUT";
            string g_MysqlConn = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;


            DataTable dt = new DataTable();

            dt.Columns.Add("StockOutID", typeof(int));
            dt.Columns.Add("StockOutNo", typeof(string));
            dt.Columns.Add("Lot", typeof(string));
            dt.Columns.Add("RFID", typeof(string));
            dt.Columns.Add("PNo", typeof(string));
            dt.Columns.Add("WKNo", typeof(string));
            dt.Columns.Add("PalletRFID", typeof(string));
            dt.Columns.Add("CreateTime", typeof(string));

            foreach (var item in datalist)
            {
                DataRow dr = dt.NewRow();

                dr["StockOutNo"] = item.StockOutNo;
                dr["Lot"] = item.Lot;
                dr["RFID"] = item.RFID;
                dr["PNo"] = item.PNo;
                dr["WKNo"] = item.WKNo;
                dr["PalletRFID"] = item.PalletRFID;
                dr["CreateTime"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                dt.Rows.Add(dr);
            }
            using (TransactionScope myScope = new TransactionScope())
            {
                //宣告連結字串
                SqlConnection conn = new SqlConnection(g_MysqlConn);

                conn.Open();

                //宣告SqlBulkCopy
                using (SqlBulkCopy sqlBC = new SqlBulkCopy(conn))
                {
                    //設定一個批次量寫入多少筆資料
                    sqlBC.BatchSize = 1000;
                    //設定逾時的秒數
                    sqlBC.BulkCopyTimeout = 60;

                    //設定 NotifyAfter 屬性，以便在每複製 10000 個資料列至資料表後，呼叫事件處理常式。
                    sqlBC.NotifyAfter = 10000;
                    //sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);

                    //設定要寫入的資料庫
                    sqlBC.DestinationTableName = "dbo." + SQLTable;

                    //對應資料行
                    sqlBC.ColumnMappings.Add("StockOutNo", "StockOutNo");
                    sqlBC.ColumnMappings.Add("Lot", "Lot");
                    sqlBC.ColumnMappings.Add("RFID", "RFID");
                    sqlBC.ColumnMappings.Add("PNo", "PNo");
                    sqlBC.ColumnMappings.Add("WKNo", "WKNo");
                    sqlBC.ColumnMappings.Add("PalletRFID", "PalletRFID");
                    sqlBC.ColumnMappings.Add("CreateTime", "CreateTime");

                    //開始寫入
                    sqlBC.WriteToServer(dt);
                }

                myScope.Complete();
            }
        }

        /// <summary>
        /// 取得尋貨單號
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public DataSet GetFindGoodsNo()
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("SELECT ORDERNO FROM CST_SER_INV", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        /// <summary>
        /// 取得尋貨明細
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        [WebMethod]
        public DataSet GetFindGoodsDetail(string OrderNo)
        {
            string sqlString = "";
            string OrderID = "";

            sqlString = "SELECT TOP 1 * FROM CST_SER_INV WHERE ORDERNO = '" + OrderNo + "'";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var Order = db.Database.SqlQuery<CST_SER_INV>(sqlString).ToList();

                if (Order.Count > 0)
                {
                    OrderID = Order[0].OrderID;
                }
            }
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("SELECT * FROM CST_SER_INV_DTL WHERE ORDERID ='" + OrderID + "'", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        /// <summary>
        /// 取得盤單單號
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public DataSet GetInventoryNo()
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("SELECT ORDERNO FROM CST_INV", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        /// <summary>
        /// 更新盤點資料
        /// </summary>
        /// <param name="_RFIDList"></param>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        [WebMethod]
        public string UpdateInventory(List<string> _RFIDList, string OrderNo)
        {
            string Message = "";

            try
            {
                string sqlString = "";

                sqlString = "SELECT TOP 1 * FROM CST_INV WHERE ORDERNO = '" + OrderNo + "'";

                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    var Order = db.Database.SqlQuery<CST_INV>(sqlString).ToList();

                    if (Order.Count > 0)
                    {
                        foreach (var item in _RFIDList)
                        {
                            sqlString = string.Format(@"UPDATE CST_INV_DTL 
                                                    SET STATUS = 'OK'  
                                                    WHERE RFID = '{0}' AND  ORDERID = '{1}'", item, Order[0].OrderID);
                            db.Database.ExecuteSqlCommand(sqlString);
                        }
                        Message = "更新成功";
                    }
                    else
                    {
                        Message = "單號[" + OrderNo + "]：不存在";
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Message;
        }

        /// <summary>
        /// 入/出庫 (手持式)
        /// </summary>
        /// <param name="lotList"></param>
        /// <param name="palletRFID"></param>
        /// <returns></returns>
        [WebMethod]
        public string StockInOutByReader(List<string> lotList, string palletRFID)
        {
            string message = "";

            string webString = ConfigurationManager.AppSettings["WepAPI"];

            if (string.IsNullOrEmpty(webString))
            {
                webString = "http://127.0.0.1/api/";
            }

            CallAPI _CallAPI = new CallAPI(webString);

            List<RFIDData> RFIDList = new List<RFIDData>();

            foreach (var lot in lotList)
            {
                RFIDList.Add(new RFIDData() { RFID = lot });
            }

            //傳遞資訊至WebService
            var result = _CallAPI.AnalysisRFID(RFIDList, palletRFID, "");

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                message = result.ErrorMessage;
            }
            else
            {
                message = result.Message;
            }
            return message;
        }

        [WebMethod]
        public string CheckStockInNo(string StockInNo)
        {
            string Message = "";

            try
            {
                var _ErpDataSet = m_ERPWebService.GetStockInData(StockInNo);

                if (_ErpDataSet.Tables[0].Rows.Count == 0)
                {
                    Message = "單號[" + StockInNo + "]：" + "數量為零，請確認是否正確";
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            return Message;
        }

        [WebMethod]
        public string CheckStockOutNo(string StockOutNo)
        {
            string Message = "";

            try
            {
                var _ErpDataSet = m_ERPWebService.GetStockOutData(StockOutNo);

                if (_ErpDataSet.Tables[0].Rows.Count == 0)
                {
                    Message = "單號[" + StockOutNo + "]：" + "數量為零，請確認是否正確";
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            return Message;
        }

        [WebMethod]
        public DataSet GetPalletByTest()
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter(@"select * from 
                                                                (select cp.PalletNumber, cr.RFIDNumber from CST_PALLET cp Left join CST_RFID cr on cr.RFIDID = cp.RFIDID) tmp
                                                                where tmp.PalletNumber like 'A%' order by PalletNumber;", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        [WebMethod]
        public DataSet GetStorageByTest()
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("select * from (select ParkingBlockName, StorageID from CST_STORAGE_PARKINGBLOCK) tmp Order by tmp.ParkingBlockName;", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;

        }
    }
}