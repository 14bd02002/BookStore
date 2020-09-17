using BookStore2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore2.Filters
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {        
        private string[] allowedUsers = new string[] { };
       
        public MyAuthorizeAttribute()
        { }
        private bool User(HttpContextBase httpContext)
        {
            if (allowedUsers.Length > 0)
            {
                return allowedUsers.Contains(httpContext.User.Identity.Name.ToLower());
            }
            return true;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!String.IsNullOrEmpty(base.Users))
            {
                allowedUsers = base.Users.Split(new char[] { ',' });

                for (int i = 0; i < allowedUsers.Length; i++)
                {
                    allowedUsers[i] = allowedUsers[i].Trim().ToLower();
                }
            }


            return httpContext.Request.IsAuthenticated &&
                 User(httpContext);
        }
    }
}