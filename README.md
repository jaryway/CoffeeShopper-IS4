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


### TODO
[x] 重命名字段，新增字段，并将原字段标识为 Deprecated
[x] 字段删除，不删除表字段，并将该字段标为 Deprecated
[ ] 回滚后删除实体类
[ ] 表名添加租户 id 前缀，注意导航属性也要处理
[ ] 增删改查
[ ] 获取导航属性
