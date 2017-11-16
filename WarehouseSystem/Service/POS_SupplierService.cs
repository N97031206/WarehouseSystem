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
    public class POS_SupplierService : IPOS_SupplierService
    {
        private IRepository<POS_Supplier> _repository;

        public POS_SupplierService(IRepository<POS_Supplier> repository)
        {
            _repository = repository;
        }

        public IResult Create(string UserID, string Password, ref string SupID)
        {
            if (string.IsNullOrEmpty(UserID) || string.IsNullOrEmpty(Password))
            {
                throw new ArgumentNullException();
            }

            IResult pResult = new Result(false);

            POS_Supplier UserProfile = new POS_Supplier();

            try
            {
                IdGenerator idg = new IdGenerator();
                SupID = idg.GetSID();

                Encrypt encoder = new Encrypt();
                string EncodePassword = encoder.EncryptSHA(Password);
                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                UserProfile.UserProfileID = SupID;
                UserProfile.UserID = UserID;
                UserProfile.Password = EncodePassword;
                UserProfile.Active = 1;
                UserProfile.CreateTime = InsertTime;
                UserProfile.LastUpdateTime = InsertTime;

                _repository.Create(UserProfile);

                pResult.Success = true;
            }
            catch (Exception ex)
            {
                pResult.Exception = ex;

                //2627 主鍵重複
                //2601 唯一索引重複
                var ErrorCode = ((System.Data.SqlClient.SqlException)((ex.InnerException).InnerException)).Number;
                if (ErrorCode == 2627) pResult.Message = "SID重複";
                if (ErrorCode == 2601) pResult.Message = "帳號已被使用，請重新申請";
            }

            return pResult;

        }


        //public IResult Update(string UserProfileID, string PropertyName, object Value)
        //{
        //    Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

        //    var instance = GetBySID(UserProfileID);

        //    if (instance == null) throw new ArgumentNullException();

        //    IResult Result = new Result(false);

        //    try
        //    {
        //        if (PropertyName == "Active")
        //        {
        //            DicUpdate.Add(PropertyName, Convert.ToInt32(Value));
        //        }
        //        else
        //        {
        //            DicUpdate.Add(PropertyName, Value);
        //        }
        //        var LastUpdateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        //        DicUpdate.Add("LastUpdateTime", LastUpdateTime);

        //        _repository.Update(instance, DicUpdate);

        //        //更新MailGroup資料
        //        if (PropertyName == "UserGroupID")
        //        {
        //            var UserProfile = GetBySID(UserProfileID);

        //            CheckMailGroupDTL(UserProfile);
        //        }

        //        Result.Success = true;

        //        Result.LastUpdateTime = LastUpdateTime;
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.Exception = ex;
        //    }

        //    return Result;
        //}

        //private void CheckMailGroupDTL(POS_Supplier UserProfile)
        //{
        //    string sqlString = "Select * From CST_MAIL_GRP_DTL Where UserProfileID = '" + UserProfile.UserProfileID + "'";

        //    using (WarehouseServerEntities db = new WarehouseServerEntities())
        //    {
        //        var _MailGrpDtlList = db.Database.SqlQuery<CST_MAIL_GRP_DTL>(sqlString).ToList();

        //        if (_MailGrpDtlList.Count == 0)
        //        {
        //            //新增資料
        //            sqlString = "Select * From CST_MAIL_GRP; ";

        //            var _MailGrpList = db.Database.SqlQuery<CST_MAIL_GRP>(sqlString).ToList();

        //            if (_MailGrpList.Count > 0)
        //            {
        //                List<string> list = new List<string>();

        //                foreach (var MailGroup in _MailGrpList)
        //                {
        //                    IdGenerator idg = new IdGenerator();

        //                    var MailDetailID = idg.GetSID(1);

        //                    sqlString = string.Format(@"Insert Into CST_MAIL_GRP_DTL (MailDetailID, MailGroupID, UserGroupID, UserProfileID) Values
        //                        ('{0}', '{1}', '{2}', '{3}'); ", MailDetailID, MailGroup.MailGroupID, UserProfile.UserGroupID, UserProfile.UserProfileID);
        //                    db.Database.ExecuteSqlCommand(sqlString);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //更新資料
        //            sqlString = string.Format(@"Update CST_MAIL_GRP_DTL Set UserGroupID = '{0}' Where UserProfileID ='{1}'", UserProfile.UserGroupID, UserProfile.UserProfileID);
        //            db.Database.ExecuteSqlCommand(sqlString);
        //        }
        //    }
        //}

        public IResult Delete(string SupID)
        {
            IResult result = new Result(false);

            if (!IsExists(SupID))
            {
                result.Message = "找不到使用者資料";
            }

            try
            {
                var instance = GetBySID(SupID);

                _repository.Delete(instance);

                string sqlString = string.Format(@"Delete CST_MAIL_GRP_DTL Where UserProfileID ='{0}'", SupID);

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

        public bool IsExists(string SupID)
        {
            return _repository.GetAll().Any(x => x.SupID == SupID);
        }

        public POS_Supplier GetBySID(string SupID)
        {
            return _repository.Get(x => x.UserProfileID == SupID);
        }

        public POS_Supplier GetByUserID(string SupID)
        {
            return _repository.Get(x => x.SupID == SupID);
        }

        public IEnumerable<POS_Supplier> GetAll()
        {
            return _repository.GetAll();
        }

        //public UserViewModel Login(string UserID, string Password)
        //{
        //    UserViewModel _UserViewModel = new UserViewModel();

        //    if (string.IsNullOrEmpty(UserID) || string.IsNullOrEmpty(Password))
        //    {
        //        _UserViewModel.ErrorMessage = "帳號密碼不得為空白！";
        //    }

        //    var UserProfile = GetByUserID(UserID);

        //    if (UserProfile != null)
        //    {
        //        Encrypt encoder = new Encrypt();

        //        string encodePassword = encoder.EncryptSHA(Password);

        //        if (encodePassword == UserProfile.Password)
        //        {
        //            _UserViewModel.UserProfile = UserProfile;
        //        }
        //        else
        //        {
        //            _UserViewModel.ErrorMessage = "輸入密碼錯誤！";
        //        }
        //    }
        //    else
        //    {
        //        _UserViewModel.ErrorMessage = "此帳號不存在！";
        //    }

        //    return _UserViewModel;
        //}

        //private IEnumerable<CST_MENU_MAIN> GetAllMainMenu()
        //{
        //    string sqlString = "";

        //    sqlString = "Select * From CST_MENU_MAIN Order by [Order] ASC";

        //    using (WarehouseServerEntities db = new WarehouseServerEntities())
        //    {
        //        var MainMenuList = db.Database.SqlQuery<CST_MENU_MAIN>(sqlString).ToList();
        //        return MainMenuList;
        //    }
        //}

        //private IEnumerable<CST_MENU_SUB> GetSubByMainMenuID(string MainMenuID)
        //{
        //    string sqlString = "";

        //    sqlString = "Select * From CST_MENU_SUB Where MainMenuID = '" + MainMenuID + "' Order by [Order] ASC";

        //    using (WarehouseServerEntities db = new WarehouseServerEntities())
        //    {
        //        var SubMenuList = db.Database.SqlQuery<CST_MENU_SUB>(sqlString).ToList();
        //        return SubMenuList;
        //    }
        //}

        //public IEnumerable<CST_MENU_MINOR> GetMinorBySubMenuID(string SubMenuID)
        //{
        //    string sqlString = "";

        //    sqlString = "Select * From CST_MENU_MINOR Where SubMenuID = '" + SubMenuID + "' Order by [Order] ASC";

        //    using (WarehouseServerEntities db = new WarehouseServerEntities())
        //    {
        //        var MinorMenuList = db.Database.SqlQuery<CST_MENU_MINOR>(sqlString).ToList();
        //        return MinorMenuList;
        //    }
        //}

        //public List<MenuGroupData> GetMenu(string UserGroupID)
        //{
        //    string sqlString = "";

        //    sqlString = string.Format(@"Select MenuGrp.*, Minor.Action as MinorAction, Minor.Controller as MinorController, Minor.[Order] as MinorOrder 
        //                    From (
	       //                     Select MenuGrp.*, Sub.Action as SubAction, Sub.Controller as SubController, Sub.[Order] as SubOrder 
	       //                     From (
		      //                      Select MenuGrp.*, Main.Action as MainAction, Main.Controller as MainController, Main.[Order] as MainOrder 
		      //                      From CST_MENU_GRP MenuGrp Left join CST_MENU_MAIN Main
		      //                      on MenuGrp.MainMenuID = Main.MainMenuID
        //                            Where MenuGrp.UserGroupID = '{0}' And MenuGrp.active = '1'
		      //                      ) MenuGrp Left join CST_MENU_SUB Sub
	       //                     on MenuGrp.SubMenuID = Sub.SubMenuID
	       //                     ) MenuGrp Left join CST_MENU_MINOR Minor
        //                    on MenuGrp.MinorMenuID = Minor.MinorMenuID
        //                    Order by MenuGrp.MainOrder, MenuGrp.SubOrder,  Minor.[Order]", UserGroupID);

        //    using (WarehouseServerEntities db = new WarehouseServerEntities())
        //    {
        //        var MenuList = db.Database.SqlQuery<MenuGroupData>(sqlString).ToList();
        //        return MenuList;
        //    }
        //}

        public IResult Update(string UserProfileID, string PropertyName, object Value)
        {
            throw new NotImplementedException();
        }

        //public IEnumerable<MenuGroupViewModel> GetMenuGroup(string UserGroupID)
        //{
        //    List<MenuGroupViewModel> _MenuGroupViewModel = new List<MenuGroupViewModel>();

        //    var MenuGroup = GetMenu(UserGroupID);

        //    for (int iGroupCount = 0; iGroupCount < MenuGroup.Count(); iGroupCount++)
        //    {
        //        //確認MainMenu是否已加入清單內
        //        if (!MainMenuIsExists(_MenuGroupViewModel, MenuGroup[iGroupCount]))
        //        {
        //            //不存在則新增項目
        //            AddMainMenu(ref _MenuGroupViewModel, MenuGroup[iGroupCount]);
        //        }

        //        if (MenuGroup[iGroupCount].SubMenuID != null && MenuGroup[iGroupCount].SubMenuID != "")
        //        {
        //            var MainMenuIndex = _MenuGroupViewModel.Count - 1;

        //            //確認SubMenu是否已加入清單內
        //            if (!SubMenuIsExists(_MenuGroupViewModel[MainMenuIndex].SubMenuList, MenuGroup[iGroupCount]))
        //            {
        //                //不存在則新增項目
        //                AddSubMenu(ref _MenuGroupViewModel[MainMenuIndex].SubMenuList, MenuGroup[iGroupCount]);
        //            }

        //            if (MenuGroup[iGroupCount].MinorMenuID != null && MenuGroup[iGroupCount].MinorMenuID != "")
        //            {
        //                var SubMenuIndex = _MenuGroupViewModel[MainMenuIndex].SubMenuList.Count - 1;

        //                //確認MinorMenu是否已加入清單內
        //                if (!MinorMenuIsExists(_MenuGroupViewModel[MainMenuIndex].SubMenuList[SubMenuIndex].MinorMenuList, MenuGroup[iGroupCount]))
        //                {
        //                    //不存在則新增項目
        //                    AddMinorMenu(ref _MenuGroupViewModel[MainMenuIndex].SubMenuList[SubMenuIndex].MinorMenuList, MenuGroup[iGroupCount]);
        //                }
        //            }
        //        }
        //    }

        //    return public IResult Update(string UserProfileID, string PropertyName, object Value)
        //{
        //    throw new NotImplementedException();
        //}

        //_MenuGroupViewModel;
        //}

        //private void AddMinorMenu(ref List<CST_MENU_MINOR> p_MinorMenuList, MenuGroupData p_MenuGroupData)
        //{
        //    CST_MENU_MINOR MinorMenu = new CST_MENU_MINOR()
        //    {
        //        SubMenuID = p_MenuGroupData.SubMenuID,
        //        MinorMenuID = p_MenuGroupData.MinorMenuID,
        //        Name = p_MenuGroupData.MinorMenuName,
        //        Action = p_MenuGroupData.MinorAction,
        //        Controller = p_MenuGroupData.MinorController,
        //        Order = p_MenuGroupData.MinorOrder
        //    };

        //    p_MinorMenuList.Add(MinorMenu);
        //}

        //private void AddSubMenu(ref List<MenuGroupViewModel.SubMenuItem> p_SubMenuList, MenuGroupData p_MenuGroupData)
        //{
        //    CST_MENU_SUB SubMenu = new CST_MENU_SUB()
        //    {
        //        MainMenuID = p_MenuGroupData.MainMenuID,
        //        SubMenuID = p_MenuGroupData.SubMenuID,
        //        Name = p_MenuGroupData.SubMenuName,
        //        Action = p_MenuGroupData.SubAction,
        //        Controller = p_MenuGroupData.SubController,
        //        Order = p_MenuGroupData.SubOrder,
        //    };

        //    p_SubMenuList.Add(new MenuGroupViewModel.SubMenuItem()
        //    {
        //        SubMenu = SubMenu
        //    });
        //}

        //private void AddMainMenu(ref List<MenuGroupViewModel> p_MainMenuList, MenuGroupData p_MenuGroupData)
        //{
        //    CST_MENU_MAIN MainMenu = new CST_MENU_MAIN()
        //    {
        //        MainMenuID = p_MenuGroupData.MainMenuID,
        //        Name = p_MenuGroupData.MainMenuName,
        //        Action = p_MenuGroupData.MainAction,
        //        Controller = p_MenuGroupData.MainController,
        //        Order = p_MenuGroupData.MainOrder
        //    };

        //    p_MainMenuList.Add(new MenuGroupViewModel()
        //    {
        //        MainMenu = MainMenu
        //    });
        //}
        //private bool MainMenuIsExists(List<MenuGroupViewModel> p_MainMenuList, MenuGroupData p_MenuGroupData)
        //{
        //    bool IsExists = false;

        //    foreach (var MenuGroup in p_MainMenuList)
        //    {
        //        if (MenuGroup.MainMenu.MainMenuID == p_MenuGroupData.MainMenuID)
        //        {
        //            IsExists = true;
        //            break;
        //        }
        //    }

        //    return IsExists;
        //}

        //private bool SubMenuIsExists(List<MenuGroupViewModel.SubMenuItem> p_SubMenuList, MenuGroupData p_MenuGroupData)
        //{
        //    bool IsExists = false;

        //    foreach (var SubMenu in p_SubMenuList)
        //    {
        //        if (SubMenu.SubMenu.SubMenuID == p_MenuGroupData.SubMenuID)
        //        {
        //            IsExists = true;
        //            break;
        //        }
        //    }

        //    return IsExists;
        //}

        //private bool MinorMenuIsExists(List<CST_MENU_MINOR> p_MinorMenuList, MenuGroupData p_MenuGroupData)
        //{
        //    bool IsExists = false;

        //    foreach (var MinorMenu in p_MinorMenuList)
        //    {
        //        if (MinorMenu.MinorMenuID == p_MenuGroupData.MinorMenuID)
        //        {
        //            IsExists = true;
        //            break;
        //        }
        //    }

        //    return IsExists;
        //}


        //public IEnumerable<MenuGroupViewModel> GetMenuGroup()
        //{
        //    List<MenuGroupViewModel> _MenuGroupViewModel = new List<MenuGroupViewModel>();

        //    //取得主選單
        //    var MainList = GetAllMainMenu().ToList();

        //    for (int iMainCount = 0; iMainCount < MainList.Count; iMainCount++)
        //    {
        //        //加入主選單項目   
        //        _MenuGroupViewModel.Add(new MenuGroupViewModel() { MainMenu = MainList[iMainCount] });

        //        var MainMenuID = MainList[iMainCount].MainMenuID;

        //        //取得子選單
        //        var SubList = GetSubByMainMenuID(MainMenuID).ToList();

        //        for (int iSubCount = 0; iSubCount < SubList.Count; iSubCount++)
        //        {
        //            //加入子選單項目
        //            _MenuGroupViewModel[iMainCount].SubMenuList.Add(new MenuGroupViewModel.SubMenuItem() { SubMenu = SubList[iSubCount] });

        //            var SubMenuID = SubList[iSubCount].SubMenuID;

        //            //取得次選單
        //            var MinorList = GetMinorBySubMenuID(SubMenuID).ToList();

        //            for (int iMinorCount = 0; iMinorCount < MinorList.Count; iMinorCount++)
        //            {
        //                //加入次選單項目
        //                _MenuGroupViewModel[iMainCount].SubMenuList[iSubCount].MinorMenuList.Add(MinorList[iMinorCount]);
        //            }
        //        }
        //    }

        //    return _MenuGroupViewModel;
        //}

        //public IResult ChangePassword(string Password, string ModifyPassword, string UserProfileID)
        //{
        //    var UserProfile = GetBySID(UserProfileID);

        //    IResult result = new Result(false);

        //    if (UserProfile != null)
        //    {
        //        Encrypt encoder = new Encrypt();
        //        string EncodePassword = encoder.EncryptSHA(Password);

        //        if (UserProfile.Password == EncodePassword)
        //        {
        //            var NewPassword = encoder.EncryptSHA(ModifyPassword);
        //            result = Update(UserProfileID, "Password", NewPassword);
        //        }
        //        else
        //        {
        //            result.Message = "原始密碼錯誤";
        //        }
        //    }
        //    else
        //    {
        //        result.Message = "SID不存在";
        //    }
        //    return result;
        //}
    }
}