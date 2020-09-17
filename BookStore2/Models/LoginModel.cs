using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore2.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Минимум 3 Максимум 30 длина Имени")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Минимум 3 Максимум 30 длина Пароля")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public List<Book> Books { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Минимум 3 Максимум 30 длина Имени")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Минимум 3 Максимум 30 длина Пароля")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int Age { get; set; }
    }
}