using iclickpro.AccessCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKC_WebApp.Controllers
{
    public class UserController : Controller
    {
        #region 全局配置
        public readonly string Const_AppKey = CommonFunction.GetAppSetting("AppKey");
        #endregion

        /// <summary>
        /// 用户主页
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 淘宝授权
        /// </summary>
        public ActionResult Login()
        {
            return Redirect("http://container.api.taobao.com/container?appkey=" + Const_AppKey);
        }
	}
}