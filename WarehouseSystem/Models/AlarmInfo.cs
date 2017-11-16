using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models
{
    public class AlarmInfo
    {
        public string Fabric { get; set; }

        public List<WarehouseSystem.Service.MailDetailService.MailList> MailList = new List<WarehouseSystem.Service.MailDetailService.MailList>();

        public string MailGroupID { get; set; }

        public List<vw_Lots> LotInfo = new List<vw_Lots>();
    }
}