using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSale.Models;
using BookSale.Data;
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
        private readonly ApplicationDbContext _context;
        private readonly SelectBooks _selectBooks;
        private readonly SelectMembers _selectMembers;
        private readonly AddCart _addCart;
        private readonly MemberInsert _memberInsert;
        private readonly SaleInsert _saleInsert;

        public HomeController(
            ApplicationDbContext context,
            SelectBooks selectBooks,
            SelectMembers selectMembers,
            AddCart addCart,
            MemberInsert memberInsert,
            SaleInsert saleInsert
        )
        {
            _context = context;
            _selectBooks = selectBooks;
            _selectMembers = selectMembers;
            _addCart = addCart;
            _memberInsert = memberInsert;
            _saleInsert = saleInsert;
        }
        

        public IActionResult Index()
        {
            List<BookModel> books = _selectBooks.GetBooks(limit: 3);
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
            List<BookModel> books = _selectBooks.GetBooks(subject: subject, type: type, author: author);
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
            var books = _selectBooks.GetBooks(0, decode);

            List<MemberModel> members;
            if (memberId == null)
            {
                members = _selectMembers.SelectMember(id: 0);
            }
            else
            {
                members = _selectMembers.SelectMember(id: decode);
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
            List<BookModel> books = _selectBooks.GetBooks(key: key);
            ViewBag.SearchKey = key;
            return View(books);
        }


        public IActionResult BookDetail(string id)
        {
            var decode = EncryptionHelper.DecryptId(id);
            List<BookModel> book = _selectBooks.GetBooks(id: decode);
            return View(book);
        }

        [HttpPost]
        public IActionResult MemberLogin(string email, string password)
        {
            var member = _context.Member.FirstOrDefault(m =>
                m.Email == email);

            if (member != null && EncryptionHelperPass.DecryptId(member.Password) == password)
            {
                string memberIdEncrypted = EncryptionHelperPass.EncryptId(member.Id.ToString());

                HttpContext.Session.SetString("memberPanel_id", memberIdEncrypted);
                return Ok("success");
            }
            else
            {
                return Ok("error");
            }
        }

        [HttpPost]
        public IActionResult AddCartBook(string Number)
        {
            var memberIdHass = HttpContext.Session.GetString("memberPanel_id");
            var message = _addCart.CartInsert(Number, memberIdHass);
            return Ok($"{message}");
        }
        
        
        [HttpPost]
        public IActionResult ContactInsert(string Name, string Mail, string Subject, string Message)
        {
            _context.Contact.Add(new ContactModel
            {
                Name = Name,
                Mail = Mail,
                Subject = Subject,
                Description = Message
            });
            _context.SaveChanges();

            return Ok("success");
        }
        
        
        [HttpPost]
        public IActionResult MemberInsert(string Name, string Phone, string Mail, string Address, string Password)
        {
            var message = _memberInsert.InsertMember(Name, Phone, Mail, Address, Password);
            return Ok($"{message}");
        }
        
        
        [HttpPost]
        public IActionResult SaleInsert(string notify, string name, string phone, string address, string mail,
            string customerId, int piece)
        {
            var message = _saleInsert.InsertSale(notify ,name, phone, address, mail, customerId, piece);
            return Ok($"{message}");
        }
        
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}