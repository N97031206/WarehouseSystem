using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IAlarmService
    {
        IResult Create(string DeviceName, string WarningDay, ref string AlarmID);

        IResult Delete(string AlarmID);

        IResult Update(string AlarmID, string PropertyName, object Value);

        bool IsExists(string AlarmID);

        CST_ALARM GetBySID(string AlarmID);

        IEnumerable<CST_ALARM> GetAll();

        CST_ALARM GetByDeviceName(string DeviceName);

        /// <summary>
        /// 依據傳入TYPE，回傳資料
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<CST_ALARM> GetDataByType(string type);
    }
}