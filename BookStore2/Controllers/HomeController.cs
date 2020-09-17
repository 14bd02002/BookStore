using BookStore2.Filters;
using BookStore2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace BookStore2.Controllers
{
    public class HomeController : Controller
    {
        BookContext db = new BookContext();

        public ActionResult Index(string Sorting_Order)
        {
            //Проверка вошел ли пользователь через cookie SetAuthCookie ( FormsAuthentication.SetAuthCookie(model.Name, true); )
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Name = User.Identity.Name;
            }
            if (db.Authors.Count() == 0) 
            {
                db.Authors.Add(new Author { AuthorName = "Тургенев" });
                db.Authors.Add(new Author { AuthorName = "Пушкин" });
                db.Authors.Add(new Author { AuthorName = "Тютчев" });
                db.SaveChanges();
            }
            
            var IndexView = db.BookAuthors
                .Include(p=>p.Book)
                .Include(p=>p.Author);


            //Sort
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Description" : "";
            switch (Sorting_Order)
            {
                case "Name_Description":
                    IndexView = IndexView.OrderByDescending(b => b.Book.BookName);
                    break;
                default:
                    IndexView = IndexView.OrderBy(b => b.Book.BookName);
                    break;
            }
            return View(IndexView.ToList());

        }
        public ActionResult BookInfo(int id)
        {
            return View(db.Books
                .Include(b=>b.Author)
                .FirstOrDefault(b => b.Id == id));
        }
        public ActionResult Author(int id)
        {
            var something = db.BookAuthors                
                .Where(p => p.Author.Id == id)
                .Include(p=>p.Author)
                .Include(p=>p.Book)
                .ToList();
            return View(something);
        }
        //фильтр авторизации. Доступно создать книгу только для авторизованных пользователей
        [Authorize]
        [MyAuthorize(Users = "dias")]
        public ActionResult Create()
        {
            ViewBag.Authors = new SelectList(db.Authors, "Id", "AuthorName");
            ViewBag.AuthorsList = new SelectList(db.Authors, "Id", "AuthorName");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        [MyAuthorize(Roles = "dias")]
        //Здесь присутствует привязчик модели Bind который указывает что включить только свойство Name
        public ActionResult Create([Bind(Include = "BookName , AuthorId, BookPrice, Genre, Year, ImagePath")] Book book, HttpPostedFileBase file)
        {
            //Валидация на клиентской стороне ModelState.IsValid
            if (ModelState.IsValid)
            {
                var tempBook = db.Books.FirstOrDefault(m => m.BookName == book.BookName);
                if (tempBook == null)
                {

                    //Добавление картинки
                    if (file != null)
                    {
                        file.SaveAs(HttpContext.Server.MapPath("~/Images/")
                                                              + file.FileName);
                        book.ImagePath = file.FileName;
                    }

                    db.Books.Add(book);
                    //Также надо добавить запись в таблицу BookAuthors так как она хранит связанные данные Для авторов и книг                              
                    db.BookAuthors.Add(new BookAuthor { BookId = book.Id, AuthorId = book.AuthorId });
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError(" ", "Данная книга существует");
            }
            return View(book);
        }
        // Ajax ссылка. Здесь указан контроллер который будет хранить url. В представлении есть ссылка которая указывает на данный url 
        // при клике, вызывается данный метод и находит первую книгу в коллекции книг. И возвращает значение в частичное представление PartialView
        // под таким же названием как контроллер. И возвращает это частичное представление в Home/index.cshtml
        [Authorize]
        [MyAuthorize(Users = "dias")]
        public ActionResult CreateAuthor()
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        [MyAuthorize(Roles = "dias")]
        public ActionResult CreateAuthor(Author author, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var checkAuthor = db.Authors.FirstOrDefault(a => a.AuthorName == author.AuthorName);
                if (checkAuthor != null)
                {
                    ModelState.AddModelError("", "Данный автор существует");
                    return View(author);
                }
                if (file != null)
                {
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/Authors/")
                                                          + file.FileName);
                    author.AuthorImageUrl = file.FileName;
                }
                db.Authors.Add(new Author()
                { AuthorName = author.AuthorName, AuthorYear = author.AuthorYear, AuthorImageUrl = author.AuthorImageUrl });
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
                return View(author);
        }
        [HttpGet]
        public ActionResult EditAuthor(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult EditAuthor(Author author, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/Authors/") + file.FileName);
                    author.AuthorImageUrl = file.FileName;
                }
                db.Entry(author).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(author);
        }
        public ActionResult BestBook()
        {
            try
            {

                List<Book> books = db.Books.ToList();
                int x = 0;
                int s = books.Count;
                for (int i = 0; i < books.Count; i++)
                {
                    if(books[i].BoughtBooks >= x)
                    {
                        x = books[i].BoughtBooks;
                    }
                }
                Book book = db.Books.FirstOrDefault(b=>b.BoughtBooks == x);
                return PartialView(book);
            }
            catch(Exception ex)
            {
                return PartialView(ex);
            }
            
        }
        public ActionResult BookSearch(string name)
        {
            
            if (!name.IsEmpty())
            {
                var allBooks = db.Books
                .Include(a => a.Author)
                .Where(a => a.Author.AuthorName.Contains(name)).ToList(); // Where property contains some Characters from Value. ( Contains(name) );
                if (allBooks.Count() <= 0)
                {
                    return PartialView("FindError");
                }
                
                return PartialView(allBooks);
            }
            return PartialView("FindError");
        }
        [HttpGet]       
        public ActionResult BuyBook(int id)
        {            
            Book book = db.Books.FirstOrDefault(b => b.Id == id);
            User user = db.Users.FirstOrDefault(u => u.Name == User.Identity.Name);
            
            return View(new BuyBookModel() {BuyBookName = book.BookName , BuyBookPrice = book.BookPrice
                , BuyUserName = user.Name, BuyUserWallet = user.Wallet });
        }
        [HttpPost]
        public ActionResult BuyBook(BuyBookModel BBModel)
        {            
            User user = db.Users.FirstOrDefault(u => u.Name == BBModel.BuyUserName);
            Book book = db.Books.FirstOrDefault(u => u.BookName == BBModel.BuyBookName);
            //Добавляем обьект покупки
            
            if (user.Wallet>=book.BookPrice) {
                //Проверка айди покупки на уникальность
                var checkPurchaseCodes = db.Purchases.Select(p => p.PurchaseCode).ToList();
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[8];
                Random rnd = new Random();
                String finalString = "";
                bool PurchaseCodeIsUnique = false;
                while (!PurchaseCodeIsUnique)
                {
                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[rnd.Next(0, chars.Length)];
                        finalString += stringChars[i];
                    }                    
                    if (db.Purchases.FirstOrDefault(p=>p.PurchaseCode == finalString) == null)
                    {
                        PurchaseCodeIsUnique = true;
                        break;
                    }
                }                     
                
                
                db.Purchases.Add(new Purchase() { PurchaseCode = finalString, Book = book, User = user, Time = DateTime.Now });
                //Создаем коллекцию из обьектов книг, и присваем эту коллекцию к свойству Books            
                user.Wallet = user.Wallet - book.BookPrice;
                book.BoughtBooks++;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }            
            return View("NotEnoughMoney");
        }
    }
}