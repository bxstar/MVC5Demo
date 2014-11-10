using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Top.Api;
using Top.Api.Response;
using Top.Api.Request;
using iclickpro.Model;
using iclickpro.AccessCommon;

namespace iclickpro.BusinessLayer
{
    public partial class BusinessTaobaoApiHandler
    {
        /// <summary>
        /// TopAPI
        /// </summary>
        private ITopClient _client = new DefaultTopClient(Constants.C_Url, CommonFunction.GetAppSetting("AppKey"), CommonFunction.GetAppSetting("AppSecret"), "json");


        /// <summary>
        /// 取得用户店铺基本信息
        /// </summary>
        /// <param name="strkeywords">关键词</param>
        /// <returns>取得类目</returns>
        public ShopGetResponse TaobaoShopGet(EntityUser session, string fields)
        {

            var req = new ShopGetRequest { Nick = session.SubUserName, Fields = fields };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 查询订购关系
        /// </summary>
        /// <param name="strtypeids">用户选择的类别编号</param>
        /// <returns></returns>
        public VasSubscribeGetResponse TaobaoSimbaSvaGet(EntityUser session)
        {
            var req = new VasSubscribeGetRequest { Nick = session.SubUserName, ArticleCode = "ts-25420" };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.tools.items.top.get 取得一个关键词的推广组排名列表 
        /// </summary>
        /// <param name="strWords">关键词</param>
        /// <param name="ip">IP</param>
        /// <returns></returns>
        private SimbaToolsItemsTopGetResponse TaobaoSimbaToolsItemsTopGet(EntityUser session, string strWords, string ip)
        {
            var req = new SimbaToolsItemsTopGetRequest { Nick = session.SubUserName, Keyword = strWords, Ip = ip };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 取得单个产品信息
        /// </summary>
        /// <returns></returns>
        public ItemGetResponse TaobaoItemGet(long numIid)
        {
            var req = new ItemGetRequest { Fields = "num_iid,nick,title,price,cid,item_img,props_name,pic_url", NumIid = numIid };
            var response = _client.Execute(req, "");
            return response;
        }

        /// <summary>
        /// 批量获取产品属性
        /// </summary>
        /// <returns></returns>
        public ItemsListGetResponse TaobaoAttrGet(string numIids)
        {
            var req = new ItemsListGetRequest { Fields = "num_iid,nick,title,price,cid,item_img,props_name,pic_url", NumIids = numIids };
            var response = _client.Execute(req, "");
            return response;
        }

        /// <summary>
        /// 类目的取得
        /// </summary>
        /// <param name="cid">属性编号</param>
        /// <returns>类目</returns>
        private ItemcatsGetResponse TaobaoItemcatsGet(long cid)
        {
            var req = new ItemcatsGetRequest { Fields = "cid,parent_cid,name,is_parent", Cids = cid.ToString() + "," };
            var response = _client.Execute(req);
            return response;
        }

        /// <summary>
        /// taobao.simba.adgroup.onlineitemsvon.get 获取用户上架在线销售的全部宝贝 
        /// </summary>
        /// <returns></returns>
        public SimbaAdgroupOnlineitemsvonGetResponse TaobaoSimbaAdgroupOnlineitemsvonGet(EntityUser session, string orderField, bool orderBy, int pageNo, int pageSize)
        {
            var req = new SimbaAdgroupOnlineitemsvonGetRequest
            {
                Nick = session.SubUserName,
                OrderField = orderField,
                OrderBy = orderBy,
                PageNo = pageNo,
                PageSize = pageSize
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }




        /// <summary>
        /// taobao.simba.campaigns.get 取得一组推广计划 
        /// </summary>
        /// <returns></returns>
        public SimbaCampaignsGetResponse TaobaoSimbaCampaignsGet(EntityUser session)
        {
            var req = new SimbaCampaignsGetRequest { Nick = session.SubUserName };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }


        /// <summary>
        /// taobao.simba.adgroups.get 批量得到推广组 
        /// </summary>
        /// <param name="campaignId">推广计划编号</param>
        /// <param name="pageSize">页面行数</param>
        /// <param name="pageNo">当前页</param>
        /// <returns></returns>
        //public SimbaAdgroupsGetResponse TaobaoSimbaAdgroupsGet(EntityUser session, long campaignId, int pageSize, int pageNo)
        //{
        //    var req = new SimbaAdgroupsGetRequest
        //                  {

        //                      Nick = session.SubUserName,
        //                      CampaignId = campaignId,
        //                      PageSize = pageSize,
        //                      PageNo = pageNo
        //                  };
        //    var response = _client.Execute(req, session.TopSessions);
        //    return response;
        //}


        public SimbaAdgroupsbycampaignidGetResponse TaobaoSimbaAdgroupsbycampaignidGet(EntityUser session, long campaignId, int pageSize, int pageNo)
        {
            var req = new SimbaAdgroupsbycampaignidGetRequest
            {

                Nick = session.SubUserName,
                CampaignId = campaignId,
                PageSize = pageSize,
                PageNo = pageNo
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }


        /// <summary>
        /// 取得一个推广组的所有关键词或者根据一个关键词Id列表取得一组关键词
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adgroupId"></param>
        /// <param name="keywordIds"></param>
        /// <returns></returns>
        public SimbaKeywordsbykeywordidsGetResponse TaobaoSimbaKeywordsbykeywordidsGet(EntityUser session, string keywordIds)
        {
            var req = new SimbaKeywordsbykeywordidsGetRequest
            {
                Nick = session.SubUserName,
                KeywordIds = keywordIds
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }


        public SimbaKeywordsbyadgroupidGetResponse TaobaoSimbaKeywordsbyadgroupidGet(EntityUser session, long adgroupId)
        {
            SimbaKeywordsbyadgroupidGetResponse response = null;
            try
            {
                var req = new SimbaKeywordsbyadgroupidGetRequest
                {
                    Nick = session.SubUserName,
                    AdgroupId = adgroupId
                };
                response = _client.Execute(req, session.TopSessions);
            }
            catch
            {

            }
            return response;

        }



        /// <summary>
        /// taobao.simba.adgroups.item.exist 商品是否推广 
        /// </summary>
        /// <returns></returns>
        private SimbaAdgroupsItemExistResponse TaobaoSimbaAdgroupsItemExist(EntityUser session, long campaignId, long itemId)
        {
            var req = new SimbaAdgroupsItemExistRequest
            {
                Nick = session.SubUserName,
                CampaignId = campaignId,
                ItemId = itemId
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 验证权限
        /// </summary>
        /// <returns></returns>
        public SimbaCustomersAuthorizedGetResponse TaobaoSimbaCustomersAuthorizedGet(EntityUser session)
        {
            var req = new SimbaCustomersAuthorizedGetRequest();
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 下载宝贝地下的关键词
        /// </summary>
        /// <param name="adGroupId"></param>
        /// <returns></returns>
        //public SimbaKeywordsGetResponse TaoBaoSimbaKeywordsGet(EntityUser session, long adGroupId)
        //{
        //    var req = new SimbaKeywordsGetRequest { Nick = session.SubUserName, AdgroupId = adGroupId };
        //    var response = _client.Execute(req, session.TopSessions);
        //    return response;
        //}

        /// <summary>
        /// 取得报表的基础数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="campaignId"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="source"></param>
        /// <param name="pageNum"></param>
        /// <param name="search_type"></param>
        /// <returns></returns>
        public SimbaRptCampadgroupbaseGetResponse TaobaoSimbaRptCampadgroupbaseGet(EntityUser session, long campaignId, string starttime, string endtime, string source, int pageNum, string search_type)
        {
            string subtoken = GetSubwayToken(session);
            var req = new SimbaRptCampadgroupbaseGetRequest { SubwayToken = subtoken, Nick = session.SubUserName, CampaignId = campaignId, StartTime = starttime, EndTime = endtime, Source = source, PageNo = pageNum, PageSize = 500, SearchType = search_type };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 取得推广组报表的基础数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="campaignId"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="source"></param>
        /// <param name="pageNum"></param>
        /// <param name="search_type"></param>
        /// <returns></returns>
        public SimbaRptAdgroupbaseGetResponse TaobaoSimbaRptAdgroupbaseGet(EntityUser session, long campaignId, long adgroupId, string starttime, string endtime)
        {
            string subtoken = GetSubwayToken(session);
            var req = new SimbaRptAdgroupbaseGetRequest { SubwayToken = subtoken, Nick = session.SubUserName, CampaignId = campaignId, AdgroupId = adgroupId, StartTime = starttime, EndTime = endtime, Source = "1,2", PageNo = 1, PageSize = 500, SearchType = "SUMMARY" };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }


        /// <summary>
        /// 取得推广组效果报表的数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="campaignId"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="source"></param>
        /// <param name="pageNum"></param>
        /// <param name="search_type"></param>
        /// <returns></returns>
        public SimbaRptAdgroupeffectGetResponse TaobaoSimbaRptAdgroupeffectGet(EntityUser session, long campaignId, long adgroupId, string starttime, string endtime)
        {
            string subtoken = GetSubwayToken(session);
            var req = new SimbaRptAdgroupeffectGetRequest { SubwayToken = subtoken, Nick = session.SubUserName, CampaignId = campaignId, AdgroupId = adgroupId, StartTime = starttime, EndTime = endtime, Source = "1,2", PageNo = 1, PageSize = 500, SearchType = "SUMMARY" };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 获取系统推荐词
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adGroupId">推广组id</param>
        /// <param name="orderBy">返回结果按照哪个排序，默认可以“search_volume”，详见API在线文档</param>
        /// <param name="pageNo">页号</param>
        /// <param name="pageSize">每次请求返回的个数</param>
        /// <returns></returns>
        public SimbaKeywordsRecommendGetResponse TaobaoSimbaKeywordsRecommendGet(EntityUser session, long adGroupId, string orderBy, int pageNo, int pageSize)
        {
            var req = new SimbaKeywordsRecommendGetRequest { AdgroupId = adGroupId, Nick = session.SubUserName, OrderBy = orderBy, PageNo = pageNo, PageSize = pageSize };
            return _client.Execute(req, session.TopSessions);
        }

        /// <summary>
        /// 取得线上用户的信息
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="nick"></param>
        /// <returns></returns>
        public UserGetResponse GetUserResponse(string session, string nick)
        {
            var req = new UserGetRequest();
            req.Fields = "user_id,nick,seller_credit";
            req.Nick = nick;
            UserGetResponse response = _client.Execute(req, session);
            return response;
        }

        /// <summary>
        /// taobao.simba.keywords.add 创建一批关键词
        /// </summary>
        /// <returns></returns>
        //public SimbaKeywordsvonAddResponse TaobaoSimbaKeywordsAdd(string session, string nick, long adgroupId, string keywordPrices)
        //{
        //    //var req = new SimbaKeywordsAddRequest { Nick = nick, AdgroupId = adgroupId, KeywordPrices = keywordPrices };
        //    //var response = _client.Execute(req, session);
        //    //return response;
        //    var req = new SimbaKeywordsvonAddRequest { Nick = nick, AdgroupId = adgroupId, KeywordPrices = keywordPrices };
        //    var response = _client.Execute(req, session);
        //    return response;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="nick"></param>
        /// <param name="adgroupId"></param>
        /// <param name="keywordPrices"></param>
        /// <returns></returns>
        public SimbaKeywordsvonAddResponse TaobaoSimbaKeywordsvonAdd(string session, string nick, long adgroupId, string keywordPrices)
        {
            var req = new SimbaKeywordsvonAddRequest { Nick = nick, AdgroupId = adgroupId, KeywordPrices = keywordPrices };
            var response = _client.Execute(req, session);
            return response;
        }




        /// <summary>
        /// taobao.simba.keywords.delete 删除一批关键词 
        /// </summary>
        /// <returns></returns>
        public SimbaKeywordsDeleteResponse TaobaoSimbaKeywordsDelete(string session, string nick, long campaignId, string keywordIds)
        {
            var req = new SimbaKeywordsDeleteRequest { Nick = nick, CampaignId = campaignId, KeywordIds = keywordIds };
            var response = _client.Execute(req, session);
            return response;
        }

        /// <summary>
        /// taobao.simba.rpt.adgroupkeywordbase.get 推广组下的词基础报表数据查询(明细数据不分类型查询)
        /// </summary>
        /// <returns></returns>
        public SimbaRptAdgroupkeywordbaseGetResponse TaobaoSimbaRptAdgroupkeywordbaseGet(EntityUser session,
                                                                             string nick,
                                                                             long campaignId,
                                                                             long adgroupId,
                                                                             string startTime,
                                                                             string endTime,
                                                                             string searchType,
                                                                             long pageSize,
                                                                             long pageNo,
                                                                             string source)
        {
            var req = new SimbaRptAdgroupkeywordbaseGetRequest { Nick = nick, SubwayToken = GetSubwayToken(session), CampaignId = campaignId, AdgroupId = adgroupId, StartTime = startTime, EndTime = endTime, PageNo = pageNo, PageSize = pageSize, SearchType = searchType, Source = source };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }



        /// <summary>
        /// 获取报表效果数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="nick"></param>
        /// <param name="subwayToken"></param>
        /// <param name="campaignId"></param>
        /// <param name="adgroupId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="searchType"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public SimbaRptAdgroupkeywordeffectGetResponse TaobaoSimbaRptAdgroupkeywordeffectGet(EntityUser session,
                                                                            string nick,
                                                                            long campaignId,
                                                                            long adgroupId,
                                                                            string startTime,
                                                                            string endTime,
                                                                            string searchType,
                                                                            long pageSize,
                                                                            long pageNo,
                                                                            string source)
        {
            var req = new SimbaRptAdgroupkeywordeffectGetRequest { Nick = nick, SubwayToken = GetSubwayToken(session), CampaignId = campaignId, AdgroupId = adgroupId, StartTime = startTime, EndTime = endTime, PageNo = pageNo, PageSize = pageSize, SearchType = searchType, Source = source };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 获取质量得分
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adgroupId"></param>
        /// <returns></returns>
        public SimbaKeywordsQscoreGetResponse TaobaoSimbaKeywordsQscoreGet(EntityUser session, long adgroupId)
        {
            SimbaKeywordsQscoreGetRequest req = new SimbaKeywordsQscoreGetRequest();
            req.Nick = session.SubUserName;
            req.AdgroupId = adgroupId;
            SimbaKeywordsQscoreGetResponse response = _client.Execute(req, session.TopSessions);
            return response;
        }
        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="session">用户信息</param>
        /// <returns></returns>
        private string GetSubwayToken(EntityUser session)
        {
            SimbaLoginAuthsignGetRequest request = new SimbaLoginAuthsignGetRequest();
            request.Nick = session.SubUserName;
            SimbaLoginAuthsignGetResponse response = _client.Execute(request, session.TopSessions);
            string subwayToken = "";
            if (response.IsError)
            {

            }
            else
            {
                subwayToken = response.SubwayToken;
            }

            return subwayToken;
        }


        /// <summary>
        /// 获取交易记录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public TradesSoldGetResponse TaobaoTradesSoldGet(EntityUser session, string startDate, string endDate, int pageIndex, int pageSize)
        {
            var req = new TradesSoldGetRequest();
            req.Fields = "seller_nick,buyer_nick,title,type,created,tid,seller_rate,buyer_rate,status,payment,discount_fee,adjust_fee,post_fee,total_fee,pay_time,end_time,modified,consign_time,buyer_obtain_point_fee,point_fee,real_point_fee,received_payment,pic_path,num_iid,num,price,cod_fee,cod_status,shipping_type, receiver_name,receiver_state,receiver_city,receiver_district,receiver_address,receiver_zip,receiver_mobile,receiver_phone";
            req.StartCreated = DateTime.Parse(startDate);
            req.EndCreated = DateTime.Parse(endDate);
            req.Status = "TRADE_FINISHED";
            req.RateStatus = "RATE_UNSELLER";
            req.PageNo = pageIndex;
            req.PageSize = pageSize;
            req.UseHasNext = true;
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 预估排名信息
        /// </summary>
        public SimbaKeywordKeywordforecastGetResponse TaobaoSimbaKeywordKeywordforecastGet(EntityUser session, long keywordId, long bidwordPrice)
        {
            var req = new SimbaKeywordKeywordforecastGetRequest();
            req.Nick = session.SubUserName;
            req.KeywordId = keywordId;
            req.BidwordPrice = bidwordPrice;
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.keywords.pricevon.set 设置一批关键词的出价
        /// </summary>
        public SimbaKeywordsPricevonSetResponse TaobaoSimbaKeywordsPricevonSet(EntityUser session, string keywordidPrices)
        {
            var req = new SimbaKeywordsPricevonSetRequest
            {
                Nick = session.SubUserName,
                KeywordidPrices = keywordidPrices
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 获取计划的基础报表
        /// </summary>
        public SimbaRptCampaignbaseGetResponse TaobaoSimbaRptCampaignbaseGet(EntityUser session, long campaignId, string startTime, string endTime)
        {
            var req = new SimbaRptCampaignbaseGetRequest
            {
                Nick = session.SubUserName,
                SubwayToken = GetSubwayToken(session),
                CampaignId = campaignId,
                SearchType = "SUMMARY",
                Source = "SUMMARY",
                StartTime = startTime,
                EndTime = endTime
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 获取计划的效果报表
        /// </summary>
        public SimbaRptCampaigneffectGetResponse TaobaoSimbaRptCampaigneffectGet(EntityUser session, long campaignId, string startTime, string endTime)
        {
            var req = new SimbaRptCampaigneffectGetRequest
            {
                Nick = session.SubUserName,
                SubwayToken = GetSubwayToken(session),
                CampaignId = campaignId,
                SearchType = "SUMMARY",
                Source = "SUMMARY",
                StartTime = startTime,
                EndTime = endTime
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }
    }
}
