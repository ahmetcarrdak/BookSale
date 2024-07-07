using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSale.Data;
using BookSale.EnDeCode;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using BookSale.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookSale.Areas.CustomerPanel.Controllers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SessionCheckAttribute : ActionFilterAttribute
{
    private readonly string _redirectUrl;

    public SessionCheckAttribute(string redirectUrl)
    {
        _redirectUrl = redirectUrl;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.Controller as Controller;
        var memberIdHass = controller?.HttpContext.Session.GetString("customerPanel_id");

        // Geçerli URL'yi al
        var currentUrl = context.HttpContext.Request.Path.Value.ToLower();

        // Yönlendirme URL'si (signin veya signup) ile eşleşmiyorsa kontrol et
        if (!currentUrl.Contains("signIn") && !currentUrl.Contains("signUp"))
        {
            if (string.IsNullOrEmpty(memberIdHass))
            {
                context.Result = new RedirectResult(_redirectUrl);
                return;
            }
            else
            {
                context.ActionArguments["customerId"] = EncryptionHelperPass.DecryptId(memberIdHass);
            }
        }

        base.OnActionExecuting(context);
    }
}

[Area("CustomerPanel")]
[SessionCheck("/AuthPanel/Auth/CustomerAuth")]
public class CustomerController : Controller
{
    private readonly ApplicationDbContext _context;

    public CustomerController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Books(string customerId)
    {
        var books = GetBooks(customerId);
        return View(books);
    }

    public IActionResult Sales(string customerId)
    {
        var sales = GetSales(customerId);

        return View(sales);
    }

    public IActionResult Profile(string customerId)
    {
        var customer = GetCustomer(customerId);
        return View(customer);
    }

    public IActionResult LogOut()
    {
        HttpContext.Session.Remove("customerPanel_id");
        return Ok("");
    }

    private List<BookModel> GetBooks(string customerId)
    {
        IQueryable<BookModel> query = _context.Books.Where(b => b.CustomerId == int.Parse(customerId));
        return query.ToList();
    }

    private List<CustomerModel> GetCustomer(string customerId)
    {
        IQueryable<CustomerModel> query = _context.Customers.Where(c => c.Id == int.Parse(customerId));
        return query.ToList();
    }

    private List<SaleModel> GetSales(string customerId)
    {
        IQueryable<SaleModel> query = _context.Sale.Where(s => s.CustomerId == int.Parse(customerId));
        return query.ToList();
    }

    
    [HttpPost]
    public IActionResult UpdateBook(int bookId, string title, string subject, string type, string price, string stock,
        string author)
    {
        try
        {
            // Kitabı veritabanından bul
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }

            // Diğer parametrelerle kitap alanlarını güncelle
            book.Title = title;
            book.Subject = subject;
            book.Type = type;

            // Price ve stock alanlarını sayıya dönüştürüp güncelle
            if (decimal.TryParse(price, out decimal bookPrice))
            {
                book.Price = bookPrice;
            }
            else
            {
                return Ok("Invalid price format.");
            }

            if (int.TryParse(stock, out int bookStock))
            {
                book.Stock = bookStock;
            }
            else
            {
                return Ok("Invalid stock format.");
            }

            book.Author = author;

            // Değişiklikleri kaydet
            _context.SaveChanges();

            return Ok("Book updated successfully.");
        }
        catch (Exception ex)
        {
            return Ok($"An error occurred: {ex.Message}");
        }
    }

    [HttpPost]
    public IActionResult DeleteBook(int bookId)
    {
        try
        {
            // Kitabı veritabanından bul
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                return Ok("Book not found.");
            }

            // Kitabı veritabanından sil
            _context.Books.Remove(book);
            _context.SaveChanges();

            return Ok($"Book {book.Title} deleted successfully");
        }
        catch (Exception ex)
        {
            return Ok($"An error occurred: {ex.Message}");
        }
    }


    [HttpPost]
   public IActionResult AddBook(string customerId, string title, string subject, string type, decimal price, int stock,
        string author,
        [FromForm] Microsoft.AspNetCore.Http.IFormFile img)
{
    try
    {
        // CustomerId ile müşteriyi bul
        var customer = _context.Customers.FirstOrDefault(c => c.Id == int.Parse(customerId));

        if (customer == null)
        {
            return BadRequest("Müşteri bulunamadı.");
        }

        // Müşterinin status kontrolü
        if (customer.status != 1)
        {
            return BadRequest("Müşterinin statusu uygun değil. Kitap ekleyemezsiniz.");
        }

        if (img != null && img.Length > 0)
        {
            // Resim dosyasını yüklemek için kullanacağınız dizin (örneğin wwwroot içinde)
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/books", img.FileName);

            // Eğer aynı isimde bir dosya varsa, yeni bir dosya adı oluşturun
            if (System.IO.File.Exists(imagePath))
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(img.FileName);
                var fileExtension = Path.GetExtension(img.FileName);
                var newFileName = $"{fileNameWithoutExtension}_{DateTime.Now.Ticks}{fileExtension}";
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/books", newFileName);
            }

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                img.CopyTo(stream);
            }


            var book = new BookModel
            {
                CustomerId = int.Parse(customerId),
                Title = title,
                Subject = subject,
                Type = type,
                Price = price,
                Stock = stock,
                Author = author,
                Image = Path.GetFileName(imagePath)
            };
            _context.Books.Add(book);
            _context.SaveChanges();

            return Ok("Kitap başarıyla eklendi.");
        }
        else
        {
            return BadRequest("Resim dosyası yüklenemedi.");
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
    }
}



    [HttpPost]
    public IActionResult UpdateSaleStatus(int bookId)
    {
        try
        {
            // Satış kaydını veritabanından bul
            var saleRecord = _context.Sale.FirstOrDefault(s => s.BookId == bookId);
            if (saleRecord == null)
            {
                return NotFound("Sale record not found.");
            }

            // Status değerini güncelle, 4'ten küçükse bir artır
            if (saleRecord.Status < 4)
            {
                saleRecord.Status += 1;

                // Değişiklikleri kaydet
                _context.SaveChanges();

                return Ok("Status updated successfully.");
            }
            else
            {
                return Ok("Status cannot be increased as it is already 4 or more.");
            }
        }
        catch (Exception ex)
        {
            return Ok($"An error occurred: {ex.Message}");
        }
    }

    
    [HttpPost]
    public IActionResult UpdateProfile(string customerId, string name, string phone, string email)
    {
        try
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id.ToString() == customerId);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            customer.Name = name;
            customer.phone = phone;
            customer.Mail = email;

            _context.SaveChanges();

            return Ok("Profile updated successfully.");
        }
        catch (Exception ex)
        {
            return Ok($"An error occurred: {ex.Message}");
        }
    }
    
}