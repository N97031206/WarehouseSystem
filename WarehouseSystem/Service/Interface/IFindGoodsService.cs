using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IFindGoodsService
    {
        IResult Create(ref string OrderID);
        bool IsExists(string OrderID);
        CST_SER_INV GetBySID(string OrderID);
        IEnumerable<CST_SER_INV> GetAll();
        CST_SER_INV GetByOrderNo(string OrderNo);
        string GetNo();
        IEnumerable<vw_Lots> Query(string WKNo, string LotNo, string InternalOrder, string Fabric, string Color, string StartTime, string EndTime, string PLot, string Status);
    }
}