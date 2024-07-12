using System.Collections.Generic;
using System.Linq;
using BookSale.Models;
using BookSale.Data;

namespace BookSale.Methods;

public class SelectMembers
{
    private readonly ApplicationDbContext _context;

    public SelectMembers(ApplicationDbContext context)
    {
        _context = context;
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
}