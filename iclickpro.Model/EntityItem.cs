using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iclickpro.Model
{
    /// <summary>
    /// 淘宝宝贝实体类
    /// </summary>
    public class EntityItem
    {

        /// <summary>
        /// 宝贝ID
        /// </summary>		
        public long item_id { get; set; }

        /// <summary>
        /// 宝贝的卖家昵称
        /// </summary>
        public string nick { get; set; }

        /// <summary>
        /// 宝贝标题
        /// </summary>
        public string item_title { get; set; }

        /// <summary>
        /// 宝贝类目，最底层的类目
        /// </summary>
        public long cid { get; set; }

        /// <summary>
        /// 类目id的路径，用空格分隔级别
        /// </summary>
        public string catpathid { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        public string categroy_name { get; set; }

        /// <summary>
        /// 宝贝的主图地址
        /// </summary>
        public string pic_url { get; set; }

        #region 扩展属性
        /// <summary>
        /// 宝贝的属性列表
        /// </summary>
        public List<string> LstPropsName { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public string price { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public string quantity { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public string sales_count { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public string publish_time { get; set; }

        /// <summary>
        /// 宝贝的网址
        /// </summary>
        public string item_url { get; set; }

        #endregion

    }
}
