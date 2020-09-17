using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BookStore2.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Автор")]
        [Required]
        public string AuthorName { get; set; }
        [Display(Name = "Год Рождения")]
        [Range(1600, 2020, ErrorMessage ="недопустимый год")]
        public int AuthorYear { get; set; }
        
        public string AuthorImageUrl { get; set; }
        public List<Book> Books { get; set; }
               
        
    }
    //public class AuthorDbInitializer : CreateDatabaseIfNotExists<BookContext>
    //{
    //    BookContext db = new BookContext();
    //    db.
    //}
}