# KoalaWiki VSCode 插件

<div align="center">
  <img src="https://github.com/AIDotNet/koalawiki/raw/main/vscode-extension/resources/koalawiki.png" alt="KoalaWiki Logo" width="100" />
  <h3>AI驱动的代码知识库</h3>
</div>

## 简介

KoalaWiki VSCode 插件是KoalaWiki代码知识库平台的官方Visual Studio Code插件。它允许您直接在VSCode中管理和访问KoalaWiki知识库，支持本地仓库分析功能。

## 功能特点

- **本地仓库分析**: 一键添加本地项目到KoalaWiki进行AI分析
- **知识库管理**: 直接在VSCode侧边栏查看您的所有仓库及其状态
- **便捷访问**: 状态栏和侧边栏快速访问KoalaWiki平台
- **自动刷新**: 实时显示仓库分析进度

## 使用方法

### 添加仓库

有三种方式可以添加仓库到KoalaWiki：

1. **右键菜单**: 在资源管理器中右键点击文件夹，选择"添加到KoalaWiki"
2. **命令面板**: 按下`Ctrl+Shift+P`(Windows/Linux)或`Cmd+Shift+P`(Mac)，输入"KoalaWiki: 添加到KoalaWiki"
3. **当前项目**: 通过命令面板运行"KoalaWiki: 分析当前项目"命令

### 查看知识库

- 点击VSCode底部状态栏中的"KoalaWiki"图标
- 点击侧边栏KoalaWiki视图中的仓库
- 通过命令面板运行"KoalaWiki: 打开KoalaWiki"命令

## 配置选项

| 设置 | 说明 | 默认值 |
|------|------|--------|
| `koalawiki.serverUrl` | KoalaWiki服务器URL | http://localhost:5000 |
| `koalawiki.aiModel` | 使用的AI模型 | gpt-4.1 |
| `koalawiki.openAIKey` | OpenAI API密钥 | - |
| `koalawiki.openAIEndpoint` | OpenAI API端点 | https://api.openai.com |

## 系统要求

- Visual Studio Code 1.70.0 或更高版本
- KoalaWiki 服务器

## 反馈与贡献

如果您有任何问题、建议或想法，请到[GitHub项目](https://github.com/AIDotNet/koalawiki)提交Issue或PR。

## 许可证

本插件基于 [MIT 许可证](LICENSE)。

---

<div align="center">
  <sub>由 ❤️ AIDotNet 团队开发</sub>
</div> 