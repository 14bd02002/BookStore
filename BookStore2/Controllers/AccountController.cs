using BookStore2.Models;
using System;
using System.Data.Entity;
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
    }
}