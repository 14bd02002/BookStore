using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore2.Models
{
    public class AccountInfo
    {
        public string UserName { get; set; }
        public List<Book> Books { get; set; }
    }
}