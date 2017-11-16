using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models
{
    public class MailBoby
    {
        public string WareHouseDataID { get; set; }
        public string DeviceName { get; set; }
        public string PalletNumber { get; set; }
        public string PalletTypeID { get; set; }
        public string ParkingBlockID { get; set; }
        public string RFIDID { get; set; }
        public string StockInTime { get; set; }
        public string StockOutTime { get; set; }
        public string Quantity { get; set; }
        public string GateReaderNumber { get; set; }
        public string WO { get; set; }
        public string Lot { get; set; }
        public string OverTime { get; set; }
        public string MailFlag { get; set; }
        public string LastUpdateTime { get; set; }
        public string ParkingBlockName { get; set; }
    }
}