using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using Zhongyu.Data;

namespace System
{
    /// <summary>
    /// html帮助
    /// </summary>
    public static class HtmlHelp
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="ajaxHelper">Ajax</param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="pageViewData"></param>
        /// <returns></returns>
        public static MvcHtmlString Page<T>(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, PageResult<T> pageViewData)
        {
            System.Text.StringBuilder pageBuilder = new Text.StringBuilder();
            pageBuilder.Append("<ul class=\"pagination pagination-split\"><li class=\"disabled\"><a href=\"#\">" + pageViewData.Total + "条</a></li>");
            
            if (pageViewData.HasPreviousPage)
            {
                pageBuilder.Append("<li>");
                RouteValueDictionary newrouteValues = new RouteValueDictionary(routeValues);
                newrouteValues.Add("page", pageViewData.PreviousPage);
                pageBuilder.Append(System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, "<<", actionName, controllerName, newrouteValues, null).ToHtmlString());
                pageBuilder.Append("</li>");
            }
            
            if (pageViewData.CurrentPage <= 5)
            {
                for (int i = 1; i < 11 && i < pageViewData.TotalPages + 1; i++)
                {
                    
                    if (i != pageViewData.CurrentPage)
                    {
                        pageBuilder.Append("<li>");
                        RouteValueDictionary newrouteValues = new RouteValueDictionary(routeValues);
                        newrouteValues.Add("page", i);
                        pageBuilder.Append(System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, i.ToString(), actionName, controllerName, newrouteValues, null).ToHtmlString());
                    }
                    else
                    {
                        pageBuilder.Append("<li class='active'>");
                        pageBuilder.Append("<a href='javascript:viod(0)'>" + i.ToString() + " </a>");
                    }
                    pageBuilder.Append("</li>");
                }
            }
            else
            {
                for (int i = pageViewData.CurrentPage - 5; i < pageViewData.CurrentPage + 5 && i < pageViewData.TotalPages + 1; i++)
                {
                    
                    if (i != pageViewData.CurrentPage)
                    {
                        pageBuilder.Append("<li>");
                        RouteValueDictionary newrouteValues = new RouteValueDictionary(routeValues);
                        newrouteValues.Add("page", i);
                        pageBuilder.Append(System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, i.ToString(), actionName, controllerName, newrouteValues, null).ToHtmlString());
                    }
                    else
                    {
                        pageBuilder.Append("<li class='active'>");
                        pageBuilder.Append("<a href='javascript:viod(0)'>" + i.ToString() + " </a>");
                        
                    }
                    pageBuilder.Append("</li>");
                }
            }

            if (pageViewData.HasNextPage)
            {
                pageBuilder.Append("<li>");
                RouteValueDictionary newrouteValues = new RouteValueDictionary(routeValues);
                newrouteValues.Add("page", pageViewData.NextPage);
                pageBuilder.Append(System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, ">>", actionName, controllerName, newrouteValues, null).ToHtmlString());
                pageBuilder.Append("</li>");
            }
            pageBuilder.Append("</ul>");
            return MvcHtmlString.Create(pageBuilder.ToString());
        }

        public static MvcHtmlString Page<T>(this HtmlHelper htmlHelper, string actionName, object routeValues, PageResult<T> pageViewData)
        {
            System.Text.StringBuilder pageBuilder = new Text.StringBuilder();
            pageBuilder.Append("<ul class=\"pagination pagination-split\"><li class=\"disabled\"><a href=\"#\">" + pageViewData.Total + "条</a></li>");

            if (pageViewData.HasPreviousPage)
            {
                pageBuilder.Append("<li>");
                RouteValueDictionary newrouteValues = new RouteValueDictionary(routeValues);
                newrouteValues.Add("page", pageViewData.PreviousPage);
                pageBuilder.Append(System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, "<<", actionName, newrouteValues, null).ToHtmlString());
                pageBuilder.Append("</li>");
            }

            if (pageViewData.CurrentPage <= 5)
            {
                for (int i = 1; i < 11 && i < pageViewData.TotalPages + 1; i++)
                {
                    
                    if (i != pageViewData.CurrentPage)
                    {
                        pageBuilder.Append("<li>");
                        RouteValueDictionary newrouteValues = new RouteValueDictionary(routeValues);
                        newrouteValues.Add("page", i);
                        pageBuilder.Append(System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, i.ToString(), actionName, newrouteValues, null).ToHtmlString());
                    }
                    else
                    {
                        pageBuilder.Append("<li class='active'>");
                        pageBuilder.Append("<a href='javascript:viod(0)'>" + i.ToString() + " </a>");
                    }
                    pageBuilder.Append("</li>");
                }
            }
            else
            {
                for (int i = pageViewData.CurrentPage - 5; i < pageViewData.CurrentPage + 5 && i < pageViewData.TotalPages + 1; i++)
                {
                    
                    if (i != pageViewData.CurrentPage)
                    {
                        pageBuilder.Append("<li>");
                        RouteValueDictionary newrouteValues = new RouteValueDictionary(routeValues);
                        newrouteValues.Add("page", i);
                        pageBuilder.Append(System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, i.ToString(), actionName, newrouteValues, null).ToHtmlString());
                    }
                    else
                    {
                        pageBuilder.Append("<li class='active'>");
                        pageBuilder.Append("<a href='javascript:viod(0)'>" + i.ToString() + " </a>");

                    }
                    pageBuilder.Append("</li>");
                }
            }

            if (pageViewData.HasNextPage)
            {
                pageBuilder.Append("<li>");
                RouteValueDictionary newrouteValues = new RouteValueDictionary(routeValues);
                newrouteValues.Add("page", pageViewData.NextPage);
                pageBuilder.Append(System.Web.Mvc.Html.LinkExtensions.ActionLink(htmlHelper, ">>", actionName, newrouteValues, null).ToHtmlString());
                pageBuilder.Append("</li>");
            }
            pageBuilder.Append("</ul>");
            return MvcHtmlString.Create(pageBuilder.ToString());
        }
        
    }
}