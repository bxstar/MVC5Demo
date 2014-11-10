using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Top.Api.Domain;
using iclickpro.Model;
using Top.Api.Response;
using iclickpro.AccessCommon;
using log4net;

namespace iclickpro.BusinessLayer
{
    /// <summary>
    /// 推广计划业务逻辑
    /// </summary>
    public class BusinessCampaignHandler
    {
        /// <summary>
        /// 写log信息
        /// </summary>
        private static ILog logger = log4net.LogManager.GetLogger("Logger");

        /// <summary>
        /// 操作API数据
        /// </summary>
        private BusinessTaobaoApiHandler bllTaoBaoApi = new BusinessTaobaoApiHandler();



        /// <summary>
        /// 获取计划信息
        /// </summary>
        public List<Campaign> GetCampaignOnline(EntityUser session)
        {
            var taobaoSimbaCampaignsGet = CommonHandler.DoTaoBaoApi<SimbaCampaignsGetResponse>(bllTaoBaoApi.TaobaoSimbaCampaignsGet, session);
            if (taobaoSimbaCampaignsGet != null)
            {
                return taobaoSimbaCampaignsGet.Campaigns;
            }
            return null;
        }

        /// <summary>
        /// 线上，下载推广计划报表
        /// </summary>
        public List<EntityCampaignReport> DownLoadCampaignReport(EntityUser session, long campaignId, int days)
        {
            List<EntityCampaignReport> lstAll = new List<EntityCampaignReport>();
            string strStartDay = DateTime.Now.AddDays(0 - days).Date.ToString("yyyy-MM-dd");
            string strEndDay = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");

            string jsonBaseRpt = DownLoadCamapginBaseReport(session, campaignId, strStartDay, strEndDay).ToLower();
            if (!string.IsNullOrEmpty(jsonBaseRpt) && jsonBaseRpt.Length > 2)
            {
                var arrBaseRpt = new DynamicJsonParser().FromJson(jsonBaseRpt);
                foreach (var item in arrBaseRpt)
                {
                    EntityCampaignReport rpt = new EntityCampaignReport();
                    rpt.date = item.date;
                    rpt.campaign_id = item.campaignid;
                    rpt.nick = item.nick;
                    rpt.impressions = item.impressions == null ? 0 : item.impressions;
                    rpt.click = item.click == null ? 0 : item.click;
                    rpt.ctr = item.ctr == null ? 0M : item.ctr;
                    rpt.cost = Convert.ToDecimal(item.cost == null ? 0M : item.cost) / 100.0M;
                    rpt.cpc = item.cpc == null ? 0M : item.cpc;
                    rpt.avgpos = item.avgpos == null ? 0M : item.avgpos;
                    rpt.source = item.source;

                    lstAll.Add(rpt);
                }
            }

            string jsonEffectRpt = DownLoadCampaignEffectReport(session, campaignId, strStartDay, strEndDay).ToLower();
            if (!string.IsNullOrEmpty(jsonEffectRpt) && jsonEffectRpt.Length > 2)
            {
                var arrEffectRpt = new DynamicJsonParser().FromJson(jsonEffectRpt);
                foreach (var item in arrEffectRpt)
                {
                    EntityCampaignReport rpt = lstAll.Find(o => o.date == item.date);
                    if (rpt == null)
                    {
                        logger.ErrorFormat("base:{0}\r\n effect:{1}", jsonBaseRpt, jsonEffectRpt);
                        continue;
                    }
                    rpt.directpay = Convert.ToDecimal(item.directpay == null ? 0M : item.directpay) / 100.0M;
                    rpt.indirectpay = Convert.ToDecimal(item.indirectpay == null ? 0M : item.indirectpay) / 100.0M;
                    rpt.directpaycount = item.directpaycount == null ? 0 : item.directpaycount;
                    rpt.indirectpaycount = item.indirectpaycount == null ? 0 : item.indirectpaycount;
                    rpt.favitemcount = item.favitemcount == null ? 0 : item.favitemcount;
                    rpt.favshopcount = item.favshopcount == null ? 0 : item.favshopcount;
                    rpt.totalpay = rpt.directpay + rpt.indirectpay;
                    rpt.totalpaycount = rpt.directpaycount + rpt.indirectpaycount;
                    rpt.totalfavcount = rpt.favitemcount + rpt.favshopcount;
                    rpt.roi = rpt.cost == 0M ? 0M : Math.Round((rpt.directpay + rpt.indirectpay) / rpt.cost, 2);
                }
            }

            return lstAll;
        }


        /// <summary>
        /// 下载推广计划的基本报表
        /// </summary>
        public string DownLoadCamapginBaseReport(EntityUser session, long campaignId, string strStartDay, string strEndDay)
        {

            var response = CommonHandler.DoTaoBaoApi<SimbaRptCampaignbaseGetResponse>(bllTaoBaoApi.TaobaoSimbaRptCampaignbaseGet, session, campaignId, strStartDay, strEndDay);
            return response.RptCampaignBaseList;
        }

        /// <summary>
        /// 下载推广计划的效果报表
        /// </summary>
        public string DownLoadCampaignEffectReport(EntityUser session, long campaignId, string strStartDay, string strEndDay)
        {
            var response = CommonHandler.DoTaoBaoApi<SimbaRptCampaigneffectGetResponse>(bllTaoBaoApi.TaobaoSimbaRptCampaigneffectGet, session, campaignId, strStartDay, strEndDay);
            return response.RptCampaignEffectList;
        }
    }
}
