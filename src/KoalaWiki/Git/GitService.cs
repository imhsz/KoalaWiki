using System.ComponentModel;
using LibGit2Sharp;

namespace KoalaWiki.Git;

public class GitService
{
    public (string localPath, string organization) GetRepositoryPath(string repositoryUrl)
    {
        // 检查是否为本地路径
        if (Path.IsPathRooted(repositoryUrl) || repositoryUrl.StartsWith("./") || repositoryUrl.StartsWith("../"))
        {
            // 如果是本地路径，直接使用该路径作为仓库路径
            var directoryInfo = new DirectoryInfo(repositoryUrl);
            var repositoryName = directoryInfo.Name;
            var orgName = directoryInfo.Parent?.Name ?? "local";
            
            // 对于本地仓库，我们可以选择复制到统一的仓库管理目录下，或直接使用原路径
            // 这里选择复制到统一管理目录
            var localRepoPath = Path.Combine(Constant.GitPath, orgName, repositoryName);
            return (localRepoPath, orgName);
        }
        
        // 解析远程仓库地址
        var uri = new Uri(repositoryUrl);
        var paths = uri.AbsolutePath.TrimStart('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        var organization = paths[0];
        var repository = paths[1].EndsWith(".git") ? paths[1].Replace(".git", "") : paths[1];
        var localPath = Path.Combine(Constant.GitPath, organization, repository);
        return (localPath, organization);
    }

    /// <summary>
    /// 拉取指定仓库
    /// </summary>
    /// <returns></returns>
    public GitRepositoryInfo PullRepository(
        [Description("仓库地址")] string repositoryUrl,
        string userName = "",
        string password = "",
        string email = "",
        [Description("分支")] string branch = "master")
    {
        var (localPath, organization) = GetRepositoryPath(repositoryUrl);

        // 检查是否为本地路径
        bool isLocalPath = Path.IsPathRooted(repositoryUrl) || repositoryUrl.StartsWith("./") || repositoryUrl.StartsWith("../");

        var names = repositoryUrl.Split('/');
        var repositoryName = isLocalPath ? new DirectoryInfo(repositoryUrl).Name : names[^1].Replace(".git", "");

        if (isLocalPath)
        {
            // 对于本地路径，检查是否为Git仓库
            if (Repository.IsValid(repositoryUrl))
            {
                // 如果是Git仓库，复制文件到目标路径
                if (!Directory.Exists(localPath))
                {
                    // 创建目标目录
                    Directory.CreateDirectory(localPath);
                    
                    // 复制所有文件和子目录
                    CopyDirectory(repositoryUrl, localPath);
                }
                
                // 获取仓库信息
                using var repo = new Repository(localPath);
                var branchName = repo.Head.FriendlyName;
                var version = repo.Head.Tip?.Sha ?? "无版本信息";
                var commitTime = repo.Head.Tip?.Committer.When.ToString() ?? DateTime.Now.ToString();
                var commitAuthor = repo.Head.Tip?.Committer.Name ?? "本地用户";
                var commitMessage = repo.Head.Tip?.Message ?? "本地导入";

                return new GitRepositoryInfo(localPath, repositoryName, organization, branchName, commitTime,
                    commitAuthor, commitMessage, version);
            }
            else
            {
                // 如果不是Git仓库，直接复制文件
                if (!Directory.Exists(localPath))
                {
                    // 创建目标目录
                    Directory.CreateDirectory(localPath);
                    
                    // 复制所有文件和子目录
                    CopyDirectory(repositoryUrl, localPath);
                }
                
                // 创建一个新的Git仓库
                Repository.Init(localPath);
                
                // 返回基本信息
                return new GitRepositoryInfo(localPath, repositoryName, organization, "main", 
                    DateTime.Now.ToString(), "本地用户", "初始导入", "无版本信息");
            }
        }
        else
        {
            var cloneOptions = new CloneOptions
            {
                FetchOptions =
                {
                    CertificateCheck = (certificate, chain, errors) => true,
                    Depth = 0,
                }
            };

            // 判断仓库是否已经存在
            if (Directory.Exists(localPath))
            {
                // 获取当前仓库的git分支
                using var repo = new Repository(localPath);
                var branchName = repo.Head.FriendlyName;
                // 获取当前仓库的git版本
                var version = repo.Head.Tip.Sha;
                // 获取当前仓库的git提交时间
                var commitTime = repo.Head.Tip.Committer.When;
                // 获取当前仓库的git提交人
                var commitAuthor = repo.Head.Tip.Committer.Name;
                // 获取当前仓库的git提交信息
                var commitMessage = repo.Head.Tip.Message;

                return new GitRepositoryInfo(localPath, repositoryName, organization, branchName, commitTime.ToString(),
                    commitAuthor, commitMessage, version);
            }
            else
            {
                if (string.IsNullOrEmpty(userName))
                {
                    var str = Repository.Clone(repositoryUrl, localPath, cloneOptions);
                }
                else
                {
                    var info = Directory.CreateDirectory(localPath);

                    cloneOptions = new CloneOptions
                    {
                        FetchOptions =
                        {
                            Depth = 0,
                            CertificateCheck = (certificate, chain, errors) => true,
                            CredentialsProvider = (_url, _user, _cred) =>
                                new UsernamePasswordCredentials
                                {
                                    Username = userName, // 对于Token认证，Username可以随便填
                                    Password = password
                                }
                        }
                    };

                    Repository.Clone(repositoryUrl, localPath, cloneOptions);
                }

                // 获取当前仓库的git分支
                using var repo = new Repository(localPath);
                var branchName = repo.Head.FriendlyName;
                // 获取当前仓库的git版本
                var version = repo.Head.Tip.Sha;
                // 获取当前仓库的git提交时间
                var commitTime = repo.Head.Tip.Committer.When;
                // 获取当前仓库的git提交人
                var commitAuthor = repo.Head.Tip.Committer.Name;
                // 获取当前仓库的git提交信息
                var commitMessage = repo.Head.Tip.Message;

                return new GitRepositoryInfo(localPath, repositoryName, organization, branchName, commitTime.ToString(),
                    commitAuthor, commitMessage, version);
            }
        }
    }
    
    /// <summary>
    /// 复制目录及其内容到目标位置
    /// </summary>
    private void CopyDirectory(string sourceDir, string targetDir)
    {
        // 创建目标目录
        Directory.CreateDirectory(targetDir);

        // 复制文件
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            var destFile = Path.Combine(targetDir, fileName);
            File.Copy(file, destFile, true);
        }

        // 复制子目录
        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            var dirName = Path.GetFileName(directory);
            // 跳过.git目录
            if (dirName == ".git")
                continue;
                
            var destDir = Path.Combine(targetDir, dirName);
            CopyDirectory(directory, destDir);
        }
    }
}