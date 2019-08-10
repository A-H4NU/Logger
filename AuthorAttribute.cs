using System;

namespace LoggerLib
{
    [Author("HANU")]
    internal class AuthorAttribute : Attribute
    {
        public string Author;
        internal AuthorAttribute(string author) => this.Author = author;
    }
}
