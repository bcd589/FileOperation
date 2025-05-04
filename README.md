# 文件类型批量管理工具

## 项目简介

文件类型批量管理工具是一个用于批量处理特定类型文件的Windows桌面应用程序。该工具可以帮助用户快速查找、复制、移动特定扩展名的文件，提高文件管理效率。

## 主要功能

- 按扩展名筛选文件
- 批量复制/移动文件
- 保存常用配置
- 显示文件详细信息（大小、修改时间等）
- 操作进度可视化

## 技术栈

- 开发语言：C#
- 界面框架：WPF (Windows Presentation Foundation)
- 目标框架：.NET 9.0 Windows

## 安装说明

### 方式一：使用安装程序

1. 下载最新的安装程序 `FileOperationSetup.exe`
2. 运行安装程序，按照向导完成安装
3. 从开始菜单或桌面快捷方式启动应用程序

### 方式二：直接运行

1. 确保您的系统已安装 .NET 9.0 运行时
2. 下载并解压应用程序文件
3. 运行 `FileOperation.exe`

## 打包说明

详细的打包流程请参考项目根目录下的 `打包说明.md` 文件，其中包含：

- 图标生成方法
- Inno Setup 安装配置
- 发布和打包步骤

## 开发环境设置

1. 安装 Visual Studio 2022 或更高版本
2. 安装 .NET 9.0 SDK
3. 克隆或下载本仓库
4. 使用 Visual Studio 打开解决方案文件 `FileOperation.sln`
5. 构建并运行项目

## 自定义图标

项目中已包含 SVG 格式的应用图标 `AppIcon.svg`，您可以按照 `打包说明.md` 中的步骤将其转换为 ICO 格式并应用到应用程序中。

## 贡献指南

欢迎对本项目进行贡献！以下是参与贡献的步骤：

1. Fork 本仓库
2. 创建您的特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交您的更改 (`git commit -m '添加了某某功能'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启一个 Pull Request



## 许可证

本项目采用 MIT 许可证。详情请参阅 LICENSE 文件。