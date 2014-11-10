using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization.Json;

namespace TKC_Silverlight.View
{
    /// <summary>
    /// 宝贝找词
    /// </summary>
    public partial class UC_SearchKeywordByItem : UserControl
    {
        /// <summary>
        /// 关键词的数据
        /// </summary>
        //private ObservableCollection<ServiceReference1.EntityWordData> lstWordData = null;
        private List<ServiceReference1.EntityWordData> lstWordData = null;

        /// <summary>
        /// Web服务代理类
        /// </summary>
        ServiceReference1.WSTopSoapClient wsProxy = new ServiceReference1.WSTopSoapClient();

        /// <summary>
        /// 加载中显示面板
        /// </summary>
        private DevExpress.Xpf.Core.WaitIndicator waitIndicator;

        public UC_SearchKeywordByItem(DevExpress.Xpf.Core.WaitIndicator c)
        {
            InitializeComponent();
            waitIndicator = c;
            wsProxy.GetItemKeywordsCompleted += new EventHandler<ServiceReference1.GetItemKeywordsCompletedEventArgs>(wsProxy_GetItemKeywordsCompleted);
            
            InitControlAndData();
        }

        private void InitControlAndData()
        {
#if DEBUG
            Uri endpoint = new Uri("http://localhost:21106/SearchWord/GetUserOnlineItems");
#else
            Uri endpoint = new Uri("http://tkc.taokuaiche.com/SearchWord/GetUserOnlineItems");
#endif
            waitIndicator.DeferedVisibility = true;
            WebRequest request = WebRequest.Create(endpoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetResponse(new AsyncCallback(ResponseReady), request);
        }

        void ResponseReady(IAsyncResult asyncResult)
        {
            WebRequest request = asyncResult.AsyncState as WebRequest;
            WebResponse response = request.EndGetResponse(asyncResult);

            using (Stream responseStream = response.GetResponseStream())
            {
                //加入System.Runtime.Serialization.Json,System.ServiceModel.Web.dll引用
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<EntityItem>));

                List<EntityItem> lstItem = jsonSerializer.ReadObject(responseStream) as List<EntityItem>;
                foreach (var item in lstItem)
                {
                    item.pic_url = item.pic_url + "_60x60.jpg";
                }

                if (this.gdItems.Dispatcher.CheckAccess())
                {
                    this.gdItems.ItemsSource = lstItem;
                    waitIndicator.DeferedVisibility = false;
                }
                else
                {
                    this.gdItems.Dispatcher.BeginInvoke(() =>
                    {
                        this.gdItems.ItemsSource = lstItem;
                        waitIndicator.DeferedVisibility = false;
                    });
                }
            }


        }

        private void btnSearchKeyword_Click(object sender, RoutedEventArgs e)
        {
            gdKeywords.ItemsSource = null;
            waitIndicator.DeferedVisibility = true;
            wsProxy.GetItemKeywordsAsync(txtItemIdOrUrl.Text);
        }

        void wsProxy_GetItemKeywordsCompleted(object sender, ServiceReference1.GetItemKeywordsCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                waitIndicator.DeferedVisibility = false;
                layoutGroupBottom.SelectedTabIndex = 1;
                //gdKeywords.ItemsSource = e.Result;
                //lstWordData = new ObservableCollection<ServiceReference1.EntityWordData>(e.Result);
                lstWordData = e.Result.ToList();
                gdKeywords.ItemsSource = lstWordData;
            }
            else
            {//会话超时
                MessageBox.Show("抱歉您很久没有操作了，为保证安全，请重新登录");
            }
        }

        private void btnOpenItem_Click(object sender, RoutedEventArgs e)
        {
            if (txtItemIdOrUrl.Text.EndsWith("xx"))
            {
                MessageBox.Show("请输入宝贝链接（或从宝贝列表中选择）后，再打开宝贝！");
                return;
            }

            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(txtItemIdOrUrl.Text), "blank");
            //HyperlinkButton link = new HyperlinkButton();
            //link.NavigateUri = new Uri(txtItemIdOrUrl.Text, UriKind.Absolute);
            //System.Windows.Automation.Peers.HyperlinkButtonAutomationPeer hyperlinkButtonAutomationPeer = new System.Windows.Automation.Peers.HyperlinkButtonAutomationPeer(link);
            //hyperlinkButtonAutomationPeer.RaiseAutomationEvent(System.Windows.Automation.Peers.AutomationEvents.InvokePatternOnInvoked);
            //System.Windows.Automation.Provider.IInvokeProvider iprovider = (System.Windows.Automation.Provider.IInvokeProvider)hyperlinkButtonAutomationPeer;

            //if (iprovider != null)
            //    iprovider.Invoke();
        }


        private void gd_EndSorting(object sender, DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs e)
        {//排序完成后，显示第一行
            DevExpress.Xpf.Grid.GridControl g = (DevExpress.Xpf.Grid.GridControl)sender;
            g.View.FocusedRowHandle = 0;
        }

        private void gdItems_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            EntityItem item = e.NewItem as EntityItem;
            if (item != null)
            {
                txtItemIdOrUrl.Text = string.Format("http://item.taobao.com/item.htm?id={0}", item.item_id);
            }
        }

    }

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
        public double price { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public long quantity { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public long sales_count { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public string publish_time { get; set; }
        #endregion

    }
}
