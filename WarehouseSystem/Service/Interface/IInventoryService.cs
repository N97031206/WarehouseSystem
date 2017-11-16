using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IInventoryService
    {
        IResult Create(ref string OrderID);
        IResult Update(string OrderID);
        IResult Delete(string OrderID);
        bool IsExists(string OrderID);
        CST_INV GetBySID(string OrderID);
        IEnumerable<CST_INV> GetAll();
        CST_INV GetByOrderNo(string OrderNo);
        string GetNo();
    }
}