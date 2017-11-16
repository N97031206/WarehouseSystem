using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IFindGoodsDetailService
    {
        bool IsExists(string OrderDetailID);
        CST_SER_INV_DTL GetBySID(string OrderDetailID);
        IEnumerable<CST_SER_INV_DTL> GetAll();
        IEnumerable<CST_SER_INV_DTL> GetDataByOrderID(string OrderID);

        IResult InsertTable(List<vw_Lots> _List, string OrderID);
    }
}