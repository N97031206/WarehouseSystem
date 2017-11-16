using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WarehouseSystem
{
    /// <summary>
    ///ERPWebService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class ERPWebService : System.Web.Services.WebService
    {
        string SqlConnString = "User ID=sa;password=qazxswedc1234;Initial Catalog=WarehouseServer;Data Source=localhost";

        [WebMethod]
        public DataSet GetStockInData(string StockIn)
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("SELECT * FROM TEMP_ERP_STOCK_IN WHERE STOCKINNO = '" + StockIn + "'", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        [WebMethod]
        public DataSet GetStockInDataByPallet(string palletNo)
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("SELECT * FROM TEMP_ERP_STOCK_IN WHERE PALLETNUMBER = '" + palletNo + "'", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        [WebMethod]
        public DataSet NoUsePlateNo()
        {
            DataSet CustDataSet = new DataSet();
            DataTable dtData = new DataTable("WorkOrder");
            dtData.Columns.Add("TYPE");
            dtData.Columns.Add("PLATENO");
            dtData.Columns.Add("PLATENODESC");

            string[] platnoList = { "999%", "0000", "666%" };

            foreach (var platno in platnoList)
            {
                DataRow drData = dtData.NewRow();
                drData["TYPE"] = "type";
                drData["PLATENO"] = platno;
                drData["PLATENODESC"] = "desc";

                dtData.Rows.Add(drData);
                dtData.AcceptChanges();
            }
            CustDataSet.Tables.Add(dtData);

            return CustDataSet;
        }

        [WebMethod]
        public bool Login ( String UserID, String Password) 
        {
            if (UserID == "1234" && Password == "1234")
                return true;
            else
                return false;
        }

        [WebMethod]
        public DataSet GetStockOutData(string StockOut)
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("Select * From TEMP_ERP_STOCK_OUT Where StockOutNo = '" + StockOut + "'", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        [WebMethod]
        public bool ClearPalletDataByPallet(string palletNumber, string palletRFID, string userID)
        {
            return true;
        }

        [WebMethod]
        public bool SetStockInData(String[] Lot, String PalletNo, String PalletRFID)
        {
            return true;
        }

        [WebMethod]
        public bool SetStockOutData(String[] Lot, String PalletNo, String PalletRFID)
        {
            return true;
        }

        [WebMethod]
        public bool SetPalletByLot(String[] Lot, String PalletNo, String PalletRFID, String UserID)
        {
            return true;
        }

        [WebMethod]
        public bool SetStorageByPallet(String[] PalletNo, String[] PalletRFID, String Storage, String StorageRFID)
        {
            return true;
        }

        [WebMethod]
        public DataSet GetInventoryData()
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("Select * From TEMP_ERP_INV_DTL", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        [WebMethod]
        public DataSet GetLotDataByStockIn(String RFID)
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("select TE.RFID, TE.Length, TE.weight, (TE.InternalOrder + '-' + TE.PNo) as PLot From TEMP_ERP_STOCK_IN TE Where RFID = '" + RFID + "'", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        [WebMethod]
        public DataSet GetLotDataByStockOut(String RFID)
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("select TE.RFID, TE.Length, TE.weight, (TE.InternalOrder + '-' + TE.PNo) as PLot From TEMP_ERP_STOCK_OUT TE Where RFID = '" + RFID + "'", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        //[WebMethod]
        //public DataSet GetLotData(String WKNo, String PNo)
        //{
        //    SqlConnection connection = new SqlConnection(SqlConnString);
        //    SqlDataAdapter CustDataAdapter = new SqlDataAdapter("select * From TEMP_ERP_STOCK_IN Where WKNo = '" + WKNo + "' AND PNo ='" + PNo + "'", connection);
        //    DataSet CustDataSet = new DataSet();
        //    connection.Open();
        //    CustDataAdapter.Fill(CustDataSet);
        //    connection.Close();
        //    return CustDataSet;
        //}

        [WebMethod]
        public DataSet GetLotData(string RFID)
        {
            SqlConnection connection = new SqlConnection(SqlConnString);
            SqlDataAdapter CustDataAdapter = new SqlDataAdapter("Select * From TEMP_ERP_STOCK_IN Where RFID='" + RFID + "'", connection);
            DataSet CustDataSet = new DataSet();
            connection.Open();
            CustDataAdapter.Fill(CustDataSet);
            connection.Close();
            return CustDataSet;
        }

        [WebMethod]
        public bool SetLotData(string Lot, string LotRFID, string PalletNo, string PalletRFID)
        {
            return true;
        }
    }
}