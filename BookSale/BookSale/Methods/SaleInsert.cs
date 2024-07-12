using BookSale.Models;
using BookSale.Data;
using BookSale.EnDeCode;
using Microsoft.EntityFrameworkCore;

namespace BookSale.Methods;

public class SaleInsert
{
    private readonly ApplicationDbContext _context;

    public SaleInsert(ApplicationDbContext context)
    {
        _context = context;
    }


    public string InsertSale(string notify, string name, string phone, string address, string mail,
        string customerId, int piece)
    {
        try
            {
                // Kontrol edilecek alanları ve hata mesajlarını tanımla
                var fields = new (string Value, string Message)[]
                {
                    (notify, "Notify cannot be empty."),
                    (name, "Name cannot be empty."),
                    (phone, "Phone cannot be empty."),
                    (address, "Address cannot be empty."),
                    (mail, "Mail cannot be empty."),
                    (customerId, "Customer ID cannot be empty.")
                };

                // Tüm alanların boş olup olmadığını kontrol et
                if (fields.All(f => string.IsNullOrWhiteSpace(f.Value)))
                {
                    return ("All fields cannot be empty.");
                }

                // Boş alan kontrolü yap
                foreach (var field in fields)
                {
                    if (string.IsNullOrWhiteSpace(field.Value))
                    {
                        return (field.Message);
                    }
                }

                // Üyelik kontrolü yap
                var member = _context.Member.FirstOrDefault(m => m.Email == mail);
                bool isMember = member != null;
                int memberId = isMember ? member.Id : 0;

                // BookId'yi çöz
                var decryptedBookId = EncryptionHelper.DecryptId(notify);
                var decryptedCustomer = EncryptionHelper.DecryptId(customerId);

                // Kitabı bul
                var book = _context.Books.FirstOrDefault(b => b.Id == decryptedBookId);
                if (book == null || book.Stock <= 0)
                {
                    return ("Requested book is out of stock or not found.");
                }

                // Satış kaydını oluştur
                var sale = new SaleModel()
                {
                    BookId = decryptedBookId,
                    Name = name,
                    Phone = phone,
                    Address = address,
                    Mail = mail,
                    IsMember = isMember,
                    CustomerId = decryptedCustomer,
                    Piece = piece != 0 ? piece : 1,
                    Status = 0,
                    MemberId = memberId,
                };

                // Satışı kaydet
                _context.Sale.Add(sale);
                _context.SaveChanges();

                // Stoktan düşür
                book.Stock--;
                _context.SaveChanges();

                return ("Success");
            }
            catch (Exception ex)
            {
                return ($"Error: {ex.Message}");
            }
    }


    public string CompliteSaleInsert(string memberId)
    {
          try
        {
            // Üye bilgilerini al
            var member = _context.Member.FirstOrDefault(m => m.Id == int.Parse(memberId));
            if (member == null)
            {
                return ("Member not found.");
            }

            // Üyenin sepetindeki kitapları al
            var cartItems = _context.Cart.Include(c => c.Book)
                .Where(c => c.MemberId == int.Parse(memberId))
                .ToList();

            if (!cartItems.Any())
            {
                return ("No items in the cart to purchase.");
            }

            // Her bir sepet öğesi için satış işlemi oluştur
            foreach (var item in cartItems)
            {
                // Stok durumunu kontrol et
                var book = _context.Books.FirstOrDefault(b => b.Id == item.BookId);
                if (book == null)
                {
                    return ("Book not found in database: " + item.BookId);
                }

                if (book.Stock < item.Piece)
                {
                    return ("Insufficient stock for book: " + book.Title);
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

            return ("success");
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