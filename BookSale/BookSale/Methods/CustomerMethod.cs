using BookSale.Models;
using BookSale.Data;
using BookSale.EnDeCode;

namespace BookSale.Methods;

public class CustomerMethod
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomerMethod(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }


    public List<CustomerModel> GetCustomers()
    {
        return _context.Customers.ToList();
    }

    
    public string Login(string email, string password)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Mail == email);
        var message = "";
        if (customer != null && EncryptionHelperPass.DecryptId(customer.Password) == password)
        {
            _httpContextAccessor.HttpContext.Session.SetString("customerPanel_id",
                EncryptionHelper.EncryptId(customer.Id));
            message = "success";
        }
        else
        {
            message = "Users Not Found.";
        }


        return ($"{message}");
    }

    public string Register(string name, string phone, string email, string password)
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
        return ($"{message}");
    }

    public string Delete(int customerId)
    {
        try
        {
            // Müşteriyi veritabanından bul
            var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
            {
                return ("Customer not found.");
            }

            // Müşteriyi veritabanından sil
            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return ("Customer deleted successfully.");
        }
        catch (Exception ex)
        {
            return ($"An error occurred: {ex.Message}");
        }
    }

    public string UpdateStatus(int id)
    {
        try
        {
            // Müşteriyi veritabanından bul
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return ("Customer not found.");
            }

            // Status 1 ise işlem yapma
            if (customer.status == 1)
            {
                return ("Customer status is already 1.");
            }

            // Status'i 1 yap
            customer.status = 1;

            // Değişiklikleri kaydet
            _context.SaveChanges();

            return ("Customer status updated successfully.");
        }
        catch (Exception ex)
        {
            return ($"An error occurred: {ex.Message}");
        }
    }
}