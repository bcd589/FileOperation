
# 文件类型批量管理工具 开发设计文档


## 一、项目概述
### 1.1 项目背景
在日常开发/运维场景中，经常需要对分散在不同目录下的特定类型文件（如`.cs`/`.png`/`.config`）进行集中管理。本工具旨在通过可视化界面实现：  
- 自定义多文件类型筛选（如同时选择`.txt`和`.md`）  
- 历史配置快速复用（避免重复输入路径和类型）  
- 实时查看遍历结果并执行复制/移动操作  


## 二、功能需求
### 2.1 核心功能清单
| 功能模块         | 需求描述                                                                 |
|------------------|--------------------------------------------------------------------------|
| **配置管理**     | 支持通过XML保存以下信息：<br>- 目标文件夹路径（源路径/目标路径）<br>- 文件扩展名列表（如`["txt","md"]`）<br>- 历史配置列表（最多保存20条） |
| **文件遍历**     | 递归扫描指定源文件夹及其所有子文件夹，筛选符合扩展名条件的文件，输出包含完整路径的文件列表 |
| **可视化交互**   | - 提供扩展名输入框（支持逗号分隔输入，如`txt,md`）<br>- 提供路径选择按钮（调用系统文件选择器）<br>- 历史配置下拉列表（选择后自动填充路径和扩展名）<br>- 文件列表展示（包含文件名/路径/大小/修改时间） |
| **文件操作**     | 支持单选/多选文件，执行复制或移动操作（需提示覆盖风险）                  |
| **异常处理**     | 对无权限访问的文件夹、已删除的文件、目标路径不存在等场景提供明确提示    |


## 三、技术选型
| 技术栈           | 选择原因                                                                 |
|------------------|--------------------------------------------------------------------------|
| 开发框架         | WPF（.NET8）：支持丰富的UI组件和数据绑定，适合Windows桌面应用开发         |
| 配置存储         | XML：结构清晰，支持嵌套数据，便于历史配置的序列化/反序列化               |
| 架构模式         | MVVM：分离UI逻辑与业务逻辑，提升可测试性和可维护性                       |
| 文件操作         | `System.IO`命名空间：提供`Directory.EnumerateFiles`（递归遍历）、`File.Copy`等API |
| 界面控件         | `DataGrid`（文件列表展示）、`ComboBox`（历史配置选择）、`TextBox`（扩展名输入） |


## 四、架构设计
### 4.1 分层架构图
```
UI层（View）         →  XAML界面（MainWindow.xaml）
                数据绑定
业务逻辑层（ViewModel）→  MainViewModel（处理遍历/操作逻辑）
                依赖注入
数据层（Model）       →  Configuration（配置类）、FileItem（文件信息类）
                序列化
存储层               →  Config.xml（历史配置）、临时文件列表（内存缓存）
```

### 4.2 核心类设计
#### 4.2.1 `Configuration` 配置类
```csharp
public class Configuration
{
    public int Id { get; set; } // 历史配置ID
    public string SourcePath { get; set; } // 源文件夹路径
    public string TargetPath { get; set; } // 目标文件夹路径
    public List<string> Extensions { get; set; } // 扩展名列表（无点号，如["txt","md"]）
    public DateTime CreateTime { get; set; } // 配置创建时间
}
```

#### 4.2.2 `FileItem` 文件信息类
```csharp
public class FileItem : INotifyPropertyChanged
{
    public string FullPath { get; set; } // 完整路径
    public string FileName { get; set; } // 文件名（含扩展名）
    public long Size { get; set; } // 文件大小（字节）
    public DateTime LastWriteTime { get; set; } // 最后修改时间
    public bool IsSelected { get; set; } // 是否选中（用于操作多选）
}
```


## 五、界面设计
### 5.1 主界面布局（草图）
```
┌───────────────────────────────────────────────────┐
│ [源路径] [浏览按钮]                               │
│ [目标路径] [浏览按钮]                             │
│ [扩展名]（输入框，提示：如"txt,md"）             │
│ [历史配置]（下拉列表） [保存配置按钮]             │
├───────────────────────────────────────────────────┤
│ 文件列表（DataGrid）                              │
│ 列：√（选择框） | 文件名 | 路径 | 大小 | 修改时间  │
├───────────────────────────────────────────────────┤
│ [开始扫描] [复制选中] [移动选中] [清空列表]       │
└───────────────────────────────────────────────────┘
```

### 5.2 关键交互逻辑
1. **历史配置加载**：启动时读取`Config.xml`，反序列化为`List<Configuration>`并绑定到`ComboBox`。  
2. **扩展名验证**：输入时自动过滤非法字符（如`*`/`?`），转换为小写并去重（如`TXT,Md` → `["txt","md"]`）。  
3. **文件扫描**：点击`开始扫描`后，异步执行`Directory.EnumerateFiles`（避免UI卡顿），筛选符合扩展名的文件并填充到`DataGrid`。  
4. **操作确认**：执行复制/移动前，若目标路径存在同名文件，弹出提示框：`是否覆盖？`（可选`全部覆盖`/`跳过`）。  


## 六、配置管理
### 6.1 XML存储格式（示例）
```xml
<?xml version="1.0" encoding="utf-8"?>
<Configurations>
  <Configuration>
    <Id>1</Id>
    <SourcePath>D:\Project\Docs</SourcePath>
    <TargetPath>E:\Backup\Docs</TargetPath>
    <Extensions>
      <Extension>txt</Extension>
      <Extension>md</Extension>
    </Extensions>
    <CreateTime>2024-05-01T14:30:00</CreateTime>
  </Configuration>
  <Configuration>
    <Id>2</Id>
    <SourcePath>C:\Pictures</SourcePath>
    <TargetPath>D:\Backup\Pics</TargetPath>
    <Extensions>
      <Extension>png</Extension>
      <Extension>jpg</Extension>
    </Extensions>
    <CreateTime>2024-05-02T09:15:00</CreateTime>
  </Configuration>
</Configurations>
```

### 6.2 配置更新规则
- 新配置保存时，若已存在相同源路径、目标路径和扩展名的记录，则更新`CreateTime`为当前时间（避免重复）。  
- 历史配置超过20条时，删除最早的1条（按`CreateTime`排序）。  


## 七、异常处理
| 异常场景                 | 处理方式                                                                 |
|--------------------------|--------------------------------------------------------------------------|
| 源路径不存在             | 弹出提示：`源路径不存在，请重新选择！`                                   |
| 无权限访问子文件夹       | 在文件列表中标记该文件夹（如红色背景），并在日志中记录`访问被拒绝：{路径}` |
| 目标路径不可写           | 弹出提示：`目标路径无写入权限，请检查！`                                 |
| 文件正在被其他进程占用   | 弹出提示：`文件被占用，无法操作：{文件名}`                               |


## 八、测试计划
### 8.1 功能测试
| 测试项                 | 测试用例                                                                 |
|------------------------|--------------------------------------------------------------------------|
| 多扩展名筛选           | 输入`txt,md`，扫描包含`.txt`/`.md`/`.docx`的文件夹，验证仅前两种文件被列出 |
| 历史配置复用           | 保存一条配置后，重启程序，从下拉列表选择该配置，验证路径和扩展名自动填充   |
| 复制操作               | 选择1个文件复制到目标路径，验证文件存在且内容一致；选择多个文件重复复制，验证覆盖逻辑 |
| 移动操作               | 移动后验证源路径文件消失，目标路径文件存在；移动过程中中断（如拔U盘），验证源文件恢复 |

### 8.2 性能测试
- 扫描10000个文件的文件夹，记录扫描完成时间（要求≤5秒）。  
- 同时复制100个500KB的文件，验证UI无卡顿（帧率≥30FPS）。  


## 九、附录
### 9.1 参考资料
- [.NET8 WPF 官方文档](https://learn.microsoft.com/zh-cn/dotnet/desktop/wpf/)  
- [XML 序列化指南](https://learn.microsoft.com/zh-cn/dotnet/standard/serialization/introducing-xml-serialization)  
- [Directory.EnumerateFiles 用法](https://learn.microsoft.com/zh-cn/dotnet/api/system.io.directory.enumeratefiles)