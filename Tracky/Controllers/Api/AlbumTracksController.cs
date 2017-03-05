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
    public class AlbumTracksController : ApiController
    {
        private readonly LibraryContext _db;

        public AlbumTracksController(LibraryContext context)
        {
            _db = context;
        }

        // GET: api/AlbumTracks
        public IQueryable<AlbumTrack> GetAlbumTracks()
        {
            return _db.AlbumTracks.OrderBy(at => at.Album.Title).ThenBy(at => at.AlbumTrackNumber);
        }

        // GET: api/AlbumTracks/5
        [ResponseType(typeof(AlbumTrack))]
        public async Task<IHttpActionResult> GetAlbumTrack(int id)
        {
            AlbumTrack albumTrack = await _db.AlbumTracks.FindAsync(id);
            if (albumTrack == null)
            {
                return NotFound();
            }

            return Ok(albumTrack);
        }

        // PUT: api/AlbumTracks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAlbumTrack(int id, AlbumTrack albumTrack)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != albumTrack.Id)
            {
                return BadRequest();
            }

            _db.Entry(albumTrack).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumTrackExists(id))
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

        // POST: api/AlbumTracks
        [ResponseType(typeof(AlbumTrack))]
        public async Task<IHttpActionResult> PostAlbumTrack(AlbumTrack albumTrack)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.AlbumTracks.Add(albumTrack);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = albumTrack.Id }, albumTrack);
        }

        // DELETE: api/AlbumTracks/5
        [ResponseType(typeof(AlbumTrack))]
        public async Task<IHttpActionResult> DeleteAlbumTrack(int id)
        {
            AlbumTrack albumTrack = await _db.AlbumTracks.FindAsync(id);
            if (albumTrack == null)
            {
                return NotFound();
            }

            _db.AlbumTracks.Remove(albumTrack);
            await _db.SaveChangesAsync();

            return Ok(albumTrack);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AlbumTrackExists(int id)
        {
            return _db.AlbumTracks.Count(e => e.Id == id) > 0;
        }
    }
}