using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseSystem.Models;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service
{
    public class UserGroupService : IUserGroupService
    {
        private IRepository<CST_USER_GRP> _repository;

        public UserGroupService(IRepository<CST_USER_GRP> repository)
        {
            _repository = repository;
        }

        public IResult Create(string GroupName, ref string UserGroupID)
        {
            if (string.IsNullOrEmpty(GroupName)) throw new ArgumentNullException();

            IResult pResult = new Result(false);

            CST_USER_GRP UserGroup = new CST_USER_GRP();

            try
            {
                IdGenerator idg = new IdGenerator();
                UserGroupID = idg.GetSID();

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                UserGroup.UserGroupID = UserGroupID;
                UserGroup.GroupName = GroupName;
                UserGroup.Active = 1;
                UserGroup.CreatTime = InsertTime;
                UserGroup.LastUpdateTime = InsertTime;

                _repository.Create(UserGroup);

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

        public IResult Update(string UserGroupID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(UserGroupID);

            if (instance == null) throw new ArgumentNullException();

            IResult Result = new Result(false);

            try
            {
                if (PropertyName == "Active")
                {
                    DicUpdate.Add(PropertyName, Convert.ToInt32(Value));
                }
                else
                {
                    DicUpdate.Add(PropertyName, Value);
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

        public IResult Delete(string UserGroupID)
        {
            IResult result = new Result(false);

            if (!IsExists(UserGroupID))
            {
                result.Message = "找不到資料";
            }

            try
            {
                var instance = GetBySID(UserGroupID);

                _repository.Delete(instance);

                string sqlString = string.Format(@"Delete CST_MENU_GRP Where UserGroupID ='{0}'", UserGroupID);

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

        public bool IsExists(string UserGroupID)
        {
            return _repository.GetAll().Any(x => x.UserGroupID == UserGroupID);
        }

        public CST_USER_GRP GetBySID(string UserGroupID)
        {
            return _repository.Get(x => x.UserGroupID == UserGroupID);
        }

        public IEnumerable<CST_USER_GRP> GetAll()
        {
            return _repository.GetAll();
        }

        public IEnumerable<CST_MENU_GRP> GetByUserGroupID(string UserGroupID)
        {
            string sqlString = "";

            sqlString = "Select * From CST_MENU_GRP Where UserGroupID = '" + UserGroupID + "'";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var MenuGroupList = db.Database.SqlQuery<CST_MENU_GRP>(sqlString).ToList();
                return MenuGroupList;
            }
        }

        public IResult Insert(List<MenuGroupViewModel> _MenuGroupViewModel, string UserGroupID)
        {
            string MainMenuID = "";
            string SubMenuID = "";
            string MinorMenuID = "";
            string MainName = "";
            string SubName = "";
            string MinorName = "";
            int Active = 0;
            string sqlString = "";

            IResult result = new Result(false);

            try
            {
                foreach (var MainMenu in _MenuGroupViewModel)
                {
                    MainMenuID = MainMenu.MainMenu.MainMenuID;
                    MainName = MainMenu.MainMenu.Name;

                    if (MainMenu.SubMenuList.Count > 0)
                    {
                        foreach (var SubMenu in MainMenu.SubMenuList)
                        {
                            SubMenuID = SubMenu.SubMenu.SubMenuID;
                            SubName = SubMenu.SubMenu.Name;

                            if (SubMenu.MinorMenuList.Count > 0)
                            {
                                foreach (var MinorMenu in SubMenu.MinorMenuList)
                                {
                                    MinorMenuID = MinorMenu.MinorMenuID;
                                    MinorName = MinorMenu.Name;

                                    sqlString += GetInsertString(UserGroupID, MainMenuID, SubMenuID, MinorMenuID, Active, MainName, SubName, MinorName);
                                }
                            }
                            else
                            {
                                MinorMenuID = "";
                                MinorName = "";
                                sqlString += GetInsertString(UserGroupID, MainMenuID, SubMenuID, MinorMenuID, Active, MainName, SubName, MinorName);
                            }
                        }
                    }
                    else
                    {
                        SubMenuID = "";
                        MinorMenuID = "";
                        SubName = "";
                        MinorName = "";
                        sqlString += GetInsertString(UserGroupID, MainMenuID, SubMenuID, MinorMenuID, Active, MainName, SubName, MinorName);
                    }
                }

                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    db.Database.ExecuteSqlCommand(sqlString);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            result.Success = true;

            return result;
        }

        private string GetInsertString(string UserGroupID, string MainMenuID,
            string SubMenuID, string MinorMenuID, int Active, string MainName,
            string SubName, string MinorName)
        {
            IdGenerator idg = new IdGenerator();

            var MenuGroupID = idg.GetSID(1);

            string sqlString = string.Format(@" Insert Into CST_MENU_GRP 
                ( MenuGroupID, UserGroupID, MainMenuID, SubMenuID, MinorMenuID, Active, MainMenuName, SubMenuName, MinorMenuName) 
                VALUES('{0}', '{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}'); ", MenuGroupID, UserGroupID, MainMenuID, SubMenuID, MinorMenuID, Active, MainName, SubName, MinorName);

            return sqlString;
        }

        public List<CST_MENU_GRP> GetByMenuGroup(string MainMenuID, string UserGroupID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select * From CST_MENU_GRP Where MainMenuID = '{0}' And UserGroupID = '{1}' ;", MainMenuID, UserGroupID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var MenuList = db.Database.SqlQuery<CST_MENU_GRP>(sqlString).ToList();
                return MenuList;
            }
        }

        public IEnumerable<JObject> GetTreeNode(string UserGroupID)
        {
            List<JObject> _MainMenuList = new List<JObject>();

            var MenuGroup = GetByUserGroupID(UserGroupID).ToList();

            var Menu = GetMenuGroup();

            for (int iMainCount = 0; iMainCount < Menu.Count; iMainCount++)
            {
                JObject jMainMenu = new JObject();

                var MainMenu = GetMainMenu(Menu[iMainCount].MainMenu.MainMenuID, MenuGroup);
                var MainMenuID = Menu[iMainCount].MainMenu.MainMenuID;
                var MainMenuName = Menu[iMainCount].MainMenu.Name;

                if (Menu[iMainCount].SubMenuList.Count > 0)
                {
                    jMainMenu.Add("id", MainMenuID);
                    jMainMenu.Add("text", MainMenuName);

                    JArray _SubMenuList = new JArray();

                    for (int iSubCount = 0; iSubCount < Menu[iMainCount].SubMenuList.Count; iSubCount++)
                    {
                        var SubMenu = GetSubMenu(Menu[iMainCount].SubMenuList[iSubCount].SubMenu.SubMenuID,MainMenu);
                        var SubMenuID = Menu[iMainCount].SubMenuList[iSubCount].SubMenu.SubMenuID;
                        var SubMenuName = Menu[iMainCount].SubMenuList[iSubCount].SubMenu.Name;

                        JObject jSubMenu = new JObject();

                        if (Menu[iMainCount].SubMenuList[iSubCount].MinorMenuList.Count > 0)
                        {
                            jSubMenu.Add("id", SubMenuID);
                            jSubMenu.Add("text", SubMenuName);

                            JArray _MinorMenuList = new JArray();

                            for (int iMinorCount = 0; iMinorCount < Menu[iMainCount].SubMenuList[iSubCount].MinorMenuList.Count; iMinorCount++)
                            {
                                JObject jMinorMenu = new JObject();

                                var MinorMenu = GetMinorMenu(Menu[iMainCount].SubMenuList[iSubCount].MinorMenuList[iMinorCount].MinorMenuID, SubMenu);
                                jMinorMenu.Add("id", MinorMenu[MinorMenu.Count-1].MenuGroupID);
                                jMinorMenu.Add("text", MinorMenu[MinorMenu.Count - 1].MinorMenuName);
                                if (MinorMenu[MinorMenu.Count - 1].Active == 1) jMinorMenu.Add("checked", "true");
                                _MinorMenuList.Add(jMinorMenu);
                            }
                            
                            jSubMenu.Add("children", _MinorMenuList);
                        }
                        else
                        {
                            jSubMenu.Add("id", SubMenu[SubMenu.Count - 1].MenuGroupID);
                            jSubMenu.Add("text", SubMenu[SubMenu.Count - 1].SubMenuName);
                            if (SubMenu[SubMenu.Count - 1].Active == 1) jSubMenu.Add("checked", "true");
                        }


                        _SubMenuList.Add(jSubMenu);
                    }

                    jMainMenu.Add("children", _SubMenuList);
                }
                else
                {
                    jMainMenu.Add("id", MainMenu[MainMenu.Count - 1].MenuGroupID);
                    jMainMenu.Add("text", MainMenu[MainMenu.Count - 1].MainMenuName);
                    if (MainMenu[MainMenu.Count - 1].Active == 1) jMainMenu.Add("checked", "true");
                }

                _MainMenuList.Add(jMainMenu);
            }

            return _MainMenuList;
        }

        private List<CST_MENU_GRP> GetMainMenu(string MainMenuID, List<CST_MENU_GRP> pMenuGroup)
        {
             List<CST_MENU_GRP> _MenuGroup = new List<CST_MENU_GRP>();

            for (int iCount = 0; iCount < pMenuGroup.Count; iCount++)
            {
                if (pMenuGroup[iCount].MainMenuID == MainMenuID)
                {
                    _MenuGroup.Add(pMenuGroup[iCount]);
                }
            }

            return _MenuGroup;
        }

        private List<CST_MENU_GRP> GetSubMenu(string SubMenuID, List<CST_MENU_GRP> pMenuGroup)
        {
            List<CST_MENU_GRP> _MenuGroup = new List<CST_MENU_GRP>();

            for (int iCount = 0; iCount < pMenuGroup.Count; iCount++)
            {
                if (pMenuGroup[iCount].SubMenuID == SubMenuID)
                {
                    _MenuGroup.Add(pMenuGroup[iCount]);
                }
            }

            return _MenuGroup;
        }

        private List<CST_MENU_GRP> GetMinorMenu(string SubMinorID, List<CST_MENU_GRP> pMenuGroup)
        {
            List<CST_MENU_GRP> _MenuGroup = new List<CST_MENU_GRP>();

            for (int iCount = 0; iCount < pMenuGroup.Count; iCount++)
            {
                if (pMenuGroup[iCount].MinorMenuID == SubMinorID)
                {
                    _MenuGroup.Add(pMenuGroup[iCount]);
                }
            }

            return _MenuGroup;
        }

        private List<MenuGroupViewModel> GetMenuGroup()
        {
            List<MenuGroupViewModel> _MenuGroupViewModel = new List<MenuGroupViewModel>();

            //取得主選單
            var MainList = GetAllMainMenu().ToList();

            for (int iMainCount = 0; iMainCount < MainList.Count; iMainCount++)
            {
                //加入主選單項目   
                _MenuGroupViewModel.Add(new MenuGroupViewModel() { MainMenu = MainList[iMainCount] });

                var MainMenuID = MainList[iMainCount].MainMenuID;

                //取得子選單
                var SubList = GetSubByMainMenuID(MainMenuID).ToList();

                for (int iSubCount = 0; iSubCount < SubList.Count; iSubCount++)
                {
                    //加入子選單項目
                    _MenuGroupViewModel[iMainCount].SubMenuList.Add(new MenuGroupViewModel.SubMenuItem() { SubMenu = SubList[iSubCount] });

                    var SubMenuID = SubList[iSubCount].SubMenuID;

                    //取得次選單
                    var MinorList = GetMinorBySubMenuID(SubMenuID).ToList();

                    for (int iMinorCount = 0; iMinorCount < MinorList.Count; iMinorCount++)
                    {
                        //加入次選單項目
                        _MenuGroupViewModel[iMainCount].SubMenuList[iSubCount].MinorMenuList.Add(MinorList[iMinorCount]);
                    }
                }
            }

            return _MenuGroupViewModel;
        }

        private IEnumerable<CST_MENU_MAIN> GetAllMainMenu()
        {
            string sqlString = "";

            sqlString = "Select * From CST_MENU_MAIN Order by [Order] ASC";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var MainMenuList = db.Database.SqlQuery<CST_MENU_MAIN>(sqlString).ToList();
                return MainMenuList;
            }
        }

        private IEnumerable<CST_MENU_SUB> GetSubByMainMenuID(string MainMenuID)
        {
            string sqlString = "";

            sqlString = "Select * From CST_MENU_SUB Where MainMenuID = '" + MainMenuID + "' Order by [Order] ASC";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var SubMenuList = db.Database.SqlQuery<CST_MENU_SUB>(sqlString).ToList();
                return SubMenuList;
            }
        }

        public IEnumerable<CST_MENU_MINOR> GetMinorBySubMenuID(string SubMenuID)
        {
            string sqlString = "";

            sqlString = "Select * From CST_MENU_MINOR Where SubMenuID = '" + SubMenuID + "' Order by [Order] ASC";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var MinorMenuList = db.Database.SqlQuery<CST_MENU_MINOR>(sqlString).ToList();
                return MinorMenuList;
            }
        }
    }
}