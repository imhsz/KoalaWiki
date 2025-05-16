using System.IO;

namespace KoalaWiki.Utils;

/// <summary>
/// 路径处理工具类
/// </summary>
public static class PathUtils
{
    /// <summary>
    /// 处理可能的Windows驱动器路径，从中提取有效的仓库所有者和名称
    /// </summary>
    /// <param name="owner">原始所有者名称，可能是Windows驱动器路径</param>
    /// <param name="name">原始仓库名称</param>
    /// <returns>处理后的(owner, name)元组</returns>
    public static (string owner, string name) NormalizePathForRepository(string owner, string name)
    {
        // 处理Windows驱动器路径
        if (owner.EndsWith(":") && Path.IsPathRooted(owner + "\\" + name))
        {
            try
            {
                var fullPath = Path.Combine(owner, name);
                var directoryInfo = new DirectoryInfo(fullPath);
                
                // 提取实际的名称
                string normalizedName = directoryInfo.Name;
                
                // 提取父目录作为所有者
                var parent = directoryInfo.Parent;
                string normalizedOwner = parent != null && !parent.Name.EndsWith(":") 
                    ? parent.Name 
                    : "local";
                
                return (normalizedOwner, normalizedName);
            }
            catch
            {
                // 如果解析失败，返回默认值
                return ("local", name);
            }
        }
        
        // 如果不是Windows路径，返回原始值
        return (owner, name);
    }
} 