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
    public class MailGroupService : IMailGroupService
    {
        private IRepository<CST_MAIL_GRP> _repository;

        public MailGroupService(IRepository<CST_MAIL_GRP> repository)
        {
            _repository = repository;
        }

        public IResult Create(string GroupName, ref string MailGroupID)
        {
            if (string.IsNullOrEmpty(GroupName))
            {
                throw new ArgumentNullException();
            }

            IResult pResult = new Result(false);

            CST_MAIL_GRP _CST_MAIL_GRP = new CST_MAIL_GRP();

            try
            {
                IdGenerator idg = new IdGenerator();
                MailGroupID = idg.GetSID();

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                _CST_MAIL_GRP.MailGroupID = MailGroupID;
                _CST_MAIL_GRP.GroupName = GroupName;
                _CST_MAIL_GRP.Active = 1;
                //_CST_MAIL_GRP.IsDelete = 0;
                _CST_MAIL_GRP.LastUpdateTime = InsertTime;
                _CST_MAIL_GRP.CreatTime = InsertTime;

                _repository.Create(_CST_MAIL_GRP);

                pResult.Success = true;
            }
            catch (Exception ex)
            {
                pResult.Exception = ex;

                //2627 主鍵重複
                //2601 唯一索引重複
                var ErrorCode = ((System.Data.SqlClient.SqlException)((ex.InnerException).InnerException)).Number;
                if (ErrorCode == 2627) pResult.Message = "SID重複";
                if (ErrorCode == 2601) pResult.Message = "名稱已被使用，請重新申請";
            }

            return pResult;

        }

        public IResult Update(string MailGroupID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(MailGroupID);

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

                var LastUpdateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                DicUpdate.Add("LastUpdateTime", LastUpdateTime);

                _repository.Update(instance, DicUpdate);

                Result.Success = true;
                Result.LastUpdateTime = LastUpdateTime;
            }
            catch (Exception ex)
            {
                Result.Exception = ex;
            }

            return Result;
        }

        public IResult Delete(string MailGroupID)
        {
            IResult result = new Result(false);

            if (!IsExists(MailGroupID))
            {
                result.Message = "找不到資料";
            }

            try
            {
                var instance = GetBySID(MailGroupID);
                _repository.Delete(instance);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string MailGroupID)
        {
            return _repository.GetAll().Any(x => x.MailGroupID == MailGroupID);
        }

        public CST_MAIL_GRP GetBySID(string MailGroupID)
        {
            return _repository.Get(x => x.MailGroupID == MailGroupID);
        }

        public IEnumerable<CST_MAIL_GRP> GetAll()
        {
            return _repository.GetAll();
        }
    }
} 
