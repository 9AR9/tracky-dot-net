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
    public class PlaylistsController : ApiController
    {
        private readonly LibraryContext _db;

        public PlaylistsController(LibraryContext context)
        {
            _db = context;
        }

        // GET: api/Playlists
        public IQueryable<Playlist> GetPlaylists()
        {
            return _db.Playlists.OrderBy(p => p.Name);
        }

        // GET: api/Playlists/5
        [ResponseType(typeof(Playlist))]
        public async Task<IHttpActionResult> GetPlaylist(int id)
        {
            Playlist playlist = await _db.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            return Ok(playlist);
        }

        // PUT: api/Playlists/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPlaylist(int id, Playlist playlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != playlist.Id)
            {
                return BadRequest();
            }

            _db.Entry(playlist).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(id))
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

        // POST: api/Playlists
        [ResponseType(typeof(Playlist))]
        public async Task<IHttpActionResult> PostPlaylist(Playlist playlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Playlists.Add(playlist);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = playlist.Id }, playlist);
        }

        // DELETE: api/Playlists/5
        [ResponseType(typeof(Playlist))]
        public async Task<IHttpActionResult> DeletePlaylist(int id)
        {
            Playlist playlist = await _db.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            _db.Playlists.Remove(playlist);
            await _db.SaveChangesAsync();

            return Ok(playlist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlaylistExists(int id)
        {
            return _db.Playlists.Count(e => e.Id == id) > 0;
        }
    }
}