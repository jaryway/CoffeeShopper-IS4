using System;
using DynamicBuilder.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicBuilder
{
    public class MyDbContext : DbContext
    {

        public int Count { get; }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {

            this.Count++;
        }

        ///// <summary>
        ///// 重写OnConfiguring方法
        ///// </summary>
        ///// <param name="optionsBuilder"></param>
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // 使用SqlServer数据库，传递连接字符串
        //    //var folder = Environment.SpecialFolder.LocalApplicationData;
        //    //var path = Environment.GetFolderPath(folder);
        //    optionsBuilder.UseSqlite("Data Source=database-test.db");
        //    base.OnConfiguring(optionsBuilder);
        //}

        //public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //EntityTypeGenerator.RegisterEntities(modelBuilder);
            base.OnModelCreating(modelBuilder);
            
            //modelBuilder.Entity<Post>().
        }
    }
}

