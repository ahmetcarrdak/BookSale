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
using BookSale.Methods;

namespace BookSale.Areas.MemberPanel.Controllers;

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
        var memberIdHass = controller?.HttpContext.Session.GetString("memberPanel_id");

        if (string.IsNullOrEmpty(memberIdHass))
        {
            // Session yoksa veya memberPanel_id null veya boş ise yönlendirme yap
            context.Result = new RedirectResult(_redirectUrl);
            return;
        }
        else
        {
            // Session'dan alınan memberId'ı action methodlarına parametre olarak ekle
            context.ActionArguments["memberId"] = EncryptionHelperPass.DecryptId(memberIdHass);
        }

        base.OnActionExecuting(context);
    }
}

[Area("MemberPanel")]
[SessionCheck("/")]
public class MemberController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SelectCart _selectCart;
    private readonly SelectSales _selectSales;
    private readonly SelectPastCart _selectPastCart;
    private readonly SaleInsert _saleInsert;
    private readonly SelectMembers _selectMembers;
    private readonly PastCart _pastCart;
    private readonly ProfileUpdate _profileUpdate;

    public MemberController(
        ApplicationDbContext context,
        SelectCart selectCart,
        SelectSales selectSales,
        SelectPastCart selectPastCart,
        SaleInsert saleInsert,
        SelectMembers selectMembers,
        PastCart pastCart,
        ProfileUpdate profileUpdate
    )
    {
        _context = context;
        _selectCart = selectCart;
        _selectSales = selectSales;
        _selectPastCart = selectPastCart;
        _saleInsert = saleInsert;
        _selectMembers = selectMembers;
        _pastCart = pastCart;
        _profileUpdate = profileUpdate;
    }


    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Cart(string memberId)
    {
        var cart = _selectCart.GetCart(memberId);
        return View(cart);
    }

    public IActionResult CartHistory(string memberId)
    {
        var cart = _selectPastCart.GetPastCart(memberId);
        return View(cart);
    }

    public IActionResult PastSale(string memberId)
    {
        var sale = _selectSales.GetSales(memberId);
        return View(sale);
    }

    public IActionResult Profile(string memberId)
    {
        var id = EncryptionHelper.DecryptId(memberId);
        var user = _selectMembers.SelectMember(id);
        return View(user);
    }
    
    [HttpPost]
    public IActionResult removeFromCart(string x1, string x2, string x3, string x4, string x5)
    {
        var message = _pastCart.AddToCart(x1 ,x2, x3, x4, x5);
        return Ok(message);
    }

    [HttpPost]
    public IActionResult removeFromPassCart(string x1, string x2, string x3, string x4, string x5)
    {
      var message = _pastCart.RemoveToCart(x1, x2, x3, x4, x5);
      return Ok(message);
    }
    

    public IActionResult ProfileUpdate(string notify, string username, string phone, string email, string address)
    {
        var message = _profileUpdate.UpdateProfile(notify, username, phone, email, address);
        return Ok(message);
    }

    public IActionResult CompletePurchase(string memberId)
    {
        var message = _saleInsert.CompliteSaleInsert(memberId);
        return Ok(message);
    }

    public IActionResult LogOut()
    {
        // Session'daki member_id'yi sil
        HttpContext.Session.Remove("memberPanel_id");
        return Ok("");
    }
    
    
    /* Price Total */
    public IActionResult CartTotal(string memberId)
    {
        var cart = _selectCart.GetCart(memberId);
        decimal total = 0;
        foreach (var item in cart)
        {
            total += item.Piece * item.Book.Price;
        }

        return Ok(total.ToString("N2"));
    }

    public IActionResult PassCartTotal(string memberId)
    {
        var cart = _selectPastCart.GetPastCart(memberId);
        decimal total = 0;
        foreach (var item in cart)
        {
            total += item.Piece * item.Book.Price;
        }

        return Ok(total.ToString("N2"));
    }
}