using BookStore2.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;

namespace BookStore2.Controllers
{
    public class AccountController : Controller
    {
        BookContext db = new BookContext();
        // GET: Account
        public ActionResult AccountInfo(string Sorting_Order)
        {
            var currentUser = db.Users.FirstOrDefault(m => m.Name == User.Identity.Name);
            if (!User.Identity.Name.IsEmpty())
                ViewBag.Wallet = currentUser.Wallet;
            var books = db.Purchases
                .Include(u => u.User)
                .Include(b => b.Book)
                .Where(u => u.User.Name == User.Identity.Name)
                .ToList();
            if (books != null)
                ViewBag.Bool = true;
            if (books == null)
                ViewBag.Bool = false;                               

             return View(books);
        }
        public ActionResult Login()
        {            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(m => m.Name == model.Name && m.Password == model.Password);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Name", "Введены неверные данные");
                }
            }
            return View(model);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            //проверка валидации со стороны модели с указанными атрибутами( введеные в представление параметры сравниваются с моделью)
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(m => m.Name == model.Name);
                if (user != null)
                {
                    ModelState.AddModelError("", "Пользователь с таким именем уже зарегестрирован");
                }
                else
                {
                    //если мы не можем добавить модель которая создана для валидации в 
                    //модель Аккаунтов то можем создать новый обьект модели и вписать в него данные
                    db.Users.Add(new User { Name = model.Name, Age = model.Age, Password = model.Password });
                    db.SaveChanges();
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(model);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult AddMoney()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddMoney(User user)
        {
            User userTemp = db.Users.FirstOrDefault(m => m.Name == User.Identity.Name);
            userTemp.Wallet += user.Wallet;
            db.SaveChanges();

            return RedirectToAction("Index", "");
        }
        [HttpGet]       

        public ActionResult Edit()
        {
            var id = db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.Id).ToList().First();
            var currentUser = db.Users.Find(id);
            if(currentUser.FirstName != null && currentUser.LastName != null && currentUser.DateOfBirth != null)
            {

                var date = currentUser.DateOfBirth.Date;
                db.SaveChanges();
                ViewBag.UserHasName = true;
            }
            else
            {
                ViewBag.UserHasName = false;
            }

            return View(currentUser);
        }
        [HttpPost]     
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            //добавляем id 
            var id = db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.Id).ToList().First();
            user.Id = id;

            string cs = ConfigurationManager.ConnectionStrings["BookContext"].ConnectionString; // Записал в cs коннекшн стринг
            using (SqlConnection sql = new SqlConnection(cs))  // создаю sql коннекшн в using чтобы потом оборвать связь/ и передаю cs в конструктор
            {
                SqlCommand cmd = new SqlCommand("EditUserProcedure", sql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", user.Id);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                cmd.Parameters.AddWithValue("@Password", user.Password);                
                sql.Open();
                cmd.ExecuteNonQuery();
                sql.Close();
            }

            return RedirectToAction("AccountInfo", "Account");
        }
    }
}