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

    public MemberController(ApplicationDbContext context)
    {
        _context = context;
    }


    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Cart(string memberId)
    {
        var cart = SelectCart(memberId);
        return View(cart);
    }

    public IActionResult CartHistory(string memberId)
    {
        var cart = SelectPastCart(memberId);
        return View(cart);
    }

    public IActionResult PastSale(string memberId)
    {
        var sale = SelectSales(memberId);
        return View(sale);
    }

    public IActionResult Profile(string memberId)
    {
        var user = SelectMember(memberId);
        return View(user);
    }


    private List<MemberModel> SelectMember(string memberId)
    {
        IQueryable<MemberModel> query = _context.Member;
        var id = int.Parse(memberId);
        query = query.Where(b => b.Id == id);
        return query.ToList();
    }

    private List<BookModel> SelectBook(int id)
    {
        IQueryable<BookModel> query = _context.Books;

        if (id > 0)
        {
            query = query.Where(b => b.Id == id);
        }

        return query.ToList();
    }

    private List<CartModel> SelectCart(string memberId)
    {
        // Üye ID'sine göre sepetten tüm öğeleri seçmek için bir sorgu oluşturuyoruz
        List<CartModel> cartItems = _context.Cart.Include(c => c.Book)
            .Where(c => c.MemberId == int.Parse(memberId))
            .ToList();

        // Sepetteki kitap ID'lerini alıyoruz
        List<int> bookIds = cartItems.Select(c => c.BookId).ToList();

        // Kitap ID'lerine göre kitapları çekiyoruz
        IQueryable<BookModel> query = _context.Books.Where(b => bookIds.Contains(b.Id));

        // Sorguyu çalıştırıp sonuçları liste olarak dönüyoruz
        List<BookModel> booksInCart = query.ToList();

        // Sepet öğelerini dönebiliriz, eğer gerekiyorsa burada sadece kitapları dönüyoruz
        return cartItems;
    }

    private List<PastCartModel> SelectPastCart(string memberId)
    {
        // Üye ID'sine göre past sepetten tüm öğeleri seçmek için bir sorgu oluşturuyoruz
        List<PastCartModel> pastCartItems = _context.PastCarts.Include(p => p.Book)
            .Where(p => p.MemberId == int.Parse(memberId))
            .ToList();

        // Sepetteki kitap ID'lerini alıyoruz
        List<int> bookIds = pastCartItems.Select(p => p.BookId).ToList();

        // Kitap ID'lerine göre kitapları çekiyoruz
        IQueryable<BookModel> query = _context.Books.Where(b => bookIds.Contains(b.Id));

        // Sorguyu çalıştırıp sonuçları liste olarak dönüyoruz
        List<BookModel> booksInCart = query.ToList();

        // Sepet öğelerini dönebiliriz
        return pastCartItems;
    }

    private List<SaleModel> SelectSales(string memberId)
    {
        // Üye ID'sine göre satış verilerini seçmek için bir sorgu oluşturuyoruz
        List<SaleModel> sales = _context.Sale
            .Where(s => s.MemberId == int.Parse(memberId))
            .ToList();

        // Satış verilerinin içindeki bookId'leri alıyoruz
        List<int> bookIds = sales.Select(s => s.BookId).ToList();

        // Kitap ID'lerine göre kitapları çekiyoruz
        IQueryable<BookModel> query = _context.Books.Where(b => bookIds.Contains(b.Id));

        // Sorguyu çalıştırıp sonuçları liste olarak dönüyoruz
        List<BookModel> booksInSales = query.ToList();

        // Satış verilerini dönebiliriz, eğer gerekiyorsa burada sadece satışları döndürüyoruz
        return sales;
    }


    private List<BookModel> GetBooks(int id = 0)
    {
        IQueryable<BookModel> query = _context.Books;
        if (id > 0)
        {
            query = query.Where(b => b.Id == id);
        }

        return query.ToList();
    }

    /** Ajax **/
    [HttpPost]
    public IActionResult removeFromCart(string x1, string x2, string x3, string x4, string x5)
    {
        try
        {
            // Şifre çözme işlemi
            int memberId = EncryptionHelper.DecryptId(x1);
            int customerId = EncryptionHelper.DecryptId(x2);
            int bookId = EncryptionHelper.DecryptId(x3);
            int cartId = EncryptionHelper.DecryptId(x4);
            int piece = EncryptionHelper.DecryptId(x5);

            // Sepetten öğeyi bul
            var cartItem = _context.Cart.FirstOrDefault(c => c.Id == cartId);
            if (cartItem != null)
            {
                // Sepetten öğeyi sil
                _context.Cart.Remove(cartItem);

                // Silinen öğeyi PastCarts tablosuna eklemek için kontrol yap
                var existsInPastCarts = _context.PastCarts
                    .Any(pc => pc.MemberId == memberId && pc.CustomerId == customerId && pc.BookId == bookId);

                if (!existsInPastCarts)
                {
                    // PastCarts tablosuna ekle
                    var pastCard = new PastCartModel
                    {
                        MemberId = memberId,
                        CustomerId = customerId,
                        BookId = bookId,
                        Piece = piece,
                    };
                    _context.PastCarts.Add(pastCard);
                }

                // Değişiklikleri kaydet
                _context.SaveChanges();

                return Ok("success");
            }
            else
            {
                return Ok("Item not found in cart.");
            }
        }
        catch (DbUpdateException ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return Ok("Database Update Error: " + innerException);
        }
        catch (Exception ex)
        {
            return Ok("Error: " + ex.Message);
        }
    }

    [HttpPost]
    public IActionResult removeFromPassCart(string x1, string x2, string x3, string x4, string x5)
    {
        try
        {
            // Şifre çözme işlemi
            int memberId = EncryptionHelper.DecryptId(x1);
            int customerId = EncryptionHelper.DecryptId(x2);
            int bookId = EncryptionHelper.DecryptId(x3);
            int pastCartId = EncryptionHelper.DecryptId(x4);
            int piece = EncryptionHelper.DecryptId(x5);

            // PastCarts tablosundan öğeyi bul
            var pastCartItem = _context.PastCarts.FirstOrDefault(pc => pc.Id == pastCartId);
            if (pastCartItem != null)
            {
                // PastCarts tablosundan öğeyi sil
                _context.PastCarts.Remove(pastCartItem);

                // Silinen öğeyi Cart tablosuna eklemek için kontrol yap
                var existsInCart = _context.Cart
                    .Any(c => c.MemberId == memberId && c.CustomerId == customerId && c.BookId == bookId);

                if (!existsInCart)
                {
                    // Cart tablosuna ekle
                    var cartItem = new CartModel
                    {
                        MemberId = memberId,
                        CustomerId = customerId,
                        BookId = bookId,
                        Piece = piece,
                    };
                    _context.Cart.Add(cartItem);
                }

                // Değişiklikleri kaydet
                _context.SaveChanges();

                return Ok("success");
            }
            else
            {
                return Ok("Item not found in past carts.");
            }
        }
        catch (DbUpdateException ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return Ok("Database Update Error: " + innerException);
        }
        catch (Exception ex)
        {
            return Ok("Error: " + ex.Message);
        }
    }

    public IActionResult ProfileUpdate(string notify, string username, string phone, string email, string address)
    {
        try
        {
            if (string.IsNullOrEmpty(notify))
            {
                return Ok("Invalid ID.");
            }

            // Şifre çözme işlemi
            int memberId = EncryptionHelper.DecryptId(notify);

            // Üye kaydını bul
            var member = _context.Member.FirstOrDefault(m => m.Id == memberId);
            if (member == null)
            {
                return Ok("Member not found.");
            }

            // Profil bilgilerini güncelle
            member.Name = username;
            member.Phone = phone;
            member.Email = email;
            member.Address = address;
            member.Password = member.Password;
            // Değişiklikleri kaydet
            _context.SaveChanges();

            return Ok("success");
        }
        catch (DbUpdateException ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return Ok("Database Update Error: " + innerException);
        }
        catch (Exception ex)
        {
            return Ok("Error: " + ex.Message);
        }
    }

    public IActionResult CompletePurchase(string memberId)
    {
        try
        {
            // Üye bilgilerini al
            var member = _context.Member.FirstOrDefault(m => m.Id == int.Parse(memberId));
            if (member == null)
            {
                return Ok("Member not found.");
            }

            // Üyenin sepetindeki kitapları al
            var cartItems = _context.Cart.Include(c => c.Book)
                .Where(c => c.MemberId == int.Parse(memberId))
                .ToList();

            if (!cartItems.Any())
            {
                return Ok("No items in the cart to purchase.");
            }

            // Her bir sepet öğesi için satış işlemi oluştur
            foreach (var item in cartItems)
            {
                // Stok durumunu kontrol et
                var book = _context.Books.FirstOrDefault(b => b.Id == item.BookId);
                if (book == null)
                {
                    return Ok("Book not found in database: " + item.BookId);
                }

                if (book.Stock < item.Piece)
                {
                    return Ok("Insufficient stock for book: " + book.Title);
                }

                // Satış kaydını oluştur
                var sale = new SaleModel
                {
                    BookId = item.BookId, // Notify alanı için bookId ekleniyor
                    Name = member.Name,
                    Phone = member.Phone,
                    Address = member.Address,
                    Mail = member.Email,
                    IsMember = true,
                    MemberId = int.Parse(memberId) // SaleModel'e MemberId ekleniyor
                };

                _context.Sale.Add(sale);

                // Stoktan düş
                book.Stock -= item.Piece;

                // Sepetten öğeyi sil
                _context.Cart.Remove(item);
            }

            // Değişiklikleri kaydet
            _context.SaveChanges();

            return Ok("success");
        }
        catch (DbUpdateException ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return Ok("Database Update Error: " + innerException);
        }
        catch (Exception ex)
        {
            return Ok("Error: " + ex.Message);
        }
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
        var cart = SelectCart(memberId);
        decimal total = 0;
        foreach (var item in cart)
        {
            total += item.Piece * item.Book.Price;
        }

        return Ok(total.ToString("N2"));
    }

    public IActionResult PassCartTotal(string memberId)
    {
        var cart = SelectPastCart(memberId);
        decimal total = 0;
        foreach (var item in cart)
        {
            total += item.Piece * item.Book.Price;
        }

        return Ok(total.ToString("N2"));
    }
}