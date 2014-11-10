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
                string cacheKey_CampaignInfo = string.Format("top_campaigninfo_nick_{0}", user.SubUserName);

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
                logger.Error(string.Format("获取计划信息出错，参数：{0}", user.SubUserName), se);
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
                string cacheKey_CampaignInfo = string.Format("top_campaigninfo_nick_{0}", user.SubUserName);

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

                string cacheKey_CampaignRpt = string.Format("top_campaignrpt_nick_{0}", user.SubUserName);
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
                logger.Error(string.Format("获取计划报表出错，参数：{0},{1},{2}", user.SubUserName, strDateStart, strDateEnd), se);
            }

            return lstResultCampaignRpt.ToArray();
        }


        [WebMethod(EnableSession = true)]
        public EntityWordData[] GetItemKeywords(string itemIdOrUrl)
        {
            //获取计划信息
            EntityUser session = HttpContext.Current.Session["user"] as EntityUser;
            if (session == null) return null;

            EntityItem itemOnline = CommonHandler.GetItemOnline(itemIdOrUrl);
            if (itemOnline == null || itemOnline.item_id == 0)
            {
                return new EntityWordData[] { };
            }
            else
            {
                logger.InfoFormat("用户：{0}，宝贝：{1}，标题：{2}", session.UserName, itemOnline.item_id, itemOnline.item_title);
            }

            //缓存获取找词的数量
            List<EntityWordData> resultFindKeyword = CommonHandler.GetUserItemFindKeywordCache(session.SubUserName, itemOnline.item_id);
            if (resultFindKeyword != null && resultFindKeyword.Count() > 100)
            {
                logger.InfoFormat("用户：{0}，宝贝：{1}，缓存取词", session.UserName, itemOnline.item_id);
                return resultFindKeyword.ToArray();
            }

            //重新找词
            logger.InfoFormat("用户：{0}，宝贝：{1}，开始找词", session.UserName, itemOnline.item_id);
            resultFindKeyword = new List<EntityWordData>();

            //发送蜘蛛抓词任务
            string exchangeName = "ex_taobao_spider_samesimilar_item";
            BusinessMQ.SendMsgToExchange(session, exchangeName, string.Format("{0},{1},{2}", itemOnline.item_id, itemOnline.item_title, itemOnline.nick));

            //更新宝贝的信息

            logger.InfoFormat("用户：{0}，宝贝：{1}，获取类目热词开始", session.UserName, itemOnline.item_id);
            List<string> lstCategoryWord = CommonHandler.GetCatTopKeyword(itemOnline.cid);
            logger.InfoFormat("用户：{0}，宝贝：{1}，获取类目热词完成，数量：{2}", session.UserName, itemOnline.item_id, lstCategoryWord.Count);

            logger.InfoFormat("用户：{0}，宝贝：{1}，获取关键词开始", session.UserName, itemOnline.item_id);
            DateTime dtStartFind = DateTime.Now;
            //标题分词在类目中出现的词，按重复字符数*长度排序，还要按照找词统计来排序
            List<string> lstMainWord = new List<string>();
            //蜘蛛抓取的关键词
            List<string> lstSpiderFindWord = new List<string>();
            //核心词排序字典
            Dictionary<string, int> dicMainWord = new Dictionary<string, int>();
            //是否找到了第一核心词
            Boolean isFindFirstMainWord = false;
            //是否通过蜘蛛找到了同款和相似宝贝的关键词
            Boolean isFindKeywordBySpider = false;
            //将宝贝标题的分词按长度排序，在类目名称中的关键词作为核心词
            List<string> lstTitleWord = CommonHandler.SplitWordFromWs(itemOnline.item_title).Split(',').OrderByDescending(o => o.Length).ToList();

            foreach (var item in lstTitleWord)
            {//标题分词中，被类目名称包含的为核心词
                if (item.Length > 1 && itemOnline.categroy_name.Contains(item) && !lstMainWord.Contains(item))
                {
                    lstMainWord.Add(item);
                    isFindFirstMainWord = true;
                }
            }

            foreach (var item in lstTitleWord)
            {//标题分词中，和类目名称有交集的，为核心词
                int sameCharCount = item.ToCharArray().Intersect(itemOnline.categroy_name.ToCharArray()).Count();
                if (item.Length > 1 && sameCharCount > 0 && !dicMainWord.ContainsKey(item))
                    dicMainWord.Add(item, sameCharCount * item.Length);
            }

            if (dicMainWord.Count > 0)
            {//核心词汇总，交集中重复字符越多，排序值最大，放最前
                lstMainWord = lstMainWord.Union(dicMainWord.OrderByDescending(o => o.Value).Select(o => o.Key).ToList()).ToList();
                isFindFirstMainWord = true;
                dicMainWord = new Dictionary<string, int>();
            }


            while ((!isFindKeywordBySpider) && (dtStartFind.AddSeconds(30) >= DateTime.Now))
            {//类目找不到词或找到不只一个词，30秒内没找到放弃
                string strFindKeywordResult = CommonHandler.GetItemFindKeywordCache(itemOnline.item_id);
                if (string.IsNullOrEmpty(strFindKeywordResult))
                {//暂时没有找到
                    System.Threading.Thread.Sleep(2000);
                    continue;
                }
                isFindKeywordBySpider = true;

                lstSpiderFindWord = strFindKeywordResult.Split(',').ToList();
                if (isFindFirstMainWord)
                {
                    //使用找词结果排序
                    foreach (var item in lstMainWord)
                    {
                        int intWordIndex = lstSpiderFindWord.FindIndex(o => o == item);
                        dicMainWord.Add(item, intWordIndex == -1 ? 9 : intWordIndex);   //不存在找词结果中的词，排最后
                    }
                    //排序值最小，放最前
                    lstMainWord = dicMainWord.OrderBy(o => o.Value).Select(o => o.Key).ToList();
                }
                else
                {
                    lstMainWord = lstSpiderFindWord.Take(2).ToList();
                }
            }
            //核心词
            string mainWord = string.Join(",", lstMainWord);

            if (lstMainWord.Count == 0)
            {
                if (itemOnline.categroy_name != "其他" && itemOnline.categroy_name != "其它")
                {
                    mainWord = string.Join(",", itemOnline.categroy_name.Split('/'));
                }
                else
                {//类目名称不能作为核心词时，使用属性词作为核心词
                    foreach (var itemProp in itemOnline.LstPropsName)
                    {
                        string str = Strings.GetChineseString(itemProp);
                        if (str != null && str.Length >= 2)
                        {
                            lstMainWord.Add(str);
                        }
                    }
                    mainWord = string.Join(",", lstMainWord);
                }
            }

            logger.InfoFormat("用户：{0}，宝贝：{1}，获取关键词完成，核心词：{2}，数量：{3}", session.UserName, itemOnline.item_id, mainWord, lstSpiderFindWord.Count);

            resultFindKeyword = CommonHandler.CombineWord(itemOnline, lstMainWord, lstSpiderFindWord.Union(lstTitleWord).Union(lstCategoryWord).ToList());
            int totalCount = resultFindKeyword.Count;

            if (totalCount < 500)
            {//数量不足500个，从词霸热词中获取

            }

            if (resultFindKeyword.Count < 20 && itemOnline.categroy_name != "其他" && itemOnline.categroy_name != "其它")
            {//无法分词，核心词可能有误，直接使用类目名称来组词
                lstMainWord.Clear();
                lstMainWord = itemOnline.categroy_name.Split('/').ToList();
                resultFindKeyword = CommonHandler.CombineWord(itemOnline, lstMainWord, lstSpiderFindWord.Union(lstTitleWord).Union(lstCategoryWord).ToList());
            }

            if (resultFindKeyword.Count < 20)
            {//找词失败，发送告警邮件 
                exchangeName = "ex_taobao_task_email";
                EntityMail mail = new EntityMail() { mail_to = "276504395@qq.com", mail_subject = "找词失败", create_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                mail.mail_body = string.Format("{0},{1},{2}", itemOnline.item_id, itemOnline.item_title, itemOnline.nick);
                BusinessMQ.SendMsgToExchange(session, exchangeName, DynamicJsonParser.FromObject(mail));
            }

            //设置缓存 
            CommonHandler.SetUserItemFindKeywordCache(session.SubUserName, itemOnline.item_id, resultFindKeyword);
            logger.InfoFormat("用户：{0}，宝贝：{1}，设置关键词缓存完成，数量：{2}", session.UserName, itemOnline.item_id, resultFindKeyword.Count);

            return resultFindKeyword.ToArray();
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
