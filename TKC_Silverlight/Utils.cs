using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

using DevExpress.Xpf.Charts;
using System.Windows.Markup;


namespace TKC_Silverlight
{
    public class Utils
    {
        /// <summary>
        /// 十六进制颜色值转为Color对象
        /// </summary>
        /// <param name="colorName">十六进制颜色值，#号后的前两个字符是不透明度一般用FF</param>
        /// <returns>Color对象</returns>
        public static Color ConvertToColor(string colorName)
        {
            if (colorName.StartsWith("#"))
                colorName = colorName.Replace("#", string.Empty);
            int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
            return new Color()
            {
                A = Convert.ToByte((v >> 24) & 255),
                R = Convert.ToByte((v >> 16) & 255),
                G = Convert.ToByte((v >> 8) & 255),
                B = Convert.ToByte((v >> 0) & 255)
            };
        }

        /// <summary>
        /// 获取所有子控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">当前控件名</param>
        /// <param name="name">特定子控件名称，如果需要遍历全部子控件，第二个参数留空即可</param>
        /// <returns></returns>
        public static List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }

                childList.AddRange(GetChildObjects<T>(child, ""));
            }

            return childList;

        }

        /// <summary>
        /// 根据当前控件，遍历查找其父控件是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">当前控件名</param>
        /// <param name="name">要查询父控件名</param>
        /// <returns></returns>
        public T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// 根据当前控件，遍历查找其子控件是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">当前控件名</param>
        /// <param name="name">要查询子控件名</param>
        /// <returns></returns>
        public T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }

            return null;

        }

        /// <summary>
        /// 用分隔符重组字符串
        /// </summary>
        public static List<string> SplitterGroupList(List<string> lst, string splitter, int groupSize)
        {
            var lstGroupItem = new List<string>();

            if (lst == null)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < lst.Count; i += groupSize)
                {
                    string strItems = string.Join(splitter,lst.GetRange(i, (lst.Count - i < groupSize) ? lst.Count - i : groupSize));
                    lstGroupItem.Add(strItems);
                }
            }

            return lstGroupItem;
        }
    }


    public class LegendItemPresentation : DependencyObject
    {
        public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register("Series", typeof(Series), typeof(LegendItemPresentation), null);

        public XYSeries Series
        {
            get { return (XYSeries)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }
    }

    public class SeriesCollectionToListOfLegendItemPresentationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SeriesCollection seriesCollection = value as SeriesCollection;
            if (seriesCollection == null || targetType != typeof(IEnumerable))
                return null;
            else
            {
                List<LegendItemPresentation> result = new List<LegendItemPresentation>();
                foreach (XYSeries series in seriesCollection)
                    result.Add(new LegendItemPresentation() { Series = series });
                return result;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 英文的状态值到中文的转换，使用MarkupExtension后，可以直接在属性中引用该类
    /// </summary>
    public class EngStatusToChsConverter : MarkupExtension, IValueConverter
    {
        public EngStatusToChsConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string status = value.ToString();
            if (status == "online")
            {
                return "推广中";
            }
            else
            {
                return "暂停";
            }
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 英文的状态值到显示颜色的转换
    /// </summary>
    public class StatusToDisplayColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value.ToString();
            if (status == "online")
            {
                return "Green";
            }
            else
            {
                return "Orange";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RowIndicatorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int rowHanlde = (int)value;
            return rowHanlde + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
