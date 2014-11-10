using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iclickpro.Model
{
    /// <summary>
    /// 返回客户端结果类型枚举类
    /// </summary>
    public enum ReturnStatus
    {
        /// <summary>
        /// 执行失败
        /// </summary>
        error,
        /// <summary>
        /// 执行成功
        /// </summary>
        success,
        /// <summary>
        /// 未托管
        /// </summary>
        unmanaged,
        /// <summary>
        /// 未二次授权，跳转至淘宝授权页面
        /// </summary>
        unauthorize,
        /// <summary>
        /// 服务器端session丢失
        /// </summary>
        server_session_missing,
        /// <summary>
        /// 唤醒用户失败
        /// </summary>
        wakeup_user_fail,
        /// <summary>
        /// 修改计划时，与已有计划重名
        /// </summary>
        campaign_name_repeat
    }

    /// <summary>
    /// 关键词性别类型
    /// </summary>
    public enum TypeKeywordSex
    {
        /// <summary>
        /// 性别无关
        /// </summary>
        None = 0,

        /// <summary>
        /// 男性
        /// </summary>
        Male = 1,

        /// <summary>
        /// 女性
        /// </summary>
        Female = 2
    }
}
