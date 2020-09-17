using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;

namespace BookStore2.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Название")]
        public string BookName { get; set; }
        [Required]
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        [Display(Name = "Цена")]
        public int BookPrice { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public int BoughtBooks { get; set; }
        
        public String ImagePath { get; set; }
        
    }
}