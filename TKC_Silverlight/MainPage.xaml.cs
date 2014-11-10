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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;


namespace TKC_Silverlight
{
    public partial class MainPage : UserControl
    {
        ServiceReference1.WSTopSoapClient wsProxy = new ServiceReference1.WSTopSoapClient();
        

        public MainPage()
        {
            InitializeComponent();
            ThemeManager.ApplicationThemeName = "DXStyle";
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DocumentPanel dp = new DocumentPanel();
            dp.Caption = "主页" + DateTime.Now.ToShortTimeString();
            documentGroup1.Add(dp);
            UserControl userControl = new View.UC_Index(waitIndicator);
            //UserControl userControl = new View.UC_SearchKeywordByItem();
            dp.Content = userControl;
            dp.IsActive = true;

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

        private void navBarItem_Click(object sender, EventArgs e)
        {
            DevExpress.Xpf.NavBar.NavBarItem navBarItem = (DevExpress.Xpf.NavBar.NavBarItem)sender;
            DocumentPanel dp = new DocumentPanel();
            dp.Caption = navBarItem.Content.ToString() + DateTime.Now.ToShortTimeString();
            documentGroup1.Add(dp);

            UserControl uc = new View.UC_Campaign();
            dp.Content = uc;

            dp.IsActive = true;
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

        private void nbiSearchKeywordByItem_Click(object sender, EventArgs e)
        {//宝贝找词
            DevExpress.Xpf.NavBar.NavBarItem navBarItem = (DevExpress.Xpf.NavBar.NavBarItem)sender;
            DocumentPanel dp = new DocumentPanel();
            dp.Caption = navBarItem.Content.ToString() + DateTime.Now.ToShortTimeString();
            documentGroup1.Add(dp);
            UserControl uc = new View.UC_SearchKeywordByItem(waitIndicator);
            dp.Content = uc;
            dp.IsActive = true;
        }

        private void nbitop20W_Click(object sender, EventArgs e)
        {
            DevExpress.Xpf.NavBar.NavBarItem navBarItem = (DevExpress.Xpf.NavBar.NavBarItem)sender;
            DocumentPanel dp = new DocumentPanel();
            dp.Caption = navBarItem.Content.ToString() + DateTime.Now.ToShortTimeString();
            documentGroup1.Add(dp);
            UserControl uc = new View.UC_Top20W();
            dp.Content = uc;
            dp.IsActive = true;
        }



    }


}
