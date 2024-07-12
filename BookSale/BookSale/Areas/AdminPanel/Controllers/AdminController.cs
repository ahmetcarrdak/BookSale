using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSale.Data;
using BookSale.EnDeCode;
using Microsoft.AspNetCore.Mvc.Filters;
using BookSale.Models;
using BookSale.Methods;

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
    private readonly CustomerMethod _customerMethod;
    private readonly BookMethod _bookMethod;
    private readonly SaleMethod _saleMethod;
    private readonly MemberMethod _memberMethod;

    public AdminController(
        ApplicationDbContext context, 
        CustomerMethod customerMethod, 
        BookMethod bookMethod,
        SaleMethod saleMethod,
        MemberMethod memberMethod
        )
    {
        _context = context;
        _customerMethod = customerMethod;
        _bookMethod = bookMethod;
        _saleMethod = saleMethod;
        _memberMethod = memberMethod;
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
        var customers = _customerMethod.GetCustomers();
        return View(customers);
    }

    public IActionResult Books()
    {
        var books = _bookMethod.GetBooks();
        return View(books);
    }

    public IActionResult Sales()
    {
        var sales = _saleMethod.GetSales();
        return View(sales);
    }

    public IActionResult Members()
    {
        var members = _memberMethod.SelectMember();
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

    
    [HttpPost]
    public IActionResult UpdateStatus(int id)
    {
        var message = _customerMethod.UpdateStatus(id);
        return Ok(message);
    }

    public IActionResult DeleteBook(int bookId)
    {
        var message = _bookMethod.DeleteBook(bookId);
        return Ok(message);
    }


    public IActionResult DeleteCustomer(int customerId)
    {
        var message = _customerMethod.Delete(customerId);
        return Ok(message);
    }
}