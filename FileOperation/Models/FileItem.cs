using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileOperation.Models
{
    /// <summary>
    /// 文件信息类，用于在UI中展示文件列表
    /// </summary>
    public class FileItem : INotifyPropertyChanged
    {
        private string _fullPath;
        private string _fileName;
        private long _size;
        private DateTime _lastWriteTime;
        private bool _isSelected;
        private bool _isProcessing;
        private bool _isProcessed;
        private int _processProgress;

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath
        {
            get => _fullPath;
            set
            {
                if (_fullPath != value)
                {
                    _fullPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 文件名（含扩展名）
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long Size
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormattedSize));
                }
            }
        }

        /// <summary>
        /// 格式化后的文件大小（KB/MB/GB）
        /// </summary>
        public string FormattedSize
        {
            get
            {
                if (Size < 1024)
                    return $"{Size} B";
                if (Size < 1024 * 1024)
                    return $"{Size / 1024.0:F2} KB";
                if (Size < 1024 * 1024 * 1024)
                    return $"{Size / (1024.0 * 1024.0):F2} MB";
                return $"{Size / (1024.0 * 1024.0 * 1024.0):F2} GB";
            }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastWriteTime
        {
            get => _lastWriteTime;
            set
            {
                if (_lastWriteTime != value)
                {
                    _lastWriteTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否正在处理（复制/移动）
        /// </summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                if (_isProcessing != value)
                {
                    _isProcessing = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否已处理成功
        /// </summary>
        public bool IsProcessed
        {
            get => _isProcessed;
            set
            {
                if (_isProcessed != value)
                {
                    _isProcessed = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 处理进度（0-100）
        /// </summary>
        public int ProcessProgress
        {
            get => _processProgress;
            set
            {
                if (_processProgress != value)
                {
                    _processProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}