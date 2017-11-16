using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem.Models;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service;
using WarehouseSystem.Service.Interface;

namespace WarehouseSystem.Controllers
{
   [AuthorizeUser]
    public class UserGroupController : Controller
    {
        private IUserGroupService m_UserGroupService;
        private IUserService m_UserService;
        private IMenuGroupService m_MenuGroupService;

        public UserGroupController(IUserGroupService p_UserGroupService, IUserService p_UserService, IMenuGroupService p_MenuGroupService)
        {

            m_UserGroupService = p_UserGroupService;
            m_UserService = p_UserService;
            m_MenuGroupService = p_MenuGroupService;
        }

        public ActionResult Create()
        {
            UserGroupViewModel _AuthorityViewModel = new UserGroupViewModel();

            return View(_AuthorityViewModel);
        }

        [HttpPost]
        public ActionResult CreateGroup(string GroupName)
        {
            string UserGroupID = "";

            var Result = m_UserGroupService.Create(GroupName, ref UserGroupID);

            if (Result.Success)
            {
                var MenuGroup = m_UserService.GetMenuGroup().ToList();
                var vResult = m_UserGroupService.Insert(MenuGroup, UserGroupID);

                return RedirectToAction("Edit", new { UserGroupID = UserGroupID });

            }
            else
            {
                UserGroupViewModel _AuthorityViewModel = new UserGroupViewModel()
                {
                    ErrorMessage = Result.Message
                };

                return View("Create", _AuthorityViewModel);
            }
        }

        public ActionResult Edit(string UserGroupID)
        {
            if (string.IsNullOrEmpty(UserGroupID))
            {
                return RedirectToAction("List");
            }
            else
            {
                var UserGroup = m_UserGroupService.GetBySID(UserGroupID);

                if (UserGroup != null)
                {
                    var MenuGroup = m_UserGroupService.GetByUserGroupID(UserGroupID).ToList();

                    UserGroupViewModel _AuthorityViewModel = new UserGroupViewModel()
                    {
                        UserGroup = UserGroup,
                        MenuGroup = MenuGroup
                    };

                    return View(_AuthorityViewModel);
                }
                else return RedirectToAction("List");
            }
        }

        public ActionResult GetTreeNodeJSON(string UserGroupID)
        {
            return Content(JsonConvert.SerializeObject(m_UserGroupService.GetTreeNode(UserGroupID)), "application/json");
        }

        public ActionResult UpdateTree(string Checked, string UnChecked, string UserGroupID)
        {
            int iFailCount = 0;

            if (Checked != null && Checked != "")
            {
                var CheckedList = Checked.Split(',');

                for (int iCount = 0; iCount < CheckedList.Length; iCount++)
                {
                    var pResult = m_MenuGroupService.Update(CheckedList[iCount], "Active", "1");
                    if (pResult.Success == false) iFailCount++;
                }
            }

            if (UnChecked != null && UnChecked != "")
            {
                var UnCheckedList = UnChecked.Split(',');

                for (int iCount = 0; iCount < UnCheckedList.Length; iCount++)
                {
                    var pResult = m_MenuGroupService.Update(UnCheckedList[iCount], "Active", "0");
                    if (pResult.Success == false) iFailCount++;
                }
            }

            return Json(new { Result = (iFailCount == 0) ? "更新成功" : "更新失敗" });
        }

        [HttpPost]
        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_UserGroupService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            if (pResult.Success)
            {
                return RedirectToAction("Edit", new { UserGroupID = _JsonType.pk });
            }
            else return RedirectToAction("List");
        }

        public ActionResult Delete(string UserGroupID)
        {
            if (!string.IsNullOrEmpty(UserGroupID))
            {
                var pResult = m_UserGroupService.Delete(UserGroupID);
                return Json(new { Result = pResult.Success });
            }

            return Json(new { Result = false });

            //return RedirectToAction("List");
        }

        public ActionResult List(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var UserGroupList = m_UserGroupService.GetAll().ToList();

            List<UserGroupViewModel> _AuthorityViewModel = new List<UserGroupViewModel>();

            foreach (var UserGroup in UserGroupList)
            {
                _AuthorityViewModel.Add(new UserGroupViewModel()
                {
                    UserGroup = UserGroup
                });
            }

            return View(_AuthorityViewModel);
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }
    }
}