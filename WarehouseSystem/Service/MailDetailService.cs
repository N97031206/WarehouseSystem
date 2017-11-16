using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service
{
    public class MailDetailService : IMailDetailService
    {
        private IRepository<CST_MAIL_GRP_DTL> _repository;

        public class MailList
        {
            public string MailDetailID { get; set; }
            public string MailGroupID { get; set; }
            public string UserGroupID { get; set; }
            public string UserProfileID { get; set; }
            public Nullable<int> Active { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string UserID { get; set; }
            public string Email { get; set; }

        }

        public MailDetailService(IRepository<CST_MAIL_GRP_DTL> repository)
        {
            _repository = repository;
        }


        public IResult Update(string MailDetailID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(MailDetailID);

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
                //Result.LastUpdateTime = LastUpdateTime;
            }
            catch (Exception ex)
            {
                Result.Exception = ex;
            }

            return Result;
        }

        public IResult Delete(string MailDetailID)
        {
            IResult result = new Result(false);

            if (!IsExists(MailDetailID))
            {
                result.Message = "找不到資料";
            }

            try
            {
                var instance = GetBySID(MailDetailID);
                _repository.Delete(instance);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string MailDetailID)
        {
            return _repository.GetAll().Any(x => x.MailDetailID == MailDetailID);
        }

        public CST_MAIL_GRP_DTL GetBySID(string MailDetailID)
        {
            return _repository.Get(x => x.MailDetailID == MailDetailID);
        }

        public IEnumerable<CST_MAIL_GRP_DTL> GetAll()
        {
            return _repository.GetAll();
        }

        public IResult Insert(string MailGroupID, List<CST_USER_PROFILE> UserList)
        {
            string sqlString = "";

            IResult result = new Result(false);

            try
            {
                foreach (var User in UserList)
                {
                    IdGenerator idg = new IdGenerator();

                    var MailDetailID = idg.GetSID(1);

                    sqlString += string.Format(@"Insert Into CST_MAIL_GRP_DTL (MailDetailID, MailGroupID, UserGroupID, UserProfileID) Values
                        ('{0}', '{1}', '{2}', '{3}'); ", MailDetailID, MailGroupID, User.UserGroupID, User.UserProfileID);
                }

                using (WarehouseServerEntities db = new WarehouseServerEntities())
                {
                    if (!sqlString.Equals("")) db.Database.ExecuteSqlCommand(sqlString);
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
            }
            return result;
        }

        public IEnumerable<JObject> GetTreeNode(string MailGroupID)
        {
            List<JObject> _MailList = new List<JObject>();

            var MailGoupList = getMailDetails(MailGroupID).ToList();

            var UserGroupList = getUserGroupList().ToList();

            //例外處理
            UserGroupList.Add(new CST_USER_GRP() { UserGroupID = "" });

            for (int iCount = 0; iCount < UserGroupList.Count; iCount++)
            {
                JObject jGroupName = new JObject();

                var GroupNode = getNode(UserGroupList[iCount].UserGroupID, MailGoupList);

                if (GroupNode.Count > 0)
                {
                    jGroupName.Add("id", UserGroupList[iCount].UserGroupID);
                    jGroupName.Add("text", UserGroupList[iCount].GroupName);

                    JArray jUserList = new JArray();

                    foreach (var UserNode in GroupNode)
                    {
                        JObject jUserNode = new JObject();
                        jUserNode.Add("id", UserNode.MailDetailID);
                        jUserNode.Add("text", UserNode.Name);
                        if (UserNode.Active == 1) jUserNode.Add("checked", "true");
                        jUserList.Add(jUserNode);
                    }

                    jGroupName.Add("children", jUserList);

                    _MailList.Add(jGroupName);
                }
            }

            return _MailList;
        }

        private IEnumerable<CST_USER_GRP> getUserGroupList()
        {
            string sqlString = "";

            sqlString = "Select * From CST_USER_GRP";

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var UserGroupList = db.Database.SqlQuery<CST_USER_GRP>(sqlString).ToList();
                return UserGroupList;
            }
        }

        private List<MailList> getNode(string UserGroupID, List<MailList> MailGroupList)
        {
            List<MailList> _List = new List<MailList>();

            foreach (var MailGroup in MailGroupList)
            {
                if (MailGroup.UserGroupID == UserGroupID)
                {
                    _List.Add(MailGroup);
                }
            }
            return _List;
        }

        private IEnumerable<MailList> getMailDetails(string MailGroupID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select CMG.*, CUP.Name, CUP.Code, CUP.UserID, CUP.Email 
                From CST_MAIL_GRP_DTL CMG 
                    Left Join CST_USER_PROFILE CUP On CUP.UserProfileID = CMG.UserProfileID 
                Where CMG.MailGroupID = '{0}'", MailGroupID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var MailGroupList = db.Database.SqlQuery<MailList>(sqlString).ToList();
                return MailGroupList;
            }
        }

        public IEnumerable<MailList> getByMailGroupID(string MailGroupID)
        {
            string sqlString = "";

            sqlString = string.Format(@"Select CMG.*, CUP.Name, CUP.Code, CUP.UserID, CUP.Email 
                From CST_MAIL_GRP_DTL CMG 
                    Left Join CST_USER_PROFILE CUP On CUP.UserProfileID = CMG.UserProfileID 
                Where CMG.MailGroupID = '{0}' And CMG.Active = 1", MailGroupID);

            using (WarehouseServerEntities db = new WarehouseServerEntities())
            {
                var MailGroupList = db.Database.SqlQuery<MailList>(sqlString).ToList();
                return MailGroupList;
            }
        }
    }
}