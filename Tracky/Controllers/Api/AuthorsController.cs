using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Tracky.Domain.Entities.Books;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;
using Tracky.Domain.Services.EF.Books;

namespace Tracky.Controllers.Api
{
    public class AuthorsController : ApiController
    {
        private readonly LibraryContext _db = new LibraryContext();
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        // GET: api/Authors
        public IQueryable<Author> GetAuthors()
        {
            //return _db.Authors;
            _db.Authors.ToList()[0].Name += _authorService.ReturnThree();   //This is merely to demonstrate that DI is working

            return _db.Authors;
        }

        // GET: api/Authors/5
        [ResponseType(typeof(Author))]
        public async Task<IHttpActionResult> GetAuthor(int id)
        {
            Author author = await _db.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        // PUT: api/Authors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAuthor(int id, Author author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != author.Id)
            {
                return BadRequest();
            }

            _db.Entry(author).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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

        // POST: api/Authors
        [ResponseType(typeof(Author))]
        public async Task<IHttpActionResult> PostAuthor(Author author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Authors.Add(author);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = author.Id }, author);
        }

        // DELETE: api/Authors/5
        [ResponseType(typeof(Author))]
        public async Task<IHttpActionResult> DeleteAuthor(int id)
        {
            Author author = await _db.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _db.Authors.Remove(author);
            await _db.SaveChangesAsync();

            return Ok(author);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AuthorExists(int id)
        {
            return _db.Authors.Count(e => e.Id == id) > 0;
        }
    }
}