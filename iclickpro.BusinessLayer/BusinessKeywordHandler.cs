using iclickpro.AccessCommon;
using iclickpro.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iclickpro.BusinessLayer
{
    /// <summary>
    /// 关键词业务处理类
    /// </summary>
    public class BusinessKeywordHandler
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


        public List<EntityWordData> GetItemKeywords(EntityUser user, string itemIdOrUrl)
        {
            //获取计划信息
            if (user == null) return null;

            EntityItem itemOnline = CommonHandler.GetItemOnline(itemIdOrUrl);
            if (itemOnline == null || itemOnline.item_id == 0)
            {
                return new List<EntityWordData>(); 
            }
            else
            {
                logger.InfoFormat("用户：{0}，宝贝：{1}，标题：{2}", user.fUserName, itemOnline.item_id, itemOnline.item_title);
            }

            //缓存获取找词的数量
            List<EntityWordData> resultFindKeyword = CommonHandler.GetUserItemFindKeywordCache(user.fSubUserName, itemOnline.item_id);
            if (resultFindKeyword != null && resultFindKeyword.Count() > 100)
            {
                logger.InfoFormat("用户：{0}，宝贝：{1}，缓存取词", user.fUserName, itemOnline.item_id);
                return resultFindKeyword;
            }

            //重新找词
            logger.InfoFormat("用户：{0}，宝贝：{1}，开始找词", user.fUserName, itemOnline.item_id);
            resultFindKeyword = new List<EntityWordData>();

            //发送蜘蛛抓词任务
            string exchangeName = "ex_taobao_spider_samesimilar_item";
            BusinessMQ.SendMsgToExchange(user, exchangeName, string.Format("{0},{1},{2}", itemOnline.item_id, itemOnline.item_title, itemOnline.nick));

            //更新宝贝的信息

            logger.InfoFormat("用户：{0}，宝贝：{1}，获取类目热词开始", user.fUserName, itemOnline.item_id);
            List<string> lstCategoryWord = CommonHandler.GetCatTopKeyword(itemOnline.cid);
            logger.InfoFormat("用户：{0}，宝贝：{1}，获取类目热词完成，数量：{2}", user.fUserName, itemOnline.item_id, lstCategoryWord.Count);

            logger.InfoFormat("用户：{0}，宝贝：{1}，获取关键词开始", user.fUserName, itemOnline.item_id);
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

            logger.InfoFormat("用户：{0}，宝贝：{1}，获取关键词完成，核心词：{2}，数量：{3}", user.fUserName, itemOnline.item_id, mainWord, lstSpiderFindWord.Count);

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
                BusinessMQ.SendMsgToExchange(user, exchangeName, DynamicJsonParser.FromObject(mail));
            }

            //设置缓存 
            CommonHandler.SetUserItemFindKeywordCache(user.fSubUserName, itemOnline.item_id, resultFindKeyword);
            logger.InfoFormat("用户：{0}，宝贝：{1}，设置关键词缓存完成，数量：{2}", user.fUserName, itemOnline.item_id, resultFindKeyword.Count);

            return resultFindKeyword;
        }

    }
}
