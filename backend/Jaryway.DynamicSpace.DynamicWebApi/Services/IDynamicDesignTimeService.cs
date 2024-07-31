using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jaryway.DynamicSpace.DynamicWebApi.Entities;

namespace Jaryway.DynamicSpace.DynamicWebApi.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDynamicDesignTimeService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<DynamicClass> GetList();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DynamicClass? Get(long id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicClass"></param>
        /// <returns></returns>
        DynamicClass Create(DynamicClass dynamicClass);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicClass"></param>
        /// <returns></returns>
        DynamicClass Update(DynamicClass dynamicClass);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicClass"></param>
        /// <returns></returns>
        int Remove(DynamicClass dynamicClass);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationName"></param>
        void Generate(string? migrationName = null);
    }
}
