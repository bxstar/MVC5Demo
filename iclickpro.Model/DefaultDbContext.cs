using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iclickpro.Model
{
    /// <summary>
    /// 默认数据上下文
    /// </summary>
    public class DefaultDbContext : DbContext
    {

        [ThreadStatic]
        protected static DefaultDbContext current;
        /// <summary>
        /// 需要线程安全时使用
        /// </summary>
        public static DefaultDbContext Current()
        {
            if (current == null)
                current = new DefaultDbContext();

            return current;
        }

        static DefaultDbContext()
        {
            Database.SetInitializer<DefaultDbContext>(null);
        }

        public DefaultDbContext()
            : base("Name=DefaultDbConn")
        {
        }


        /// <summary>
        /// 用户表
        /// </summary>
        public DbSet<EntityUser> User { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new EntityUserMap());
            //移除复数表名
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Types().Configure(c => c.ToTable(GetTableName(c.ClrType)));


            modelBuilder.Types<EntityUser>()
            .Configure(c =>
            {
                c.HasKey(cust => cust.fID);
                c.Property(cust => cust.fID).HasColumnName("fID");
                c.Property(t => t.fUserID).HasColumnName("fUserID");
                c.Property(t => t.fUserName).HasColumnName("fUserName");
                c.Property(t => t.fSubUserName).HasColumnName("fSubUserName");
                c.Property(t => t.fSession).HasColumnName("fSession");
                c.Property(t => t.fLoginUrl).HasColumnName("fLoginUrl");
                c.Property(t => t.fIsPoxy).HasColumnName("fIsPoxy");
                c.Property(t => t.CreateDate).HasColumnName("CreateDate");
                c.Ignore(t => t.FeeCode);
                c.Ignore(t => t.DeadLine);
                c.Ignore(t => t.PoxyUserList);
                c.ToTable("tb_user");
            });
        }

        /// <summary>
        /// 更改表的命名约定
        /// </summary>
        private string GetTableName(System.Type type)
        {
            string oldPrefix = "Entity";
            string prefix = "ad_";
            var result = System.Text.RegularExpressions.Regex.Replace(type.Name.Replace(oldPrefix, ""), ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);

            return prefix + result.ToLower();
        }
    }
}
