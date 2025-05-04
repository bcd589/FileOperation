using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace FileOperation.Converters
{
    /// <summary>
    /// 将集合索引转换为显示序号（从1开始）的转换器
    /// </summary>
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                // 确保序号从1开始递增显示，处理-1的情况（未选择项）
                return index < 0 ? "0" : (index + 1).ToString();
            }
            else if (value is DataGridRow row)
            {
                // 获取行索引并从1开始显示
                return (row.GetIndex() + 1).ToString();
            }
            else if (value == null)
            {
                // 确保序号不为空
                return "1";
            }

            return "1";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}