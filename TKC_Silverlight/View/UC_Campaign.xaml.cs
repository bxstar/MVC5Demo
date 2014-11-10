using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TKC_Silverlight.View
{
    public partial class UC_Campaign : UserControl
    {
        ServiceReference1.WSTopSoapClient wsProxy = new ServiceReference1.WSTopSoapClient();

        public UC_Campaign()
        {
            InitializeComponent();

            wsProxy.HelloWorldCompleted += new EventHandler<ServiceReference1.HelloWorldCompletedEventArgs>(wsProxy_HelloWorldCompleted);
            wsProxy.GetAllCampaignOnlineCompleted += new EventHandler<ServiceReference1.GetAllCampaignOnlineCompletedEventArgs>(wsProxy_GetAllCampaignOnlineCompleted);
        }

        void wsProxy_GetAllCampaignOnlineCompleted(object sender, ServiceReference1.GetAllCampaignOnlineCompletedEventArgs e)
        {
            List<ServiceReference1.Campaign> lstCampaign = e.Result.ToList();
            gridControl1.ItemsSource = lstCampaign;
        }

        void wsProxy_HelloWorldCompleted(object sender, ServiceReference1.HelloWorldCompletedEventArgs e)
        {
            string strPara = e.UserState.ToString();
            MessageBox.Show(e.Result + strPara);
        }

        private void btnGetAllCampaign_Click(object sender, RoutedEventArgs e)
        {//测试访问WebService，获取数据
            wsProxy.GetAllCampaignOnlineAsync();
        }

        private void btnGetUserInfo_Click(object sender, RoutedEventArgs e)
        {//获取用户的登录授权信息
            string strMsg = string.Empty;
            strMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            wsProxy.HelloWorldAsync(strMsg);
        }
    }
}
