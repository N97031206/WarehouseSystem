using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service
{
    public class MinorMenuService : IMinorMenuService
    {
        private IRepository<CST_MENU_MINOR> _repository;

        public MinorMenuService(IRepository<CST_MENU_MINOR> repository)
        {
            _repository = repository;
        }

        public IResult Create(string MinorMenuName, string SubMenuID, ref string MinorMenuID)
        {
            if (string.IsNullOrEmpty(MinorMenuName)) throw new ArgumentNullException();

            IResult pResult = new Result(false);

            CST_MENU_MINOR MinorMenu = new CST_MENU_MINOR();

            try
            {
                IdGenerator idg = new IdGenerator();
                MinorMenuID = idg.GetSID();
                //string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                MinorMenu.SubMenuID = SubMenuID;
                MinorMenu.MinorMenuID = MinorMenuID;
                MinorMenu.Name = MinorMenuName;

                _repository.Create(MinorMenu);

                #region 新增一筆資料[CST_MENU_GRP]
                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    string sqlString = "";

                    #region 取得相同SUBMENUID的資料
                    sqlString = string.Format(@"SELECT * FROM CST_MENU_GRP WHERE SUBMENUID = '{0}'", SubMenuID);

                    var dataListTmp = db.Database.SqlQuery<CST_MENU_GRP>(sqlString).ToList();
                    var dataList = dataListTmp.Select(item => item.UserGroupID).Distinct().ToList();
                    #endregion

                    #region 新增資料
                    foreach(var data in dataList)
                    {
                        sqlString = string.Format(@"INSERT INTO CST_MENU_GRP VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                            idg.GetSID(1), data, dataListTmp[0].MainMenuID, dataListTmp[0].MainMenuName, dataListTmp[0].SubMenuID, dataListTmp[0].SubMenuName,
                            MinorMenuID, MinorMenuName, "0");
                        db.Database.ExecuteSqlCommand(sqlString);
                    }
                    #endregion
                }
                #endregion

                pResult.Success = true;
            }
            catch (Exception ex)
            {
                pResult.Exception = ex;

                //2627 主鍵重複
                //2601 唯一索引重複
                var ErrorCode = ((System.Data.SqlClient.SqlException)((ex.InnerException).InnerException)).Number;
                if (ErrorCode == 2627) pResult.Message = "SID重複";
                if (ErrorCode == 2601) pResult.Message = "RFID編號已被使用，請重新申請";
            }

            return pResult;

        }

        public IResult Update(string MinorMenuID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(MinorMenuID);

            if (instance == null) throw new ArgumentNullException();

            IResult Result = new Result(false);

            try
            {
                DicUpdate.Add(PropertyName, Value);
                
                _repository.Update(instance, DicUpdate);

                if (PropertyName.ToUpper() == "NAME")
                {
                    string sqlString = string.Format(@"UPDATE CST_MENU_GRP SET MINORMENUNAME = '{0}' WHERE MINORMENUID = '{1}'", Value, MinorMenuID);

                    using (WarehouseServerEntities db = new WarehouseServerEntities())
                    {
                        db.Database.ExecuteSqlCommand(sqlString);
                    }
                }

                Result.Success = true;
            }
            catch (Exception ex)
            {
                Result.Exception = ex;
            }

            return Result;
        }

        public IResult Delete(string MinorMenuID)
        {
            IResult result = new Result(false);

            if (!IsExists(MinorMenuID))
            {
                result.Message = "找不到選單資料";
            }

            try
            {
                var instance = GetBySID(MinorMenuID);

                _repository.Delete(instance);

                string sqlString = string.Format(@"DELETE CST_MENU_GRP WHERE MINORMENUID ='{0}'", MinorMenuID);

                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    db.Database.ExecuteSqlCommand(sqlString);
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string MinorMenuID)
        {
            return _repository.GetAll().Any(x => x.MinorMenuID == MinorMenuID);
        }

        public CST_MENU_MINOR GetBySID(string MinorMenuID)
        {
            return _repository.Get(x => x.MinorMenuID == MinorMenuID);
        }

        public IEnumerable<CST_MENU_MINOR> GetAll()
        {
            return _repository.GetAll();
        }

        public IEnumerable<CST_MENU_SUB> GetBySubMenuID(string SubMenuID)
        {
            string sqlString = "";

            sqlString = "Select * From CST_MENU_SUB Where SubMenuID = '" + SubMenuID + "'";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var SubMenuList = db.Database.SqlQuery<CST_MENU_SUB>(sqlString).ToList();
                return SubMenuList;
            }
        }
    }
}