using System.Linq;
using BookSale.Models;
using BookSale.Data;
using BookSale.EnDeCode;
using Microsoft.EntityFrameworkCore;


namespace BookSale.Methods
{
    public class CartMethod
    {
        private readonly ApplicationDbContext _context;
        private readonly BookMethod _bookMethod;

        public CartMethod(ApplicationDbContext context, BookMethod bookMethod)
        {
            _context = context;
            _bookMethod = bookMethod;
        }

        public string CartInsert(string Number, string? memberIdHass)
        {
            if (memberIdHass == null)
            {
                return "noSession";
            }

            var memberId = EncryptionHelper.DecryptId(memberIdHass);
            var id = EncryptionHelper.DecryptId(Number);

            // Check if the book with given id exists
            var book = _bookMethod.GetBooks(id: id).FirstOrDefault();
            if (book == null)
            {
                return "error"; // Or handle the case where the book with given id does not exist
            }

            // Check if the book is already in the cart
            var cart = _context.Cart.FirstOrDefault(c =>
                c.MemberId == memberId && c.CustomerId == book.CustomerId && c.BookId == book.Id && c.Piece == 1);

            if (cart == null)
            {
                // Add the book to the cart
                _context.Cart.Add(new CartModel
                {
                    MemberId = memberId,
                    CustomerId = book.CustomerId,
                    BookId = book.Id,
                    Piece = 1
                });
                _context.SaveChanges();
                return "success";
            }
            else
            {
                // Increment the piece count of the existing cart item
                cart.Piece++;
                _context.SaveChanges();
                return "update";
            }
        }
        
        public List<CartModel> GetCart(string memberId)
        {
            return _context.Cart.Include(c => c.Book)
                .Where(c => c.MemberId == int.Parse(memberId))
                .ToList();
        }
    }
}