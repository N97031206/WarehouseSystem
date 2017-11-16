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
    public class MainMenuService : IMainMenuService
    {
        private IRepository<CST_MENU_MAIN> _repository;

        public MainMenuService(IRepository<CST_MENU_MAIN> repository)
        {
            _repository = repository;
        }

        public IResult Create(string MainMenuName, ref string MainMenuID)
        {
            if (string.IsNullOrEmpty(MainMenuName)) throw new ArgumentNullException();

            IResult pResult = new Result(false);

            CST_MENU_MAIN MainMenu = new CST_MENU_MAIN();

            try
            {
                IdGenerator idg = new IdGenerator();
                MainMenuID = idg.GetSID();

                //string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                MainMenu.MainMenuID = MainMenuID;
                MainMenu.Name = MainMenuName;

                _repository.Create(MainMenu);

                #region 新增一筆資料[CST_MENU_GRP]
                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    string sqlString = "";

                    #region 取得相同MAINMENUID的資料
                    sqlString = string.Format(@"SELECT * FROM CST_USER_GRP WHERE ACTIVE = '1'");

                    var dataList = db.Database.SqlQuery<CST_USER_GRP>(sqlString).ToList();
                    #endregion

                    #region 新增資料
                    foreach (var data in dataList)
                    {
                        sqlString = string.Format(@"INSERT INTO CST_MENU_GRP VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                            idg.GetSID(1), data.UserGroupID, MainMenuID, MainMenuName, "", "", "", "", "0");
                           
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
                if (ErrorCode == 2601) pResult.Message = "名稱已被使用，請重新申請";
            }

            return pResult;

        }

        public IResult Update(string MainMenuID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(MainMenuID);

            if (instance == null) throw new ArgumentNullException();

            IResult Result = new Result(false);

            try
            {
                DicUpdate.Add(PropertyName, Value);

                if (PropertyName.ToUpper() == "NAME")
                {
                    string sqlString = string.Format(@"UPDATE CST_MENU_GRP SET MAINMENUNAME = '{0}' WHERE MAINMENUID = '{1}'", Value, MainMenuID);

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

        public IResult Delete(string MainMenuID)
        {
            IResult result = new Result(false);

            if (!IsExists(MainMenuID))
            {
                result.Message = "找不到選單資料";
            }

            try
            {
                var instance = GetBySID(MainMenuID);
                _repository.Delete(instance);

                string sqlString = string.Format(@"DELETE CST_MENU_GRP WHERE MAINMENUID ='{0}'", MainMenuID);

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

        public bool IsExists(string MainMenuID)
        {
            return _repository.GetAll().Any(x => x.MainMenuID == MainMenuID);
        }

        public CST_MENU_MAIN GetBySID(string MainMenuID)
        {
            return _repository.Get(x => x.MainMenuID == MainMenuID);
        }

        public IEnumerable<CST_MENU_MAIN> GetAll()
        {
            return _repository.GetAll();
        }

        public IEnumerable<CST_MENU_SUB> GetByMainMenuID(string MainMenuID)
        {
            string sqlString = "";

            sqlString = "Select * From CST_MENU_SUB Where MainMenuID = '" + MainMenuID + "'";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var SubMenuList = db.Database.SqlQuery<CST_MENU_SUB>(sqlString).ToList();
                return SubMenuList;
            }
        }
    }
}