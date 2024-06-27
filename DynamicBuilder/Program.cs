// See https://aka.ms/new-console-template for more information
using DynamicBuilder;
using DynamicBuilder.Data;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using DynamicBuilder.Metadata;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Reflection.Emit;

var connectionString = "server=localhost;uid=root;pwd=123456;database=test";
var serverVersion = ServerVersion.AutoDetect(connectionString);//new MySqlServerVersion(Version.Parse("8.0.0"));
var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseMySql(connectionString, serverVersion);
//builder.UseInternalServiceProvider(new MyServicePrivoder());

using (var applicationDbContext = new ApplicationDbContext(builder.Options))
{
    SeedData.Initialize(applicationDbContext);
    //var dbConnection = applicationDbContext.Database.GetDbConnection();
    //var list = applicationDbContext.DynamicEntities.Select(m => m.Name).ToList();
    //var metadata = new MetadataEntity();

    //var metadataQuerySet = (IQueryable<DynamicEntity>)applicationDbContext
    //    .GetType()
    //    .GetMethod("Set")!
    //    .MakeGenericMethod(metadata.EntityType)
    //    .Invoke(applicationDbContext, null)!;

    //var id = 1;
    //metadataQuerySet.Where("Id == @0", id);

    //applicationDbContext.SetDynamicModeOff();
    //applicationDbContext.Database.Migrate();

    //var factory = new DynamicBuilder.DynamicClassFactory();
    ////factory.CreateDynamicTypeBuilder

    ////System.Linq.Dynamic.Core.DynamicClassFactory.
    ////AppDomain.CurrentDomain.CreateInstance("",)

    AssemblyName assemblyName = new AssemblyName("MyDynamicAssembly");
    AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);

    //// 创建一个动态模块
    ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MyDynamicAssembly.dll");

    // 创建一个动态类型
    TypeBuilder typeBuilder = moduleBuilder.DefineType("MyDynamicType", TypeAttributes.Public);

    // 定义一个字段
    FieldBuilder fieldBuilder = typeBuilder.DefineField("myField", typeof(string), FieldAttributes.Public);

    // 定义一个方法
    MethodBuilder methodBuilder = typeBuilder.DefineMethod("MyMethod", MethodAttributes.Public, typeof(int), new[] { typeof(int), typeof(int) });
    ILGenerator ilGenerator = methodBuilder.GetILGenerator();
    ilGenerator.Emit(OpCodes.Ldarg_1); // 将第一个参数推送到栈上
    ilGenerator.Emit(OpCodes.Ldarg_2); // 将第二个参数推送到栈上
    ilGenerator.Emit(OpCodes.Add); // 执行加法操作
    ilGenerator.Emit(OpCodes.Ret); // 返回结果

    var ddd = (Assembly) assemblyBuilder;
    //var ss= Assembly.Load("MyDynamicAssembly.dll");
    //assemblyBuilder.

    //// 创建动态类型
    //Type dynamicType = typeBuilder.CreateType();

    //// 获取动态方法
    //MethodInfo dynamicMethod = dynamicType.GetMethod("MyMethod");

    //// 调用动态方法并获取结果
    //int result = (int)dynamicMethod.Invoke(null, new object[] { 10, 20 });

    // 保存动态程序集
    //assemblyBuil

    //var generator = new DynamicSpace.DynamicDbContextGenerator(applicationDbContext);

    //generator.AddEntity("Test", "public int Id { get; set; } public string Name {get; set;} public string Name1 {get; set;} public string Name3 {get; set;} public string Name4 {get; set;} public string Name2 {get; set;}", tableName: "Tests");
    //generator.AddEntity("Author", "public int Id { get; set; } public string FirstName {get; set;} public string LastName {get; set;} public ICollection<Book> Books { get; set; }", tableName: "Authors");
    //generator.AddEntity("Book", "public int Id { get; set; } public string Title {get; set;} public int AuthorId {get; set;} public string Created {get; set;} public Author Author { get; set; }", tableName: "Books");

    //generator.AddMigration();
    //generator.AddEntity("Test01", "public int Id { get; set; } public string Name {get; set;}", tableName: "Tests01");
    //generator.AddMigration();
    //generator.UpdateDatabase();
    //generator.AddEntity("Test02", "public int Id { get; set; } public string Name {get; set;} public string Name1 {get; set;}", tableName: "Tests02");
    //generator.AddEntity("Test03", "public int Id { get; set; } public string Name {get; set;}", tableName: "Test03");
    //generator.AddMigration();
    //generator.RemoveMigration(false);
    //generator.UpdateDatabase();

    //var list = new List<bool>().Where

    //var author = generator.Get("Author", 1);


    //Console.WriteLine("Hello, World!", author);
    Console.ReadLine();
}

//applicationDbContext.Dispose();



