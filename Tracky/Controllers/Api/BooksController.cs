using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Tracky.Domain.Entities.Books;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;

namespace Tracky.Controllers.Api
{
    /// <summary>
    /// This controller for the simple Book domain object (with single dependency to another domain object, Author)
    /// demonstrates some basic cases for employing DTO objects. 
    /// </summary>
    public class BooksController : ApiController
    {
        private LibraryContext db = new LibraryContext();

        // GET: api/Books
        public IQueryable<BookDto> GetBooks()
        {
            // Here we see a DTO employed to obscure all but the essential details to identify each book, primarily for display purposes, for the full book list.
            // The return type for the IQueryable has been changed from Book to BookDto, and the method now loops through each raw Book to create its DTO counterpart.
            var books = from b in db.Books
                        select new BookDto()
                        {
                            Id = b.Id,
                            Title = b.Title,
                            AuthorName = b.Author.Name
                        };
            return books;
        }

        // GET: api/Books/5
        [ResponseType(typeof(BookDetailDto))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            // Here we see a DTO employed to provide a fuller set of details for a single requested book. The type in the ResponseType attribute has been
            // changed from Book to BookDetailDto, and the method has changed how it requests the data. Instead of merely calling .FindAsync(id) on the
            // Books DbSet, it instead uses .SingleOrDefaultAsync(b => b.Id == id) to select the requested Book along with its associated Author, which
            // gets dereferenced in the Select's lambda expression that transforms the selected book into a BookDetailDto. 
            var book = await db.Books.Select(b =>
                new BookDetailDto()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Year = b.Year,
                    Price = b.Price,
                    AuthorName = b.Author.Name,
                    Genre = b.Genre
                }).SingleOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // PUT: api/Books/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Books
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> PostBook(Book book)
        {
            // Here we also see the same simplified Book DTO employed for the response to a successful posting of a new Book. Instead of the default behavior of
            // returning the CreatedAtRoute using the raw book as the 3rd parameter (which is content), the method has been changed to first load the related Author
            // object, then build a simplified DTO, which it then uses in place of the raw Book as the content parameter of CreatedAtRoute.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Books.Add(book);
            await db.SaveChangesAsync();

            db.Entry(book).Reference(x => x.Author).Load();

            var dto = new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.Author.Name
            };

            return CreatedAtRoute("DefaultApi", new { id = book.Id }, dto);
        }

        // DELETE: api/Books/5
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            await db.SaveChangesAsync();

            return Ok(book);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.Id == id) > 0;
        }
    }
}