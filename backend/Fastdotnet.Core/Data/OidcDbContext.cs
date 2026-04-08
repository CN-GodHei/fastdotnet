using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace Fastdotnet.Core.Data
{
    /// <summary>
    /// OpenIddict 专用的 Entity Framework Core 数据库上下文
    /// 用于存储 OIDC 应用、作用域、令牌等数据
    /// </summary>
    public class OidcDbContext : DbContext
    {
        public OidcDbContext(DbContextOptions<OidcDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置 OpenIddict 实体
            modelBuilder.UseOpenIddict();
        }
    }
}
