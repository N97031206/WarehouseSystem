using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseSystem.Service.Interface;

namespace WarehouseSystem.Service
{
    public class Record : IRecord
    {
        public IEnumerable<vw_Record> Query(string Code, string Name, string StartTime, string EndTime)
        {
            string sqlString = "";
            string WhereString = "";

            List<string> _List = new List<string>();

            if (!string.IsNullOrEmpty(Code)) _List.Add(" Code = '" + Code + "'");
            if (!string.IsNullOrEmpty(Name)) _List.Add(" Name = '" + Name + "'");
            if (!string.IsNullOrEmpty(StartTime)) _List.Add(" CreateTime >= '" + StartTime + " 00:00:00'");
            if (!string.IsNullOrEmpty(EndTime)) _List.Add(" CreateTime <= '" + EndTime + " 23:59:59'");


            for (int i = 0; i < _List.Count; i++)
            {
                if (i != 0)
                {
                    WhereString += " AND " + _List[i];
                }
                else
                {
                    WhereString += "Where " + _List[i];
                }
            }


            sqlString = "Select * From vw_Record " + WhereString + " Order by CreateTime desc;";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _Logs = db.Database.SqlQuery<vw_Record>(sqlString).ToList();

                return _Logs;
            }
        }
    }
}