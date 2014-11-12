using iclickpro.AccessCommon;
using iclickpro.BusinessLayer;
using iclickpro.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Top.Api.Domain;

namespace TKC_WebApp
{
    /// <summary>
    /// WSTop 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WSTop : System.Web.Services.WebService
    {
        /// <summary>
        /// 日志输出类
        /// </summary>
        private ILog logger = LogManager.GetLogger("Logger");

        /// <summary>
        /// 推广计划业务逻辑
        /// </summary>
        private BusinessCampaignHandler bllCampaign = new BusinessCampaignHandler();

        /// <summary>
        /// 关键词业务逻辑
        /// </summary>
        private BusinessKeywordHandler bllKeyword = new BusinessKeywordHandler();

        /// <summary>
        /// Web服务代理类，用于宝贝找词，词相关指数的存取
        /// </summary>
        private iclickpro.BusinessLayer.WService.FindWord.WebServiceForKeywordForecast wsProxyFindWord = new iclickpro.BusinessLayer.WService.FindWord.WebServiceForKeywordForecast();

        /// <summary>
        /// Web服务代理类，用于词大盘数据的获取
        /// </summary>
        private iclickpro.BusinessLayer.WService.WebServiceForKeywordForecast wsProxyWordData = new iclickpro.BusinessLayer.WService.WebServiceForKeywordForecast();

        /// <summary>
        /// Web服务代理类，用于淘词数据获取
        /// </summary>
        private iclickpro.BusinessLayer.WService.TaoCi.WebServiceForTaoCi wsProxyTaoCi = new iclickpro.BusinessLayer.WService.TaoCi.WebServiceForTaoCi();


        [WebMethod(EnableSession = true)]
        public string HelloWorld()
        {
            //测试session，使用EnableSession = true
            EntityUser user = HttpContext.Current.Session["user"] as EntityUser;

            if (user != null)
            {
                string strUserInfo = DynamicJsonParser.FromObject(user);
                return strUserInfo;
            }
            else
            {
                return null;
            }
        }


        [WebMethod(EnableSession = true)]
        public Campaign[] GetAllCampaignOnline()
        {//线上，获取所有的计划
            EntityUser user = HttpContext.Current.Session["user"] as EntityUser;
            if (user == null) return null;

            //获取用户所有的推广计划信息
            List<Campaign> lstCampaignInfoOnline = new List<Campaign>();
            try
            {
                string cacheKey_CampaignInfo = string.Format("top_campaigninfo_nick_{0}", user.fSubUserName);

                string cacheValue_CampaignInfo = wsProxyFindWord.GetValue(cacheKey_CampaignInfo);
                if (cacheValue_CampaignInfo == null)
                {
                    lstCampaignInfoOnline = bllCampaign.GetCampaignOnline(user);
                    if (lstCampaignInfoOnline != null)
                        wsProxyFindWord.SetValueTimeOut(cacheKey_CampaignInfo, DynamicJsonParser.FromObject(lstCampaignInfoOnline), 15 * 60);      //缓存15分钟
                }
                else
                {
                    lstCampaignInfoOnline = DynamicJsonParser.ToObject<List<Campaign>>(cacheValue_CampaignInfo);
                }
            }
            catch (Exception se)
            {
                logger.Error(string.Format("获取计划信息出错，参数：{0}", user.fSubUserName), se);
            }

            return lstCampaignInfoOnline.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public EntityCampaignReport[] GetAllCampaignRptOnline(string strDateStart, string strDateEnd)
        {
            //获取计划信息
            EntityUser user = HttpContext.Current.Session["user"] as EntityUser;
            if (user == null) return null;
            List<EntityCampaignReport> lstResultCampaignRpt = new List<EntityCampaignReport>();
            try
            {
                //定义计划报表
                List<EntityCampaignReport> lstCampaignRptOnline = new List<EntityCampaignReport>();

                //获取用户所有的推广计划信息
                List<Campaign> lstCampaignInfoOnline = null;
                string cacheKey_CampaignInfo = string.Format("top_campaigninfo_nick_{0}", user.fSubUserName);

                string cacheValue_CampaignInfo = wsProxyFindWord.GetValue(cacheKey_CampaignInfo);
                if (cacheValue_CampaignInfo == null)
                {
                    lstCampaignInfoOnline = bllCampaign.GetCampaignOnline(user);
                    if (lstCampaignInfoOnline != null)
                        wsProxyFindWord.SetValueTimeOut(cacheKey_CampaignInfo, DynamicJsonParser.FromObject(lstCampaignInfoOnline), 15 * 60);      //缓存15分钟
                }
                else
                {
                    lstCampaignInfoOnline = DynamicJsonParser.ToObject<List<Campaign>>(cacheValue_CampaignInfo);
                }

                string cacheKey_CampaignRpt = string.Format("top_campaignrpt_nick_{0}", user.fSubUserName);
                string cacheValue_CampaignRpt = wsProxyFindWord.GetValue(cacheKey_CampaignRpt);
                if (cacheValue_CampaignRpt == null)
                {
                    //获取每个计划的报表
                    foreach (var campaignId in lstCampaignInfoOnline.Select(o => o.CampaignId))
                    {
                        var lstRptResult = bllCampaign.DownLoadCampaignReport(user, campaignId, 90);    //最多获取90天的报表数据
                        lstCampaignRptOnline.AddRange(lstRptResult);
                    }
                    DateTime dtNow = DateTime.Now;
                    if (lstCampaignRptOnline != null && lstCampaignRptOnline.Count > 0 && DateTime.Now.Hour >= 8 && lstCampaignRptOnline.Where(o => o.impressions > 0 && o.date == dtNow.AddDays(-1).ToString("yyyy-MM-dd")).Count() > 0)
                    {//8点后，如果昨日有计划的展现数据大于0则缓存
                        TimeSpan ts = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day).AddDays(1) - DateTime.Now;
                        wsProxyFindWord.SetValueTimeOut(cacheKey_CampaignRpt, DynamicJsonParser.FromObject(lstCampaignRptOnline), (Int32)ts.TotalSeconds);      //缓存到24:00
                    }
                }
                else
                {
                    lstCampaignRptOnline = DynamicJsonParser.ToObject<List<EntityCampaignReport>>(cacheValue_CampaignRpt);
                }

                DateTime dtStart = Convert.ToDateTime(strDateStart);
                DateTime dtEnd = Convert.ToDateTime(strDateEnd);

                lstResultCampaignRpt = lstCampaignRptOnline.Where(o => Convert.ToDateTime(o.date) >= dtStart && Convert.ToDateTime(o.date) <= dtEnd).ToList();
                foreach (var item in lstResultCampaignRpt)
                {
                    Campaign c = lstCampaignInfoOnline.Find(o => o.CampaignId == item.campaign_id);
                    if (c != null)
                    {
                        item.campaign_name = c.Title;
                        item.campaign_status = c.OnlineStatus;
                    }
                }
            }
            catch (Exception se)
            {
                logger.Error(string.Format("获取计划报表出错，参数：{0},{1},{2}", user.fSubUserName, strDateStart, strDateEnd), se);
            }

            return lstResultCampaignRpt.ToArray();
        }


        [WebMethod(EnableSession = true)]
        public EntityWordData[] GetItemKeywords(string itemIdOrUrl)
        {
            //获取计划信息
            EntityUser session = HttpContext.Current.Session["user"] as EntityUser;
            return bllKeyword.GetItemKeywords(session, itemIdOrUrl).ToArray();
        }


        #region CBF
        [WebMethod(EnableSession = true)]
        public string GetProvidedCatInfo()
        {
            return wsProxyTaoCi.GetTaoCiOneLevelCatsCache("");
        }

        #endregion
    }
}
