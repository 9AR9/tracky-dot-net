using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Tracky.Domain.EF.DataContexts;
using Tracky.Domain.EF.Music;

namespace Tracky.Controllers.Api
{
    public class GenresController : ApiController
    {
        private readonly LibraryContext _db;

        public GenresController(LibraryContext context)
        {
            _db = context;
        }

        // GET: api/Genres
        public IQueryable<Genre> GetGenres()
        {
            return _db.Genres.OrderBy(g => g.Name);
        }

        // GET: api/Genres/5
        [ResponseType(typeof(Genre))]
        public async Task<IHttpActionResult> GetGenre(int id)
        {
            Genre genre = await _db.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        // PUT: api/Genres/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGenre(int id, Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != genre.Id)
            {
                return BadRequest();
            }

            _db.Entry(genre).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
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

        // POST: api/Genres
        [ResponseType(typeof(Genre))]
        public async Task<IHttpActionResult> PostGenre(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Genres.Add(genre);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = genre.Id }, genre);
        }

        // DELETE: api/Genres/5
        [ResponseType(typeof(Genre))]
        public async Task<IHttpActionResult> DeleteGenre(int id)
        {
            Genre genre = await _db.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            _db.Genres.Remove(genre);
            await _db.SaveChangesAsync();

            return Ok(genre);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GenreExists(int id)
        {
            return _db.Genres.Count(e => e.Id == id) > 0;
        }
    }
}