using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSale.Models;
using BookSale.Data;
using BookSale.EnDeCode;
using System.Diagnostics;

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

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var books = GetBooks(3, 0);
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
            var books = GetBooks(0, 0, null, subject, type, author);
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
            var books = GetBooks(0, decode);

            List<MemberModel> members;
            if (memberId == null)
            {
                members = SelectMember(0);
            }
            else
            {
                members = SelectMember(decode);
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
            var books = GetBooks(0, 0, key: key);
            ViewBag.SearchKey = key;
            return View(books);
        }


        /* System Progress */


        public IActionResult BookDetail(string id)
        {
            var decode = EncryptionHelper.DecryptId(id);
            var book = GetBooks(0, decode);
            return View(book);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        /* DB Progress */

        private List<BookModel> GetBooks(int limit = 0, int id = 0, string key = null, string subject = null,
            string type = null, string author = null)
        {
            IQueryable<BookModel> query = _context.Books;

            if (id > 0)
            {
                query = query.Where(b => b.Id == id);
            }

            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(b => b.Desc.Contains(key));
            }

            if (!string.IsNullOrEmpty(subject))
            {
                query = query.Where(b => b.Subject.Contains(subject));
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(b => b.Type.Contains(type));
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.Contains(author));
            }

            if (limit > 0)
            {
                query = query.Take(limit);
            }

            return query.ToList();
        }


        private List<CartModel> GetCarts(int memberId = 0, int customerId = 0, int booksId = 0)
        {
            IQueryable<CartModel> query = _context.Cart
                .Where(c => c.MemberId == memberId && c.CustomerId == customerId && c.BookId == booksId);

            return query.ToList();
        }

        private List<MemberModel> SelectMember(int id = 0)
        {
            IQueryable<MemberModel> query = _context.Member;

            if (id > 0)
            {
                query = query.Where(m => m.Id == id);
            }
            else
            {
                return new List<MemberModel>(); // veya return null; 
            }

            return query.ToList();
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
            var message = "";
            if (memberIdHass == null)
            {
                return Ok("noSession");
            }
            else
            {
                var memberId = EncryptionHelper.DecryptId(memberIdHass);
                var decode = EncryptionHelper.DecryptId(Number);
                var books = GetBooks(0, decode);

                foreach (var book in books)
                {
                    var customerId = book.CustomerId;
                    var BookId = book.Id;
                    var BookPrice = book.Price;

                    var cart = _context.Cart
                        .FirstOrDefault(c =>
                            c.MemberId == memberId && c.CustomerId == customerId && c.BookId == BookId && c.Piece == 1);

                    if (cart == null)
                    {
                        _context.Cart.Add(new CartModel
                        {
                            MemberId = memberId,
                            CustomerId = customerId,
                            BookId = BookId,
                            Piece = 1
                        });
                        _context.SaveChanges();
                        message = "success";
                    }
                    else
                    {
                        cart.Piece++;
                        _context.SaveChanges();
                        message = "update";
                    }
                }
            }

            return Ok($"{message}");
        }

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

        public IActionResult MemberInsert(string Name, string Phone, string Mail, string Address, string Password)
        {
            try
            {
                // Önce e-posta adresinin benzersiz olup olmadığını kontrol etmek iyi bir uygulamadır
                var existingMember = _context.Member.FirstOrDefault(m => m.Email == Mail);
                if (existingMember != null)
                {
                    return Ok("Bu e-posta adresi zaten kullanımda.");
                }

                // Şifreleme işlemi uygulanıyor
                var encryptedPassword = EncryptionHelperPass.EncryptId(Password);

                // Yeni üye ekleyin
                var newMember = new MemberModel
                {
                    Name = Name,
                    Phone = Phone,
                    Email = Mail,
                    Address = Address,
                    Password = encryptedPassword
                };

                _context.Member.Add(newMember);
                _context.SaveChanges();

                return Ok("success");
            }
            catch (Exception ex)
            {
                // Hata oluşursa loglama yapabilir veya uygun hata mesajı döndürebilirsiniz
                return Ok($"Bir hata oluştu: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult SaleInsert(string notify, string name, string phone, string address, string mail,
            string customerId, int piece)
        {
            try
            {
                // Kontrol edilecek alanları ve hata mesajlarını tanımla
                var fields = new (string Value, string Message)[]
                {
                    (notify, "Notify cannot be empty."),
                    (name, "Name cannot be empty."),
                    (phone, "Phone cannot be empty."),
                    (address, "Address cannot be empty."),
                    (mail, "Mail cannot be empty."),
                    (customerId, "Customer ID cannot be empty.")
                };

                // Tüm alanların boş olup olmadığını kontrol et
                if (fields.All(f => string.IsNullOrWhiteSpace(f.Value)))
                {
                    return Ok("All fields cannot be empty.");
                }

                // Boş alan kontrolü yap
                foreach (var field in fields)
                {
                    if (string.IsNullOrWhiteSpace(field.Value))
                    {
                        return Ok(field.Message);
                    }
                }

                // Üyelik kontrolü yap
                var member = _context.Member.FirstOrDefault(m => m.Email == mail);
                bool isMember = member != null;
                int memberId = isMember ? member.Id : 0;

                // BookId'yi çöz
                var decryptedBookId = EncryptionHelper.DecryptId(notify);
                var decryptedCustomer = EncryptionHelper.DecryptId(customerId);

                // Kitabı bul
                var book = _context.Books.FirstOrDefault(b => b.Id == decryptedBookId);
                if (book == null || book.Stock <= 0)
                {
                    return Ok("Requested book is out of stock or not found.");
                }

                // Satış kaydını oluştur
                var sale = new SaleModel()
                {
                    BookId = decryptedBookId,
                    Name = name,
                    Phone = phone,
                    Address = address,
                    Mail = mail,
                    IsMember = isMember,
                    CustomerId = decryptedCustomer,
                    Piece = piece != 0 ? piece : 1,
                    Status = 0,
                    MemberId = memberId,
                };

                // Satışı kaydet
                _context.Sale.Add(sale);
                _context.SaveChanges();

                // Stoktan düşür
                book.Stock--;
                _context.SaveChanges();

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok($"Error: {ex.Message}");
            }
        }


        /* End */
    }
}