using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseSystem.Models;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service
{
    public class WarehouseLogService : IWarehouseLogService
    {
        private IRepository<CST_WAREHOUSE_LOG> _repository;

        public WarehouseLogService(IRepository<CST_WAREHOUSE_LOG> repository)
        {
            _repository = repository;
        }

        public void Insert(CST_WAREHOUSE_LOG data)
        {
            IdGenerator idg = new IdGenerator();
            var ID = idg.GetSID(1);

            data.WareHouseDataLogID = ID;

            _repository.Create(data);
        }

        public IEnumerable<CST_WAREHOUSE_LOG> GetByLot(List<string> lstLot)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select *
                                        From CST_WAREHOUSE_LOG CW");
                                        

            for (int i = 0; i < lstLot.Count; i++)
            {
                if (i == 0) sqlString += " WHERE LOT IN (" + "'" + lstLot[0] + "'";
                else sqlString += "," + "'" + lstLot[i] + "'";
            }

            sqlString += ")";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _InfoList = db.Database.SqlQuery<CST_WAREHOUSE_LOG>(sqlString).ToList();

                return _InfoList;
            }
        }

        public IEnumerable<CST_WAREHOUSE_LOG> GetByLot(string Lot)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select *
                            From CST_WAREHOUSE_LOG CW 
                            Where CW.Lot = '{0}' ", Lot);
                            

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var _InfoList = db.Database.SqlQuery<CST_WAREHOUSE_LOG>(sqlString).ToList();

                return _InfoList;
            }
        }
    }
}