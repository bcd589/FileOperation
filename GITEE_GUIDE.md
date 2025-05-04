# Gitee 上传指南

本文档提供了将 FileOperation 项目上传到 Gitee 的详细步骤。

## 准备工作

1. 确保您已经安装了 Git。如果没有，请从 [Git 官网](https://git-scm.com/downloads) 下载并安装。
2. 在 Gitee 上注册账号并登录。

## 创建 Gitee 仓库

1. 登录 Gitee 后，点击右上角的 "+" 按钮，选择 "新建仓库"。
2. 填写仓库信息：
   - 仓库名称：FileOperation
   - 路径：FileOperation（默认与仓库名称相同）
   - 仓库介绍：文件类型批量管理工具 - 一个用于批量处理特定类型文件的Windows桌面应用程序
   - 开源协议：MIT
   - 其他选项保持默认设置
3. 点击 "创建" 按钮完成仓库创建。

## 上传项目到 Gitee

### 方法一：使用命令行（推荐）

1. 打开命令提示符或 PowerShell，导航到项目目录：
   ```
   cd "c:\Users\anshaofeng\WPSDrive\972721572\WPS云盘\code\Csharp\FileOperation\FileOperation"
   ```

2. 初始化 Git 仓库：
   ```
   git init
   ```

3. 添加所有文件到暂存区：
   ```
   git add .
   ```

4. 提交更改：
   ```
   git commit -m "初始提交：文件类型批量管理工具"
   ```

5. 添加远程仓库：
   ```
   git remote add origin https://gitee.com/您的用户名/FileOperation.git
   ```
   注意：请将 "您的用户名" 替换为您的 Gitee 用户名。

6. 推送代码到远程仓库：
   ```
   git push -u origin master
   ```

7. 根据提示输入您的 Gitee 用户名和密码。

### 方法二：使用 Gitee 客户端

1. 下载并安装 [Gitee 客户端](https://gitee.com/app/download)。
2. 登录您的 Gitee 账号。
3. 点击 "克隆/新建" -> "新建" -> "新建仓库"。
4. 选择本地项目路径和远程仓库。
5. 点击 "创建并推送" 完成上传。

## 验证上传

1. 在浏览器中访问您的 Gitee 仓库页面：`https://gitee.com/您的用户名/FileOperation`
2. 确认所有文件都已正确上传。
3. 检查 README.md 是否正确显示。

## 后续维护

每次对项目进行更改后，您可以使用以下命令将更改推送到 Gitee：

```
git add .
git commit -m "更新说明：简要描述您的更改"
git push
```

## 常见问题

### 推送失败

如果推送失败，可能是因为远程仓库有您本地没有的更改。尝试先拉取远程更改：

```
git pull --rebase origin master
```

然后再次推送：

```
git push origin master
```

### 凭证管理

如果不想每次推送都输入用户名和密码，可以配置凭证存储：

```
git config --global credential.helper store
```

下次推送时输入的凭证将被保存。

---

如有任何问题，请参考 [Gitee 帮助文档](https://gitee.com/help) 或联系项目维护者。