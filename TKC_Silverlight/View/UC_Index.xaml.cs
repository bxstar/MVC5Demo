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
using DevExpress.Xpf.Charts;


namespace TKC_Silverlight.View
{
    /// <summary>
    /// 主页
    /// </summary>
    public partial class UC_Index : UserControl
    {
        /// <summary>
        /// Web服务代理类
        /// </summary>
        ServiceReference1.WSTopSoapClient wsProxy = new ServiceReference1.WSTopSoapClient();

        /// <summary>
        /// 日期段选择控件
        /// </summary>
        List<HyperlinkButton> lstDtBtn = new List<HyperlinkButton>();

        /// <summary>
        /// 加载中显示面板
        /// </summary>
        private DevExpress.Xpf.Core.WaitIndicator waitIndicator;

        public UC_Index(DevExpress.Xpf.Core.WaitIndicator c)
        {
            InitializeComponent();
            waitIndicator = c;
            wsProxy.GetAllCampaignRptOnlineCompleted += new EventHandler<ServiceReference1.GetAllCampaignRptOnlineCompletedEventArgs>(wsProxy_GetAllCampaignRptOnlineCompleted);
            //Loaded += new RoutedEventHandler(UC_Index_Loaded);//改方法会在切换tab时再次执行，影响效率
            InitControlAndData();
        }

        void InitControlAndData()
        {
            DateTime dtStartValue = DateTime.Now.AddDays(-7);
            DateTime dtEndValue = DateTime.Now.AddDays(-1);

            dtStart.EditValue = dtStartValue;
            dtEnd.EditValue = dtEndValue;
            waitIndicator.DeferedVisibility = true;
            wsProxy.GetAllCampaignRptOnlineAsync(dtStartValue.ToString("yyyy-MM-dd"), dtEndValue.ToString("yyyy-MM-dd"));
        }


        void wsProxy_GetAllCampaignRptOnlineCompleted(object sender, ServiceReference1.GetAllCampaignRptOnlineCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                List<ServiceReference1.EntityCampaignReport> lstCampaignRpt = e.Result.ToList();
                //计算数字区域
                int impressions = lstCampaignRpt.Sum(o => o.impressions);
                int click = lstCampaignRpt.Sum(o => o.click);
                tbkImpressions.Text = string.Format("{0:N0}", impressions);
                
                tbkClick.Text = string.Format("{0:N0}", click);
                if (click != 0)
                {
                    tbkCtr.Text = string.Format("{0:N2}", 100.0 * click / impressions) + "%";
                }
                else
                {
                    tbkCtr.Text = "0.00%";
                }
                decimal cost = lstCampaignRpt.Sum(o => o.cost);
                tbkCost.Text = string.Format("{0:N2}", cost);
                if (click != 0)
                    tbkCpc.Text = string.Format("{0:N2}", cost / click);
                else
                    tbkCpc.Text = "0.00";
                decimal totalpay = lstCampaignRpt.Sum(o => o.totalpay);
                tbkTotalPay.Text = string.Format("{0:N2}", totalpay);
                tbkTotalfavcount.Text = lstCampaignRpt.Sum(o => o.totalfavcount).ToString();
                if (cost != 0M)
                    tbkRoi.Text = string.Format("{0:N2}", totalpay / cost);
                else
                    tbkRoi.Text = "0.00";

                //绘制统计图表
                foreach (var itemSeries in xyDiagram2D.Series)
                {//清理上次的绘制 
                    itemSeries.Points.Clear();
                }
                
                DateTime dtStartValue = (DateTime)dtStart.EditValue;
                DateTime dtEndValue = (DateTime)dtEnd.EditValue;

                for (; dtStartValue <= dtEndValue; dtStartValue = dtStartValue.AddDays(1))
                {
                    string strDate = dtStartValue.ToString("yy-MM-dd");
                    List<ServiceReference1.EntityCampaignReport> lstOneDayRpt = lstCampaignRpt.Where(o => o.date == dtStartValue.ToString("yyyy-MM-dd")).ToList();

                    int nImpressions = lstOneDayRpt.Sum(o => o.impressions);
                    SeriesPoint sp展现 = new SeriesPoint();
                    sp展现.Argument = strDate;
                    sp展现.Value = nImpressions;
                    series展现.Points.Add(sp展现);

                    int nClick = lstOneDayRpt.Sum(o => o.click);
                    SeriesPoint sp点击 = new SeriesPoint();
                    sp点击.Argument = strDate;
                    sp点击.Value = lstOneDayRpt.Sum(o => o.click);
                    series点击.Points.Add(sp点击);

                    double nCost = Convert.ToDouble(lstOneDayRpt.Sum(o => o.cost));
                    SeriesPoint sp花费 = new SeriesPoint();
                    sp花费.Argument = strDate;
                    sp花费.Value = nCost;
                    series花费.Points.Add(sp花费);

                    double nCtr = 0.00;
                    if (nClick != 0)
                        nCtr = (nClick * 100.0 / nImpressions);
                    SeriesPoint sp点击率 = new SeriesPoint();
                    sp点击率.Argument = strDate;
                    sp点击率.Value = nCtr;
                    series点击率.Points.Add(sp点击率);

                    double nCpc = 0.00;
                    if (nClick != 0)
                        nCpc = nCost / nClick;
                    SeriesPoint sp平均点击花费 = new SeriesPoint();
                    sp平均点击花费.Argument = strDate;
                    sp平均点击花费.Value = nCpc;
                    series平均点击花费.Points.Add(sp平均点击花费);

                    SeriesPoint sp直接成交金额 = new SeriesPoint();
                    sp直接成交金额.Argument = strDate;
                    sp直接成交金额.Value = Convert.ToDouble(lstOneDayRpt.Sum(o => o.directpay));
                    series直接成交金额.Points.Add(sp直接成交金额);

                    SeriesPoint sp直接成交笔数 = new SeriesPoint();
                    sp直接成交笔数.Argument = strDate;
                    sp直接成交笔数.Value = lstOneDayRpt.Sum(o => o.directpaycount);
                    series直接成交笔数.Points.Add(sp直接成交笔数);

                    SeriesPoint sp间接成交金额 = new SeriesPoint();
                    sp间接成交金额.Argument = strDate;
                    sp间接成交金额.Value = Convert.ToDouble(lstOneDayRpt.Sum(o => o.indirectpay));
                    series间接成交金额.Points.Add(sp间接成交金额);

                    SeriesPoint sp间接成交笔数 = new SeriesPoint();
                    sp间接成交笔数.Argument = strDate;
                    sp间接成交笔数.Value = Convert.ToDouble(lstOneDayRpt.Sum(o => o.indirectpaycount));
                    series间接成交笔数.Points.Add(sp间接成交笔数);

                    SeriesPoint sp收藏宝贝数 = new SeriesPoint();
                    sp收藏宝贝数.Argument = strDate;
                    sp收藏宝贝数.Value = lstOneDayRpt.Sum(o => o.favitemcount);
                    series收藏宝贝数.Points.Add(sp收藏宝贝数);

                    SeriesPoint sp收藏店铺数 = new SeriesPoint();
                    sp收藏店铺数.Argument = strDate;
                    sp收藏店铺数.Value = lstOneDayRpt.Sum(o => o.favshopcount);
                    series收藏店铺数.Points.Add(sp收藏店铺数);

                    double nTotalpay = Convert.ToDouble(lstOneDayRpt.Sum(o => o.totalpay));
                    SeriesPoint sp总成交金额 = new SeriesPoint();
                    sp总成交金额.Argument = strDate;
                    sp总成交金额.Value = nTotalpay;
                    series总成交金额.Points.Add(sp总成交金额);

                    double nRoi = 0.00;
                    if (nCost != 0)
                        nRoi = nTotalpay / nCost;
                    SeriesPoint sp投入产出比 = new SeriesPoint();
                    sp投入产出比.Argument = strDate;
                    sp投入产出比.Value = nRoi;
                    series投入产出比.Points.Add(sp投入产出比);

                    int nTotalPaycount = lstOneDayRpt.Sum(o => o.totalpaycount);
                    SeriesPoint sp总成交笔数 = new SeriesPoint();
                    sp总成交笔数.Argument = strDate;
                    sp总成交笔数.Value = nTotalPaycount;
                    series总成交笔数.Points.Add(sp总成交笔数);

                    SeriesPoint sp总收藏数 = new SeriesPoint();
                    sp总收藏数.Argument = strDate;
                    sp总收藏数.Value = lstOneDayRpt.Sum(o => o.totalfavcount);
                    series总收藏数.Points.Add(sp总收藏数);

                    double nClickRate = 0.00;
                    if (nClick != 0)
                        nClickRate = (nTotalPaycount * 100.0 / nClick);
                    SeriesPoint sp点击转化率 = new SeriesPoint();
                    sp点击转化率.Argument = strDate;
                    sp点击转化率.Value = nClickRate;
                    series点击转化率.Points.Add(sp点击转化率);

                }

                List<ServiceReference1.EntityCampaignReport> lstGirdCampaignRpt = (from a in lstCampaignRpt
                                                                                   group a by a.campaign_id into b
                                                                                   select new ServiceReference1.EntityCampaignReport()
                                                                                   {
                                                                                       campaign_id = b.Key,
                                                                                       campaign_name = b.Last().campaign_name,
                                                                                       campaign_status = b.Last().campaign_status,
                                                                                       impressions = b.Sum(c => c.impressions),
                                                                                       click = b.Sum(c => c.click),
                                                                                       cost = Math.Round(b.Sum(c => c.cost), 2),
                                                                                       ctr = b.Sum(c => c.impressions) == 0 ? 0M : Math.Round(b.Sum(c => c.click) * 1.0M / b.Sum(c => c.impressions), 4),
                                                                                       cpc = b.Sum(c => c.click) == 0 ? 0M : Math.Round(b.Sum(c => c.cost) / b.Sum(c => c.click), 2),
                                                                                       directpay = Math.Round(b.Sum(c => c.directpay), 2),
                                                                                       indirectpay = Math.Round(b.Sum(c => c.indirectpay), 2),
                                                                                       totalpay = Math.Round(b.Sum(c => c.totalpay), 2),
                                                                                       roi = b.Sum(c => c.cost) == 0M ? 0M : Math.Round(b.Sum(c => c.totalpay) / b.Sum(c => c.cost), 2),
                                                                                       directpaycount = b.Sum(c => c.directpaycount),
                                                                                       indirectpaycount = b.Sum(c => c.indirectpaycount),
                                                                                       totalpaycount = b.Sum(c => c.totalpaycount),
                                                                                       favitemcount = b.Sum(c => c.favitemcount),
                                                                                       favshopcount = b.Sum(c => c.favshopcount),
                                                                                       totalfavcount = b.Sum(c => c.totalfavcount),
                                                                                       rate = b.Sum(c => c.click) == 0 ? 0M : Math.Round(b.Sum(c => c.totalpaycount) * 1.0M / b.Sum(c => c.click), 4)
                                                                                   }).ToList();

                //绑定推广计划列表
                gridControl1.ItemsSource = lstGirdCampaignRpt;
            }
            else
            {//会话超时
                MessageBox.Show("抱歉您很久没有操作了，为保证安全，请重新登录");
            }
            waitIndicator.DeferedVisibility = false;
        }

        private void btnShowDt_Click(object sender, RoutedEventArgs e)
        {
            popup1.IsOpen = !popup1.IsOpen;
        }

        private void dtSelect_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            SelectDtBtnWithBrush(btn.Name);
            DateTime dtNow = DateTime.Now;

            if (btn.Name == "dtZuoTian")
            {
                dtStart.EditValue = dtNow.AddDays(-1);
                dtEnd.EditValue = dtNow.AddDays(-1);
                btnShowDt.Content = "【时间范围：昨天【更改";
            }
            else if (btn.Name == "dtQianTian")
            {
                dtStart.EditValue = dtNow.AddDays(-2);
                dtEnd.EditValue = dtNow.AddDays(-2);
                btnShowDt.Content = "【时间范围：前天【更改";
            }
            else if (btn.Name == "dtLast7Day")
            {
                dtStart.EditValue = dtNow.AddDays(-7);
                dtEnd.EditValue = dtNow.AddDays(-1);
                btnShowDt.Content = "【时间范围：过去7天【更改";
            }
            else if (btn.Name == "dtLast14Day")
            {
                dtStart.EditValue = dtNow.AddDays(-14);
                dtEnd.EditValue = dtNow.AddDays(-1);
                btnShowDt.Content = "【时间范围：过去14天【更改";
            }
            else if (btn.Name == "dtLast30Day")
            {
                dtStart.EditValue = dtNow.AddDays(-30);
                dtEnd.EditValue = dtNow.AddDays(-1);
                btnShowDt.Content = "【时间范围：过去30天【更改";
            }
            else if (btn.Name == "dtLastMonth")
            {
                DateTime dtLastMonthFirstDay = new DateTime(dtNow.Year, dtNow.Month - 1, 1);
                DateTime dtLastMonthLastDay = new DateTime(dtNow.Year, dtNow.Month, 1).AddDays(-1);
                dtStart.EditValue = dtLastMonthFirstDay;
                dtEnd.EditValue = dtLastMonthLastDay;
                btnShowDt.Content = "【时间范围：上月【更改";
            }
            popup1.IsOpen = false;

            DateTime dtStartValue = (DateTime)dtStart.EditValue;
            DateTime dtEndValue = (DateTime)dtEnd.EditValue;

            waitIndicator.DeferedVisibility = true;
            gridControl1.ItemsSource = null;
            wsProxy.GetAllCampaignRptOnlineAsync(dtStartValue.ToString("yyyy-MM-dd"), dtEndValue.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// 选中日前的范围
        /// </summary>
        /// <param name="dtBtnName">日前段控件的名称</param>
        private void SelectDtBtnWithBrush(string dtBtnName)
        {
            Brush selectBackgroundBrush = new SolidColorBrush(Utils.ConvertToColor("#FF84B2E7"));
            Brush selectForegroundBrush = new SolidColorBrush(Colors.White);

            Brush unselectBackgroundBrush = new SolidColorBrush(Colors.White);
            Brush unselectForegroundBrush = new SolidColorBrush(Utils.ConvertToColor("#FF73AADE"));

            if (lstDtBtn.Count == 0)
            {
                lstDtBtn.Add(dtZuoTian); lstDtBtn.Add(dtQianTian); lstDtBtn.Add(dtLast7Day); lstDtBtn.Add(dtLast14Day); lstDtBtn.Add(dtLast30Day); lstDtBtn.Add(dtLastMonth);
            }

            foreach (var itemDt in lstDtBtn)
            {
                if (itemDt.Name == dtBtnName)
                {
                    itemDt.Background = selectBackgroundBrush;
                    itemDt.Foreground = selectForegroundBrush;
                }
                else
                {
                    itemDt.Background = unselectBackgroundBrush;
                    itemDt.Foreground = unselectForegroundBrush;
                }
            }
        }

        private void btnDtOk_Click(object sender, RoutedEventArgs e)
        {
            DateTime dtStartValue = (DateTime)dtStart.EditValue;
            DateTime dtEndValue = (DateTime)dtEnd.EditValue;
            btnShowDt.Content = string.Format("【时间范围：{0}至{1}【更改", dtStartValue.ToString("yyyy-MM-dd"), dtEndValue.ToString("yyyy-MM-dd"));
            popup1.IsOpen = false;
            waitIndicator.DeferedVisibility = true;
            gridControl1.ItemsSource = null;
            wsProxy.GetAllCampaignRptOnlineAsync(dtStartValue.ToString("yyyy-MM-dd"), dtEndValue.ToString("yyyy-MM-dd"));
        }

        private void btnDtCancel_Click(object sender, RoutedEventArgs e)
        {
            popup1.IsOpen = false;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            DependencyObject cp = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(chk));
            List<CheckBox> lstCheckBox = Utils.GetChildObjects<CheckBox>(cp, null);
            if (lstCheckBox.Where(o => o.IsChecked.Value).Count() == 0)
            {
                chk.IsChecked = true;
                MessageBox.Show("至少选择一项");
                return;
            }
        }


        long totalImpressions = 0; long totalClick = 0; double totalCost = 0; double totalPay = 0; int totalPaycount = 0;
        private void gridControl1_CustomSummary(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {//自定义计划合计
            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
            {
                totalImpressions = totalClick = totalPaycount = 0;
                totalCost = totalPay = 0D;
            }
            else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
            {
                long impressions = Convert.ToInt64(e.GetValue("impressions"));
                totalImpressions += impressions;
                long click = Convert.ToInt64(e.GetValue("click"));
                totalClick += click;
                double cost = Convert.ToDouble(e.GetValue("cost"));
                totalCost += cost;
                double pay = Convert.ToDouble(e.GetValue("totalpay"));
                totalPay += pay;
                int paycount = Convert.ToInt32(e.GetValue("totalpaycount"));
                totalPaycount += paycount;
            }
            else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            {
                DevExpress.Xpf.Grid.GridSummaryItem item = e.Item as DevExpress.Xpf.Grid.GridSummaryItem;

                if (totalImpressions != 0 && item.FieldName == "ctr")
                {
                    e.TotalValue = string.Format("平均={0}%", Math.Round(totalClick * 100.0 / totalImpressions, 2));
                }
                else if (totalClick != 0 && item.FieldName == "cpc")
                {
                    e.TotalValue = string.Format("平均={0}", Math.Round(totalCost / totalClick, 2));
                }
                else if (totalCost != 0 && item.FieldName == "roi")
                {
                    e.TotalValue = string.Format("平均={0}", Math.Round(totalPay / totalCost, 2));
                }
                else if (totalClick != 0 && item.FieldName == "rate")
                {
                    e.TotalValue = string.Format("平均={0}%", Math.Round(totalPaycount * 100.0 / totalClick, 2));
                }
                else
                {
                    e.TotalValue = "平均=0.00%";
                }
                
            }

        }

        private void gridControl1_EndSorting(object sender, DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs e)
        {//排序完成后，显示第一行
            DevExpress.Xpf.Grid.GridControl g = (DevExpress.Xpf.Grid.GridControl)sender;
            g.View.FocusedRowHandle = 0;
        }

        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {//popup1外点击，关闭popup1
            popup1.IsOpen = false;
        }


    }
}
