using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jaryway.DynamicSpace.IdentityServer.Data
{
    public class AspNetIdentityDbContext : IdentityDbContext
    {
        public AspNetIdentityDbContext(DbContextOptions<AspNetIdentityDbContext> options)
          : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //// 设置 Identity 表的前缀
        //    //modelBuilder.Entity<IdentityUser>().ToTable("tbl_Users");
        //    //modelBuilder.Entity<IdentityRole>().ToTable("tbl_Roles");
        //    //modelBuilder.Entity<IdentityUserRole<string>>().ToTable("tbl_UserRoles");
        //    //modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("tbl_UserClaims");
        //    //modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("tbl_UserLogins");
        //    //modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("tbl_RoleClaims");
        //    //modelBuilder.Entity<IdentityUserToken<string>>().ToTable("tbl_UserTokens");
        //}
    }
}
