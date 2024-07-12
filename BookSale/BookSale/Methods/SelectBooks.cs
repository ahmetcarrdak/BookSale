using System.Collections.Generic;
using System.Linq;
using BookSale.Models;
using BookSale.Data;

namespace BookSale.Methods
{
    public class SelectBooks
    {
        private readonly ApplicationDbContext _context;

        public SelectBooks(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public List<BookModel> GetBooks(int limit = 0, int id = 0, string key = null, string subject = null,
            string type = null, string author = null)
        {
            IQueryable<BookModel> query = _context.Books;

            if (id > 0)
            {
                query = query.Where(b => b.Id == id);
            }

            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(b => b.Desc.Contains(key));
            }

            if (!string.IsNullOrEmpty(subject))
            {
                query = query.Where(b => b.Subject.Contains(subject));
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(b => b.Type.Contains(type));
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.Contains(author));
            }

            if (limit > 0)
            {
                query = query.Take(limit);
            }

            return query.ToList();
        }
    }
}