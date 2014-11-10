using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iclickpro.Model
{
    /// <summary>
    /// 关键词大盘数据
    /// </summary>
    public class EntityWordData
    {
        /// <summary>
        /// 内部ID
        /// </summary>
        public string id { get; set; } 

        /// <summary>
        /// 关键词
        /// </summary>
        public string word { get; set; }

        /// <summary>
        /// 点击
        /// </summary>
        public long click { get; set; }

        /// <summary>
        /// 竞争度
        /// </summary>
        public double competition { get; set; }

        /// <summary>
        /// 花费
        /// </summary>
        public double cost { get; set; }

        /// <summary>
        /// 点击转化率
        /// </summary>
        public double rate { get; set; }

        /// <summary>
        /// 平均点击花费
        /// </summary>
        public double cpc { get; set; }

        /// <summary>
        /// 点击率
        /// </summary>
        public double ctr { get; set; }

        /// <summary>
        /// 直接成交金额
        /// </summary>
        public double directpay { get; set; }

        /// <summary>
        /// 直接成交笔数
        /// </summary>
        public long directpaycount { get; set; }

        /// <summary>
        /// 宝贝收藏数
        /// </summary>
        public long favitemcount { get; set; }

        /// <summary>
        /// 店铺收藏数
        /// </summary>
        public long favshopcount { get; set; }

        /// <summary>
        /// 总收藏数
        /// </summary>
        public long totalfavcount { get; set; }

        /// <summary>
        /// 展现
        /// </summary>
        public long impressions { get; set; }

        /// <summary>
        /// 间接成交金额
        /// </summary>
        public double indirectpay { get; set; }

        /// <summary>
        /// 间接成交笔数
        /// </summary>
        public long indirectpaycount { get; set; }

        /// <summary>
        /// 投入产出比
        /// </summary>
        public double roi { get; set; }

        /// <summary>
        /// 总成交数
        /// </summary>
        public long totalpaycount { get; set; }

        /// <summary>
        /// 总成交金额
        /// </summary>
        public double totalpay { get; set; }

        /// <summary>
        /// 关键词和宝贝的相似度，1到10之间，越大表示该词越相似宝贝
        /// </summary>
        public double similar { get; set; }
    }
}
