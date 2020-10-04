using BookStore2.Filters;
using BookStore2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace BookStore2.Controllers
{
    public class HomeController : Controller
    {
        BookContext db = new BookContext();

        public ActionResult Index(string Sorting_Order, int page = 1)
        {
            //Проверка вошел ли пользователь через cookie SetAuthCookie ( FormsAuthentication.SetAuthCookie(model.Name, true); )
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Name = User.Identity.Name;
            }

            //var IndexView = db.BookAuthors
            //    .Include(p=>p.Book)
            //    .Include(p=>p.Author);


            //Pagination
            var books = db.BookAuthors
                .Include(b => b.Book)
                .Include(b => b.Author)
                .ToList();
            int pageSize = 5;
            IEnumerable<BookAuthor> booksPerPage = books.Skip((page - 1) * pageSize).Take(pageSize);
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = books.Count };
            IndexViewModel ivm = new IndexViewModel { PageInfo = pageInfo, BookAuthorsList = booksPerPage };

            //Sort
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Description" : "";
            switch (Sorting_Order)
            {
                case "Name_Description":
                    ivm.BookAuthorsList = ivm.BookAuthorsList.OrderByDescending(b => b.Book.BookName);
                    break;
                default:
                    ivm.BookAuthorsList = ivm.BookAuthorsList.OrderBy(b => b.Book.BookName);
                    break;
            }
            ViewBag.SortingAuthor = String.IsNullOrEmpty(Sorting_Order) ? "Author_Description" : "";
            switch (Sorting_Order)
            {
                case "Author_Description":
                    ivm.BookAuthorsList = ivm.BookAuthorsList.OrderByDescending(b => b.Author.AuthorName);
                    break;
                default:
                    ivm.BookAuthorsList = ivm.BookAuthorsList.OrderBy(b => b.Author.AuthorName);
                    break;
            }
            return View(ivm);


        }
        [HttpGet]
        public ActionResult BookInfo(int id)
        {
            if (User.Identity.IsAuthenticated)
            {

                var user = db.Users.First(u => u.Name == User.Identity.Name);
                ViewBag.User = user;                
                string cs = ConfigurationManager.ConnectionStrings["BookContext"].ConnectionString;
                using (SqlConnection con = new SqlConnection(cs))
                {
                    //Проверяем книгу на наличие
                    SqlCommand cmd2 = new SqlCommand("CheckUserHasBook", con);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@BookId", id);
                    cmd2.Parameters.AddWithValue("@UserId", user.Id);
                    con.Open();                    
                    SqlDataReader rdr2 = cmd2.ExecuteReader();
                    ViewBag.UserHasBook = 0;
                    while (rdr2.Read())
                    {

                        if (Convert.ToInt32(rdr2["BookId"]) == id && Convert.ToInt32(rdr2["UserId"]) == user.Id)
                        {
                            ViewBag.UserHasBook = 1;
                        }
                        

                    }
                    con.Close();
                    //Выборка Книги и лайка, если есть то передаем значение лайка для юзера
                    SqlCommand cmd = new SqlCommand("SelectBookLikeByParameter", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@UserId", user.Id);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ViewBag.BookLike = Convert.ToInt32(rdr["BookLikeValue"]);
                        ViewBag.CurrentBookLikeUserId = Convert.ToInt32(rdr["UserId"]);
                    }
                    con.Close();
                }
                return View(db.Books
                .Include(b => b.Author)
                .FirstOrDefault(b => b.Id == id));
            }
            return View(db.Books
                .Include(b => b.Author)
                .FirstOrDefault(b => b.Id == id));
        }
        [HttpPost]
        public ActionResult BookInfo(BookUserLikes model)
        {
            string cs = ConfigurationManager.ConnectionStrings["BookContext"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {


                con.Open();

                //Если есть оценка то изменим ее            
                bool UserLikeBookBool = false;
                SqlCommand CheckUserLiked = new SqlCommand("CheckUserLikedBook", con);
                CheckUserLiked.CommandType = CommandType.StoredProcedure;
                CheckUserLiked.Parameters.AddWithValue("@UserId", model.UserId);
                CheckUserLiked.Parameters.AddWithValue("@BookId", model.BookId);
                SqlDataReader rdr = CheckUserLiked.ExecuteReader();
                while (rdr.Read())
                {
                    if (Convert.ToInt32(rdr["UserId"]) == model.UserId && Convert.ToInt32(rdr["BookId"]) == model.BookId)
                    {
                        UserLikeBookBool = true;
                    }
                }
                con.Close();
                if (!UserLikeBookBool)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("BookLikeProcedure", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@BookId", model.BookId);
                    cmd.Parameters.AddWithValue("@BookLikeValue", model.BookLikeValue);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    con.Open();
                    SqlCommand UpdateLikeValue = new SqlCommand("UpdateLikeValueProcedure", con);
                    UpdateLikeValue.CommandType = CommandType.StoredProcedure;
                    UpdateLikeValue.Parameters.AddWithValue("@UserId", model.UserId);
                    UpdateLikeValue.Parameters.AddWithValue("@BookId", model.BookId);
                    UpdateLikeValue.Parameters.AddWithValue("@UserLikeValue", model.BookLikeValue);
                    UpdateLikeValue.ExecuteNonQuery();
                    con.Close();
                }
                //Добавление оценки если ее нет
               
            
            }
            return RedirectToAction("BookInfo", "Home", new { id = model.BookId });
        }
        public ActionResult Author(int id)
        {
            var something = db.BookAuthors
                .Where(p => p.Author.Id == id)
                .Include(p => p.Author)
                .Include(p => p.Book)
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

        [MyAuthorize(Users = "dias")]
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
        [MyAuthorize(Users = "dias")]
        [HttpGet]
        public ActionResult EditAuthor(int id)
        {
            return View();
        }
        [HttpPost]
        [MyAuthorize(Users = "dias")]
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
                    if (books[i].BoughtBooks >= x)
                    {
                        x = books[i].BoughtBooks;
                    }
                }
                Book book = db.Books.FirstOrDefault(b => b.BoughtBooks == x);
                return PartialView(book);
            }
            catch (Exception ex)
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
                .Where(a => a.BookName.Contains(name)).ToList();
                if (allBooks.Count() <= 0)
                {
                    return PartialView("FindError");
                }

                return PartialView(allBooks);
            }
            return PartialView("FindError");
        }
        [HttpGet]
        [Authorize]
        public ActionResult BuyBook(int id)
        {
            Book book = db.Books.FirstOrDefault(b => b.Id == id);
            User user = db.Users.FirstOrDefault(u => u.Name == User.Identity.Name);

            return View(new BuyBookModel()
            {
                BuyBookName = book.BookName,
                BuyBookPrice = book.BookPrice
                ,
                BuyUserName = user.Name,
                BuyUserWallet = user.Wallet
            });
        }
        [HttpPost]
        public ActionResult BuyBook(BuyBookModel BBModel)
        {
            User user = db.Users.FirstOrDefault(u => u.Name == BBModel.BuyUserName);
            Book book = db.Books.FirstOrDefault(u => u.BookName == BBModel.BuyBookName);
            //Добавляем обьект покупки

            if (user.Wallet >= book.BookPrice)
            {
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
                    if (db.Purchases.FirstOrDefault(p => p.PurchaseCode == finalString) == null)
                    {
                        PurchaseCodeIsUnique = true;
                        break;
                    }
                    finalString = "";
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
        [MyAuthorize(Users = "dias")]
        public ActionResult DeleteBook(int id)
        {
            db.Books.Remove(db.Books.Find(id));
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}