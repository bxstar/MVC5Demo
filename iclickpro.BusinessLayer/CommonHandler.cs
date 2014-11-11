using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iclickpro.Model;
using log4net;
using iclickpro.AccessCommon;

namespace iclickpro.BusinessLayer
{
    public class CommonHandler
    {
        /// <summary>
        /// 写log信息
        /// </summary>
        private static ILog logger = log4net.LogManager.GetLogger("Logger");

        #region 缓存时间
        /// <summary>
        /// 缓存3天，以秒计算
        /// </summary>
        public const int constThreeDayAsSecond = 3 * 24 * 3600;

        /// <summary>
        /// 缓存7天，以秒计算
        /// </summary>
        public const int constSevenDayAsSecond = 7 * 24 * 3600;

        /// <summary>
        /// 缓存一个月，以秒计算
        /// </summary>
        public const int constMonthAsSecond = 30 * 24 * 3600; 
        #endregion

        #region 区分男女的一级类目
        /// <summary>
        /// 女性类目
        /// </summary>
        public static string[] constGirlCategorys = { "16", "50006843", "50022517" };
        /// <summary>
        /// 男性类目
        /// </summary>
        public static string[] constBoyCategorys = { "30", "50011740"};
        #endregion
        

        /// <summary>
        /// 缓存获取淘宝词指数和类目预测代理类
        /// </summary>
        private static WService.WebServiceForKeywordForecast wsKeywordForecastProxy = new WService.WebServiceForKeywordForecast();

        /// <summary>
        /// 找词服务代理类，为加快指数获取速度，关键词指数缓存使用长时间过期
        /// </summary>
        private static WService.FindWord.WebServiceForKeywordForecast wsFindWordProxy = new WService.FindWord.WebServiceForKeywordForecast();

        /// <summary>
        /// 分词服务代理类
        /// </summary>
        private static WService.ServiceSplitWord wsSplitWordProxy = new WService.ServiceSplitWord();

        /// <summary>
        /// 线上，获取宝贝信息
        /// </summary>
        public static EntityItem GetItemOnline(string itemIdOrUrl)
        {
            EntityItem result = null;
            try
            {
                string strItem = wsKeywordForecastProxy.GetItemInfoCache(null, itemIdOrUrl);
                result = DynamicJsonParser.ToObject<EntityItem>(strItem);
            }
            catch (Exception se)
            {
                logger.Error("缓存，获取宝贝信息错误", se);
                return null;
            }
            return result;
        }

        /// <summary>
        /// 获取关键词的指数以及最相关的类目信息，从WebService中获取
        /// </summary>
        /// <param name="lstword">需要预测的关键词列表</param>
        public static List<EntityWordData> GetkeywordIndexFromWs(List<string> lstword)
        {
            //存储结果集
            var resultLst = new List<EntityWordData>();

            string strKeywords = string.Join(",", lstword.ToArray());

            WService.FindWord.EntityWordBase[] arrWordBase = wsFindWordProxy.KeywordBaseLongTimeCache(strKeywords);
            if (arrWordBase == null)
            {
                return null;
            }
            int intWordIndex = 0;
            foreach (var wordBase in arrWordBase)
            {
                EntityWordData itemWord = new EntityWordData();
                if (wordBase.reord_base != null && wordBase.reord_base.Count() > 0)
                {
                    itemWord.id = Convert.ToString(++intWordIndex);
                    itemWord.word = wordBase.word;
                    var wordIndex = wordBase.reord_base[0];

                    itemWord.impressions = wordIndex.impression;
                    itemWord.click = wordIndex.click;
                    itemWord.ctr = string.IsNullOrEmpty(wordIndex.ctr) ? 0 : Convert.ToDouble(wordIndex.ctr);
                    itemWord.cpc = wordIndex.avg_price;
                    itemWord.competition = wordIndex.competition;

                    itemWord.cost = wordIndex.cost;
                    itemWord.directpay = wordIndex.directpay;
                    itemWord.directpaycount = wordIndex.directpaycount;
                    itemWord.favitemcount = wordIndex.favitemcount;
                    itemWord.favshopcount = wordIndex.favshopcount;
                    itemWord.indirectpay = wordIndex.indirectpay;
                    itemWord.indirectpaycount = wordIndex.indirectpaycount;
                    itemWord.rate = string.IsNullOrEmpty(wordIndex.rate) ? 0 : Convert.ToDouble(wordIndex.rate);
                    itemWord.roi = string.IsNullOrEmpty(wordIndex.roi) ? 0 : Convert.ToDouble(wordIndex.roi);
                    itemWord.totalfavcount = wordIndex.totalfavcount;
                    itemWord.totalpay = wordIndex.totalpay;
                    itemWord.totalpaycount = wordIndex.totalpaycount;
                    resultLst.Add(itemWord);

                }
            }

            return resultLst;
        }


        /// <summary>
        /// 获取关键词的指数以及最相关的类目信息，从WebService中获取
        /// </summary>
        /// <param name="itemTitle">宝贝标题，用来计算关键词的相似度，权重0.3</param>
        /// <param name="strMainWord">核心词，用来计算关键词的相似度，权重1</param>
        /// <param name="lstword">需要预测的关键词列表ID</param>
        public static List<EntityWordData> GetkeywordIndexFromWs(string itemTitle, string strMainWord, List<string> lstword)
        {
            //存储结果集
            var resultLst = new List<EntityWordData>();

            string strKeywords = string.Join(",", lstword.ToArray());

            WService.FindWord.EntityWordBase[] arrWordBase = wsFindWordProxy.KeywordBaseLongTimeCache(strKeywords);
            if (arrWordBase == null)
            {
                return null;
            }

            int intWordIndex = 0;
            foreach (var wordBase in arrWordBase)
            {
                EntityWordData itemWord = new EntityWordData();
                if (wordBase.reord_base != null && wordBase.reord_base.Count() > 0)
                {
                    itemWord.id = Convert.ToString(++intWordIndex);
                    itemWord.word = wordBase.word;
                    var wordIndex = wordBase.reord_base[0];

                    itemWord.impressions = wordIndex.impression;
                    itemWord.click = wordIndex.click;
                    itemWord.ctr = string.IsNullOrEmpty(wordIndex.ctr) ? 0 : Convert.ToDouble(wordIndex.ctr);
                    itemWord.cpc = wordIndex.avg_price;
                    itemWord.competition = wordIndex.competition;

                    itemWord.cost = wordIndex.cost;
                    itemWord.directpay = wordIndex.directpay;
                    itemWord.directpaycount = wordIndex.directpaycount;
                    itemWord.favitemcount = wordIndex.favitemcount;
                    itemWord.favshopcount = wordIndex.favshopcount;
                    itemWord.indirectpay = wordIndex.indirectpay;
                    itemWord.indirectpaycount = wordIndex.indirectpaycount;
                    itemWord.rate = string.IsNullOrEmpty(wordIndex.rate) ? 0 : Convert.ToDouble(wordIndex.rate);
                    itemWord.roi = string.IsNullOrEmpty(wordIndex.roi) ? 0 : Convert.ToDouble(wordIndex.roi);
                    itemWord.totalfavcount = wordIndex.totalfavcount;
                    itemWord.totalpay = wordIndex.totalpay;
                    itemWord.totalpaycount = wordIndex.totalpaycount;

                    //关键词相似度
                    char[] arrItemWord = itemWord.word.ToCharArray();
                    double matchDegree = strMainWord.ToCharArray().Intersect(arrItemWord).Count() / strMainWord.Length * 1.0D
                                            + 0.1D * (itemTitle.ToCharArray().Intersect(arrItemWord).Count());
                    itemWord.similar = Math.Round(matchDegree, 2);

                    resultLst.Add(itemWord);
                }
            }

            return resultLst.OrderByDescending(o => o.impressions).OrderByDescending(o => Convert.ToDecimal(o.similar)).ToList();
        }

        /// <summary>
        /// 组词，返回词指数
        /// </summary>
        /// <param name="itemTitle">宝贝标题</param>
        /// <param name="lstMainWord">核心词</param>
        /// <param name="lstOtherWord">属性词或类目热词</param>
        public static List<EntityWordData> CombineWord(EntityItem itemOnline, List<string> lstMainWord, List<string> lstOtherWord)
        {
            //关键词字典：键关键词，值权重
            Dictionary<string, int> dicResult = new Dictionary<string, int>();

            //属性词+核心词
            foreach (var itemMainWord in lstMainWord)
            {
                foreach (var itemOtherWord in lstOtherWord)
                {
                    if (itemOtherWord != itemMainWord)
                    {
                        if (!dicResult.ContainsKey(itemOtherWord + itemMainWord))
                        {
                            dicResult.Add(itemOtherWord + itemMainWord, 10);
                        }
                    }
                }

                //核心词
                if (!dicResult.ContainsKey(itemMainWord))
                {
                    dicResult.Add(itemMainWord, 8);
                }
            }



            //核心词，属性词+核心词，淘宝拓展
            string strKeywordTopExtend = string.Join(",", dicResult.Select(o => o.Key));
            string strExtendWords = GetRelatedwordsByKeywordCache(strKeywordTopExtend, 1);
            if (!string.IsNullOrEmpty(strExtendWords))
            {
                string[] arrRelWord = strExtendWords.Split(',');
                foreach (var itemRelWord in arrRelWord)
                {
                    if (!dicResult.ContainsKey(itemRelWord))
                    {
                        dicResult.Add(itemRelWord, 7);
                    }
                }
            }

            //TODO淘宝拓词后，没有核心词的词可以和核心词组词
            //TODO标题词的分词，可以用属性词+属性词+核心词组

            int wordCount = dicResult.Count;
            logger.InfoFormat("组词总数量：{0}", wordCount);

            List<EntityWordData> lstKeywordWithIndex = GetkeywordIndexFromWs(itemOnline.item_title, string.Join("", lstMainWord), dicResult.Select(o => o.Key).Distinct().ToList());
            
            string parentCatId = string.Empty;
            if (string.IsNullOrEmpty(itemOnline.catpathid))
            {//获取宝贝类目所属的顶级类目，用于判断是否需要进行性别区分
                
                try
                {
                    string strResponse = wsKeywordForecastProxy.GetCatsFullInfoOnline(null, "1", itemOnline.cid.ToString());
                    if (!string.IsNullOrEmpty(strResponse))
                    {
                        List<dynamic> lstCatFullInfo = DynamicJsonParser.ToObject<List<dynamic>>(strResponse);
                        if (lstCatFullInfo != null && lstCatFullInfo.Count > 0)
                        {
                            string strCatPathId = lstCatFullInfo[0]["catpathid"].ToString();
                            parentCatId = strCatPathId.Trim().Replace(" ", ",").Split(',')[0];
                        }
                    }
                }
                catch (Exception se)
                {
                    logger.Error(string.Format("获取顶级类目错误，宝贝id：{0}，类目id：{1}", itemOnline.item_id, itemOnline.cid), se);
                }
            }
            else
            {
                parentCatId = itemOnline.catpathid.Replace(" ", ",").Split(',')[0];
            }

            if (lstKeywordWithIndex != null && lstKeywordWithIndex.Count > 0)
                return FilterSpecialWord(lstKeywordWithIndex, itemOnline.item_title, parentCatId);
            else
                return new List<EntityWordData>();

        }

        /// <summary>
        /// 过滤特殊词（男女性别过滤，纯数字过滤，单个字符过滤）
        /// </summary>
        private static List<EntityWordData> FilterSpecialWord(List<EntityWordData> lstKeyword, string title, string parentCatId)
        {
            TypeKeywordSex sexType = TypeKeywordSex.None;
            // 宝贝标题中含有"男","女",则关键词需要区分男女
            if (title.Contains("男") && !title.Contains("女"))
            {
                sexType = TypeKeywordSex.Male;
            }
            if (title.Contains("女") && !title.Contains("男"))
            {
                sexType = TypeKeywordSex.Female;
            }
            // 男性类目，则关键词需要区分男女
            if (constBoyCategorys.Contains(parentCatId))
            {
                sexType = TypeKeywordSex.Male;
            }
            // 女性类目，则关键词需要区分男女
            if (constGirlCategorys.Contains(parentCatId))
            {
                sexType = TypeKeywordSex.Female;
            }

            if (sexType != TypeKeywordSex.None)
            {//需要区分男女
                // 存储过滤后的词
                List<EntityWordData> listFilterWord = new List<EntityWordData>();
                foreach (var strkeyword in lstKeyword)
                {
                    if (sexType == TypeKeywordSex.Male && !strkeyword.word.Contains("女"))
                    {
                        listFilterWord.Add(strkeyword);
                    }
                    if (sexType == TypeKeywordSex.Female && !strkeyword.word.Contains("男"))
                    {
                        listFilterWord.Add(strkeyword);
                    }
                }
                // 过滤后重新赋值给结果变量
                lstKeyword = listFilterWord;
            }

            //过滤纯数字和字符长度为1的英文字符串
            lstKeyword = lstKeyword.Where(o => !Strings.IsShuZhi(o.word) && !Strings.IsOneEnglishChar(o.word)).ToList();

            return lstKeyword;
        }

        /// <summary>
        /// 线上，类目TOP100关键词，旧接口已废弃
        /// </summary>
        public static List<string> GetCatTop100Keyword(long catId)
        {
            List<string> result = null;
            try
            {
                string strItem = wsKeywordForecastProxy.GetCatTop100Keyword(null,catId.ToString());
                result = strItem.Split(',').ToList();
            }
            catch (Exception se)
            {
                logger.Error("缓存，获取类目TOP100关键词错误", se);
                return null;
            }
            return result;
        }

        /// <summary>
        /// 线上，类目下热门词，替代GetCatTop100Keyword
        /// </summary>
        public static List<string> GetCatTopKeyword(long catId)
        {
            List<string> result = null;
            try
            {
                string strItem = wsKeywordForecastProxy.GetCatTopKeyword(null, catId.ToString());
                result = strItem.Split(',').ToList();
            }
            catch (Exception se)
            {
                logger.Error("缓存，获取类目下热门词错误", se);
                return null;
            }
            return result;
        }

        /// <summary>
        /// 缓存，获取宝贝的蜘蛛任务找词结果
        /// </summary>
        public static string GetItemFindKeywordCache(long itemId)
        {
            string result = null;
            string cacheKey = string.Format("top_findkeyword_itemid_{0}", itemId);
            try
            {
                result = wsFindWordProxy.GetValue(cacheKey);
            }
            catch (Exception se)
            {
                logger.Error("缓存，获取宝贝的蜘蛛找词结果错误", se);
            }
            return result;
        }

        /// <summary>
        /// 缓存，获取用户店铺下所有的宝贝
        /// </summary>
        public static List<EntityItem> GetUserOnlineItemCache(string nick)
        {
            List<EntityItem> result = null;
            string cacheKey = string.Format("top_onlineitems_nick_{0}", nick);
            try
            {
                string cacheValue = wsFindWordProxy.GetValueTimeOut(cacheKey);
                if (cacheValue != null)
                    result = DynamicJsonParser.ToObject<List<EntityItem>>(CommonFunction.Decompress(cacheValue));
            }
            catch (Exception se)
            {
                logger.Error(string.Format("缓存，获取{0}的宝贝失败", nick), se);
            }
            return result;
        }

        /// <summary>
        /// 缓存，设置用户店铺下所有的宝贝
        /// </summary>
        public static Boolean SetUserOnlineItemCache(string nick, List<EntityItem> lstItem)
        {
            string cacheKey = string.Format("top_onlineitems_nick_{0}", nick);
            try
            {
                string cacheValue = CommonFunction.Compress(DynamicJsonParser.FromObject(lstItem));
                return wsFindWordProxy.SetValueTimeOut(cacheKey, cacheValue, constMonthAsSecond);
            }
            catch (Exception se)
            {
                logger.Error(string.Format("缓存，设置{0}的宝贝失败", nick), se);
            }
            return false;
        }

        /// <summary>
        /// 缓存，获取用户宝贝找词的结果
        /// </summary>
        public static List<EntityWordData> GetUserItemFindKeywordCache(string nick, long itemId)
        {
            List<EntityWordData> result = null;
            string cacheKey = string.Format("top_findkeyword_itemid_{0}_nick_{1}", itemId, nick);
            try
            {
                string cacheValue = wsFindWordProxy.GetValueTimeOut(cacheKey);
                if (cacheValue != null)
                    result = DynamicJsonParser.ToObject<List<EntityWordData>>(CommonFunction.Decompress(cacheValue));
            }
            catch (Exception se)
            {
                logger.Error(string.Format("缓存，获取宝贝：{0}，用户：{1}找词结果失败", itemId, nick), se);
            }
            return result;
        }

        /// <summary>
        /// 缓存，设置用户宝贝找词的结果，（时长：默认3天）
        /// </summary>
        public static Boolean SetUserItemFindKeywordCache(string nick, long itemId, List<EntityWordData> findKewordResult)
        {
            string cacheKey = string.Format("top_findkeyword_itemid_{0}_nick_{1}", itemId, nick);
            try
            {
                //findKewordResult.wd = null; //由于数据太多暂时不放进缓存
                string cacheValue = CommonFunction.Compress(DynamicJsonParser.FromObject(findKewordResult));
                return wsFindWordProxy.SetValueTimeOut(cacheKey, cacheValue, constThreeDayAsSecond);
            }
            catch (Exception se)
            {
                logger.Error(string.Format("缓存，设置宝贝：{0}，用户：{1}找词结果失败", itemId, nick), se);
            }
            return false;
        }

        /// <summary>
        /// 缓存，设置用户宝贝找词的结果（时长：7天）
        /// </summary>
        public static Boolean SetUserItemFindKeywordCacheWeek(string nick, long itemId, List<EntityWordData> findKewordResult)
        {
            string cacheKey = string.Format("top_findkeyword_itemid_{0}_nick_{1}", itemId, nick);
            try
            {
                //findKewordResult.wd = null; //由于数据太多暂时不放进缓存
                string cacheValue = CommonFunction.Compress(DynamicJsonParser.FromObject(findKewordResult));
                return wsFindWordProxy.SetValueTimeOut(cacheKey, cacheValue, constSevenDayAsSecond);
            }
            catch (Exception se)
            {
                logger.Error(string.Format("缓存，设置宝贝：{0}，用户：{1}找词结果失败", itemId, nick), se);
            }
            return false;
        }

        /// <summary>
        /// 缓存，获取关键词的相关词
        /// </summary>
        public static string GetRelatedwordsByKeywordCache(string keywords,int depth)
        {
            string result = null;
            try
            {
                result = wsFindWordProxy.GetRelatedwordsByKeyword(null, keywords, depth);
            }
            catch (Exception se)
            {
                logger.Error(string.Format("缓存，获取关键词：{0}的相关词失败", keywords), se);
            }
            return result;
        }

        /// <summary>
        /// 缓存，获取用户上架在线销售的全部宝贝，默认从缓存那数据
        /// </summary>
        public static List<EntityItem> GetUserOnlineItems(EntityUser session, Boolean isFromCache = true)
        {
            List<EntityItem> lstItem = null;
            try
            {
                string strResult = wsFindWordProxy.GetUserOnlineItems(null, session.fSubUserName, session.fSession, "tkc", isFromCache);
                if (strResult != null)
                {
                    lstItem = DynamicJsonParser.ToObject<List<EntityItem>>(CommonFunction.Decompress(strResult));
                }
            }
            catch (Exception se)
            {
                logger.Error("缓存，获取类目下热门词错误", se);
                return null;
            }
            return lstItem;
        }

        /// <summary>
        /// 缓存，根据类目ID获取类目信息
        /// </summary>
        public static List<Top.Api.Domain.InsightCategoryInfoDTO> GetCatsFullInfoOnline(string categoryIds)
        {
            if (string.IsNullOrEmpty(categoryIds))
                return new List<Top.Api.Domain.InsightCategoryInfoDTO>();
            List<Top.Api.Domain.InsightCategoryInfoDTO> lstResult = null;
            try
            {
                string strResponse = wsKeywordForecastProxy.GetCatsFullInfoOnline(null, "1", categoryIds);
                lstResult = DynamicJsonParser.ToObject<List<Top.Api.Domain.InsightCategoryInfoDTO>>(strResponse);
            }
            catch (Exception se)
            {
                logger.Error(string.Format("缓存，获取类目id：{0}的类目信息失败", categoryIds), se);
            }
            return lstResult;
        }

        /// <summary>
        /// 分词
        /// </summary>
        public static string SplitWordFromWs(string keywords)
        {
            string result = null;
            try
            {
                result = wsSplitWordProxy.SplitWordPanGu(keywords);
            }
            catch (Exception se)
            {
                logger.Error("分词调用错误", se);
            }
            return result;
        }

        /// <summary>
        /// 调用淘宝API出错后，返回结果是否是频繁访问
        /// </summary>
        public static Boolean IsBanMsg(Top.Api.TopResponse response)
        {
            if (response.ErrMsg == null || response.SubErrMsg == null)
            {
                return false;
            }
            if (response.ErrMsg.Contains("Limited") || response.SubErrMsg.Contains("ban"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 调用淘宝API封装，参数类型string
        /// </summary>
        public static T DoTaoBaoApi<T>(Func<string, T> apiMethod, string strPara, int executionTimeout = 60) where T : Top.Api.TopResponse
        {
            T response = default(T);
            try
            {
                response = apiMethod(strPara);
                DateTime dtStart = DateTime.Now;
                int banCount = 0;
                if (response.IsError)
                {
                    if (CommonHandler.IsBanMsg(response))
                    {//遇到频繁访问的错误，需要多次访问
                        Boolean isBanError = true;
                        while (isBanError)
                        {
                            banCount++;
                            System.Threading.Thread.Sleep(2000 * banCount);
                            response = apiMethod(strPara);
                            if (response.IsError && IsBanMsg(response) && dtStart.AddSeconds(executionTimeout) > DateTime.Now)
                            {//默认，超过1分钟放弃
                                isBanError = true;
                            }
                            else
                            {
                                if (dtStart.AddSeconds(executionTimeout) <= DateTime.Now)
                                {
                                    return response;
                                }
                                isBanError = false;
                            }
                        }
                    }
                    else
                    {
                        return response;
                    }
                }
            }
            catch (Exception se1)
            {
                logger.Error(string.Format("taobao api 1次出错:{0}", strPara), se1);
                System.Threading.Thread.Sleep(100);
                try
                {
                    response = apiMethod(strPara);
                }
                catch (Exception se2)
                {
                    logger.Error(string.Format("taobao api 2次出错:{0}", strPara), se2);
                    System.Threading.Thread.Sleep(200);
                    try
                    {
                        response = apiMethod(strPara);
                    }
                    catch (Exception se3)
                    {
                        logger.Error(string.Format("taobao api 3次出错:{0}", strPara), se3);
                        return null;
                    }
                }
            }

            return response;
        }


        /// <summary>
        /// 调用淘宝API封装，参数类型EntityUser
        /// </summary>
        public static T DoTaoBaoApi<T>(Func<EntityUser, T> apiMethod, EntityUser user, int executionTimeout = 60) where T : Top.Api.TopResponse
        {
            T response = default(T);
            try
            {
                response = apiMethod(user);
                DateTime dtStart = DateTime.Now;
                int banCount = 0;
                if (response.IsError)
                {
                    if (CommonHandler.IsBanMsg(response))
                    {//遇到频繁访问的错误，需要多次访问
                        Boolean isBanError = true;
                        while (isBanError)
                        {
                            banCount++;
                            System.Threading.Thread.Sleep(2000 * banCount);
                            response = apiMethod(user);
                            if (response.IsError && IsBanMsg(response) && dtStart.AddSeconds(executionTimeout) > DateTime.Now)
                            {//默认，超过1分钟放弃
                                isBanError = true;
                            }
                            else
                            {
                                if (dtStart.AddSeconds(executionTimeout) <= DateTime.Now)
                                {
                                    return response;
                                }
                                isBanError = false;
                            }
                        }
                    }
                    else
                    {
                        return response;
                    }
                }
            }
            catch (Exception se1)
            {
                logger.Error(string.Format("taobao api 1次出错:{0}", user.fSession), se1);
                System.Threading.Thread.Sleep(100);
                try
                {
                    response = apiMethod(user);
                }
                catch (Exception se2)
                {
                    logger.Error(string.Format("taobao api 2次出错:{0}", user.fSession), se2);
                    System.Threading.Thread.Sleep(200);
                    try
                    {
                        response = apiMethod(user);
                    }
                    catch (Exception se3)
                    {
                        logger.Error(string.Format("taobao api 3次出错:{0}", user.fSession), se3);
                        return null;
                    }
                }
            }

            return response;
        }


        /// <summary>
        /// 调用淘宝API封装，参数类型EntityUser,long,String,String
        /// </summary>
        public static T DoTaoBaoApi<T>(Func<EntityUser, long, string, string, T> apiMethod, EntityUser user, long longPara1, string strPara2, string strPara3, int reDoTimes = 0, int executionTimeout = 60) where T : Top.Api.TopResponse
        {
            T response = default(T);
            try
            {
                response = apiMethod(user, longPara1, strPara2, strPara3);
                DateTime dtStart = DateTime.Now;
                int banCount = 0;
                if (response.IsError)
                {
                    if (CommonHandler.IsBanMsg(response))
                    {//遇到频繁访问的错误，需要多次访问
                        Boolean isBanError = true;
                        while (isBanError)
                        {
                            banCount++;
                            System.Threading.Thread.Sleep(2000 * banCount);
                            response = apiMethod(user, longPara1, strPara2, strPara3);
                            if (response.IsError && IsBanMsg(response) && dtStart.AddSeconds(executionTimeout) > DateTime.Now)
                            {//默认，超过1分钟放弃
                                isBanError = true;
                            }
                            else
                            {
                                if (dtStart.AddSeconds(executionTimeout) <= DateTime.Now)
                                {
                                    return response;
                                }
                                isBanError = false;
                            }
                        }
                    }
                    else if (reDoTimes > 0)
                    {//遇到一般性错误重试
                        int times = 1;
                        while (response.IsError && times <= reDoTimes)
                        {
                            times++;
                            System.Threading.Thread.Sleep(300);
                            response = apiMethod(user, longPara1, strPara2, strPara3);
                        }
                    }
                    else
                    {
                        return response;
                    }
                }            
            }
            catch (Exception se1)
            {
                logger.Error(string.Format("taobao api 1次出错:{0},{1},{2},{3}", user.fSession, longPara1, strPara2, strPara3), se1);
                System.Threading.Thread.Sleep(100);
                try
                {
                    response = apiMethod(user, longPara1, strPara2, strPara3);
                }
                catch (Exception se2)
                {
                    logger.Error(string.Format("taobao api 2次出错:{0},{1},{2},{3}", user.fSession, longPara1, strPara2, strPara3), se2);
                    System.Threading.Thread.Sleep(200);
                    try
                    {
                        response = apiMethod(user, longPara1, strPara2, strPara3);
                    }
                    catch (Exception se3)
                    {
                        logger.Error(string.Format("taobao api 3次出错:{0},{1},{2},{3}", user.fSession, longPara1, strPara2, strPara3), se3);
                        return null;
                    }
                }
            }

            return response;
        }


        /// <summary>
        /// 调用淘宝API封装，参数类型EntityUser,String,Boolean,Int,Int
        /// </summary>
        public static T DoTaoBaoApi<T>(Func<EntityUser, string,Boolean, int, int, T> apiMethod, EntityUser user, string strPara1, Boolean boolPara2, int intPara3,int intPara4, int reDoTimes = 0, int executionTimeout = 60) where T : Top.Api.TopResponse
        {
            T response = default(T);
            try
            {
                response = apiMethod(user, strPara1, boolPara2, intPara3, intPara4);
                DateTime dtStart = DateTime.Now;
                int banCount = 0;
                if (response.IsError)
                {
                    if (CommonHandler.IsBanMsg(response))
                    {//遇到频繁访问的错误，需要多次访问
                        Boolean isBanError = true;
                        while (isBanError)
                        {
                            banCount++;
                            System.Threading.Thread.Sleep(2000 * banCount);
                            response = apiMethod(user, strPara1, boolPara2, intPara3, intPara4);
                            if (response.IsError && IsBanMsg(response) && dtStart.AddSeconds(executionTimeout) > DateTime.Now)
                            {//默认，超过1分钟放弃
                                isBanError = true;
                            }
                            else
                            {
                                if (dtStart.AddSeconds(executionTimeout) <= DateTime.Now)
                                {
                                    return response;
                                }
                                isBanError = false;
                            }
                        }
                    }
                    else if (reDoTimes > 0)
                    {//遇到一般性错误重试
                        int times = 1;
                        while (response.IsError && times <= reDoTimes)
                        {
                            times++;
                            System.Threading.Thread.Sleep(300);
                            response = apiMethod(user, strPara1, boolPara2, intPara3, intPara4);
                        }
                    }
                    else
                    {
                        return response;
                    }
                }
            }
            catch (Exception se1)
            {
                logger.Error(string.Format("taobao api 1次出错:{0},{1},{2},{3},{4}", user.fSession, strPara1, boolPara2, intPara3, intPara4), se1);
                System.Threading.Thread.Sleep(100);
                try
                {
                    response = apiMethod(user, strPara1, boolPara2, intPara3, intPara4);
                }
                catch (Exception se2)
                {
                    logger.Error(string.Format("taobao api 2次出错:{0},{1},{2},{3},{4}", user.fSession, strPara1, boolPara2, intPara3, intPara4), se2);
                    System.Threading.Thread.Sleep(200);
                    try
                    {
                        response = apiMethod(user, strPara1, boolPara2, intPara3, intPara4);
                    }
                    catch (Exception se3)
                    {
                        logger.Error(string.Format("taobao api 3次出错:{0},{1},{2},{3},{4}", user.fSession, strPara1, boolPara2, intPara3, intPara4), se3);
                        return null;
                    }
                }
            }

            return response;
        }

    }
}
