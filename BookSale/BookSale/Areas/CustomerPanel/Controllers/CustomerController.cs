using Microsoft.AspNetCore.Mvc;
using BookSale.Data;
using BookSale.EnDeCode;
using Microsoft.AspNetCore.Mvc.Filters;
using BookSale.Models;
using BookSale.Methods;

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
    public readonly BookMethod _bookMethod;
    public readonly SaleMethod _saleMethod;
    public readonly ProfileMethod _profileMethod;

    public CustomerController(
        ApplicationDbContext context,
        BookMethod bookMethod,
        SaleMethod saleMethod,
        ProfileMethod profileMethod
    )
    {
        _context = context;
        _bookMethod = bookMethod;
        _saleMethod = saleMethod;
        _profileMethod = profileMethod;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Books(string customerId)
    {
        var books = _bookMethod.GetBooks(customerId: int.Parse(customerId));
        return View(books);
    }

    public IActionResult Sales(string customerId)
    {
        var sales = _saleMethod.GetSales(customerId: customerId);
        return View(sales);
    }

    public IActionResult Profile(string customerId)
    {
        IQueryable<CustomerModel> query = _context.Customers.Where(c => c.Id == int.Parse(customerId));
        List<CustomerModel> customer = query.ToList();

        return View(customer);
    }

    public IActionResult LogOut()
    {
        HttpContext.Session.Remove("customerPanel_id");
        return Ok("");
    }


    [HttpPost]
    public IActionResult UpdateBook(int bookId, string title, string subject, string type, string price, string stock,
        string author)
    {
        var message = _bookMethod.UpdateBook(bookId, title, subject, type, price, stock, author);
        return Ok(message);
    }

    [HttpPost]
    public IActionResult DeleteBook(int bookId)
    {
        var message = _bookMethod.DeleteBook(bookId);
        return Ok(message);
    }


    [HttpPost]
    public IActionResult AddBook(string customerId, string title, string subject, string type, decimal price, int stock,
        string author,
        [FromForm] IFormFile img)
    {
        var message = _bookMethod.AddBook(int.Parse(customerId), title, subject, type, price, stock, author, img);
        return Ok(message);
    }


    [HttpPost]
    public IActionResult UpdateSaleStatus(int bookId)
    {
        var message = _saleMethod.UpdateCustomerToMemberSaleStatus(bookId);
        return Ok(message);
    }


    [HttpPost]
    public IActionResult UpdateProfile(string customerId, string name, string phone, string email)
    {
        var message = _profileMethod.UpdateCustomerProfile(customerId, name, phone, email);
        return Ok(message);
    }
}