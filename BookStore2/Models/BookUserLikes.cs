using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore2.Models
{
    public class BookUserLikes
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int BookLikeValue { get; set; }

    }
}