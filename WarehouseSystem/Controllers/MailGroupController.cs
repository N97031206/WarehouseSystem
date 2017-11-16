using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem.Models;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Interface;

namespace WarehouseSystem.Controllers
{
    [AuthorizeUser]
    public class MailGroupController : Controller
    {
        private IUserService m_UserService;
        private IMailDetailService m_MailDetailService;
        private IMailGroupService m_MailGroupService;

        public MailGroupController(IMailDetailService p_MailDetailService, IUserService p_UserService, IMailGroupService p_MailGroupService)
        {
            m_UserService = p_UserService;
            m_MailDetailService = p_MailDetailService;
            m_MailGroupService = p_MailGroupService;
        }

        public ActionResult Create()
        {
            MailGroupViewModel _MailGroupViewModel = new MailGroupViewModel();

            return View(_MailGroupViewModel);
        }

        [HttpPost]
        public ActionResult CreateGroup(string GroupName)
        {
            string MailGroupID = "";

            var Result = m_MailGroupService.Create(GroupName, ref MailGroupID);

            if (Result.Success)
            {
                var UserList = m_UserService.GetAll().ToList();

                m_MailDetailService.Insert(MailGroupID, UserList);

                return RedirectToAction("Edit", new { MailGroupID = MailGroupID });

            }
            else
            {
                MailGroupViewModel _MailGroupViewModel = new MailGroupViewModel()
                {
                    ErrorMessage = Result.Message
                };

                return View("Create", _MailGroupViewModel);
            }
        }

        public ActionResult Edit(string MailGroupID)
        {
            if (string.IsNullOrEmpty(MailGroupID))
            {
                return RedirectToAction("List");
            }
            else
            {
                var MailGroup = m_MailGroupService.GetBySID(MailGroupID);

                if (MailGroup != null)
                {
                    MailGroupViewModel _MailGroupViewModel = new MailGroupViewModel()
                    {
                        MailGroup = MailGroup,
                    };

                    return View(_MailGroupViewModel);
                }
                else return RedirectToAction("List");
            }
        }

        public ActionResult GetTreeNodeJSON(string MailGroupID)
        {
            return Content(JsonConvert.SerializeObject(m_MailDetailService.GetTreeNode(MailGroupID)), "application/json");
        }

        [HttpPost]
        public ActionResult UpdateTree(string Checked, string UnChecked)
        {
            int iFailCount = 0;

            if (Checked != null && Checked != "")
            {
                var CheckedList = Checked.Split(',');

                for (int iCount = 0; iCount < CheckedList.Length; iCount++)
                {
                    var pResult = m_MailDetailService.Update(CheckedList[iCount], "Active", "1");
                    if (pResult.Success == false) iFailCount++;
                }
            }

            if (UnChecked != null && UnChecked != "")
            {
                var UnCheckedList = UnChecked.Split(',');

                for (int iCount = 0; iCount < UnCheckedList.Length; iCount++)
                {
                    var pResult = m_MailDetailService.Update(UnCheckedList[iCount], "Active", "0");
                    if (pResult.Success == false) iFailCount++;
                }
            }

            return Json(new { Result = (iFailCount == 0) ? "更新成功" : "更新失敗" });
        }

        [HttpPost]
        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_MailGroupService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            return Json(new { Result = pResult });
            //if (pResult.Success)
            //{
            //    return RedirectToAction("Edit", new { UserGroupID = _JsonType.pk });
            //}
            //else return RedirectToAction("List");
        }

        public ActionResult Delete(string MailGroupID)
        {
            if (!string.IsNullOrEmpty(MailGroupID))
            {
                var pResult = m_MailGroupService.Delete(MailGroupID);
                return Json(new { Result = pResult.Success });
            }

            return Json(new { Result = false });

            //return RedirectToAction("List");
        }

        public ActionResult List(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var MailGroupList = m_MailGroupService.GetAll().ToList();

            List<MailGroupViewModel> _MailGroupViewModel = new List<MailGroupViewModel>();

            foreach (var MailGroup in MailGroupList)
            {
                _MailGroupViewModel.Add(new MailGroupViewModel()
                {
                    MailGroup = MailGroup
                });
            }

            return View(_MailGroupViewModel);
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }
    }
}