using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace WarehouseSystem.Models
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 程式編號
        /// </summary>
        //public bool Deployer { get; set; }
        //public bool Manager { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");

            }

            //if (!httpContext.User.Identity.IsAuthenticated)
            //{
            //    return false;
            //}

            
            ////取得使用角色
            //FormsIdentity id = httpContext.User.Identity as FormsIdentity;
            //FormsAuthenticationTicket ticket = id.Ticket;
            ////string[] currentRoles = ticket.UserData.Split(',');
            //string currentRoles = ticket.UserData;

            //return true;

            return httpContext.User.Identity.IsAuthenticated;

        }
    }
}