using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jaryway.DynamicSpace.DynamicWebApi.Controllers;

namespace Jaryway.DynamicSpace.DynamicWebApi.Models
{
    //[GenericController("api/test")]
    public class Test : DynamicClassBase
    {
        //public int Id { get; set; }

        public string Name { get; set; }
    }
}
