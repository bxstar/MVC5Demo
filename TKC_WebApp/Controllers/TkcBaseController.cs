using iclickpro.AccessCommon;
using iclickpro.BusinessLayer;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKC_WebApp.Controllers
{
    /// <summary>
    /// 淘快词Control基类
    /// </summary>
    public class TkcBaseController : Controller
    {
        /// <summary>
        /// 写log信息
        /// </summary>
        protected static ILog logger = log4net.LogManager.GetLogger("Logger");

        /// <summary>
        /// 用户操作对象
        /// </summary>
        protected BusinessUserHandler bllUserHandler = new BusinessUserHandler();

        #region 全局配置
        public readonly string Const_AppKey = CommonFunction.GetAppSetting("AppKey");
        #endregion


        /// <summary>
        /// 获取客户端IP
        /// </summary>
        public string GetRequestIP()
        {
            
            string ip_address = "127.0.0.1";
            if (HttpContext.Request.ServerVariables["HTTP_VIA"] != null)
            {
                ip_address = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                ip_address = HttpContext.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            return ip_address;
        }

    }
}