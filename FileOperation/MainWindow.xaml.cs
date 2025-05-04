using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using FileOperation.ViewModels;
using System.Linq;
using CheckBox = System.Windows.Controls.CheckBox;
using FileOperation.Models;

namespace FileOperation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // 确保命令可以正常更新UI状态
            CommandManager.RequerySuggested += CommandManager_RequerySuggested;
        }
        
        /// <summary>
        /// 滚动到指定的文件项
        /// </summary>
        /// <param name="item">要滚动到的文件项</param>
        public void ScrollToItem(FileItem item)
        {
            if (item == null) return;
            
            // 查找包含DataGrid的控件
            var dataGrid = this.FindName("FileListDataGrid") as DataGrid;
            if (dataGrid == null)
            {
                // 尝试查找第一个DataGrid控件
                dataGrid = this.FindVisualChild<DataGrid>();
            }
            
            if (dataGrid != null)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(item);
            }
        }
        
        /// <summary>
        /// 查找指定类型的可视子元素
        /// </summary>
        private T FindVisualChild<T>() where T : Visual
        {
            var count = VisualTreeHelper.GetChildrenCount(this);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(this, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }
                
                var childOfChild = FindVisualChildInElement<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 在元素中查找指定类型的可视子元素
        /// </summary>
        private T FindVisualChildInElement<T>(DependencyObject element) where T : Visual
        {
            if (element == null) return null;
            
            var count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }
                
                var childOfChild = FindVisualChildInElement<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 全选/取消全选事件处理
        /// </summary>
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                bool isChecked = (sender as CheckBox)?.IsChecked ?? false;
                viewModel.IsAllSelected = isChecked;
                
                // 更新所有文件项的选中状态
                foreach (var item in viewModel.FileItems)
                {
                    item.IsSelected = isChecked;
                }
            }
        }

        private void CommandManager_RequerySuggested(object sender, EventArgs e)
        {
            // 触发所有RelayCommand的CanExecuteChanged事件
            if (DataContext is MainViewModel viewModel)
            {
                (viewModel.ScanCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (viewModel.CopyCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (viewModel.MoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (viewModel.ClearCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (viewModel.SaveConfigCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CommandManager.RequerySuggested -= CommandManager_RequerySuggested;
        }
    }
}