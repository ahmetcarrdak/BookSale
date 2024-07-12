using BookSale.Models;
using BookSale.Data;

namespace BookSale.Methods;

public class ContactMethod
{

    private readonly ApplicationDbContext _context;

    public ContactMethod(ApplicationDbContext context)
    {
        _context = context;
    }
    public string ContactInsert(string Name, string Mail, string Subject, string Message)
    {
        _context.Contact.Add(new ContactModel
        {
            Name = Name,
            Mail = Mail,
            Subject = Subject,
            Description = Message
        });
        _context.SaveChanges();

        return ("success");
    }
}