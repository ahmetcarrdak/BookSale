using Microsoft.AspNetCore.Mvc;
using BookSale.Models;
using BookSale.EnDeCode;
using System.Diagnostics;
using BookSale.Methods;

namespace BookSale.Controllers
{
    public class SaleViewModel
    {
        public List<BookModel> Books { get; set; }
        public List<MemberModel> Members { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly BookMethod _bookMethod;
        private readonly MemberMethod _memberMethod;
        private readonly CartMethod _cartMethod;
        private readonly SaleMethod _saleMethod;
        private readonly ContactMethod _contactMethod;

        public HomeController(
            BookMethod bookMethod,
            CartMethod cartMethod,
            MemberMethod memberMethod,
            SaleMethod saleMethod,
            ContactMethod contactMethod
        )
        {
            _bookMethod = bookMethod;
            _cartMethod = cartMethod;
            _memberMethod = memberMethod;
            _saleMethod = saleMethod;
            _contactMethod = contactMethod;
        }


        public IActionResult Index()
        {
            List<BookModel> books = _bookMethod.GetBooks(limit: 3);
            return View(books);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Package(string type, string subject, string author)
        {
            ViewBag.t = $"{type}{subject}{author}";
            List<BookModel> books = _bookMethod.GetBooks(subject: subject, type: type, author: author);
            return View(books);
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Sale(string id)
        {
            var memberId = HttpContext.Session.GetString("memberPanel_id");
            var decode = EncryptionHelper.DecryptId(id);
            var books = _bookMethod.GetBooks(0, decode);

            List<MemberModel> members;
            if (memberId == null)
            {
                members = _memberMethod.SelectMember(id: 0);
            }
            else
            {
                members = _memberMethod.SelectMember(id: decode);
            }

            var viewModel = new SaleViewModel
            {
                Books = books,
                Members = members
            };

            return View(viewModel);
        }

        public IActionResult BookSearch(string key)
        {
            List<BookModel> books = _bookMethod.GetBooks(key: key);
            ViewBag.SearchKey = key;
            return View(books);
        }


        public IActionResult BookDetail(string id)
        {
            var decode = EncryptionHelper.DecryptId(id);
            List<BookModel> book = _bookMethod.GetBooks(id: decode);
            return View(book);
        }

        [HttpPost]
        public IActionResult MemberLogin(string email, string password)
        {
            var message = _memberMethod.MemberLogin(email: email, password: password);
            return Ok($"{message}");
        }

        [HttpPost]
        public IActionResult AddCartBook(string Number)
        {
            var memberIdHass = HttpContext.Session.GetString("memberPanel_id");
            var message = _cartMethod.CartInsert(Number, memberIdHass);
            return Ok($"{message}");
        }


        [HttpPost]
        public IActionResult ContactInsert(string Name, string Mail, string Subject, string Message)
        {
            var message = _contactMethod.ContactInsert(Name, Mail, Subject, Message);
            return Ok($"{message}");
        }


        [HttpPost]
        public IActionResult MemberInsert(string Name, string Phone, string Mail, string Address, string Password)
        {
            var message = _memberMethod.InsertMember(Name, Phone, Mail, Address, Password);
            return Ok($"{message}");
        }


        [HttpPost]
        public IActionResult SaleInsert(string notify, string name, string phone, string address, string mail,
            string customerId, int piece)
        {
            var message = _saleMethod.InsertSale(notify, name, phone, address, mail, customerId, piece);
            return Ok($"{message}");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}