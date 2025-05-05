import * as vscode from 'vscode';
import axios from 'axios';
import { RepositoryProvider } from './repositoryProvider';

let statusBarItem: vscode.StatusBarItem;

export async function activate(context: vscode.ExtensionContext) {
  console.log('KoalaWiki插件已激活');

  // 创建状态栏项
  statusBarItem = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Right, 100);
  statusBarItem.text = "$(book) KoalaWiki";
  statusBarItem.tooltip = "打开KoalaWiki";
  statusBarItem.command = 'koalawiki.openWiki';
  statusBarItem.show();
  context.subscriptions.push(statusBarItem);

  // 创建知识库视图提供程序
  const repositoryProvider = new RepositoryProvider();
  vscode.window.registerTreeDataProvider('koalaWikiRepositories', repositoryProvider);

  // 添加仓库命令
  const addRepositoryCommand = vscode.commands.registerCommand('koalawiki.addRepository', async (folderUri?: vscode.Uri) => {
    try {
      let folderPath: string | undefined;

      // 如果是从右键菜单调用，使用选中的文件夹
      if (folderUri) {
        folderPath = folderUri.fsPath;
      } 
      // 否则提示用户选择一个文件夹
      else {
        const options: vscode.OpenDialogOptions = {
          canSelectFiles: false,
          canSelectFolders: true,
          canSelectMany: false,
          openLabel: '选择代码仓库文件夹'
        };

        const folderUris = await vscode.window.showOpenDialog(options);
        if (folderUris && folderUris.length > 0) {
          folderPath = folderUris[0].fsPath;
        }
      }

      if (!folderPath) {
        return;
      }

      // 显示进度提示
      vscode.window.withProgress({
        location: vscode.ProgressLocation.Notification,
        title: "正在添加本地仓库到KoalaWiki",
        cancellable: false
      }, async (progress) => {
        progress.report({ increment: 10, message: "正在准备..." });

        // 获取配置
        const config = vscode.workspace.getConfiguration('koalawiki');
        const serverUrl = config.get<string>('serverUrl') || 'http://localhost:5000';
        const model = config.get<string>('aiModel') || 'gpt-4.1';
        const openAIKey = config.get<string>('openAIKey') || '';
        const openAIEndpoint = config.get<string>('openAIEndpoint') || 'https://api.openai.com';

        try {
          progress.report({ increment: 40, message: "正在上传仓库信息..." });

          // 调用API添加仓库
          const response = await axios.post(`${serverUrl}/api/warehouse`, {
            address: folderPath,
            branch: "main",
            model: model,
            openAIKey: openAIKey,
            openAIEndpoint: openAIEndpoint,
            type: "local"
          });

          progress.report({ increment: 50, message: "仓库添加成功，正在刷新..." });
          
          // 刷新视图
          repositoryProvider.refresh();

          vscode.window.showInformationMessage("本地仓库已成功添加到KoalaWiki！");
          
          return response.data;
        } catch (error) {
          console.error('添加仓库失败:', error);
          vscode.window.showErrorMessage(`添加仓库失败: ${error instanceof Error ? error.message : '未知错误'}`);
          throw error;
        }
      });
    } catch (error) {
      console.error('命令执行失败:', error);
      vscode.window.showErrorMessage(`命令执行失败: ${error instanceof Error ? error.message : '未知错误'}`);
    }
  });

  // 打开KoalaWiki命令
  const openWikiCommand = vscode.commands.registerCommand('koalawiki.openWiki', async () => {
    try {
      const config = vscode.workspace.getConfiguration('koalawiki');
      const serverUrl = config.get<string>('serverUrl') || 'http://localhost:5000';
      
      vscode.env.openExternal(vscode.Uri.parse(serverUrl));
    } catch (error) {
      vscode.window.showErrorMessage(`打开KoalaWiki失败: ${error instanceof Error ? error.message : '未知错误'}`);
    }
  });

  // 分析当前项目命令
  const analyzeCurrentProjectCommand = vscode.commands.registerCommand('koalawiki.analyzeCurrentProject', async () => {
    try {
      if (!vscode.workspace.workspaceFolders || vscode.workspace.workspaceFolders.length === 0) {
        vscode.window.showWarningMessage('请先打开一个项目文件夹');
        return;
      }

      const folderPath = vscode.workspace.workspaceFolders[0].uri.fsPath;
      
      // 直接调用添加仓库命令
      vscode.commands.executeCommand('koalawiki.addRepository', vscode.Uri.file(folderPath));
    } catch (error) {
      vscode.window.showErrorMessage(`分析当前项目失败: ${error instanceof Error ? error.message : '未知错误'}`);
    }
  });

  context.subscriptions.push(addRepositoryCommand, openWikiCommand, analyzeCurrentProjectCommand);
}

export function deactivate() {
  // 清理资源
  if (statusBarItem) {
    statusBarItem.dispose();
  }
} 