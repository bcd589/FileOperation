using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using FileOperation.Models;
using Microsoft.Win32;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using FileOperation;

namespace FileOperation.ViewModels
{
    /// <summary>
    /// 主窗口的ViewModel，处理业务逻辑
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string CONFIG_FILE_PATH = "Config.xml";
        private const int MAX_HISTORY_COUNT = 20;

        private string _sourcePath;
        private string _targetPath;
        private string _extensionsText;
        private Configuration _selectedConfiguration;
        private bool _isScanning;
        private string _statusMessage;
        private string _filePrefix;
        private bool _isAllSelected;
        private double _progressValue;
        private string _progressPercentText;
        private System.Windows.Visibility _progressBarVisibility = System.Windows.Visibility.Collapsed;
        private int _selectedConfigurationIndex;

        public ObservableCollection<FileItem> FileItems { get; private set; }
        public ObservableCollection<Configuration> Configurations { get; private set; }

        /// <summary>
        /// 源文件夹路径
        /// </summary>
        public string SourcePath
        {
            get => _sourcePath;
            set
            {
                if (_sourcePath != value)
                {
                    _sourcePath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 目标文件夹路径
        /// </summary>
        public string TargetPath
        {
            get => _targetPath;
            set
            {
                if (_targetPath != value)
                {
                    _targetPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 扩展名文本（逗号分隔，如"txt,md"）
        /// </summary>
        public string ExtensionsText
        {
            get => _extensionsText;
            set
            {
                if (_extensionsText != value)
                {
                    _extensionsText = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 选中的历史配置
        /// </summary>
        public Configuration SelectedConfiguration
        {
            get => _selectedConfiguration;
            set
            {
                if (_selectedConfiguration != value)
                {
                    _selectedConfiguration = value;
                    OnPropertyChanged();
                    if (_selectedConfiguration != null)
                    {
                        // 加载选中的配置
                        SourcePath = _selectedConfiguration.SourcePath;
                        TargetPath = _selectedConfiguration.TargetPath;
                        ExtensionsText = string.Join(",", _selectedConfiguration.Extensions);
                        // Update the index when selection changes
                        SelectedConfigurationIndex = Configurations.IndexOf(_selectedConfiguration);
                    }
                    else
                    {
                        SelectedConfigurationIndex = -1; // Reset index if nothing is selected
                    }
                }
            }
        }

        /// <summary>
        /// 选中的历史配置的索引
        /// </summary>
        public int SelectedConfigurationIndex
        {
            get => _selectedConfigurationIndex;
            set
            {
                if (_selectedConfigurationIndex != value)
                {
                    _selectedConfigurationIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否正在扫描
        /// </summary>
        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                if (_isScanning != value)
                {
                    _isScanning = value;
                    OnPropertyChanged();
                    // 更新命令可执行状态
                    (ScanCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (CopyCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (MoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (ClearCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (SaveConfigCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// 状态消息
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 文件前缀
        /// </summary>
        public string FilePrefix
        {
            get => _filePrefix;
            set
            {
                if (_filePrefix != value)
                {
                    _filePrefix = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否全选
        /// </summary>
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (_isAllSelected != value)
                {
                    _isAllSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 进度条值（0-100）
        /// </summary>
        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 进度百分比文本
        /// </summary>
        public string ProgressPercentText
        {
            get => _progressPercentText;
            set
            {
                if (_progressPercentText != value)
                {
                    _progressPercentText = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 进度条可见性
        /// </summary>
        public System.Windows.Visibility ProgressBarVisibility
        {
            get => _progressBarVisibility;
            set
            {
                if (_progressBarVisibility != value)
                {
                    _progressBarVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        // 命令
        public ICommand BrowseSourceCommand { get; private set; }
        public ICommand BrowseTargetCommand { get; private set; }
        public ICommand ScanCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand MoveCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand SaveConfigCommand { get; private set; }
        public ICommand DeleteConfigCommand { get; private set; }
        public ICommand UseCurrentTimeCommand { get; private set; }

        // 常见视频文件扩展名列表
        private readonly string[] _defaultVideoExtensions = {
            "mp4", "avi", "mkv", "mov", "wmv", "flv", "webm", "m4v", "mpg", "mpeg", "3gp", "ts", "vob"
        };

        public MainViewModel()
        {
            FileItems = new ObservableCollection<FileItem>();
            Configurations = new ObservableCollection<Configuration>();

            // 初始化命令
            BrowseSourceCommand = new RelayCommand(BrowseSource);
            BrowseTargetCommand = new RelayCommand(BrowseTarget);
            ScanCommand = new RelayCommand(ScanFiles, CanScan);
            CopyCommand = new RelayCommand(CopyFiles, CanCopyOrMove);
            MoveCommand = new RelayCommand(MoveFiles, CanCopyOrMove);
            ClearCommand = new RelayCommand(ClearFiles, CanClear);
            SaveConfigCommand = new RelayCommand(SaveConfig, CanSaveConfig);
            DeleteConfigCommand = new RelayCommand(DeleteConfig);
            UseCurrentTimeCommand = new RelayCommand(UseCurrentTime);

            // 设置默认视频文件扩展名
            ExtensionsText = string.Join(",", _defaultVideoExtensions);

            // 设置默认源路径为用户下载文件夹
            SourcePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            
            // 设置默认目标路径为桌面下的"整理"文件夹
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            TargetPath = Path.Combine(desktopPath, "整理");

            // 加载历史配置
            LoadConfigurations();
            
            // 如果有历史配置，自动加载最后一次配置
            if (Configurations.Count > 0)
            {
                SelectedConfiguration = Configurations[0];
            }
        }

        /// <summary>
        /// 浏览源文件夹
        /// </summary>
        private void BrowseSource(object parameter)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "选择源文件夹",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SourcePath = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 浏览目标文件夹
        /// </summary>
        private void BrowseTarget(object parameter)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "选择目标文件夹",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TargetPath = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 扫描文件
        /// </summary>
        private async void ScanFiles(object parameter)
        {
            if (string.IsNullOrWhiteSpace(SourcePath))
            {
                System.Windows.MessageBox.Show("请选择源文件夹！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Directory.Exists(SourcePath))
            {
                System.Windows.MessageBox.Show("源路径不存在，请重新选择！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(ExtensionsText))
            {
                System.Windows.MessageBox.Show("请输入文件扩展名！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 解析扩展名
            var extensions = ExtensionsText.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ext => ext.Trim().ToLower())
                .Where(ext => !string.IsNullOrWhiteSpace(ext))
                .Select(ext => ext.StartsWith(".") ? ext : "." + ext)
                .Distinct()
                .ToList();

            if (extensions.Count == 0)
            {
                System.Windows.MessageBox.Show("请输入有效的文件扩展名！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsScanning = true;
            StatusMessage = "正在扫描文件...";
            FileItems.Clear();
            
            // 重置全选按钮状态
            IsAllSelected = false;

            try
            {
                await Task.Run(() =>
                {
                    var files = new List<FileItem>();
                    var searchOption = SearchOption.AllDirectories;

                    try
                    {
                        foreach (var extension in extensions)
                        {
                            try
                            {
                                var matchedFiles = Directory.EnumerateFiles(SourcePath, "*" + extension, searchOption);
                                foreach (var filePath in matchedFiles)
                                {
                                    try
                                    {
                                        var fileInfo = new FileInfo(filePath);
                                        var fileItem = new FileItem
                                        {
                                            FullPath = filePath,
                                            FileName = fileInfo.Name,
                                            Size = fileInfo.Length,
                                            LastWriteTime = fileInfo.LastWriteTime,
                                            IsSelected = false
                                        };
                                        files.Add(fileItem);
                                    }
                                    catch (Exception ex)
                                    {
                                        // 忽略单个文件的访问错误
                                        System.Diagnostics.Debug.WriteLine($"访问文件出错：{filePath}, {ex.Message}");
                                    }
                                }
                            }
                            catch (UnauthorizedAccessException)
                            {
                                // 忽略无权限访问的文件夹
                                System.Diagnostics.Debug.WriteLine($"无权限访问文件夹：{SourcePath}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"扫描文件时出错：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }

                    // 在UI线程更新集合
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var file in files)
                        {
                            FileItems.Add(file);
                        }
                        StatusMessage = $"找到 {FileItems.Count} 个文件";
                    });
                });
            }
            finally
            {
                IsScanning = false;
            }
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        private async void CopyFiles(object parameter)
        {
            await ProcessFiles(false);
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        private async void MoveFiles(object parameter)
        {
            await ProcessFiles(true);
        }

        /// <summary>
        /// 处理文件（复制或移动）
        /// </summary>
        /// <summary>
        /// 使用当前时间作为文件前缀
        /// </summary>
        private void UseCurrentTime(object parameter)
        {
            // 使用当前时间作为前缀，格式：yyyyMMdd_HHmmss_
            FilePrefix = DateTime.Now.ToString("yyyyMMdd_HHmmss_");
        }
        
        /// <summary>
        /// 处理文件（复制或移动）
        /// </summary>
        private async Task ProcessFiles(bool isMove)
        {
            if (string.IsNullOrWhiteSpace(TargetPath))
            {
                System.Windows.MessageBox.Show("请选择目标文件夹！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Directory.Exists(TargetPath))
            {
                // 检查是否是默认的"整理"文件夹
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string defaultTargetPath = Path.Combine(desktopPath, "整理");
                
                if (TargetPath == defaultTargetPath)
                {
                    // 自动创建默认的"整理"文件夹
                    try
                    {
                        Directory.CreateDirectory(TargetPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"创建目标路径失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    // 对于非默认路径，询问用户是否创建
                    var result = MessageBox.Show("目标路径不存在，是否创建？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Directory.CreateDirectory(TargetPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"创建目标路径失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            var selectedFiles = FileItems.Where(f => f.IsSelected).ToList();
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("请选择要操作的文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsScanning = true;
            StatusMessage = isMove ? "正在移动文件..." : "正在复制文件...";
            
            // 显示进度条并初始化进度
            ProgressBarVisibility = System.Windows.Visibility.Visible;
            ProgressValue = 0;
            ProgressPercentText = "0%";
            
            // 重置所有文件的处理状态
            foreach (var item in FileItems)
            {
                item.IsProcessing = false;
            }

            try
            {
                bool overwriteAll = false;
                bool skipAll = false;

                await Task.Run(async () =>
                {
                    int successCount = 0;
                    int failCount = 0;
                    int totalFiles = selectedFiles.Count;
                    int processedFiles = 0;

                    foreach (var file in selectedFiles)
                    {
                        try
                        {
                            // 更新状态消息并标记当前处理的文件
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                // 重置所有文件的处理状态
                                foreach (var item in FileItems)
                                {
                                    item.IsProcessing = false;
                                }
                                // 设置当前文件为处理中
                                file.IsProcessing = true;
                                // 设置当前文件的进度为0，开始处理
                                file.ProcessProgress = 0;
                                StatusMessage = $"{(isMove ? "正在移动" : "正在复制")}文件 ({processedFiles + 1}/{totalFiles})";
                                
                                // 更新进度条显示总体进度
                                double progressPercent = (double)processedFiles / totalFiles * 100;
                                ProgressValue = progressPercent;
                                ProgressPercentText = $"{Math.Round(progressPercent)}%";
                                
                                // 滚动到当前处理的文件，确保它在可视范围内
                                if (Application.Current.MainWindow is MainWindow mainWindow)
                                {
                                    mainWindow.ScrollToItem(file);
                                }
                            });
                            
                            // 模拟文件处理进度更新
                            for (int progress = 0; progress <= 100; progress += 10)
                            {
                                await Application.Current.Dispatcher.InvokeAsync(() =>
                                {
                                    file.ProcessProgress = progress;
                                });
                                
                                // 添加短暂延迟以便UI更新
                                await Task.Delay(50);
                            }
                            
                            // 设置文件为已处理，保留进度条
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                file.IsProcessed = true;
                            });
                            
                            // 处理完成后增加已处理文件计数
                            processedFiles++;

                            // 处理文件名前缀
                            string fileName = file.FileName;
                            if (!string.IsNullOrEmpty(FilePrefix))
                            {
                                fileName = FilePrefix + fileName;
                            }

                            string targetFilePath = Path.Combine(TargetPath, fileName);
                            bool proceed = true;

                            // 检查目标文件是否存在
                            if (File.Exists(targetFilePath))
                            {
                                // 自动处理重名文件，添加序号
                                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                                string fileExt = Path.GetExtension(fileName);
                                int counter = 1;
                                string newTargetPath = targetFilePath;

                                while (File.Exists(newTargetPath))
                                {
                                    string newFileName = $"{fileNameWithoutExt}({counter}){fileExt}";
                                    newTargetPath = Path.Combine(TargetPath, newFileName);
                                    counter++;
                                }

                                targetFilePath = newTargetPath;
                            }

                            if (proceed && !skipAll)
                            {
                                if (isMove)
                                {
                                    File.Move(file.FullPath, targetFilePath);
                                }
                                else
                                {
                                    File.Copy(file.FullPath, targetFilePath, overwriteAll);
                                }
                                successCount++;
                                
                                // 标记文件为已处理成功
                                await Application.Current.Dispatcher.InvokeAsync(() =>
                                {
                                    file.IsProcessed = true;
                                    // 确保进度条显示100%完成
                                    file.ProcessProgress = 100;
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            System.Diagnostics.Debug.WriteLine($"处理文件出错：{file.FullPath}, {ex.Message}");
                        }
                    }

                    // 更新状态消息
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        // 重置所有文件的处理中状态
                        foreach (var item in FileItems)
                        {
                            item.IsProcessing = false;
                            // 如果文件未被处理成功，重置进度条
                            if (!item.IsProcessed)
                            {
                                item.ProcessProgress = 0;
                            }
                            // 注意：保留IsProcessed状态，以便显示绿色背景
                        }

                        // 设置进度条为100%完成
                        ProgressValue = 100;
                        ProgressPercentText = "100%";
                        StatusMessage = $"{(isMove ? "移动" : "复制")}完成：成功 {successCount} 个，失败 {failCount} 个，总计 {totalFiles} 个";

                        // 如果是移动操作，从列表中移除成功移动的文件
                        if (isMove && successCount > 0)
                        {
                            var movedFiles = selectedFiles.Where(f => !File.Exists(f.FullPath)).ToList();
                            foreach (var file in movedFiles)
                            {
                                FileItems.Remove(file);
                            }
                        }
                    });
                });
            }
            finally
            {
                IsScanning = false;
                // 操作完成后隐藏进度条
                await Task.Delay(1000); // 延迟一秒后隐藏进度条，让用户能看到100%的状态
                ProgressBarVisibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 清空文件列表
        /// </summary>
        private void ClearFiles(object parameter)
        {
            FileItems.Clear();
            StatusMessage = "文件列表已清空";
        }

        /// <summary>
        /// 保存当前配置
        /// </summary>
        private void SaveConfig(object parameter)
        {
            if (string.IsNullOrWhiteSpace(SourcePath) || string.IsNullOrWhiteSpace(TargetPath) || string.IsNullOrWhiteSpace(ExtensionsText))
            {
                MessageBox.Show("请填写完整的配置信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 解析扩展名
            var extensions = ExtensionsText.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ext => ext.Trim().ToLower())
                .Where(ext => !string.IsNullOrWhiteSpace(ext))
                .Select(ext => ext.StartsWith(".") ? ext.Substring(1) : ext)
                .Distinct()
                .ToList();

            if (extensions.Count == 0)
            {
                MessageBox.Show("请输入有效的文件扩展名！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 检查是否已存在相同配置
            var existingConfig = Configurations.FirstOrDefault(c =>
                c.SourcePath == SourcePath &&
                c.TargetPath == TargetPath &&
                c.Extensions.OrderBy(e => e).SequenceEqual(extensions.OrderBy(e => e)));

            if (existingConfig != null)
            {
                // 更新现有配置的时间
                existingConfig.CreateTime = DateTime.Now;
            }
            else
            {
                // 创建新配置
                var newConfig = new Configuration
                {
                    Id = Configurations.Count > 0 ? Configurations.Max(c => c.Id) + 1 : 1,
                    SourcePath = SourcePath,
                    TargetPath = TargetPath,
                    Extensions = extensions,
                    CreateTime = DateTime.Now
                };

                Configurations.Add(newConfig);

                // 限制历史配置数量
                if (Configurations.Count > MAX_HISTORY_COUNT)
                {
                    var oldestConfig = Configurations.OrderBy(c => c.CreateTime).First();
                    Configurations.Remove(oldestConfig);
                }
            }

            // 保存配置到XML文件
            SaveConfigurations();
            StatusMessage = "配置已保存";
        }

        /// <summary>
        /// 删除历史配置
        /// </summary>
        private void DeleteConfig(object parameter)
        {
            if (parameter is Configuration config)
            {
                int index = Configurations.IndexOf(config) + 1; // 显示给用户的序号从1开始
                var result = MessageBox.Show($"确定要删除配置 [{index}] {config.SourcePath} 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Configurations.Remove(config);
                    SaveConfigurations();
                    StatusMessage = "配置已删除";
                }
            }
        }

        /// <summary>
        /// 加载历史配置
        /// </summary>
        private void LoadConfigurations()
        {
            try
            {
                if (File.Exists(CONFIG_FILE_PATH))
                {
                    using (var stream = new FileStream(CONFIG_FILE_PATH, FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(List<Configuration>));
                        var configs = (List<Configuration>)serializer.Deserialize(stream);

                        Configurations.Clear();
                        foreach (var config in configs.OrderByDescending(c => c.CreateTime))
                        {
                            Configurations.Add(config);
                        }

                        StatusMessage = $"已加载 {Configurations.Count} 条历史配置";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载配置出错：{ex.Message}");
                StatusMessage = "加载历史配置失败";
            }
        }

        /// <summary>
        /// 保存历史配置
        /// </summary>
        private void SaveConfigurations()
        {
            try
            {
                using (var stream = new FileStream(CONFIG_FILE_PATH, FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(List<Configuration>));
                    serializer.Serialize(stream, Configurations.ToList());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存配置出错：{ex.Message}");
                MessageBox.Show($"保存配置失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region 命令可执行条件
        private bool CanScan(object parameter)
        {
            return !IsScanning;
        }

        private bool CanCopyOrMove(object parameter)
        {
            return !IsScanning && FileItems.Count > 0 && FileItems.Any(f => f.IsSelected);
        }

        private bool CanClear(object parameter)
        {
            return !IsScanning && FileItems.Count > 0;
        }

        private bool CanSaveConfig(object parameter)
        {
            return !IsScanning && !string.IsNullOrWhiteSpace(SourcePath) && !string.IsNullOrWhiteSpace(TargetPath) && !string.IsNullOrWhiteSpace(ExtensionsText);
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    /// <summary>
    /// 命令实现类
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}