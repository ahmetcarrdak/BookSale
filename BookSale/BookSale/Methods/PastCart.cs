using BookSale.Models;
using BookSale.Data;
using BookSale.EnDeCode;
using Microsoft.EntityFrameworkCore;

namespace BookSale.Methods;

public class AddPastCart
{
    private readonly ApplicationDbContext _context;

    public AddPastCart(ApplicationDbContext context)
    {
        _context = context;
    }

    public string AddToCart(string x1, string x2, string x3, string x4, string x5)
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

                return ("success");
            }
            else
            {
                return ("Item not found in cart.");
            }
        }
        catch (DbUpdateException ex)
        {
            var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return ("Database Update Error: " + innerException);
        }
        catch (Exception ex)
        {
            return ("Error: " + ex.Message);
        }
    }
}