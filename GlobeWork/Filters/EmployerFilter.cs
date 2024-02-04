using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace GlobeWork.Filters
{
    public class EmployerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies[".ASPXAUTHEMPLOYER"];
            if (cookie != null)
            {
                var ticketInfo = FormsAuthentication.Decrypt(cookie.Value);
                if (ticketInfo != null)
                {
                    var data = ticketInfo.UserData;
                    var arrData = data.Split('|');
                    filterContext.RouteData.Values["Avatar"] = arrData[0];
                    if (arrData.Length > 1)
                    {
                        filterContext.RouteData.Values["Id"] = arrData[1];
                    }
                    if (arrData.Length > 2)
                    {
                        filterContext.RouteData.Values["Email"] = arrData[2];
                    }
                    if (arrData.Length > 3)
                    {
                        filterContext.RouteData.Values["FullName"] = arrData[3];
                    }
                    if (arrData.Length > 4)
                    {
                        filterContext.RouteData.Values["Amount"] = arrData[4];
                    }
                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {{"action", "EmployerLogin"}, {"controller", "Employer"}});
            }
            base.OnActionExecuting(filterContext);
        }
    }
    public class EmployerLoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies[".ASPXAUTHEMPLOYER"];
            if (cookie == null)
            {
                filterContext.RouteData.Values["Avatar"] = "";
                filterContext.RouteData.Values["Id"] = "";
                filterContext.RouteData.Values["Email"] = "";
                filterContext.RouteData.Values["FullName"] = "";
                filterContext.RouteData.Values["Amount"] = 0;
            }
            else
            {
                var ticketInfo = FormsAuthentication.Decrypt(cookie.Value);
                if (ticketInfo != null)
                {
                    var arrData = ticketInfo.UserData.Split('|');

                    filterContext.RouteData.Values["Avatar"] = arrData[0];
                    if (arrData.Length > 1)
                    {
                        filterContext.RouteData.Values["Id"] = arrData[1];
                    }
                    if (arrData.Length > 2)
                    {
                        filterContext.RouteData.Values["Email"] = arrData[2];
                    }
                    if (arrData.Length > 3)
                    {
                        filterContext.RouteData.Values["FullName"] = arrData[3];
                    }
                    if (arrData.Length > 4)
                    {
                        filterContext.RouteData.Values["Amount"] = arrData[4];
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}