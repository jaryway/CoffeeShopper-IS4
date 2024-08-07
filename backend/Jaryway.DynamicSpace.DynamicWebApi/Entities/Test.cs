﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jaryway.DynamicSpace.DynamicWebApi.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Jaryway.DynamicSpace.DynamicWebApi.Entities
{
    //[GenericController("api/test")]
    /// <summary>
    /// 
    /// </summary>

    [Index(nameof(Name), IsUnique = true)]
    public class Test : DynamicClassBase
    {
        //public int Id { get; set; }
        private string _name = "";

        public string Name
        {
            get
            {
                return _name;
            }
            set => _name = value;
        }

        [DefaultValue("123")]
        public string Label { get; set; }

        public int Age { get; set; } = 1;
    }
}
