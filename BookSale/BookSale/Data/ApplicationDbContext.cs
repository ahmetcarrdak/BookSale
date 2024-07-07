using Microsoft.EntityFrameworkCore;
using BookSale.Models;

namespace BookSale.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BookModel> Books { get; set; }
        public DbSet<MemberModel> Member { get; set; }
        public DbSet<CartModel> Cart { get; set; }
        public DbSet<ContactModel> Contact { get; set; }
        public DbSet<SaleModel> Sale { get; set; }
        public DbSet<PastCartModel> PastCarts { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<AdminModel> Admins { get; set; }
    }
}