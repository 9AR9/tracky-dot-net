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
    public class AlbumsController : ApiController
    {
        private readonly LibraryContext _db;

        public AlbumsController(LibraryContext context)
        {
            _db = context;
        }

        // GET: api/Albums
        public IQueryable<Album> GetAlbums()
        {
            return _db.Albums.OrderBy(a => a.Title);
        }

        // GET: api/Albums/5
        [ResponseType(typeof(Album))]
        public async Task<IHttpActionResult> GetAlbum(int id)
        {
            Album album = await _db.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            return Ok(album);
        }

        // PUT: api/Albums/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAlbum(int id, Album album)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != album.Id)
            {
                return BadRequest();
            }

            _db.Entry(album).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(id))
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

        // POST: api/Albums
        [ResponseType(typeof(Album))]
        public async Task<IHttpActionResult> PostAlbum(Album album)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Albums.Add(album);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = album.Id }, album);
        }

        // DELETE: api/Albums/5
        [ResponseType(typeof(Album))]
        public async Task<IHttpActionResult> DeleteAlbum(int id)
        {
            Album album = await _db.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            var tracks = _db.AlbumTracks.Where(at => at.AlbumId == id);
            foreach (AlbumTrack track in tracks)
            {
                _db.AlbumTracks.Remove(track);
            }
            _db.Albums.Remove(album);
            await _db.SaveChangesAsync();

            return Ok(album);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AlbumExists(int id)
        {
            return _db.Albums.Count(e => e.Id == id) > 0;
        }
    }
}