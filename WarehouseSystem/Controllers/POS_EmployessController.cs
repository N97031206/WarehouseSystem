using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem;
using WarehouseSystem.Models;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Misc;
using System.Web.Security;

namespace WarehouseSystem.Controllers
{
    [AuthorizeUser]
    public class POS_EmployessController : Controller
    {
            private IUserService m_UserService;
            private IUserGroupService m_UserGroupService;

            public POS_EmployessController(IUserService pUserService, IUserGroupService pUserGroupService)
            {
                m_UserService = pUserService;
                m_UserGroupService = pUserGroupService;
            }

        public ActionResult Create()
        {
            UserViewModel pUserViewModel = new UserViewModel();

            return View(pUserViewModel);
        }

        [HttpPost]
        public ActionResult CreateUserProfile(string UserID, string Password, string ConfirmPassword)
        {
            UserViewModel pUserViewModel = new UserViewModel();

            if (!string.IsNullOrEmpty(UserID) &&
                !string.IsNullOrEmpty(Password) &&
                !string.IsNullOrEmpty(ConfirmPassword) &&
                ModelState.IsValid)
            {
                //確認密碼是否一致
                if (Password == ConfirmPassword)
                {
                    string UserProfileID = "";

                    var pResult = m_UserService.Create(UserID, Password, ref UserProfileID);

                    if (pResult.Success == false)
                    {
                        pUserViewModel.UserProfile.UserID = UserID;
                        pUserViewModel.UserProfile.Password = Password;
                        pUserViewModel.ErrorMessage = pResult.Message;

                        return View("Create", pUserViewModel);
                    }
                    else
                    {
                        return RedirectToAction("Edit");
                    }
                }
                else
                {
                    pUserViewModel.UserProfile.UserID = UserID;
                    pUserViewModel.UserProfile.Password = Password;
                    pUserViewModel.ErrorMessage = "確認密碼不一致";
                }
            }
            else
            {
                pUserViewModel.UserProfile.UserID = UserID;
                pUserViewModel.UserProfile.Password = Password;
                pUserViewModel.ErrorMessage = "Email空白或密碼空白";
            }

            return View("Create", pUserViewModel);
        }


        public ActionResult Edit(string UserProfileID)
        {
            if (string.IsNullOrEmpty(UserProfileID))
            {
                return RedirectToAction("List");
            }
            else
            {
                var UserProfile = m_UserService.GetBySID(UserProfileID);
                var UserGroup = m_UserGroupService.GetAll().ToList();
                var UserGroupName = UserGroup.Where(e => e.UserGroupID == UserProfile.UserGroupID).ToList();

                UserViewModel pUserViewModel = new UserViewModel()
                {
                    UserProfile = UserProfile,
                    UserGroup = UserGroup,
                    UserGroupName = (UserGroupName.Count > 0) ? UserGroupName[0].GroupName : "",
                    ErrorMessage = ""
                };

                return View(pUserViewModel);
            }
        }
        public ActionResult DefaultEdit(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            var userData = ticket.UserData.Split(',');

            string UserProfileID = userData[0];

            if (string.IsNullOrEmpty(UserProfileID))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var UserProfile = m_UserService.GetBySID(UserProfileID);
                var UserGroup = m_UserGroupService.GetAll().ToList();
                var UserGroupName = UserGroup.Where(e => e.UserGroupID == UserProfile.UserGroupID).ToList();

                UserViewModel pUserViewModel = new UserViewModel()
                {
                    UserProfile = UserProfile,
                    UserGroup = UserGroup,
                    UserGroupName = (UserGroupName.Count > 0) ? UserGroupName[0].GroupName : "",
                    ErrorMessage = ""
                };

                return View(pUserViewModel);
            }
        }

        [HttpPost]
        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_UserService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            return Json(new { Result = pResult });
            //if (pResult.Success)
            //{
            //    var User = m_UserGroupService.GetBySID(_JsonType.pk);

            //    return Json(new { User = User });
            //}
            //else return RedirectToAction("List");
        }

        public ActionResult Delete(string UserProfileID)
        {
            if (!string.IsNullOrEmpty(UserProfileID))
            {
                if (m_UserService.IsExists(UserProfileID))
                {
                    var pResult = m_UserService.Delete(UserProfileID);
                    return Json(new { Result = pResult.Success });
                }
            }

            return Json(new { Result = false });
        }

        public ActionResult List(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var UserProfileList = m_UserService.GetAll().ToList();

            List<UserViewModel> UserViewModelList = new List<UserViewModel>();

            foreach (var UserProfile in UserProfileList)
            {
                UserViewModelList.Add(new UserViewModel()
                {
                    UserProfile = UserProfile
                });
            }

            return View(UserViewModelList);
        }

        public ActionResult ModifyPassword(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == true)
            {
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;

                var userData = ticket.UserData.Split(',');

                CST_USER_PROFILE UserProfile = new CST_USER_PROFILE()
                {
                    UserProfileID = userData[0]
                };

                UserViewModel _UserViewModel = new UserViewModel()
                {
                    UserProfile = UserProfile
                };

                return View(_UserViewModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(string Password, string ModifyPassword, string UserProfileID)
        {
            var Result = m_UserService.ChangePassword(Password, ModifyPassword, UserProfileID);

            if (Result.Success)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                UserViewModel _UserViewModel = new UserViewModel()
                {
                    UserProfile = new CST_USER_PROFILE() { UserProfileID = UserProfileID },
                    ErrorMessage = Result.Message
                };

                return View("ModifyPassword", _UserViewModel);
            }
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            //使用者登出
            FormsAuthentication.SignOut();

            //清除所有的 session
            Session.RemoveAll();

            //建立一個同名的 Cookie 來覆蓋原本的 Cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            //建立 ASP.NET 的 Session Cookie 同樣是為了覆蓋
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            //Session.Clear();
            Session.Abandon();

            UserViewModel _UserViewModel = new UserViewModel();

            return View(_UserViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string UserID, string Password)
        {
            var pUserProfile = m_UserService.Login(UserID, Password);

            if (string.IsNullOrEmpty(pUserProfile.ErrorMessage))
            {
                //使用MVC內建登入並利用自訂權限[AuthorizeUser]功能
                LoginProcess(pUserProfile.UserProfile, false);
                SettingMenuHtml(pUserProfile.UserProfile.UserGroupID);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                pUserProfile.UserProfile = new CST_USER_PROFILE();

                return View(pUserProfile);
            }
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            //使用者登出
            FormsAuthentication.SignOut();

            //清除所有的 session
            Session.RemoveAll();

            //建立一個同名的 Cookie 來覆蓋原本的 Cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            //建立 ASP.NET 的 Session Cookie 同樣是為了覆蓋
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            //Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult AuthauthorityCheckFail()
        {
            return View();
        }

        private void LoginProcess(CST_USER_PROFILE UserProfile, bool IsRemember)
        {
            var now = DateTime.Now;

            string roles = "";

            roles += UserProfile.UserProfileID + ",";
            roles += UserProfile.UserID + ",";
            roles += UserProfile.Name + ",";
            roles += UserProfile.Code + ",";
            roles += UserProfile.UserGroupID;

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                UserProfile.UserID.ToString(),//使用者ID
                DateTime.Now,//核發日期
                DateTime.Now.AddMinutes(1800),//到期日期 30分鐘 
                IsRemember,//永續性
                roles,//使用者定義的資料
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.Expires = ticket.Expiration;
            Response.Cookies.Add(cookie);
            //Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
        }

        private void SettingMenuHtml(string UserGroupID)
        {
            Session["MainMenu"] = "";
            Session["SubMenu"] = "";
            Session["MinorMenu"] = "";

            var _MenuGroupViewModel = m_UserService.GetMenuGroup(UserGroupID).ToList();

            string HtmlString = GetMainHtmlString(_MenuGroupViewModel);

            Session["HtmlString"] = HtmlString;
        }

        private string GetMainHtmlString(List<MenuGroupViewModel> MainMenuList)
        {
            string HtmlString = "";

            foreach (var Menu in MainMenuList)
            {
                var Action = Menu.MainMenu.Action;
                var Controller = Menu.MainMenu.Controller;
                var Name = Menu.MainMenu.Name;
                string UrlAction = "#";
                string TreeViewID = "M" + Menu.MainMenu.MainMenuID;

                //判斷是否要加入Action
                if (Action != null && Action != "" &&
                Controller != null && Controller != "")
                {
                    UrlAction = "/" + Controller + "/" + Action + "?MainMenu=" + TreeViewID;
                }

                HtmlString += string.Format(@"<li class=""treeview"" id=""{2}""><a href=""{0}""><i class=""fa fa-dashboard"">
                    </i><span>{1}</span><i class=""fa fa-angle-left pull-right""></i></a>", UrlAction, Name, TreeViewID);

                var BobyString = GetSubHtmlString(Menu.SubMenuList, TreeViewID);

                if (BobyString != null && BobyString != "") HtmlString += @"<ul class=""treeview-menu"">" + BobyString + "</ul>";

                HtmlString += "</li>";
            }
            return HtmlString;
        }

        private string GetSubHtmlString(List<MenuGroupViewModel.SubMenuItem> SubMenuList, string MainTreeID)
        {
            string HtmlString = "";

            foreach (var Menu in SubMenuList)
            {
                var Action = Menu.SubMenu.Action;
                var Controller = Menu.SubMenu.Controller;
                var Name = Menu.SubMenu.Name;
                string UrlAction = "#";
                string TreeViewID = "S" + Menu.SubMenu.SubMenuID;

                //判斷是否要加入Action
                if (Action != null && Action != "" &&
                Controller != null && Controller != "")
                {
                    UrlAction = "/" + Controller + "/" + Action + "?MainMenu=" + MainTreeID + "&SubMenu=" + TreeViewID;
                }

                HtmlString += string.Format(@"<li class=""treeview"" id = ""{2}""><a href=""{0}""><i class=""fa fa-edit"">
                    </i><span>{1}</span><i class=""fa fa-angle-left pull-right""></i></a>", UrlAction, Name, TreeViewID);


                var BobyString = GetMinorHtmlString(Menu.MinorMenuList, MainTreeID, TreeViewID);

                if (BobyString != null && BobyString != "") HtmlString += @"<ul class=""treeview-menu"">" + BobyString + "</ul>";

                HtmlString += "</li>";

            }
            return HtmlString;
        }

        private string GetMinorHtmlString(List<CST_MENU_MINOR> MinorMenuList, string MainTreeID, string SubTreeID)
        {
            string HtmlString = "";

            foreach (var Menu in MinorMenuList)
            {
                var Action = Menu.Action;
                var Controller = Menu.Controller;
                var Name = Menu.Name;
                string UrlAction = "#";
                string TreeViewID = "N" + Menu.MinorMenuID;

                //判斷是否要加入Action
                if (Action != null && Action != "" &&
                Controller != null && Controller != "")
                {
                    //UrlAction = string.Format(@"@Url.Action(""{0}"", ""{1}"")", Action, Controller);
                    UrlAction = "/" + Controller + "/" + Action + "?MainMenu=" + MainTreeID + "&SubMenu=" + SubTreeID + "&MinorMenu=" + TreeViewID;
                }

                HtmlString += string.Format(@"<li class="""" id=""{2}""><a href=""{0}""><i class=""fa fa-circle-o""></i>{1}</a></li>", UrlAction, Name, TreeViewID);
            }

            return HtmlString;
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }


    }
}