using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IMailDetailService
    {

        IResult Delete(string MailGroupID);

        IResult Update(string MailGroupID, string PropertyName, object Value);

        bool IsExists(string MailGroupID);

        CST_MAIL_GRP_DTL GetBySID(string MailDetailID);

        IEnumerable<CST_MAIL_GRP_DTL> GetAll();

        IResult Insert(string MailGroupID, List<CST_USER_PROFILE> UserList);

        IEnumerable<JObject> GetTreeNode(string MailGroupID);

        IEnumerable<WarehouseSystem.Service.MailDetailService.MailList> getByMailGroupID(string MailGroupID);
    }
}