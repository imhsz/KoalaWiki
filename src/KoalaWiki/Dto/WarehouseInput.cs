namespace KoalaWiki.Dto;

public class WarehouseInput
{
    /// <summary>
    /// 仓库地址
    /// </summary>
    /// <returns></returns>
    public required string Address { get; set; }

    /// <summary>
    /// 使用模型
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// OpenAI 密钥
    /// </summary>
    public string OpenAIKey { get; set; } = string.Empty;

    /// <summary>
    /// OpenAI 端点
    /// </summary>
    public string OpenAIEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// 私有化git账号
    /// </summary>
    public string? GitUserName { get; set; }

    /// <summary>
    /// 私有化git密码
    /// </summary>
    public string? GitPassword { get; set; }
    
    /// <summary>
    ///  私有化git邮箱
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// 仓库类型（"git"或"local"）
    /// </summary>
    public string? Type { get; set; } = "git";
    
    /// <summary>
    /// 分支名称
    /// </summary>
    public string? Branch { get; set; } = "main";
    
    /// <summary>
    /// 仓库描述
    /// </summary>
    public string? Description { get; set; }
}