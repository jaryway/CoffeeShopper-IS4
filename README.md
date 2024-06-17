# CoffeeShopper-IS4
A project integrating ASP.NET Identity with IdentityServer 4

NOTE: please check out the different branches based on the 4 stages of this tutorial


# Database

### 1、更新迁移文件
dotnet ef migrations add InitialIdentityDbContextMigration -p Server -c PersistedGrantDbContext
dotnet ef migrations add InitialIdentityDbContextMigration -p Server -c ConfigurationDbContext
dotnet ef migrations add InitialIdentityDbContextMigration -p Server -c AspNetIdentityDbContext
dotnet ef migrations add InitialIdentityDbContextMigration -s API -project DataAccess -c ApplicationDbContext

### 2、创建数据库
dotnet ef database update --project Server --context PersistedGrantDbContext
dotnet ef database update --project Server --context ConfigurationDbContext
dotnet ef database update --project Server --context AspNetIdentityDbContext
dotnet ef database update --startup-project API --project DataAccess --context ApplicationDbContext

### 3、初始化数据Ï
dotnet run --project Server /seed