using System.Linq;
using BookSale.Models;
using BookSale.Data;
using BookSale.EnDeCode;

namespace BookSale.Methods;

public class MemberInsert
{
    private readonly ApplicationDbContext _context;

    public MemberInsert(ApplicationDbContext context)
    {
        _context = context;
    }

    public string InsertMember(string Name, string Phone, string Mail, string Address, string Password)
    {
        try
        {
            var existingMember = _context.Member.FirstOrDefault(m => m.Email == Mail);
            if (existingMember != null)
            {
                return ("Bu e-posta adresi zaten kullanımda.");
            }

            var encryptedPassword = EncryptionHelperPass.EncryptId(Password);

            var newMember = new MemberModel
            {
                Name = Name,
                Phone = Phone,
                Email = Mail,
                Address = Address,
                Password = encryptedPassword
            };

            _context.Member.Add(newMember);
            _context.SaveChanges();

            return ("success");
        }
        catch (Exception ex)
        {
            return $"Bir hata oluştu: {ex.Message}";
        }
    }

}