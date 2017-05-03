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
    public class SongsController : ApiController
    {
        private readonly LibraryContext _db;

        public SongsController(LibraryContext context)
        {
            _db = context;
        }

        // GET: api/Songs
        public IQueryable<Song> GetSongs()
        {
            return _db.Songs.Include(s => s.Artist).Include(s => s.Genre).OrderBy(s => s.Artist.Name).ThenBy(s => s.Title);
        }

        // GET: api/Songs/5
        [ResponseType(typeof(Song))]
        public async Task<IHttpActionResult> GetSong(int id)
        {
            Song song = await _db.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            return Ok(song);
        }

        // PUT: api/Songs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSong(int id, Song song)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != song.Id)
            {
                return BadRequest();
            }

            _db.Entry(song).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
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

        // POST: api/Songs
        [ResponseType(typeof(Song))]
        public async Task<IHttpActionResult> PostSong(Song song)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Songs.Add(song);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = song.Id }, song);
        }

        // DELETE: api/Songs/5
        [ResponseType(typeof(Song))]
        public async Task<IHttpActionResult> DeleteSong(int id)
        {
            Song song = await _db.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            _db.Songs.Remove(song);
            await _db.SaveChangesAsync();

            return Ok(song);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SongExists(int id)
        {
            return _db.Songs.Count(e => e.Id == id) > 0;
        }
    }
}