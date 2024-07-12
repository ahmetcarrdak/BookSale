using Microsoft.AspNetCore.Mvc;
using BookSale.Data;
using BookSale.EnDeCode;
using BookSale.Models;
using BookSale.Methods;

namespace BookSale.Areas.AuthPanel.Controllers;

[Area("AuthPanel")]
public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CustomerMethod _customerMethod;

    public AuthController(
        ApplicationDbContext context,
        CustomerMethod customerMethod
    )
    {
        _context = context;
        _customerMethod = customerMethod;
    }

    public IActionResult CustomerAuth()
    {
        return View();
    }

    public IActionResult AdminAuth()
    {
        return View();
    }


    /* Customer */
    [HttpPost]
    public IActionResult CustomerLogin(string email, string password)
    {
        var message = _customerMethod.Login(email, password);
        return Ok($"{message}");
    }


    [HttpPost]
    public IActionResult CustomerRegister(string name, string phone, string email, string password)
    {
        var message = _customerMethod.Register(name, phone, email, password);
        return Ok($"{message}");
    }


    /* Admin */
    [HttpPost]
    public IActionResult AdminLogin(string email, string password)
    {
        var admin = _context.Admins.FirstOrDefault(a => a.Mail == email);
        var message = "";
        if (admin != null && EncryptionHelperPass.DecryptId(admin.Password) == password)
        {
            HttpContext.Session.SetString("adminPanel_id", EncryptionHelper.EncryptId(admin.Id));
            message = "success";
        }
        else
        {
            message = "Users Not Found.";
        }


        return Ok($"{message}");
    }
}