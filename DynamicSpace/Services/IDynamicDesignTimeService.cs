using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicSpace.Models;

namespace DynamicSpace.Services
{
    public interface IDynamicDesignTimeService
    {
        IEnumerable<DynamicClass> GetList();
        DynamicClass? Get(long id);
        DynamicClass Create(DynamicClass dynamicClass);
        DynamicClass Update(DynamicClass dynamicClass);
        int Remove(DynamicClass dynamicClass);

        void Generate(string? migrationName = null);
    }
}
