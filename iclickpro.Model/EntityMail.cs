using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iclickpro.Model
{
    /// <summary>
    /// 邮件实体类
    /// </summary>
    public class EntityMail
    {
        /// <summary>
        /// 收件人地址
        /// </summary>
        public string mail_to { get; set; }

        /// <summary>
        /// 邮件标题
        /// </summary>		
        public string mail_subject { get; set; }

        /// <summary>
        /// 邮件正文
        /// </summary>		
        public string mail_body { get; set; }

        /// <summary>
        /// 附件路径
        /// </summary>		
        public string file_path { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string create_time { get; set; }
    }
}
