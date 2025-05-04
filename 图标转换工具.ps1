# 图标转换工具 - PowerShell脚本
# 此脚本帮助将SVG文件转换为ICO文件

# 检查是否已安装必要的模块
function Check-Prerequisites {
    Write-Host "检查必要组件..."
    
    # 检查是否安装了ImageMagick
    $magick = Get-Command magick -ErrorAction SilentlyContinue
    if (-not $magick) {
        Write-Host "未找到ImageMagick。请先安装ImageMagick，它是转换图像所必需的。" -ForegroundColor Yellow
        Write-Host "下载地址: https://imagemagick.org/script/download.php" -ForegroundColor Cyan
        return $false
    }
    
    return $true
}

# 转换SVG到ICO
function Convert-SvgToIco {
    param (
        [string]$svgPath,
        [string]$icoPath
    )
    
    if (-not (Test-Path $svgPath)) {
        Write-Host "错误: 找不到SVG文件 '$svgPath'" -ForegroundColor Red
        return $false
    }
    
    try {
        # 使用ImageMagick转换SVG到ICO
        Write-Host "正在转换SVG到ICO..."
        magick convert "$svgPath" -background transparent -define icon:auto-resize=256,128,64,48,32,16 "$icoPath"
        
        if (Test-Path $icoPath) {
            Write-Host "转换成功! ICO文件已保存到: $icoPath" -ForegroundColor Green
            return $true
        } else {
            Write-Host "转换失败: 未能生成ICO文件" -ForegroundColor Red
            return $false
        }
    } catch {
        Write-Host "转换过程中发生错误: $_" -ForegroundColor Red
        return $false
    }
}

# 主函数
function Main {
    Clear-Host
    Write-Host "===== SVG到ICO转换工具 =====" -ForegroundColor Cyan
    Write-Host 
    
    if (-not (Check-Prerequisites)) {
        Write-Host "请安装必要组件后再运行此脚本。" -ForegroundColor Yellow
        return
    }
    
    # 设置默认路径
    $defaultSvgPath = Join-Path $PSScriptRoot "FileOperation\AppIcon.svg"
    $defaultIcoPath = Join-Path $PSScriptRoot "FileOperation\AppIcon.ico"
    
    # 询问用户是否使用默认路径
    Write-Host "默认SVG文件: $defaultSvgPath"
    $useDefault = Read-Host "是否使用默认SVG文件? (Y/N)"
    
    if ($useDefault -eq "Y" -or $useDefault -eq "y") {
        $svgPath = $defaultSvgPath
    } else {
        $svgPath = Read-Host "请输入SVG文件的完整路径"
    }
    
    Write-Host "默认输出ICO文件: $defaultIcoPath"
    $useDefaultOutput = Read-Host "是否使用默认输出路径? (Y/N)"
    
    if ($useDefaultOutput -eq "Y" -or $useDefaultOutput -eq "y") {
        $icoPath = $defaultIcoPath
    } else {
        $icoPath = Read-Host "请输入ICO文件的输出完整路径"
    }
    
    # 执行转换
    $result = Convert-SvgToIco -svgPath $svgPath -icoPath $icoPath
    
    if ($result) {
        Write-Host 
        Write-Host "转换完成后，您可以运行打包脚本来创建安装程序。" -ForegroundColor Cyan
    }
    
    Write-Host 
    Write-Host "按任意键退出..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

# 运行主函数
Main