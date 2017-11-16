using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Service.Interface
{
    public interface IRecord
    {
        IEnumerable<vw_Record> Query(string Code, string Name, string StartTime, string EndTime);
    }
}