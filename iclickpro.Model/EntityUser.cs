using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iclickpro.Model
{
    public class EntityUser
    {
        /// <summary>
        /// 主键，本地用户ID
        /// </summary>
        public int fID { get; set; }

        /// <summary>
        /// sessionkey
        /// </summary>
        public string fSession { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string fUserName { get; set; }

        /// <summary>
        /// 子用户名
        /// </summary>
        public string fSubUserName { get; set; }

        /// <summary>
        /// 订购收费代码
        /// </summary>
        public string FeeCode { get; set; }

        /// <summary>
        /// 订购到期时间
        /// </summary>
        public string DeadLine { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long fUserID { get; set; }

        /// <summary>
        /// 登录系统的url参数
        /// </summary>
        public string fLoginUrl { get; set; }

        /// <summary>
        /// 是否是代理
        /// </summary>
        public string fIsPoxy { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 代理账户
        /// </summary>
        public List<string> PoxyUserList { get; set; }
    }

    public class EntityUserMap : EntityTypeConfiguration<EntityUser>
    {
        public EntityUserMap()
        {
            // Primary Key
            this.HasKey(t => t.fID);


            // Table & Column Mappings
            this.ToTable("tb_user");
            //this.Property(t => t.fID).HasColumnName("fID");
            //this.Property(t => t.fUserID).HasColumnName("fUserID");
            //this.Property(t => t.fUserName).HasColumnName("fUserName");
            //this.Property(t => t.fSubUserName).HasColumnName("fSubUserName");
            //this.Property(t => t.fSession).HasColumnName("fSession");
            //this.Property(t => t.fLoginUrl).HasColumnName("fLoginUrl");
            //this.Property(t => t.fIsPoxy).HasColumnName("fIsPoxy");
            //this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Ignore(t => t.FeeCode);
            this.Ignore(t => t.DeadLine);
            this.Ignore(t => t.PoxyUserList);
        }
    }
}
