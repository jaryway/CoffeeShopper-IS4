using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicSpace.Controllers;

namespace DynamicSpace.Models
{
    [GenericController("api/test")]
    public class Test : DynamicClassBase
    {
        //public int Id { get; set; }

        public string Name { get; set; }
    }
}
