﻿using System;
using System.Collections;
namespace TestWeb.Models
{
    public class Author
    {
        //public Author()
        //{

        //}
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}

