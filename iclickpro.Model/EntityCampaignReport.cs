using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iclickpro.Model
{
    /// <summary>
    /// 计划报表
    /// </summary>
    public class EntityCampaignReport
    {
        /// <summary>
        /// 推广计划编号
        /// </summary>
        public long campaign_id { get; set; }

        /// <summary>
        /// 推广计划名称
        /// </summary>
        public string campaign_name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string campaign_status { get; set; }


        public string date { get; set; }

        /// <summary>
        /// 展现
        /// </summary>
        public int impressions { get; set; }

        /// <summary>
        /// 点击
        /// </summary>
        public int click { get; set; }

        /// <summary>
        /// 花费（元）
        /// </summary>
        public decimal cost { get; set; }

        /// <summary>
        /// 点击率
        /// </summary>
        public decimal ctr { get; set; }

        /// <summary>
        /// 平均点击花费（元）
        /// </summary>
        public decimal cpc { get; set; }

        /// <summary>
        /// 直接成交金额（元）
        /// </summary>
        public decimal directpay { get; set; }

        /// <summary>
        /// 间接成交金额（元）
        /// </summary>
        public decimal indirectpay { get; set; }

        /// <summary>
        /// 成交总额（元）
        /// </summary>
        public decimal totalpay { get; set; }

        /// <summary>
        /// 投入产出比
        /// </summary>
        public decimal roi { get; set; }

        /// <summary>
        /// 总成交数=（直接成交+间接成交）
        /// </summary>
        public int totalpaycount { get; set; }

        /// <summary>
        /// 直接成交数
        /// </summary>
        public int directpaycount { get; set; }

        /// <summary>
        /// 间接成交数
        /// </summary>
        public int indirectpaycount { get; set; }

        /// <summary>
        /// 总收藏数=（收藏宝贝数+收藏店铺数）
        /// </summary>
        public int totalfavcount { get; set; }

        /// <summary>
        /// 收藏宝贝数
        /// </summary>
        public int favitemcount { get; set; }

        /// <summary>
        /// 收藏店铺数
        /// </summary>
        public int favshopcount { get; set; }


        /// <summary>
        /// 点击成交转化率
        /// </summary>
        public decimal rate { get; set; }

        /// <summary>
        /// 平均排名
        /// </summary>
        public decimal avgpos { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string nick { get; set; }
    }
}
