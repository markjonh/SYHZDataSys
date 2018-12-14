using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace System.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SigninAttribute : FilterAttribute, IAuthorizationFilter
    {
        #region IAuthorizationFilter Members

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            SigninUser luser = UserHelper.GetSigninUser;
            bool isAjaxRequest = IsAjaxRequest(filterContext.RequestContext.HttpContext.Request);

            if (luser == null && !isAjaxRequest)
            {
                filterContext.Result = new RedirectResult("/Sign/In?url=" + filterContext.HttpContext.Request.RawUrl);
                return;
            }

            if (luser == null && isAjaxRequest)
            {
                filterContext.Result = new RedirectResult("/Error/Timeout");
                return;
            }
        }

        #endregion
        private bool IsAjaxRequest(HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            return request["X-Requested-With"] == "XMLHttpRequest" || (request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest");
        }
    }
}