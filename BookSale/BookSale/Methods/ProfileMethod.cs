using BookSale.Models;
using BookSale.Data;
using BookSale.EnDeCode;
using Microsoft.EntityFrameworkCore;

namespace BookSale.Methods;

public class ProfileMethod
{
    private readonly ApplicationDbContext _context;

    public ProfileMethod(ApplicationDbContext context)
    {
        _context = context;
    }

    public string UpdateMemberProfile(string notify, string username, string phone, string email, string address)
    {
        try
        {
            if (string.IsNullOrEmpty(notify))
            {
                return ("Invalid ID.");
            }

            // Şifre çözme işlemi
            int memberId = EncryptionHelper.DecryptId(notify);

            // Üye kaydını bul
            var member = _context.Member.FirstOrDefault(m => m.Id == memberId);
            if (member == null)
            {
                return ("Member not found.");
            }

            // Profil bilgilerini güncelle
            member.Name = username;
            member.Phone = phone;
            member.Email = email;
            member.Address = address;
            member.Password = member.Password;
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

    public string UpdateCustomerProfile(string customerId, string name, string phone, string email)
    {
        try
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id.ToString() == customerId);
            if (customer == null)
            {
                return ("Customer not found.");
            }

            customer.Name = name;
            customer.phone = phone;
            customer.Mail = email;

            _context.SaveChanges();

            return ("Profile updated successfully.");
        }
        catch (Exception ex)
        {
            return ($"An error occurred: {ex.Message}");
        }
    }
    
}