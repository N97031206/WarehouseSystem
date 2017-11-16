using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models.ViewModel
{
    public class AlarmMailViewModel
    {
        public CST_ALARM Alarm = new CST_ALARM();

        public string GroupName { get; set; }

        public List<CST_MAIL_GRP> MailGroup = new List<CST_MAIL_GRP>();

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}