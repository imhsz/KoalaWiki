import * as vscode from 'vscode';
import axios from 'axios';

// 仓库信息接口
interface Repository {
  id: string;
  name: string;
  organizationName: string;
  description: string;
  status: string;
  progress: number;
}

// 仓库树节点类
export class RepositoryItem extends vscode.TreeItem {
  constructor(
    public readonly repository: Repository,
    public readonly collapsibleState: vscode.TreeItemCollapsibleState
  ) {
    super(repository.name, collapsibleState);

    // 设置图标和上下文
    this.iconPath = new vscode.ThemeIcon('repo');
    this.tooltip = `${repository.name} (${repository.organizationName})\n${repository.description || '无描述'}`;
    this.description = repository.status === 'Completed' 
      ? '已完成' 
      : repository.status === 'Processing' 
        ? `分析中 ${repository.progress}%` 
        : repository.status === 'Failed' 
          ? '失败' 
          : '等待中';
    
    // 设置节点的颜色
    if (repository.status === 'Failed') {
      this.iconPath = new vscode.ThemeIcon('error');
    } else if (repository.status === 'Processing') {
      this.iconPath = new vscode.ThemeIcon('sync');
    } else if (repository.status === 'Completed') {
      this.iconPath = new vscode.ThemeIcon('check');
    }

    // 定义命令
    this.command = {
      title: '查看知识库',
      command: 'koalawiki.openWiki',
      arguments: [repository.id]
    };
  }
}

// 仓库视图提供程序
export class RepositoryProvider implements vscode.TreeDataProvider<RepositoryItem> {
  private _onDidChangeTreeData: vscode.EventEmitter<RepositoryItem | undefined | null | void> = new vscode.EventEmitter<RepositoryItem | undefined | null | void>();
  readonly onDidChangeTreeData: vscode.Event<RepositoryItem | undefined | null | void> = this._onDidChangeTreeData.event;

  private refreshTimer: NodeJS.Timeout | null = null;

  constructor() {
    // 自动定时刷新
    this.startAutoRefresh();
  }

  private startAutoRefresh() {
    // 每30秒自动刷新一次
    this.refreshTimer = setInterval(() => {
      this.refresh();
    }, 30000);
  }

  private stopAutoRefresh() {
    if (this.refreshTimer) {
      clearInterval(this.refreshTimer);
      this.refreshTimer = null;
    }
  }

  dispose() {
    this.stopAutoRefresh();
  }

  refresh(): void {
    this._onDidChangeTreeData.fire();
  }

  getTreeItem(element: RepositoryItem): vscode.TreeItem {
    return element;
  }

  async getChildren(element?: RepositoryItem): Promise<RepositoryItem[]> {
    if (element) {
      // 暂时没有子节点
      return [];
    } else {
      try {
        const repositories = await this.getRepositories();
        return repositories.map(repo => new RepositoryItem(repo, vscode.TreeItemCollapsibleState.None));
      } catch (error) {
        console.error('获取仓库列表失败:', error);
        vscode.window.showErrorMessage(`获取仓库列表失败: ${error instanceof Error ? error.message : '未知错误'}`);
        return [];
      }
    }
  }

  private async getRepositories(): Promise<Repository[]> {
    try {
      const config = vscode.workspace.getConfiguration('koalawiki');
      const serverUrl = config.get<string>('serverUrl') || 'http://localhost:5000';
      
      const response = await axios.get(`${serverUrl}/api/warehouses`);
      return response.data;
    } catch (error) {
      console.error('获取仓库列表失败:', error);
      throw error;
    }
  }
} 