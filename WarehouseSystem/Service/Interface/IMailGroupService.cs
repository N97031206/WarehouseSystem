using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service.Interface
{
    public interface IMailGroupService
    {
        IResult Create(string GroupName, ref string MailGroupID);

        IResult Update(string MailGroupID, string PropertyName, object Value);

        IResult Delete(string MailGroupID);

        bool IsExists(string MailGroupID);

        CST_MAIL_GRP GetBySID(string MailGroupID);

        IEnumerable<CST_MAIL_GRP> GetAll();
    }
}