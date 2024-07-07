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

namespace BookSale.Areas.AdminPanel.Controllers;

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
        var memberIdHass = controller?.HttpContext.Session.GetString("adminPanel_id");

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
                context.ActionArguments["adminId"] = EncryptionHelperPass.DecryptId(memberIdHass);
            }
        }

        base.OnActionExecuting(context);
    }
}



[Area("AdminPanel")]
[SessionCheck("/AuthPanel/Auth/AdminAuth")]

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Tables()
    {
        return View();
    }

    public IActionResult Customers()
    {
        var customers = GetCustomers();
        return View(customers);
    }

    public IActionResult Books()
    {
        var books = GetBooks();
        return View(books);
    }

    public IActionResult Sales()
    {
        var sales = GetSales();
        return View(sales);
    }

    public IActionResult Members()
    {
        var members = GetMembers();
        return View(members);
    }

    public IActionResult Profile()
    {
        return View();
    }

    public IActionResult LogOut()
    {
        HttpContext.Session.Remove("adminPanel_id");
        return Ok("");
    }


    private List<CustomerModel> GetCustomers()
    {
        return _context.Customers.ToList();
    }

    private List<BookModel> GetBooks()
    {
        return _context.Books.ToList();
    }

    private List<SaleModel> GetSales()
    {
        return _context.Sale
            .Include(s => s.Book)
            .Include(s => s.Customer)
            .ToList();
    }

    private List<MemberModel> GetMembers()
    {
        return _context.Member.ToList();
    }

    [HttpPost]
    public IActionResult UpdateStatus(int id)
    {
        try
        {
            // Müşteriyi veritabanından bul
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Status 1 ise işlem yapma
            if (customer.status == 1)
            {
                return Ok("Customer status is already 1.");
            }

            // Status'i 1 yap
            customer.status = 1;

            // Değişiklikleri kaydet
            _context.SaveChanges();

            return Ok("Customer status updated successfully.");
        }
        catch (Exception ex)
        {
            return Ok($"An error occurred: {ex.Message}");
        }
    }
    
    public IActionResult DeleteBook(int bookId)
    {
        try
        {
            // Kitabı veritabanından bul
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }

            // Kitabı veritabanından sil
            _context.Books.Remove(book);
            _context.SaveChanges();

            return Ok("Book deleted successfully.");
        }
        catch (Exception ex)
        {
            return Ok($"An error occurred: {ex.Message}");
        }
    }


    public IActionResult DeleteCustomer(int customerId)
    {
        try
        {
            // Müşteriyi veritabanından bul
            var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Müşteriyi veritabanından sil
            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return Ok("Customer deleted successfully.");
        }
        catch (Exception ex)
        {
            return Ok($"An error occurred: {ex.Message}");
        }
    }


}