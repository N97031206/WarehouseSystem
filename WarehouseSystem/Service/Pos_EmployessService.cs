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
    public class PosEmployessService : IPosUserService
    {
        //改資料連結
        private IRepository<CST_USER_PROFILE> _repository;

        public PosEmployessService(IRepository<CST_USER_PROFILE> repository)
        {
            _repository = repository;
        }

        public IResult ChangePassword(string Password, string ModifyPassword, string UserProfileID)
        {
            throw new NotImplementedException();
        }

        public IResult Create(string UserID, string Password, string Title, ref string UserProfileID)
        {
            throw new NotImplementedException();
        }

        public IResult Delete(string UserProfileID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<POS_Employee> GetAll()
        {
            throw new NotImplementedException();
        }

        public POS_Employee GetBySID(string UserProfileID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MenuGroupViewModel> GetMenuGroup()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MenuGroupViewModel> GetMenuGroup(string UserGroupID)
        {
            throw new NotImplementedException();
        }

        public bool IsExists(string UserProfileID)
        {
            throw new NotImplementedException();
        }

        public UserViewModel Login(string UserID, string Password)
        {
            throw new NotImplementedException();
        }

        public IResult Update(string UserProfileID, string PropertyName, object Value)
        {
            throw new NotImplementedException();
        }
    }
}