"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || (function () {
    var ownKeys = function(o) {
        ownKeys = Object.getOwnPropertyNames || function (o) {
            var ar = [];
            for (var k in o) if (Object.prototype.hasOwnProperty.call(o, k)) ar[ar.length] = k;
            return ar;
        };
        return ownKeys(o);
    };
    return function (mod) {
        if (mod && mod.__esModule) return mod;
        var result = {};
        if (mod != null) for (var k = ownKeys(mod), i = 0; i < k.length; i++) if (k[i] !== "default") __createBinding(result, mod, k[i]);
        __setModuleDefault(result, mod);
        return result;
    };
})();
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.RepositoryProvider = exports.RepositoryItem = void 0;
const vscode = __importStar(require("vscode"));
const axios_1 = __importDefault(require("axios"));
// 仓库树节点类
class RepositoryItem extends vscode.TreeItem {
    repository;
    collapsibleState;
    constructor(repository, collapsibleState) {
        super(repository.name, collapsibleState);
        this.repository = repository;
        this.collapsibleState = collapsibleState;
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
        }
        else if (repository.status === 'Processing') {
            this.iconPath = new vscode.ThemeIcon('sync');
        }
        else if (repository.status === 'Completed') {
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
exports.RepositoryItem = RepositoryItem;
// 仓库视图提供程序
class RepositoryProvider {
    _onDidChangeTreeData = new vscode.EventEmitter();
    onDidChangeTreeData = this._onDidChangeTreeData.event;
    refreshTimer = null;
    constructor() {
        // 自动定时刷新
        this.startAutoRefresh();
    }
    startAutoRefresh() {
        // 每30秒自动刷新一次
        this.refreshTimer = setInterval(() => {
            this.refresh();
        }, 30000);
    }
    stopAutoRefresh() {
        if (this.refreshTimer) {
            clearInterval(this.refreshTimer);
            this.refreshTimer = null;
        }
    }
    dispose() {
        this.stopAutoRefresh();
    }
    refresh() {
        this._onDidChangeTreeData.fire();
    }
    getTreeItem(element) {
        return element;
    }
    async getChildren(element) {
        if (element) {
            // 暂时没有子节点
            return [];
        }
        else {
            try {
                const repositories = await this.getRepositories();
                return repositories.map(repo => new RepositoryItem(repo, vscode.TreeItemCollapsibleState.None));
            }
            catch (error) {
                console.error('获取仓库列表失败:', error);
                vscode.window.showErrorMessage(`获取仓库列表失败: ${error instanceof Error ? error.message : '未知错误'}`);
                return [];
            }
        }
    }
    async getRepositories() {
        try {
            const config = vscode.workspace.getConfiguration('koalawiki');
            const serverUrl = config.get('serverUrl') || 'http://localhost:5000';
            const response = await axios_1.default.get(`${serverUrl}/api/warehouses`);
            return response.data;
        }
        catch (error) {
            console.error('获取仓库列表失败:', error);
            throw error;
        }
    }
}
exports.RepositoryProvider = RepositoryProvider;
//# sourceMappingURL=repositoryProvider.js.map