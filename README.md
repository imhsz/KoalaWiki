# KoalaWiki

<div align="center">
  <img src="https://github.com/user-attachments/assets/f91e3fe7-ef4d-4cfb-8b57-36eb1c449238" alt="KoalaWiki Logo" width="200" />
  <h3>AI驱动的代码知识库</h3>
</div>

## 📖 项目介绍

KoalaWiki 是一个强大的AI驱动代码知识库平台，可以自动分析您的代码仓库，生成详细的文档和见解，帮助开发团队更深入地理解代码结构和工作原理。无论是新加入团队的开发人员快速上手，还是项目维护者梳理代码逻辑，KoalaWiki 都能提供智能化的辅助。

## ✨ 核心功能

- **仓库管理**：支持添加和管理多个Git代码仓库
- **本地仓库支持**：分析本地文件夹代码，无需Git仓库
- **AI代码分析**：利用先进的AI技术分析代码结构和关系
- **自动文档生成**：自动为代码库生成详细的文档
- **知识库导航**：直观的目录树结构，便于浏览和查找
- **支持多种模型**：集成OpenAI等多种AI模型，灵活配置
- **VSCode插件**：提供VSCode插件便捷使用

## 🔧 技术栈

### 后端
- .NET 8.0/9.0
- Microsoft Semantic Kernel
- Entity Framework Core
- FastService API
- SQLite 数据库
- LibGit2Sharp

### 前端
- Next.js 15.3
- React 19
- Ant Design 5.24
- TypeScript
- Markdown 渲染支持

### VSCode 插件
- TypeScript
- VSCode Extension API
- Axios

## 🚀 快速开始

### 系统要求
- .NET 8.0 SDK 或更高版本
- Node.js 18+ 和 npm/yarn
- Git

### 安装步骤

1. **安装.NET SDK**
   ```
   # 从 https://dotnet.microsoft.com/download/dotnet/8.0 下载.NET 8.0 SDK
   # 安装后，验证安装
   dotnet --version
   ```

2. **克隆项目**
   ```
   git clone https://github.com/yourusername/koalawiki.git
   cd koalawiki
   ```

3. **运行后端**
   ```
   cd src/KoalaWiki
   dotnet run
   ```

4. **运行前端**
   ```
   cd web
   npm install
   npm run dev
   ```

5. **VSCode插件打包**
   ```
   cd vscode-extension
   npm install
   npm run package
   ```
   打包后的.vsix文件可在插件目录中找到，可通过VSCode的"从VSIX安装..."功能安装。

## 💡 使用指南

### 添加仓库

1. 通过Web界面添加Git仓库
2. 使用VSCode插件添加本地仓库
3. 支持私有仓库（需填写凭据）

### 访问知识库

1. 打开Web界面浏览仓库内容
2. 通过VSCode插件直接访问

## 🔄 提交Git和打包插件

### 提交Git

```bash
# 添加所有更改
git add .

# 提交更改
git commit -m "你的提交信息"

# 推送到远程仓库
git push origin main
```

### 打包VSCode插件

```bash
# 进入插件目录
cd vscode-extension

# 安装依赖
npm install

# 编译TypeScript
npm run compile

# 打包插件
npm run package

# 或者使用打包脚本
node scripts/package.js
```

## 🤝 贡献指南

欢迎贡献代码、报告问题或提出改进建议！

## 📝 许可证

[MIT](LICENSE)

## 📚 相关资源

- [项目博客](https://github.com/AIDotNet/koalawiki/blog)
- [API文档](https://github.com/AIDotNet/koalawiki/api-docs)
- [使用教程](https://github.com/AIDotNet/koalawiki/tutorials)

---

<div align="center">
  <sub>由 ❤️ AIDotNet 团队开发</sub>
</div>
