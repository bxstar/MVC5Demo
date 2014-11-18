using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iclickpro.Model
{
    /// <summary>
    /// 创意分析类
    /// </summary>
    public class EntityCreativeAnalysis
    {
        [Key]
        /// <summary>
        /// 主键
        /// </summary>
        public long local_id { get; set; }

        public long campaign_id { get; set; }

        public long adgroup_id { get; set; }

        public long creative_id { get; set; }

        /// <summary>
        /// 创意平均七天的最后一天日期
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string nick { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal impressions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal click { get; set; }

        public decimal ctr { get; set; }

        /// <summary>
        /// 淘快词，淘快车，安心代价
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 创意图片URL
        /// </summary>
        public string creative_url { get; set; }

        /// <summary>
        /// 创意文字
        /// </summary>
        public string creative_text { get; set; }

        public string cat_id { get; set; }

        public string cat_name { get; set; }
    }
}
