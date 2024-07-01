using System;
using TestWeb.Attributes;

namespace TestWeb
{
    [GeneratedController("api/book")]
    public class Book
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }
    }
}

