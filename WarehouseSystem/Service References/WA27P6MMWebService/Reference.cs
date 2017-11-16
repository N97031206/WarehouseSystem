﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.18408
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WarehouseSystem.WA27P6MMWebService {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WA27P6MMWebService.WA27P6MMServiceSoap")]
    public interface WA27P6MMServiceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ReturnToStock", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet ReturnToStock();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ReturnToStock", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> ReturnToStockAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetRemarkByReturnToStock", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SetRemarkByReturnToStock(string[] Lot, string[] PalletNo, string[] PalletRFID, bool Flag);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetRemarkByReturnToStock", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> SetRemarkByReturnToStockAsync(string[] Lot, string[] PalletNo, string[] PalletRFID, bool Flag);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ReleasePalletAndReturnToStock", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet ReleasePalletAndReturnToStock();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ReleasePalletAndReturnToStock", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> ReleasePalletAndReturnToStockAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UndoReturnToStock", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet UndoReturnToStock();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UndoReturnToStock", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> UndoReturnToStockAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetStockInData", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetStockInData(string StockIn);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetStockInData", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> GetStockInDataAsync(string StockIn);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetStockInDataByPallet", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetStockInDataByPallet(string palletNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetStockInDataByPallet", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> GetStockInDataByPalletAsync(string palletNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/NoUsePlateNo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet NoUsePlateNo();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/NoUsePlateNo", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> NoUsePlateNoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool Login(string UserID, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> LoginAsync(string UserID, string Password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetStockOutData", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetStockOutData(string StockOut);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetStockOutData", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> GetStockOutDataAsync(string StockOut);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetStockInData", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SetStockInData(string[] Lot, string PalletNo, string PalletRFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetStockInData", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> SetStockInDataAsync(string[] Lot, string PalletNo, string PalletRFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetStockOutData", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SetStockOutData(string[] Lot, string PalletNo, string PalletRFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetStockOutData", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> SetStockOutDataAsync(string[] Lot, string PalletNo, string PalletRFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetPalletByLot", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SetPalletByLot(string[] Lot, string PalletNo, string PalletRFID, string UserID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetPalletByLot", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> SetPalletByLotAsync(string[] Lot, string PalletNo, string PalletRFID, string UserID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetStorageByPallet", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SetStorageByPallet(string[] PalletNo, string[] PalletRFID, string Storage, string StorageRFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetStorageByPallet", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> SetStorageByPalletAsync(string[] PalletNo, string[] PalletRFID, string Storage, string StorageRFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetInventoryData", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetInventoryData();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetInventoryData", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> GetInventoryDataAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetLotDataByStockIn", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetLotDataByStockIn(string RFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetLotDataByStockIn", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> GetLotDataByStockInAsync(string RFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetLotDataByStockOut", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetLotDataByStockOut(string RFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetLotDataByStockOut", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> GetLotDataByStockOutAsync(string RFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPalletDataIsNull", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool CheckPalletDataIsNull(string PalletNo, string PalletRFID, string UserID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckPalletDataIsNull", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> CheckPalletDataIsNullAsync(string PalletNo, string PalletRFID, string UserID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetPalletByWIPLot", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SetPalletByWIPLot(string[] Lot, string SpecialPallet, string PalletNo, string PalletRFID, string UserID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetPalletByWIPLot", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> SetPalletByWIPLotAsync(string[] Lot, string SpecialPallet, string PalletNo, string PalletRFID, string UserID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ClearPalletDataByPallet", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool ClearPalletDataByPallet(string PalletNo, string PalletRFID, string UserID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ClearPalletDataByPallet", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> ClearPalletDataByPalletAsync(string PalletNo, string PalletRFID, string UserID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetLotData", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetLotData(string RFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetLotData", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> GetLotDataAsync(string RFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetLotData1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetLotData1(string WKNo, string PNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetLotData1", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> GetLotData1Async(string WKNo, string PNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetLotData", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SetLotData(string Lot, string LotRFID, string PalletNo, string PalletRFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetLotData", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> SetLotDataAsync(string Lot, string LotRFID, string PalletNo, string PalletRFID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetPalletAndGradeByWIPLot", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SetPalletAndGradeByWIPLot(string[] Lot, string SpecialPallet, string PalletNo, string PalletRFID, string[] Grade, string UserID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SetPalletAndGradeByWIPLot", ReplyAction="*")]
        System.Threading.Tasks.Task<bool> SetPalletAndGradeByWIPLotAsync(string[] Lot, string SpecialPallet, string PalletNo, string PalletRFID, string[] Grade, string UserID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetStartDate", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string GetStartDate();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetStartDate", ReplyAction="*")]
        System.Threading.Tasks.Task<string> GetStartDateAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WA27P6MMServiceSoapChannel : WarehouseSystem.WA27P6MMWebService.WA27P6MMServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WA27P6MMServiceSoapClient : System.ServiceModel.ClientBase<WarehouseSystem.WA27P6MMWebService.WA27P6MMServiceSoap>, WarehouseSystem.WA27P6MMWebService.WA27P6MMServiceSoap {
        
        public WA27P6MMServiceSoapClient() {
        }
        
        public WA27P6MMServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WA27P6MMServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WA27P6MMServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WA27P6MMServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Data.DataSet ReturnToStock() {
            return base.Channel.ReturnToStock();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> ReturnToStockAsync() {
            return base.Channel.ReturnToStockAsync();
        }
        
        public bool SetRemarkByReturnToStock(string[] Lot, string[] PalletNo, string[] PalletRFID, bool Flag) {
            return base.Channel.SetRemarkByReturnToStock(Lot, PalletNo, PalletRFID, Flag);
        }
        
        public System.Threading.Tasks.Task<bool> SetRemarkByReturnToStockAsync(string[] Lot, string[] PalletNo, string[] PalletRFID, bool Flag) {
            return base.Channel.SetRemarkByReturnToStockAsync(Lot, PalletNo, PalletRFID, Flag);
        }
        
        public System.Data.DataSet ReleasePalletAndReturnToStock() {
            return base.Channel.ReleasePalletAndReturnToStock();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> ReleasePalletAndReturnToStockAsync() {
            return base.Channel.ReleasePalletAndReturnToStockAsync();
        }
        
        public System.Data.DataSet UndoReturnToStock() {
            return base.Channel.UndoReturnToStock();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> UndoReturnToStockAsync() {
            return base.Channel.UndoReturnToStockAsync();
        }
        
        public System.Data.DataSet GetStockInData(string StockIn) {
            return base.Channel.GetStockInData(StockIn);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> GetStockInDataAsync(string StockIn) {
            return base.Channel.GetStockInDataAsync(StockIn);
        }
        
        public System.Data.DataSet GetStockInDataByPallet(string palletNo) {
            return base.Channel.GetStockInDataByPallet(palletNo);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> GetStockInDataByPalletAsync(string palletNo) {
            return base.Channel.GetStockInDataByPalletAsync(palletNo);
        }
        
        public System.Data.DataSet NoUsePlateNo() {
            return base.Channel.NoUsePlateNo();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> NoUsePlateNoAsync() {
            return base.Channel.NoUsePlateNoAsync();
        }
        
        public bool Login(string UserID, string Password) {
            return base.Channel.Login(UserID, Password);
        }
        
        public System.Threading.Tasks.Task<bool> LoginAsync(string UserID, string Password) {
            return base.Channel.LoginAsync(UserID, Password);
        }
        
        public System.Data.DataSet GetStockOutData(string StockOut) {
            return base.Channel.GetStockOutData(StockOut);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> GetStockOutDataAsync(string StockOut) {
            return base.Channel.GetStockOutDataAsync(StockOut);
        }
        
        public bool SetStockInData(string[] Lot, string PalletNo, string PalletRFID) {
            return base.Channel.SetStockInData(Lot, PalletNo, PalletRFID);
        }
        
        public System.Threading.Tasks.Task<bool> SetStockInDataAsync(string[] Lot, string PalletNo, string PalletRFID) {
            return base.Channel.SetStockInDataAsync(Lot, PalletNo, PalletRFID);
        }
        
        public bool SetStockOutData(string[] Lot, string PalletNo, string PalletRFID) {
            return base.Channel.SetStockOutData(Lot, PalletNo, PalletRFID);
        }
        
        public System.Threading.Tasks.Task<bool> SetStockOutDataAsync(string[] Lot, string PalletNo, string PalletRFID) {
            return base.Channel.SetStockOutDataAsync(Lot, PalletNo, PalletRFID);
        }
        
        public bool SetPalletByLot(string[] Lot, string PalletNo, string PalletRFID, string UserID) {
            return base.Channel.SetPalletByLot(Lot, PalletNo, PalletRFID, UserID);
        }
        
        public System.Threading.Tasks.Task<bool> SetPalletByLotAsync(string[] Lot, string PalletNo, string PalletRFID, string UserID) {
            return base.Channel.SetPalletByLotAsync(Lot, PalletNo, PalletRFID, UserID);
        }
        
        public bool SetStorageByPallet(string[] PalletNo, string[] PalletRFID, string Storage, string StorageRFID) {
            return base.Channel.SetStorageByPallet(PalletNo, PalletRFID, Storage, StorageRFID);
        }
        
        public System.Threading.Tasks.Task<bool> SetStorageByPalletAsync(string[] PalletNo, string[] PalletRFID, string Storage, string StorageRFID) {
            return base.Channel.SetStorageByPalletAsync(PalletNo, PalletRFID, Storage, StorageRFID);
        }
        
        public System.Data.DataSet GetInventoryData() {
            return base.Channel.GetInventoryData();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> GetInventoryDataAsync() {
            return base.Channel.GetInventoryDataAsync();
        }
        
        public System.Data.DataSet GetLotDataByStockIn(string RFID) {
            return base.Channel.GetLotDataByStockIn(RFID);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> GetLotDataByStockInAsync(string RFID) {
            return base.Channel.GetLotDataByStockInAsync(RFID);
        }
        
        public System.Data.DataSet GetLotDataByStockOut(string RFID) {
            return base.Channel.GetLotDataByStockOut(RFID);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> GetLotDataByStockOutAsync(string RFID) {
            return base.Channel.GetLotDataByStockOutAsync(RFID);
        }
        
        public bool CheckPalletDataIsNull(string PalletNo, string PalletRFID, string UserID) {
            return base.Channel.CheckPalletDataIsNull(PalletNo, PalletRFID, UserID);
        }
        
        public System.Threading.Tasks.Task<bool> CheckPalletDataIsNullAsync(string PalletNo, string PalletRFID, string UserID) {
            return base.Channel.CheckPalletDataIsNullAsync(PalletNo, PalletRFID, UserID);
        }
        
        public bool SetPalletByWIPLot(string[] Lot, string SpecialPallet, string PalletNo, string PalletRFID, string UserID) {
            return base.Channel.SetPalletByWIPLot(Lot, SpecialPallet, PalletNo, PalletRFID, UserID);
        }
        
        public System.Threading.Tasks.Task<bool> SetPalletByWIPLotAsync(string[] Lot, string SpecialPallet, string PalletNo, string PalletRFID, string UserID) {
            return base.Channel.SetPalletByWIPLotAsync(Lot, SpecialPallet, PalletNo, PalletRFID, UserID);
        }
        
        public bool ClearPalletDataByPallet(string PalletNo, string PalletRFID, string UserID) {
            return base.Channel.ClearPalletDataByPallet(PalletNo, PalletRFID, UserID);
        }
        
        public System.Threading.Tasks.Task<bool> ClearPalletDataByPalletAsync(string PalletNo, string PalletRFID, string UserID) {
            return base.Channel.ClearPalletDataByPalletAsync(PalletNo, PalletRFID, UserID);
        }
        
        public System.Data.DataSet GetLotData(string RFID) {
            return base.Channel.GetLotData(RFID);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> GetLotDataAsync(string RFID) {
            return base.Channel.GetLotDataAsync(RFID);
        }
        
        public System.Data.DataSet GetLotData1(string WKNo, string PNo) {
            return base.Channel.GetLotData1(WKNo, PNo);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> GetLotData1Async(string WKNo, string PNo) {
            return base.Channel.GetLotData1Async(WKNo, PNo);
        }
        
        public bool SetLotData(string Lot, string LotRFID, string PalletNo, string PalletRFID) {
            return base.Channel.SetLotData(Lot, LotRFID, PalletNo, PalletRFID);
        }
        
        public System.Threading.Tasks.Task<bool> SetLotDataAsync(string Lot, string LotRFID, string PalletNo, string PalletRFID) {
            return base.Channel.SetLotDataAsync(Lot, LotRFID, PalletNo, PalletRFID);
        }
        
        public bool SetPalletAndGradeByWIPLot(string[] Lot, string SpecialPallet, string PalletNo, string PalletRFID, string[] Grade, string UserID) {
            return base.Channel.SetPalletAndGradeByWIPLot(Lot, SpecialPallet, PalletNo, PalletRFID, Grade, UserID);
        }
        
        public System.Threading.Tasks.Task<bool> SetPalletAndGradeByWIPLotAsync(string[] Lot, string SpecialPallet, string PalletNo, string PalletRFID, string[] Grade, string UserID) {
            return base.Channel.SetPalletAndGradeByWIPLotAsync(Lot, SpecialPallet, PalletNo, PalletRFID, Grade, UserID);
        }
        
        public string GetStartDate() {
            return base.Channel.GetStartDate();
        }
        
        public System.Threading.Tasks.Task<string> GetStartDateAsync() {
            return base.Channel.GetStartDateAsync();
        }
    }
}
