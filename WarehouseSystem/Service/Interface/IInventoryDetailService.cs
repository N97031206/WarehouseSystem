using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IInventoryDetailService
    {
        //IResult Create(CST_INV_DTL OrderDetail, ref int OrderDetailID);
        //IResult Update(int OrderDetailID);
        //IResult Delete(int OrderDetailID);
       // bool IsExists(int OrderDetailID);
        //CST_INV_DTL GetBySID(int OrderDetailID);
        //IEnumerable<CST_INV_DTL> GetAll();
        int GetTotalCount(string OrderID);

        IResult InsertTable(List<CST_INV_DTL> _List);

        IEnumerable<CST_INV_DTL> GetData(string OrderID, int Start, int End);
    }
}