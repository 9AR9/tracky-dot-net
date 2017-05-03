using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;

namespace Tracky.Controllers.Api
{
    public class ArtistsController : ApiController
    {
        private readonly LibraryContext _db;

        public ArtistsController(LibraryContext context)
        {
            _db = context;
        }

        // GET: api/Artists
        public IQueryable<Artist> GetArtists()
        {
            return _db.Artists.OrderBy(a => a.Name);
        }

        // GET: api/Artists/5
        [ResponseType(typeof(Artist))]
        public async Task<IHttpActionResult> GetArtist(int id)
        {
            Artist artist = await _db.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            return Ok(artist);
        }

        // PUT: api/Artists/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutArtist(int id, Artist artist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != artist.Id)
            {
                return BadRequest();
            }

            _db.Entry(artist).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(id))
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

        // POST: api/Artists
        [ResponseType(typeof(Artist))]
        public async Task<IHttpActionResult> PostArtist(Artist artist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Artists.Add(artist);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = artist.Id }, artist);
        }

        // DELETE: api/Artists/5
        [ResponseType(typeof(Artist))]
        public async Task<IHttpActionResult> DeleteArtist(int id)
        {
            Artist artist = await _db.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            _db.Artists.Remove(artist);
            await _db.SaveChangesAsync();

            return Ok(artist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ArtistExists(int id)
        {
            return _db.Artists.Count(e => e.Id == id) > 0;
        }
    }
}