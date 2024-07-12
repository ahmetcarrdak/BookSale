using BookSale.Data;
using BookSale.Models;

namespace BookSale.Methods
{
    public class BookMethod
    {
        private readonly ApplicationDbContext _context;

        public BookMethod(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<BookModel> GetBooks(int limit = 0, int id = 0, int customerId = 0, string key = null, string subject = null,
            string type = null, string author = null)
        {
            IQueryable<BookModel> query = _context.Books;

            if (id > 0)
            {
                query = query.Where(b => b.Id == id);
            }

            if (customerId > 0)
            {
                query = query.Where(b => b.CustomerId == customerId);
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

        public string UpdateBook(int bookId, string title, string subject, string type, string price, string stock,
            string author)
        {
            try
            {
                // Kitabı veritabanından bul
                var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                {
                    return ("Book not found.");
                }

                // Diğer parametrelerle kitap alanlarını güncelle
                book.Title = title;
                book.Subject = subject;
                book.Type = type;

                // Price ve stock alanlarını sayıya dönüştürüp güncelle
                if (decimal.TryParse(price, out decimal bookPrice))
                {
                    book.Price = bookPrice;
                }
                else
                {
                    return ("Invalid price format.");
                }

                if (int.TryParse(stock, out int bookStock))
                {
                    book.Stock = bookStock;
                }
                else
                {
                    return ("Invalid stock format.");
                }

                book.Author = author;

                // Değişiklikleri kaydet
                _context.SaveChanges();

                return ("Book updated successfully.");
            }
            catch (Exception ex)
            {
                return ($"An error occurred: {ex.Message}");
            }
        }

        public string DeleteBook(int bookId)
        {
            try
            {
                // Kitabı veritabanından bul
                var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                {
                    return ("Book not found.");
                }

                // Kitabı veritabanından sil
                _context.Books.Remove(book);
                _context.SaveChanges();

                return ($"Book {book.Title} deleted successfully");
            }
            catch (Exception ex)
            {
                return ($"An error occurred: {ex.Message}");
            }
        }

        public string AddBook(int customerId, string title, string subject, string type, decimal price, int stock,
            string author,
            Microsoft.AspNetCore.Http.IFormFile img)
        {
            try
            {
                // CustomerId ile müşteriyi bul
                var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);

                if (customer == null)
                {
                    return "Müşteri bulunamadı.";
                }

                // Müşterinin status kontrolü
                if (customer.status != 1)
                {
                    return "Müşterinin statusu uygun değil. Kitap ekleyemezsiniz.";
                }

                // Resim dosyasını yüklemek için kullanacağınız dizin (örneğin wwwroot içinde)
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/books", img.FileName);

                // Eğer aynı isimde bir dosya varsa, yeni bir dosya adı oluşturun
                if (System.IO.File.Exists(imagePath))
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(img.FileName);
                    var fileExtension = Path.GetExtension(img.FileName);
                    var newFileName = $"{fileNameWithoutExtension}_{DateTime.Now.Ticks}{fileExtension}";
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/books", newFileName);
                }

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    img.CopyTo(stream);
                }

                var book = new BookModel
                {
                    CustomerId = customerId,
                    Title = title,
                    Subject = subject,
                    Type = type,
                    Price = price,
                    Stock = stock,
                    Author = author,
                    Image = Path.GetFileName(imagePath)
                };

                _context.Books.Add(book);
                _context.SaveChanges();

                return "Kitap başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                return $"Bir hata oluştu: {ex.Message}";
            }
        }
    }
}
