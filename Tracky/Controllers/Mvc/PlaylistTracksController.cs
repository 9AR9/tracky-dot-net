using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;

namespace Tracky.Controllers.Mvc
{
    public class PlaylistTracksController : Controller
    {
        private readonly LibraryContext _db;

        public PlaylistTracksController(LibraryContext context)
        {
            _db = context;
        }

        // GET: PlaylistTracks
        public async Task<ActionResult> Index()
        {
            var playlistTracks = _db.PlaylistTracks.Include(pt => pt.Playlist).Include(pt => pt.Song).OrderBy(pt => pt.Playlist.Name).ThenBy(pt => pt.PlaylistTrackNumber);
            return View(await playlistTracks.ToListAsync());
        }

        // GET: PlaylistTracks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlaylistTrack playlistTrack = await _db.PlaylistTracks.FindAsync(id);
            if (playlistTrack == null)
            {
                return HttpNotFound();
            }
            return View(playlistTrack);
        }

        // GET: PlaylistTracks/Create
        public ActionResult Create()
        {
            ViewBag.PlaylistId = new SelectList(_db.Playlists, "Id", "Name");
            ViewBag.SongId = new SelectList(_db.Songs, "Id", "Title");
            return View();
        }

        // POST: PlaylistTracks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,PlaylistId,PlaylistTrackNumber,SongId")] PlaylistTrack playlistTrack)
        {
            if (ModelState.IsValid)
            {
                _db.PlaylistTracks.Add(playlistTrack);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PlaylistId = new SelectList(_db.Playlists, "Id", "Name", playlistTrack.PlaylistId);
            ViewBag.SongId = new SelectList(_db.Songs, "Id", "Title", playlistTrack.SongId);
            return View(playlistTrack);
        }

        // GET: PlaylistTracks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlaylistTrack playlistTrack = await _db.PlaylistTracks.FindAsync(id);
            if (playlistTrack == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlaylistId = new SelectList(_db.Playlists, "Id", "Name", playlistTrack.PlaylistId);
            ViewBag.SongId = new SelectList(_db.Songs, "Id", "Title", playlistTrack.SongId);
            return View(playlistTrack);
        }

        // POST: PlaylistTracks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,PlaylistId,PlaylistTrackNumber,SongId")] PlaylistTrack playlistTrack)
        {
            if (ModelState.IsValid)
            {
                _db.SetModified(playlistTrack); // This replaces the default <_db.Entry(playlistTrack).State = EntityState.Modified;> which cannot otherwise be mocked for unit testing
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PlaylistId = new SelectList(_db.Playlists, "Id", "Name", playlistTrack.PlaylistId);
            ViewBag.SongId = new SelectList(_db.Songs, "Id", "Title", playlistTrack.SongId);
            return View(playlistTrack);
        }

        // GET: PlaylistTracks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlaylistTrack playlistTrack = await _db.PlaylistTracks.FindAsync(id);
            if (playlistTrack == null)
            {
                return HttpNotFound();
            }
            return View(playlistTrack);
        }

        // POST: PlaylistTracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PlaylistTrack playlistTrack = await _db.PlaylistTracks.FindAsync(id);
            _db.PlaylistTracks.Remove(playlistTrack);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
