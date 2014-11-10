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
        public int UserID { get; set; }

        /// <summary>
        /// sessionkey
        /// </summary>
        public string TopSessions { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 子用户名
        /// </summary>
        public string SubUserName { get; set; }

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
        public long OnlineUserID { get; set; }

        /// <summary>
        /// 登录系统的url参数
        /// </summary>
        public string LoginUrl { get; set; }

        /// <summary>
        /// 是否是代理
        /// </summary>
        public string IsProxy { get; set; }

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
            this.HasKey(t => t.UserID);


            // Table & Column Mappings
            this.ToTable("tb_user");
            this.Property(t => t.UserID).HasColumnName("fID");
            this.Property(t => t.OnlineUserID).HasColumnName("fUserID");
            this.Property(t => t.UserName).HasColumnName("fUserName");
            this.Property(t => t.SubUserName).HasColumnName("fSubUserName");
            this.Property(t => t.TopSessions).HasColumnName("fSession");
            this.Property(t => t.LoginUrl).HasColumnName("fLoginUrl");
            this.Property(t => t.IsProxy).HasColumnName("fIsPoxy");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Ignore(t => t.FeeCode);
            this.Ignore(t => t.DeadLine);
            this.Ignore(t => t.PoxyUserList);
        }
    }
}
