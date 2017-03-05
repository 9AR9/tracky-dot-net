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
    public class PlaylistTracksController : ApiController
    {
        private readonly LibraryContext _db;

        public PlaylistTracksController(LibraryContext context)
        {
            _db = context;
        }

        // GET: api/PlaylistTracks
        public IQueryable<PlaylistTrack> GetPlaylistTracks()
        {
            return _db.PlaylistTracks.Include(pt => pt.Playlist).Include(pt => pt.Song).OrderBy(pt => pt.Playlist.Name).ThenBy(pt => pt.PlaylistTrackNumber);
        }

        // GET: api/PlaylistTracks/5
        [ResponseType(typeof(PlaylistTrack))]
        public async Task<IHttpActionResult> GetPlaylistTrack(int id)
        {
            PlaylistTrack playlistTrack = await _db.PlaylistTracks.FindAsync(id);
            if (playlistTrack == null)
            {
                return NotFound();
            }

            return Ok(playlistTrack);
        }

        // PUT: api/PlaylistTracks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPlaylistTrack(int id, PlaylistTrack playlistTrack)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != playlistTrack.Id)
            {
                return BadRequest();
            }

            _db.Entry(playlistTrack).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistTrackExists(id))
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

        // POST: api/PlaylistTracks
        [ResponseType(typeof(PlaylistTrack))]
        public async Task<IHttpActionResult> PostPlaylistTrack(PlaylistTrack playlistTrack)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.PlaylistTracks.Add(playlistTrack);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = playlistTrack.Id }, playlistTrack);
        }

        // DELETE: api/PlaylistTracks/5
        [ResponseType(typeof(PlaylistTrack))]
        public async Task<IHttpActionResult> DeletePlaylistTrack(int id)
        {
            PlaylistTrack playlistTrack = await _db.PlaylistTracks.FindAsync(id);
            if (playlistTrack == null)
            {
                return NotFound();
            }

            _db.PlaylistTracks.Remove(playlistTrack);
            await _db.SaveChangesAsync();

            return Ok(playlistTrack);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlaylistTrackExists(int id)
        {
            return _db.PlaylistTracks.Count(e => e.Id == id) > 0;
        }
    }
}