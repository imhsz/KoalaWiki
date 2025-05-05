using FastService;
using KoalaWiki.Core.DataAccess;
using KoalaWiki.Dto;
using KoalaWiki.Entities;
using KoalaWiki.Git;
using KoalaWiki.KoalaWarehouse;
using LibGit2Sharp;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KoalaWiki.Services;

public class WarehouseService(IKoalaWikiContext access, IMapper mapper, WarehouseStore warehouseStore, ILogger<WarehouseService> logger) : FastApi
{
    /// <summary>
    /// 更新仓库信息
    /// </summary>
    private async Task UpdateWarehouseAsync(WarehouseInput input, Warehouse warehouse, HttpContext context)
    {
        warehouse.Address = input.Address;
        warehouse.Model = input.Model;
        warehouse.Status = WarehouseStatus.Pending;
        
        if (!string.IsNullOrEmpty(input.Branch))
        {
            warehouse.Branch = input.Branch;
        }
        
        // 如果有描述信息则更新
        if (!string.IsNullOrEmpty(input.Description))
        {
            warehouse.Description = input.Description;
        }
        
        access.Warehouses.Update(warehouse);
        await access.SaveChangesAsync();
        
        await context.Response.WriteAsJsonAsync(new { message = "仓库已更新，等待处理" });
    }

    /// <summary>
    /// 查询上次提交的仓库
    /// </summary>
    /// <returns></returns>
    public async Task<object> GetLastWarehouseAsync(string address)
    {
        // 判断是否.git结束，如果不是需要添加
        if (!address.EndsWith(".git"))
        {
            address += ".git";
        }

        var query = await access.Warehouses
            .AsNoTracking()
            .Where(x => x.Address == address)
            .FirstOrDefaultAsync();

        // 如果没有找到仓库，返回空列表
        if (query == null)
        {
            throw new NotFoundException("仓库不存在");
        }

        return new
        {
            query.Name,
            query.Address,
            query.Description,
            query.Version,
            query.Status,
            query.Error,
            query.Progress
        };
    }

    public async Task<DocumentCommitRecord?> GetChangeLogAsync(string owner, string name)
    {
        var warehouse = await access.Warehouses
            .AsNoTracking()
            .Where(x => x.Name == name && x.OrganizationName == owner)
            .FirstOrDefaultAsync();

        // 如果没有找到仓库，返回空列表
        if (warehouse == null)
        {
            throw new NotFoundException("仓库不存在");
        }

        
        var commit = await access.DocumentCommitRecords.FirstOrDefaultAsync(x => x.WarehouseId == warehouse.Id);


        return commit;
    }

    /// <summary>
    /// 提交仓库
    /// </summary>
    public async Task SubmitWarehouseAsync(WarehouseInput input, HttpContext context)
    {
        try
        {
            // 检查仓库类型
            bool isLocalRepository = input.Type?.ToLower() == "local";

            // 对于非本地仓库，确保地址以.git结尾
            if (!isLocalRepository && !input.Address.EndsWith(".git"))
            {
                input.Address += ".git";
            }

            // 根据仓库地址检查是否已存在
            var warehouse = await access.Warehouses.FirstOrDefaultAsync(x => x.Address == input.Address);
            if (warehouse != null)
            {
                await UpdateWarehouseAsync(input, warehouse, context);
                return;
            }

            var id = Guid.NewGuid().ToString("N");
            warehouse = new Warehouse
            {
                Id = id,
                Address = input.Address,
                Model = input.Model,
                // 设置Description字段的默认值，避免NOT NULL约束失败
                Description = input.Description ?? $"Repository from {input.Address}"
            };

            // 使用GitService解析仓库信息
            GitService gitService = new GitService();
            var (repoPath, orgName) = gitService.GetRepositoryPath(input.Address);
            warehouse.Name = Path.GetFileNameWithoutExtension(input.Address);
            warehouse.OrganizationName = orgName;
            warehouse.Status = WarehouseStatus.Pending;

            // 设置分支信息
            if (!string.IsNullOrEmpty(input.Branch))
            {
                warehouse.Branch = input.Branch;
            }
            else
            {
                warehouse.Branch = "main"; // 默认分支为main
            }

            // 设置其他必填字段的默认值
            warehouse.Error = string.Empty;
            warehouse.Prompt = string.Empty;
            warehouse.Version = "1.0.0";
            warehouse.Type = input.Type ?? "git";

            await access.Warehouses.AddAsync(warehouse);
            await access.SaveChangesAsync();

            // 返回响应
            await context.Response.WriteAsJsonAsync(new
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Organization = warehouse.OrganizationName,
                Status = warehouse.Status
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "submit warehouse error");
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// 获取仓库概述
    /// </summary>
    public async Task GetWarehouseOverviewAsync(string owner, string name, HttpContext context)
    {
        var query = await access.Warehouses
            .AsNoTracking()
            .Where(x => x.Name == name && x.OrganizationName == owner)
            .FirstOrDefaultAsync();

        // 如果没有找到仓库，返回空列表
        if (query == null)
        {
            throw new NotFoundException("仓库不存在");
        }

        var document = await access.Documents
            .AsNoTracking()
            .Where(x => x.WarehouseId == query.Id)
            .FirstOrDefaultAsync();

        var overview = await access.DocumentOverviews.FirstOrDefaultAsync(x => x.DocumentId == document.Id);

        if (overview == null)
        {
            throw new NotFoundException("没有找到概述");
        }

        await context.Response.WriteAsJsonAsync(new
        {
            content = overview.Content,
            title = overview.Title
        });
    }

    public async Task<PageDto<Warehouse>> GetWarehouseListAsync(int page, int pageSize)
    {
        var query = access.Warehouses
            .AsNoTracking();

        var total = await query.CountAsync();
        var list = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PageDto<Warehouse>(total, list);
    }
}