using BookSale.Models;
using BookSale.Data;
using BookSale.EnDeCode;

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
    
}