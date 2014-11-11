using iclickpro.AccessCommon;
using iclickpro.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Top.Api.Response;

namespace iclickpro.BusinessLayer
{
    /// <summary>
    /// 用户业务逻辑
    /// </summary>
    public class BusinessUserHandler
    {

        /// <summary>
        /// 操作API数据
        /// </summary>
        private BusinessTaobaoApiHandler bllTaoBaoApi = new BusinessTaobaoApiHandler();

        /// <summary>
        /// 写log信息
        /// </summary>
        private static ILog logger = log4net.LogManager.GetLogger("Logger");

        /// <summary>
        /// 取得用户的订购信息，收费代码和到期时间，使用"|"分隔
        /// </summary>
        public String GetUserVas(EntityUser session, ref string message)
        {
            string userSubscribe = string.Empty;
            string cacheKey_subscribe = "tkci_user_subscribe_" + session.fSubUserName;
            int cacheTimeOut = 30 * 60; //缓存30分钟
            WService.WebServiceForKeywordForecast wsProxy = new WService.WebServiceForKeywordForecast();
            try
            {
                userSubscribe = wsProxy.GetValueTimeOut(cacheKey_subscribe);
                if (!string.IsNullOrEmpty(userSubscribe))
                    return userSubscribe;
            }
            catch (Exception se)
            {
                logger.Error("缓存获取用户收费代码失败", se);
            }

            VasSubscribeGetResponse response = CommonHandler.DoTaoBaoApi<VasSubscribeGetResponse>(bllTaoBaoApi.TaobaoSimbaSvaGet, session);
            if (response.IsError)
            {
                message = response.SubErrMsg;
                logger.ErrorFormat("API获取用户收费代码失败，{0}", response.Body);
            }
            else
            {
                if (response != null && response.ArticleUserSubscribes != null && response.ArticleUserSubscribes.Count > 0)
                {
                    string itemCode = string.Join(",", response.ArticleUserSubscribes.Select(o => o.ItemCode).ToArray());
                    string deadLine = string.Join(",", response.ArticleUserSubscribes.Select(o => o.Deadline).ToArray());
                    userSubscribe = itemCode + "|" + deadLine;
                    try
                    {
                        wsProxy.SetValueTimeOut(cacheKey_subscribe, userSubscribe, cacheTimeOut);
                    }
                    catch (Exception se)
                    {
                        logger.Error("缓存设置用户收费代码失败", se);
                    }
                }
            }


            return userSubscribe;
        }

        /// <summary>
        /// 判断用户是否VIP
        /// </summary>
        public Boolean CheckUserVip(string userName)
        {
            Boolean result = false;
            string cacheKey = "tkci_user_vips";
            WService.WebServiceForKeywordForecast wsProxy = new WService.WebServiceForKeywordForecast();
            try
            {
                string vips = wsProxy.GetValueTimeOut(cacheKey);
                if (!string.IsNullOrEmpty(vips))
                {
                    string[] arrUserName = vips.Split(',');
                    if (arrUserName.Contains(userName))
                    {
                        result = true;
                    }
                }
            }
            catch (Exception se)
            {
                logger.Error("缓存获取VIP用户失败", se);
            }
            return result;
        }


        /// <summary>
        /// 判断用户是否是测试初级版的
        /// </summary>
        public Boolean IsUserTestBasicVersion(string userName)
        {
            Boolean result = false;
            string cacheKey = "tkci_basic_version_user";
            WService.WebServiceForKeywordForecast wsProxy = new WService.WebServiceForKeywordForecast();
            try
            {
                string cacheValue = wsProxy.GetValueTimeOut(cacheKey);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    string[] arrUserName = cacheValue.Split(',');
                    if (arrUserName.Contains(userName))
                    {
                        result = true;
                    }
                }
            }
            catch (Exception se)
            {
                logger.Error("缓存获取初级版测试用户失败", se);
            }
            return result;
        }


        /// <summary>
        /// 保存或更新用户信息，自己登录更新Session及LoginUrl，代理用户登录使用被代理用户的登录信息，不更新LoginUrl
        /// </summary>
        /// <param name="param">登录参数</param>
        /// <param name="IsProxyLogin">是否代理方式登录</param>
        public EntityUser UpdateUserInfo(Dictionary<string, object> param, Boolean IsProxyLogin)
        {
            EntityUser session = null;
            string subUserName = param["subUserName"].ToString();

            EntityUser existUser = DefaultDbContext.Current().User.FirstOrDefault(o => o.fUserName == o.fSubUserName && o.fSubUserName == subUserName);


            if (existUser != null)
            {
                if (IsProxyLogin && !string.IsNullOrEmpty(existUser.fLoginUrl))
                {//代理用户登录使用被代理用户的登录信息，不更新LoginUrl
                    param["loginUrl"] = existUser.fLoginUrl; //使用被代理账户的登录参数，如果被代理账户从未登录过则还是使用代理账户的登录参数
                }
                //使用被代理账户的用户编号，保证编号唯一
                param["userId"] = existUser.fUserID;
            }
            else
            {
                param["userId"] = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            }


            var dsUserInfo = DefaultDbContext.Current().Database.SqlQuery<EntityUser>("exec pro_add_or_update_user_info @userName,@subUserName,@userId,@isPoxy,@session,@Ip,@Versions,@loginUrl,@FeeCode", 
                SqlNameAndParamer.ConvertSqlParameter(param)).ToList();


            if (dsUserInfo != null && dsUserInfo.Count > 0)
            {
                session = dsUserInfo.First();
            }

            //保留收费代码
            session.FeeCode = param["FeeCode"].ToString();
            return session;
        }


    }
}
