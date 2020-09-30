using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore2.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public string PurchaseCode { get; set; }
        public Book Book { get; set; }
        public int BookId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        
        public DateTime Time { get; set; }
    }
}