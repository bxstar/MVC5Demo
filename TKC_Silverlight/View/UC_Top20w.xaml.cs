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
    public partial class UC_Top20W : UserControl
    {
        public UC_Top20W()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Web服务代理类
        /// </summary>
        ServiceReference1.WSTopSoapClient wsProxy = new ServiceReference1.WSTopSoapClient();

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            ////先去构建树形1级目录
            wsProxy.GetProvidedCatInfoCompleted+=new EventHandler<ServiceReference1.GetProvidedCatInfoCompletedEventArgs>(wsProxy_GetProvidedCatInfoCompleted);
            wsProxy.GetProvidedCatInfoAsync();
            //client.GetTaoCiOneLevelCatsCacheAsync("");
        }



        void wsProxy_GetProvidedCatInfoCompleted(object sender, ServiceReference1.GetProvidedCatInfoCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                string result = e.Result.ToString();
                string[] strs = result.Split(',');
                string[] tmpstrs;
                if (strs.Length > 0)
                {
                    for (int i = 0; i < strs.Length; i++)
                    {
                        tmpstrs = strs[i].Split('ÿ');
                        treeRoot.Items.Add(tmpstrs[1]);
                    }
                }
                treeRoot.IsExpanded = true;
            }
        }

    }
}
