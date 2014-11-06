using System.Net;
using System.Text;
using System.Net.Mail;
using System;

namespace iclickpro.AccessCommon
{
    /// <summary>  
    /// 发送邮件的类  
    /// </summary>  
    public class SendMail
    {
        /// <summary>
        /// 发送Email
        /// </summary>
        /// <param name="mailTo">收件人地址</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <returns></returns>
        public static bool SendEmail(string mailTo, string subject, string body)
        {
            try
            {
                return SendEmailUrl(Config.GetStrConfig(Constants.C_MailServerName),
                    Config.GetStrConfig(Constants.C_MailFrom), mailTo, subject, body,
                    Config.GetStrConfig(Constants.C_MailPassword));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 发送Mail
        /// </summary>
        /// <param name="mailServerName">服务器</param>
        /// <param name="mailFrom">发件人地址</param>
        /// <param name="mailTo">收件人地址</param>
        /// <param name="subject">标题</param>
        /// <param name="body">正文</param>
        /// <param name="password">密码</param>
        private static bool SendEmailUrl(string mailServerName, string mailFrom, string mailTo, string subject, string body, string password)
        {
            var from = new MailAddress(mailFrom);
            //设置收件人信箱,及显示名字
            var to = new MailAddress(mailTo);
            //创建一个MailMessage对象
            var oMail = new MailMessage(from, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                BodyEncoding = Encoding.GetEncoding(Constants.C_Code),
                Priority = MailPriority.High,

            };
            //邮件标题
            //邮件内容
            //指定邮件格式,支持HTML格式
            //邮件采用的编码
            //设置邮件的优先级为高
            //发送邮件服务器
            var client = new SmtpClient
            {
                Host = mailServerName,
                Credentials = new NetworkCredential(mailFrom, password),
                Port = Convert.ToInt32(Config.GetStrConfig(Constants.C_MailPort)),
                EnableSsl = true
            };
            //指定邮件服务器
            //指定服务器邮件,及密码

            //发送
            try
            {
                client.Send(oMail); //发送邮件
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
