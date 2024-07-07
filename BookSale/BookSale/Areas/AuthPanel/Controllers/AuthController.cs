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

namespace BookSale.Areas.AuthPanel.Controllers;

[Area("AuthPanel")]
public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
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
        var customer = _context.Customers.FirstOrDefault(c => c.Mail == email);
        var message = "";
        if (customer != null && EncryptionHelperPass.DecryptId(customer.Password) == password)
        {
            HttpContext.Session.SetString("customerPanel_id", EncryptionHelper.EncryptId(customer.Id));
            message = "success";
        }
        else
        {
            message = "Users Not Found.";
        }


        return Ok($"{message}");
    }


    [HttpPost]
    public IActionResult CustomerRegister(string name, string phone, string email, string password, string address)
    {
        var existingCustomer = _context.Customers.FirstOrDefault(c => c.Mail == email);
        var message = "";
        if (existingCustomer != null)
        {
            message = "This email is already registered.";
        }

        var hashedPassword = EncryptionHelperPass.EncryptId(password);
        var customer = new CustomerModel
        {
            Name = name,
            phone = phone,
            Mail = email,
            Password = hashedPassword,
        };

        _context.Customers.Add(customer);
        _context.SaveChanges();

        message = "success";
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