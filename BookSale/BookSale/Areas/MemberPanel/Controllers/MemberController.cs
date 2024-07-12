using Microsoft.AspNetCore.Mvc;
using BookSale.Data;
using BookSale.EnDeCode;
using Microsoft.AspNetCore.Mvc.Filters;
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
    private readonly CartMethod _cartMethod;
    private readonly SaleMethod _saleMethod;
    private readonly PastCartMethod _pastCartMethod;
    private readonly MemberMethod _memberMethod;
    private readonly ProfileMethod _profileMethod;

    public MemberController(
        ApplicationDbContext context,
        CartMethod cartMethod,
        SaleMethod saleMethod,
        PastCartMethod pastCartMethod,
        MemberMethod memberMethod,
        ProfileMethod profileMethod
    )
    {
        _context = context;
        _cartMethod = cartMethod;
        _saleMethod = saleMethod;
        _pastCartMethod = pastCartMethod;
        _memberMethod = memberMethod;
        _profileMethod = profileMethod;
    }


    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Cart(string memberId)
    {
        var cart = _cartMethod.GetCart(memberId);
        return View(cart);
    }

    public IActionResult CartHistory(string memberId)
    {
        var cart = _pastCartMethod.GetPastCart(memberId);
        return View(cart);
    }

    public IActionResult PastSale(string memberId)
    {
        var sale = _saleMethod.GetSales(memberId: memberId);
        return View(sale);
    }

    public IActionResult Profile(string memberId)
    {
        var id = EncryptionHelper.DecryptId(memberId);
        var user = _memberMethod.SelectMember(id);
        return View(user);
    }
    
    [HttpPost]
    public IActionResult removeFromCart(string x1, string x2, string x3, string x4, string x5)
    {
        var message = _pastCartMethod.AddToCart(x1 ,x2, x3, x4, x5);
        return Ok(message);
    }

    [HttpPost]
    public IActionResult removeFromPassCart(string x1, string x2, string x3, string x4, string x5)
    {
      var message = _pastCartMethod.RemoveToCart(x1, x2, x3, x4, x5);
      return Ok(message);
    }
    

    public IActionResult ProfileUpdate(string notify, string username, string phone, string email, string address)
    {
        var message = _profileMethod.UpdateMemberProfile(notify, username, phone, email, address);
        return Ok(message);
    }

    public IActionResult CompletePurchase(string memberId)
    {
        var message = _saleMethod.CompliteSaleInsert(memberId);
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
        var cart = _cartMethod.GetCart(memberId);
        decimal total = 0;
        foreach (var item in cart)
        {
            total += item.Piece * item.Book.Price;
        }

        return Ok(total.ToString("N2"));
    }

    public IActionResult PassCartTotal(string memberId)
    {
        var cart = _pastCartMethod.GetPastCart(memberId);
        decimal total = 0;
        foreach (var item in cart)
        {
            total += item.Piece * item.Book.Price;
        }

        return Ok(total.ToString("N2"));
    }
}