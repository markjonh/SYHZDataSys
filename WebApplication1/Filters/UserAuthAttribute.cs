using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace System.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class UserAuthAttribute : FilterAttribute, IAuthorizationFilter
    {
        private string _namespace;
        private string _actionName;
        private string _controllerName;
        private string[] _roleId;

        #region IAuthorizationFilter Members

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            SigninUser luser = UserHelper.GetSigninUser;
            if (luser == null)
            {
                filterContext.Result = new RedirectResult("/Sign/In?url=" + filterContext.HttpContext.Request.RawUrl);
                return;
            }
            _roleId = luser.RoleId.ToArray();
            _namespace = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.Namespace;
            _actionName = filterContext.ActionDescriptor.ActionName;
            _controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.Name;

            if (AuthorizeCore(filterContext.HttpContext))
            {
                // ** IMPORTANT **
                // Since we're performing authorization at the action level, the authorization code runs
                // after the output caching module. In the worst case this could allow an authorized user
                // to cause the page to be cached, then an unauthorized user would later be served the
                // cached page. We work around this by telling proxies not to cache the sensitive page,
                // then we hook our custom authorization code into the caching mechanism so that we have
                // the final say on whether a page should be served from the cache.

                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            }
            else
            {
                filterContext.Result = new RedirectResult("/Sign/In?m=您没有该功能的访问权限！");
                return;
            }
        }

        protected virtual bool AuthorizeCore(HttpContextBase httpContext)
        {
            return PermissionHelper.CheckPermission(_roleId, _namespace, _controllerName, _actionName);
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus) { validationStatus = OnCacheAuthorization(new HttpContextWrapper(context)); }

        protected virtual void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Returns HTTP 401 - see comment in HttpUnauthorizedResult.cs.
            filterContext.Result = new HttpUnauthorizedResult();
        }

        // This method must be thread-safe since it is called by the caching module.
        protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            bool isAuthorized = AuthorizeCore(httpContext);
            return (isAuthorized) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }

        internal static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            IEnumerable<string> split = from piece in original.Split(',')
                                        let trimmed = piece.Trim()
                                        where !String.IsNullOrEmpty(trimmed)
                                        select trimmed;
            return split.ToArray();
        }

        #endregion

    }
}