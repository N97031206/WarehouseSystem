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
    public class FindGoodsDetailService : IFindGoodsDetailService
    {
        private IRepository<CST_SER_INV_DTL> _repository;

        public FindGoodsDetailService(IRepository<CST_SER_INV_DTL> repository)
        {
            _repository = repository;
        }


        public IResult InsertTable(List<vw_Lots> _List, string OrderID)
        {
            //string SQLDataSource = "127.0.0.1";
            //string SQLUserID = "sa";
            //string SQLPassword = "qazxswedc1234";
            //string SQLDBName = "WarehouseServer";
            string SQLTable = "CST_SER_INV_DTL";

            //string g_MysqlConn = "Data Source=" + SQLDataSource + ";user id=" + SQLUserID + ";Password=" + SQLPassword + ";persist security info=True;database=" + SQLDBName;

            string g_MysqlConn = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

            IResult pResult = new Result(false);

            IdGenerator idg = new IdGenerator();

            try
            {
                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                DataTable dt = new DataTable();

                dt.Columns.Add("OrderDetailID", typeof(string));
                dt.Columns.Add("OrderID", typeof(string));
                dt.Columns.Add("RFID", typeof(string));
                dt.Columns.Add("Lot", typeof(string));
                dt.Columns.Add("WKNo", typeof(string));
                dt.Columns.Add("LotNo", typeof(string));
                dt.Columns.Add("Order", typeof(string));
                dt.Columns.Add("Fabric", typeof(string));
                dt.Columns.Add("Color", typeof(string));
                dt.Columns.Add("Width", typeof(string));
                dt.Columns.Add("YdWt", typeof(string));
                dt.Columns.Add("Weight", typeof(string));
                dt.Columns.Add("Length", typeof(string));
                dt.Columns.Add("PNo", typeof(string));
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("CreateTime", typeof(string));
                dt.Columns.Add("LastUpdateTime", typeof(string));
                dt.Columns.Add("Location", typeof(string));
                dt.Columns.Add("InternalOrder", typeof(string));
                dt.Columns.Add("Status", typeof(string));
                


                for (int iCount = 0; iCount < _List.Count; iCount++)
                {
                    DataRow dr = dt.NewRow();

                    vw_Lots item = _List[iCount];

                    dr["OrderDetailID"] = idg.GetSID(1);
                    dr["OrderID"] = OrderID;
                    dr["RFID"] = item.RFID;
                    dr["Lot"] = item.PLot;
                    dr["WKNo"] = item.WKNo;
                    dr["LotNo"] = item.LotNo;
                    dr["Order"] = item.Order;
                    dr["Fabric"] = item.Fabric;
                    dr["Color"] = item.Color;
                    dr["Width"] = item.Width;
                    dr["YdWt"] = item.YdWt;
                    dr["Weight"] = item.Weight;
                    dr["Length"] = item.Length;
                    dr["PNo"] = item.PNo;
                    dr["Date"] = item.Date;
                    dr["CreateTime"] = InsertTime;
                    dr["LastUpdateTime"] = InsertTime;
                    dr["Location"] = item.ParkingBlockName;
                    dr["InternalOrder"] = item.InternalOrder;
                    dr["Status"] = item.Status;
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
                    sqlBC.ColumnMappings.Add("RFID", "RFID");
                    sqlBC.ColumnMappings.Add("Lot", "Lot");
                    sqlBC.ColumnMappings.Add("WKNo", "WKNo");
                    sqlBC.ColumnMappings.Add("LotNo", "LotNo");
                    sqlBC.ColumnMappings.Add("Order", "Order");
                    sqlBC.ColumnMappings.Add("Fabric", "Fabric");
                    sqlBC.ColumnMappings.Add("Color", "Color");
                    sqlBC.ColumnMappings.Add("Width", "Width");
                    sqlBC.ColumnMappings.Add("YdWt", "YdWt");
                    sqlBC.ColumnMappings.Add("Weight", "Weight");
                    sqlBC.ColumnMappings.Add("Length", "Length");
                    sqlBC.ColumnMappings.Add("PNo", "PNo");
                    sqlBC.ColumnMappings.Add("Date", "Date");
                    sqlBC.ColumnMappings.Add("CreateTime", "CreateTime");
                    sqlBC.ColumnMappings.Add("LastUpdateTime", "LastUpdateTime");
                    sqlBC.ColumnMappings.Add("Location", "Location");
                    sqlBC.ColumnMappings.Add("InternalOrder", "InternalOrder");
                    sqlBC.ColumnMappings.Add("Status", "Status");

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

        public bool IsExists(string OrderDetailID)
        {
            return _repository.GetAll().Any(x => x.OrderDetailID == OrderDetailID);
        }

        public CST_SER_INV_DTL GetBySID(string OrderDetailID)
        {
            return _repository.Get(x => x.OrderDetailID == OrderDetailID);
        }

        public IEnumerable<CST_SER_INV_DTL> GetAll()
        {
            return _repository.GetAll();
        }

        public IEnumerable<CST_SER_INV_DTL> GetDataByOrderID(string OrderID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select * From CST_SER_INV_DTL Where OrderID = '{0}'", OrderID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _List = db.Database.SqlQuery<CST_SER_INV_DTL>(sqlString).ToList();

                return _List;
            }
        }
    }
}