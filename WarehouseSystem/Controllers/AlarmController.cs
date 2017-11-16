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
    public class AlarmController : Controller
    {
        private IAlarmService m_AlarmService;
        private IMailGroupService m_MailGroupService;

        public AlarmController(IAlarmService p_AlarmService, IMailGroupService p_MailGroupService)
        {
            m_AlarmService = p_AlarmService;
            m_MailGroupService = p_MailGroupService;
        }

        public ActionResult Create()
        {
            AlarmMailViewModel _AlarmMailViewModel = new AlarmMailViewModel();

            return View(_AlarmMailViewModel);
        }

        [HttpPost]
        public ActionResult CreateDevice(string DeviceName, string WarningDay)
        {
            string AlarmID = "";

            var Result = m_AlarmService.Create(DeviceName, WarningDay, ref AlarmID);

            if (Result.Success)
            {
                return RedirectToAction("Edit", new { AlarmID = AlarmID });
            }
            else
            {
                AlarmMailViewModel _AlarmMailViewModel = new AlarmMailViewModel()
                {
                    ErrorMessage = Result.Message
                };

                return View("Create", _AlarmMailViewModel);
            }
        }

        /// <summary>
        /// 棧板尚未設定儲位之逾時分鐘數
        /// </summary>
        /// <param name="MainMenu"></param>
        /// <param name="SubMenu"></param>
        /// <param name="MinorMenu"></param>
        /// <returns></returns>
        public ActionResult EditLocation(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            //取得設定資料
            var Alarm = m_AlarmService.GetDataByType("LOCATION").ToList();

            string GroupName = "";

            if (Alarm != null)
            {
                var MailGroupList = m_MailGroupService.GetAll().ToList();

                foreach (var MailGroup in MailGroupList)
                {
                    if (MailGroup.MailGroupID == Alarm[0].MailGroupID)
                    {
                        GroupName = MailGroup.GroupName;
                        break;
                    }
                }

                AlarmMailViewModel _AlarmMailViewModel = new AlarmMailViewModel()
                {
                    Alarm = Alarm[0],
                    MailGroup = MailGroupList,
                    GroupName = GroupName
                };

                return View(_AlarmMailViewModel);
            }
            else
            {
                return RedirectToAction("List");
            }

        }

        public ActionResult Edit(string AlarmID)
        {
            if (string.IsNullOrEmpty(AlarmID))
            {
                return RedirectToAction("List");
            }
            else
            {
                var Alarm = m_AlarmService.GetBySID(AlarmID);
                string GroupName = "";

                if (Alarm != null)
                {
                    var MailGroupList = m_MailGroupService.GetAll().ToList();

                    foreach (var MailGroup in MailGroupList)
                    {
                        if (MailGroup.MailGroupID == Alarm.MailGroupID)
                        {
                            GroupName = MailGroup.GroupName;
                            break;
                        }
                    }

                    AlarmMailViewModel _AlarmMailViewModel = new AlarmMailViewModel()
                    {
                        Alarm = Alarm,
                        MailGroup = MailGroupList,
                        GroupName = GroupName
                    };

                    return View(_AlarmMailViewModel);
                }
                else return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult Update(JsonType _JsonType)
        {
            var pResult = m_AlarmService.Update(_JsonType.pk, _JsonType.Name, _JsonType.Value);

            return Json(new { Result = pResult });
        }

        public ActionResult Delete(string AlarmID)
        {
            if (!string.IsNullOrEmpty(AlarmID))
            {
                var pResult = m_AlarmService.Delete(AlarmID);
                return Json(new { Result = pResult.Success });
            }

            return Json(new { Result = false });

        }

        public ActionResult List(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var AlarmList = m_AlarmService.GetDataByType("DEVICE").ToList();

            List<AlarmMailViewModel> _AlarmMailViewModel = new List<AlarmMailViewModel>();

            foreach (var Alarm in AlarmList)
            {
                _AlarmMailViewModel.Add(new AlarmMailViewModel()
                {
                    Alarm = Alarm
                });
            }

            return View(_AlarmMailViewModel);
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }
    }
}