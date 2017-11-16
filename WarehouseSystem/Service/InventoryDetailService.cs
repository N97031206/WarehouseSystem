using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service
{
    public class InventoryDetailService : IInventoryDetailService
    {
        private IRepository<CST_INV_DTL> _repository;

        public InventoryDetailService(IRepository<CST_INV_DTL> repository)
        {
            _repository = repository;
        }

        //public IResult Create(CST_INV_DTL OrderDetail, ref int OrderDetailID)
        //{
        //    if (OrderDetail == null) throw new ArgumentNullException();

        //    IResult pResult = new Result(false);

        //    try
        //    {
        //        IdGenerator idg = new IdGenerator();

        //        //OrderDetailID = idg.GetSID(1);

        //        string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        //        //OrderDetail.OrderDetailID = OrderDetailID;

        //        _repository.Create(OrderDetail);

        //        pResult.Success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        pResult.Exception = ex;

        //        //2627 主鍵重複
        //        //2601 唯一索引重複
        //        var ErrorCode = ((System.Data.SqlClient.SqlException)((ex.InnerException).InnerException)).Number;
        //        if (ErrorCode == 2627) pResult.Message = "SID重複";
        //        if (ErrorCode == 2601) pResult.Message = "RFID編號已被使用，請重新申請";
        //    }

        //    return pResult;

        //}

        public IResult InsertTable(List<CST_INV_DTL> _List)
        {
            //string SQLDataSource = "127.0.0.1";
            //string SQLUserID = "sa";
            //string SQLPassword = "qazxswedc1234";
            //string SQLDBName = "WarehouseServer";
            string SQLTable = "CST_INV_DTL";

            //string g_MysqlConn = "Data Source=" + SQLDataSource + ";user id=" + SQLUserID + ";Password=" + SQLPassword + ";persist security info=True;database=" + SQLDBName;

            //string g_MysqlConn = System.Configuration.ConfigurationManager.ConnectionStrings["WarehouseServerEntities"].ConnectionString;

            string g_MysqlConn = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            IResult pResult = new Result(false);

            IdGenerator idg = new IdGenerator();

            try
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("OrderDetailID", typeof(int));
                dt.Columns.Add("OrderID", typeof(string));
                dt.Columns.Add("RFID", typeof(string));
                dt.Columns.Add("Location", typeof(string));
                dt.Columns.Add("PalletNo", typeof(string));
                dt.Columns.Add("PalletRFID", typeof(string));
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("LotNo", typeof(string));
                dt.Columns.Add("PNo", typeof(string));
                dt.Columns.Add("Weight", typeof(string));
                dt.Columns.Add("Length", typeof(string));
                dt.Columns.Add("Order", typeof(string));
                dt.Columns.Add("InternalOrder", typeof(string));
                dt.Columns.Add("WKNo", typeof(string));
                dt.Columns.Add("Fabric", typeof(string));
                dt.Columns.Add("ColorCode", typeof(string));
                dt.Columns.Add("Color", typeof(string));
                dt.Columns.Add("Lot", typeof(string));


                for (int iCount = 0; iCount < _List.Count; iCount++)
                {
                    DataRow dr = dt.NewRow();

                    CST_INV_DTL item = _List[iCount];

                    //dr["OrderDetailID"] = idg.GetSID(1);
                    dr["OrderID"] = item.OrderID;
                    dr["RFID"] = item.RFID;
                    dr["Location"] = item.Location;
                    dr["PalletNo"] = item.PalletNo;
                    dr["PalletRFID"] = item.PalletRFID;
                    dr["Date"] = item.Date;
                    dr["LotNo"] = item.LotNo;
                    dr["PNo"] = item.PNo;
                    dr["Weight"] = item.Weight;
                    dr["Length"] = item.Length;
                    dr["Order"] = item.Order;
                    dr["InternalOrder"] = item.InternalOrder;
                    dr["WKNo"] = item.WKNo;
                    dr["Fabric"] = item.Fabric;
                    dr["ColorCode"] = item.ColorCode;
                    dr["Color"] = item.Color;
                    dr["Lot"] = item.Lot;

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
                    sqlBC.ColumnMappings.Add("OrderDetailID", "OrderDetailID");
                    sqlBC.ColumnMappings.Add("OrderID", "OrderID");
                    sqlBC.ColumnMappings.Add("Location", "Location");
                    sqlBC.ColumnMappings.Add("PalletNo", "PalletNo");
                    sqlBC.ColumnMappings.Add("PalletRFID", "PalletRFID");
                    sqlBC.ColumnMappings.Add("RFID", "RFID");
                    sqlBC.ColumnMappings.Add("Date", "Date");
                    sqlBC.ColumnMappings.Add("LotNo", "LotNo");
                    sqlBC.ColumnMappings.Add("PNo", "PNo");
                    sqlBC.ColumnMappings.Add("Weight", "Weight");
                    sqlBC.ColumnMappings.Add("Length", "Length");
                    sqlBC.ColumnMappings.Add("Order", "Order");
                    sqlBC.ColumnMappings.Add("InternalOrder", "InternalOrder");
                    sqlBC.ColumnMappings.Add("Lot", "Lot");
                    sqlBC.ColumnMappings.Add("WKNo", "WKNo");
                    sqlBC.ColumnMappings.Add("Fabric", "Fabric");
                    sqlBC.ColumnMappings.Add("ColorCode", "ColorCode");
                    sqlBC.ColumnMappings.Add("Color", "Color");

                    //開始寫入
                    sqlBC.WriteToServer(dt);
                }
                conn.Dispose();


                pResult.Success = true;
            }
            catch (Exception ex)
            {
                pResult.Message = ex.ToString();
                pResult.Success = false;
            }

            return pResult;
        }

        //public IResult Update(int OrderDetailID)
        //{
        //    Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

        //    var instance = GetBySID(OrderDetailID);

        //    if (instance == null) throw new ArgumentNullException();

        //    IResult Result = new Result(false);

        //    try
        //    {
        //        var LastUpdateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        //        DicUpdate.Add("LastUpdateTime", LastUpdateTime);

        //        _repository.Update(instance, DicUpdate);

        //        Result.Success = true;
        //        Result.LastUpdateTime = LastUpdateTime;
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.Exception = ex;
        //    }

        //    return Result;
        //}

        //public IResult Delete(int OrderDetailID)
        //{
        //    IResult result = new Result(false);

        //    if (!IsExists(OrderDetailID)) result.Message = "找不到單據資料";
        //    else
        //    {
        //        try
        //        {
        //            var instance = GetBySID(OrderDetailID);
        //            _repository.Delete(instance);
        //            result.Success = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            result.Exception = ex;
        //        }
        //    }
        //    return result;
        //}

        //public bool IsExists(int OrderDetailID)
        //{
        //    return _repository.GetAll().Any(x => x.OrderDetailID == OrderDetailID);
        //}

        //public CST_INV_DTL GetBySID(int OrderDetailID)
        //{
        //    return _repository.Get(x => x.OrderDetailID == OrderDetailID);
        //}

        //public IEnumerable<CST_INV_DTL> GetAll()
        //{
        //    return _repository.GetAll();
        //}

        public int GetTotalCount(string OrderID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select * From CST_INV_DTL Where OrderID = '{0}'", OrderID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _List = db.Database.SqlQuery<CST_INV_DTL>(sqlString).ToList();

                return _List.Count();
            }
        }

        public IEnumerable<CST_INV_DTL> GetData(string OrderID, int Start, int End)
        {
            string sqlString = "";

            Start = End - (Start - 1);

            sqlString = string.Format(@"Select Top {0} * From (Select Top {1} * From CST_INV_DTL CID Where CID.OrderID = '{2}' Order by CID.OrderDetailID) Tmp Order by Tmp.OrderDetailID desc;",
               Start.ToString(), End.ToString(), OrderID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _List = db.Database.SqlQuery<CST_INV_DTL>(sqlString).ToList();

                return _List;
            }
        }
    }
}