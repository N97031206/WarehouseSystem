using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseSystem.Models;
using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Service
{
    public class MenuGroupService : IMenuGroupService
    {
        private IRepository<CST_MENU_GRP> _repository;

        public MenuGroupService(IRepository<CST_MENU_GRP> repository)
        {
            _repository = repository;
        }

        public IResult Update(string MenuGroupID, string PropertyName, object Value)
        {
            Dictionary<string, object> DicUpdate = new Dictionary<string, object>();

            var instance = GetBySID(MenuGroupID);

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
            }
            catch (Exception ex)
            {
                Result.Exception = ex;
            }

            return Result;
        }

        public IResult Delete(string MenuGroupID)
        {
            IResult result = new Result(false);

            if (!IsExists(MenuGroupID))
            {
                result.Message = "找不到資料";
            }

            try
            {
                var instance = GetBySID(MenuGroupID);
                _repository.Delete(instance);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string MenuGroupID)
        {
            return _repository.GetAll().Any(x => x.MenuGroupID == MenuGroupID);
        }

        public CST_MENU_GRP GetBySID(string MenuGroupID)
        {
            return _repository.Get(x => x.MenuGroupID == MenuGroupID);
        }

        public IEnumerable<CST_MENU_GRP> GetAll()
        {
            return _repository.GetAll();
        }
    }
}