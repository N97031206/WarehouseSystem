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
    public class SubMenuService : ISubMenuService
    {
        private IRepository<CST_MENU_SUB> _repository;

        public SubMenuService(IRepository<CST_MENU_SUB> repository)
        {
            _repository = repository;
        }

        public IResult Create(string SubMenuName, string MainMenuID, ref string SubMenuID)
        {
            if (string.IsNullOrEmpty(SubMenuName)) throw new ArgumentNullException();

            IResult pResult = new Result(false);

            CST_MENU_SUB SubMenu = new CST_MENU_SUB();

            try
            {
                IdGenerator idg = new IdGenerator();
                SubMenuID = idg.GetSID();

                //string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                SubMenu.SubMenuID = SubMenuID;
                SubMenu.MainMenuID = MainMenuID;
                SubMenu.Name = SubMenuName;

                _repository.Create(SubMenu);

                #region 新增一筆資料[CST_MENU_GRP]
                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    string sqlString = "";

                    #region 取得相同MAINMENUID的資料
                    sqlString = string.Format(@"SELECT * FROM CST_MENU_GRP WHERE MAINMENUID = '{0}'", MainMenuID);

                    var dataListTmp = db.Database.SqlQuery<CST_MENU_GRP>(sqlString).ToList();
                    var dataList = dataListTmp.Select(item => item.UserGroupID).Distinct().ToList();
                    #endregion

                    #region 新增資料
                    foreach (var data in dataList)
                    {
                        sqlString = string.Format(@"INSERT INTO CST_MENU_GRP VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                            idg.GetSID(1), data, dataListTmp[0].MainMenuID, dataListTmp[0].MainMenuName, SubMenuID, SubMenuName,
                            "", "", "0");
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

        public IResult Update(string SubMenuID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(SubMenuID);

            if (instance == null) throw new ArgumentNullException();

            IResult Result = new Result(false);

            try
            {
                DicUpdate.Add(PropertyName, Value);

                if (PropertyName.ToUpper() == "NAME")
                {
                    string sqlString = string.Format(@"UPDATE CST_MENU_GRP SET SUBMENUNAME = '{0}' WHERE SUBMENUID = '{1}'", Value, SubMenuID);

                    using (WarehouseServerEntities db = new WarehouseServerEntities())
                    {
                        db.Database.ExecuteSqlCommand(sqlString);
                    }
                }

                _repository.Update(instance, DicUpdate);

                Result.Success = true;
            }
            catch (Exception ex)
            {
                Result.Exception = ex;
            }

            return Result;
        }

        public IResult Delete(string SubMenuID)
        {
            IResult result = new Result(false);

            if (!IsExists(SubMenuID))
            {
                result.Message = "找不到選單資料";
            }

            try
            {
                var instance = GetBySID(SubMenuID);

                _repository.Delete(instance);

                string sqlString = string.Format(@"DELETE CST_MENU_GRP WHERE SUBMENUID ='{0}'", SubMenuID);

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

        public bool IsExists(string SubMenuID)
        {
            return _repository.GetAll().Any(x => x.SubMenuID == SubMenuID);
        }

        public CST_MENU_SUB GetBySID(string SubMenuID)
        {
            return _repository.Get(x => x.SubMenuID == SubMenuID);
        }

        public IEnumerable<CST_MENU_SUB> GetAll()
        {
            return _repository.GetAll();
        }

        public IEnumerable<CST_MENU_MINOR> GetBySubMenuID(string SubMenuID)
        {
            string sqlString = "";

            sqlString = "Select * From CST_MENU_MINOR Where SubMenuID = '" + SubMenuID + "'";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var MinorMenuList = db.Database.SqlQuery<CST_MENU_MINOR>(sqlString).ToList();
                return MinorMenuList;
            }
        }

        public IEnumerable<CST_MENU_MAIN> GetByMainMenuID(string MainMenuID)
        {
            string sqlString = "";

            sqlString = "Select * From CST_MENU_MAIN Where MainMenuID = '" + MainMenuID + "'";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var MainMenuList = db.Database.SqlQuery<CST_MENU_MAIN>(sqlString).ToList();
                return MainMenuList;
            }
        }
    }
}