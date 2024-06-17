using System;
namespace TestWeb.Models
{
    public class Book
    {
        //public Book()
        //{

        //}

        public int Id { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }
    }
}

