using System.ComponentModel.DataAnnotations.Schema;

namespace BookSale.Models
{
    public class BookModel
    {
        public int Id { get; set; }

        [Column("customer_id")] public int CustomerId { get; set; }

        public string Image { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }

        [Column("description")] public string Desc { get; set; }
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string Author { get; set; }
    }

    public class CartModel
    {
        public int Id { get; set; }
        [Column("member_id")] public int MemberId { get; set; }

        [Column("customer_id")] public int CustomerId { get; set; }

        [Column("books_id")] public int BookId { get; set; }
        public int Piece { get; set; }
        public BookModel Book { get; set; }
    }

    public class PastCartModel
    {
        public int Id { get; set; }

        [Column("member_id")] public int MemberId { get; set; }

        [Column("customer_id")] public int CustomerId { get; set; }

        [Column("book_id")] public int BookId { get; set; }
        public int Piece { get; set; }
        public BookModel Book { get; set; }
    }

    public class MemberModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Column("mail")] public string Email { get; set; }
        [Column("password")] public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    public class ContactModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Subject { get; set; }

        [Column("description")] public string Description { get; set; }
    }

    public class SaleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Mail { get; set; }
        public bool IsMember { get; set; }
        [Column("book_id")] public int BookId { get; set; }
        [Column("member_id")] public int MemberId { get; set; }
        [Column("customer_id")] public int CustomerId { get; set; }
        public int Status { get; set; }
        public int Piece { get; set; }
        public BookModel Book { get; set; }
        public CustomerModel Customer { get; set; }
    }


    public class CustomerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string phone { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public int status { get; set; }
    }

    public class AdminModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
    }
}