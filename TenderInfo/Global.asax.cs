using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using TenderInfo.App_Start;
using System.Web.Optimization;

namespace TenderInfo
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public Global()
        {
            AuthorizeRequest += new EventHandler(Application_AuthenticateRequest);
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            var isAjax = Context.Request.Headers.Get("x-requested-with");
            if (isAjax == "XMLHttpRequest")
            {
                var url = Context.Request.RawUrl;
                if (url == "/NoticeInfo/GetNoticeListForLogin" || url == "/NoticeInfo/GetNoticeInfoForLogin" || url == "/Home/GetTestUserList")
                {
                    return;
                }
                if (authCookie == null || authCookie.Value == "")
                {
                    Context.Response.StatusCode = 499;
                }
            }

            if (authCookie == null || authCookie.Value == "")
            {
                return;
            }
            else
            {
                FormsAuthenticationTicket authTicket = null;
                try
                {
                    authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                }
                catch
                {
                    return;
                }
                string[] roles = authTicket.UserData.Split(new char[] { ',' });
                if (Context.User != null)
                {
                    Context.User = new System.Security.Principal.GenericPrincipal(Context.User.Identity, roles);
                }
            }
        }
    }
}