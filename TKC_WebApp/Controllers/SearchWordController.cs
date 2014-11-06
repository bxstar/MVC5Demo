using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKC_WebApp.Controllers
{
    public class SearchWordController : Controller
    {
        /// <summary>
        /// 主页视图
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 宝贝找词
        /// </summary>
        public ActionResult ByItem()
        {
            return View();
        }

        /// <summary>
        /// 成交词拓展
        /// </summary>
        public ActionResult ByWord()
        {
            return View();
        }
	}
}