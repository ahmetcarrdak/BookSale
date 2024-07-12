using BookSale.Models;
using BookSale.Data;
using BookSale.EnDeCode;


namespace BookSale.Methods;

public class MemberMethod
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MemberMethod(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
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
    
    public List<MemberModel> SelectMember(int id = 0)
    {
        IQueryable<MemberModel> query = _context.Member;

        if (id > 0)
        {
            query = query.Where(m => m.Id == id);
        }
        else
        {
            return new List<MemberModel>(); // veya return null; 
        }

        return query.ToList();
    }

    public string MemberLogin(string email, string password)
    {
        var member = _context.Member.FirstOrDefault(m => m.Email == email);

        if (member != null && EncryptionHelperPass.DecryptId(member.Password) == password)
        {
            string memberIdEncrypted = EncryptionHelperPass.EncryptId(member.Id.ToString());

            _httpContextAccessor.HttpContext.Session.SetString("memberPanel_id", memberIdEncrypted);
            return "success";
        }
        else
        {
            return "error";
        }
    }

}