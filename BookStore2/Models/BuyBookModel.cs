using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore2.Models
{
    public class BuyBookModel
    {        
        public string BuyBookName { get; set; }
        public string BuyUserName { get; set; }
        public int BuyBookPrice { get; set; }
        public int BuyUserWallet { get; set; }
    }
}