using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Models;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IWarehouseLogService
    {
        /// <summary>
        /// 新增一筆資料
        /// </summary>
        /// <param name="data"></param>
        void Insert(CST_WAREHOUSE_LOG data);

        IEnumerable<CST_WAREHOUSE_LOG> GetByLot(List<string> lstLot);

        IEnumerable<CST_WAREHOUSE_LOG> GetByLot(string Lot);
    }
}