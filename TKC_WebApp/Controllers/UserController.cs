using iclickpro.AccessCommon;
using iclickpro.BusinessLayer;
using iclickpro.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKC_WebApp.Controllers
{
    public class UserController : TkcBaseController
    {
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

        /// <summary>
        /// 用户登录
        /// </summary>
        public ActionResult UserLogin()
        {

            if (Request["top_appkey"] != null && Request["top_parameters"] != null && Request["top_session"] != null && Request["top_sign"] != null)
            {
                string topSession = Request["top_session"];
                string UserName = CommonFunction.GetUserName("visitor_nick", CryptHelper.GetTopParamer(Request["top_parameters"]));
                if (UserName == "商家测试帐号17")
                {//屏蔽测试账号
                    return null;
                }
                // 定义User对象
                EntityUser session = new EntityUser();
                session.fUserName = UserName;
                session.fSession = topSession;
                session.fSubUserName = UserName;
                session.fLoginUrl = string.Format("top_appkey={0}&top_parameters={1}&top_session={2}&top_sign={3}", Request["top_appkey"], HttpUtility.UrlEncode(Request["top_parameters"]), Request["top_session"], Request["top_sign"]);
                string message = string.Empty;
                string ips = Request.ServerVariables.Get("Remote_Addr").ToString();

                string userSubscribe = bllUserHandler.GetUserVas(session, ref message);
                if (userSubscribe.Split('|').Length == 2)
                {
                    logger.InfoFormat("用户：{0}，订购信息{1}", session.fSubUserName, userSubscribe);
                    session.FeeCode = userSubscribe.Split('|')[0];
                    session.DeadLine = userSubscribe.Split('|')[1];
                }

                Session["user"] = session;

                Boolean isVipUser = bllUserHandler.CheckUserVip(session.fUserName);
                if (isVipUser && Request.Url.ToString().ToLower().Contains("tkc.taokuaiche.com"))
                {//是VIP用户，并且请求的是淘快词的Url，则跳转到测试网站
                    string vipUrl = string.Format("http://vip.taokuaiche.com/login?{0}", session.fLoginUrl);
                    return Redirect(vipUrl);
                }

                if (session.FeeCode == "ts-25420-1" || session.FeeCode == "ts-25420-3" || session.FeeCode == "ts-25420-v4")
                {//初级版
                    // 数据库中取出用户信息
                    var param = new Dictionary<string, object>();
                    param.Add("userName", session.fUserName);
                    param.Add("subUserName", session.fSubUserName);
                    param.Add("userId", "");
                    param.Add("isPoxy", "1");
                    param.Add("session", session.fSession);
                    param.Add("Ip", GetRequestIP());
                    param.Add("Versions", "1.0");
                    //主账户使用淘宝返回的地址参数更新数据库，代理账户需要从数据库中取得loginUrl，所以要使该值的最终值有效，被代理用户必须先于代理用户登录，该值在数据库中才存在，该值的做法类似userId
                    param.Add("loginUrl", session.fLoginUrl);
                    param.Add("FeeCode", session.FeeCode);
                    // 获取用户信息
                    session = bllUserHandler.UpdateUserInfo(param, true);
                    
                    // 存储Session
                    Session["user"] = session;

                    if (session.FeeCode == "ts-25420-1" || bllUserHandler.IsUserTestBasicVersion(session.fUserName))
                    {//跳转初级版主页
                        return RedirectToAction("Index", "SearchWord");
                    }
                    else if (session.FeeCode == "ts-25420-3")
                    {//跳转智能版主页
                        return Redirect("~/sl.html");
                    }
                    else if (session.FeeCode == "ts-25420-v4")
                    {//跳转托管版主页
                        return Redirect("http://ao.taokuaiche.com/User/UserLogin?" + session.fLoginUrl + "&itemCode=" + session.FeeCode + "&deadLine=" + session.DeadLine);
                    }
                }
                else if (session.FeeCode == "ts-25420-2")
                {//专家开车版本
                    return Redirect("http://ao.taokuaiche.com/User/UserLogin?" + session.fLoginUrl + "&itemCode=" + session.FeeCode + "&deadLine=" + session.DeadLine);
                }
            }

            return Redirect("http://tkc.taokuaiche.com");
        }
	}
}