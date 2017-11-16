using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Models;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IRFIDErrorMessage
    {
        IResult Create(RFIDErrorMessageInfo _RFIDErrorMessageInfo);

        CST_RFID_ERROR_LOG GetByLot(string RFID, string OrderNo, string UpLoadType);

        IEnumerable<CST_RFID_ERROR_LOG> GetAll();

        IResult Update(RFIDErrorMessageInfo _RFIDErrorMessageInfo);

        IEnumerable<vw_RFIDLog> Query(string Lot, string OrderNo, string StartTime, string EndTime);
    }
}