using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WarehouseSystem.Models;


namespace WarehouseSystem
{
    public class CallAPI
    {
        public static string _Url = "";

        public CallAPI(string Url)
        {
            _Url = Url;
        }

        public static T upLoad8Get<T, V>(string url, V upJson)
            where T : new()
            where V : new()
        {
            string responseJson = string.Empty;



            try
            {
                string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(upJson);

                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.Accept] = "application/json";
                    client.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";

                    byte[] response = client.UploadData(_Url + url, Encoding.UTF8.GetBytes(requestJson));
                    responseJson = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception ex)
            { }

            return !string.IsNullOrEmpty(responseJson) ? JsonConvert.DeserializeObject<T>(responseJson) : new T();

        }

        public WarehouseSystem.WareHouseWebService.Result AnalysisRFID(List<RFIDData> rfidList, string palletRFID, string readerNumber)
        {
            WarehouseSystem.WareHouseWebService.Result result = new WarehouseSystem.WareHouseWebService.Result();

            AnalysisData analysisData = new AnalysisData()
            {
                rfidList = rfidList,
                palletRFID = palletRFID,
                readerNumber = readerNumber
            };

            result = upLoad8Get<WarehouseSystem.WareHouseWebService.Result, AnalysisData>("AnalysisRFID", analysisData);

            return result;
        }
        //public WarehouseSystem.WareHouseWebService.Result StockInOut(TagInfo _TagInfo)
        //{
        //    //string FunctionName = "StockInOut";

        //    WarehouseSystem.WareHouseWebService.Result result = new WareHouseWebService.Result();

        //    try
        //    {
        //        result = upLoad8Get<WarehouseSystem.WareHouseWebService.Result, StockInOutInfo>("StockInOut", _TagInfo.LotInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        // _Log.SaveErrorMessage(ex.ToString(), FunctionName);
        //    }
        //    return result;
        //}

        //public WarehouseSystem.WareHouseWebService.Result InsertRFIDErrorMsg(RFIDErrorMessageInfo _RFIDErrorMessageInfo)
        //{
        //    //string FunctionName = "InsertRFIDErrorMsg";

        //    WarehouseSystem.WareHouseWebService.Result result = new WarehouseSystem.WareHouseWebService.Result();

        //    try
        //    {
        //        result = upLoad8Get<WarehouseSystem.WareHouseWebService.Result, RFIDErrorMessageInfo>("InsertRFIDErrorMsg", _RFIDErrorMessageInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        //_Log.SaveErrorMessage(ex.ToString(), FunctionName);
        //    }
        //    return result;
        //}

        //public WarehouseSystem.WareHouseWebService.Result UpdateRFIDErrorMsg(RFIDErrorMessageInfo _RFIDErrorMessageInfo)
        //{
        //    string FunctionName = "UpdateRFIDErrorMsg";

        //    WarehouseSystem.WareHouseWebService.Result result = new WarehouseSystem.WareHouseWebService.Result();

        //    try
        //    {
        //        result = upLoad8Get<WarehouseSystem.WareHouseWebService.Result, RFIDErrorMessageInfo>("UpdateRFIDErrorMsg", _RFIDErrorMessageInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        // _Log.SaveErrorMessage(ex.ToString(), FunctionName);
        //    }
        //    return result;
        //}
    }
}