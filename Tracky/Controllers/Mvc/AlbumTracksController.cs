using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tracky.Domain.EF.DataContexts;
using Tracky.Domain.EF.Music;

namespace Tracky.Controllers.Mvc
{
    public class AlbumTracksController : Controller
    {
        private readonly LibraryContext _db;

        public AlbumTracksController(LibraryContext context)
        {
            _db = context;
        }

        // GET: AlbumTracks
        public async Task<ActionResult> Index()
        {
            var albumTracks = _db.AlbumTracks.Include(a => a.Album).Include(a => a.Song).OrderBy(at => at.Album.Title).ThenBy(at => at.AlbumTrackNumber);
            return View(await albumTracks.ToListAsync());
        }

        // GET: AlbumTracks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlbumTrack albumTrack = await _db.AlbumTracks.FindAsync(id);
            if (albumTrack == null)
            {
                return HttpNotFound();
            }
            return View(albumTrack);
        }

        // GET: AlbumTracks/Create
        public ActionResult Create()
        {
            ViewBag.AlbumId = new SelectList(_db.Albums, "Id", "Title");
            ViewBag.SongId = new SelectList(_db.Songs, "Id", "Title");
            return View();
        }

        // POST: AlbumTracks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,AlbumId,AlbumTrackNumber,SongId")] AlbumTrack albumTrack)
        {
            if (ModelState.IsValid)
            {
                _db.AlbumTracks.Add(albumTrack);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AlbumId = new SelectList(_db.Albums, "Id", "Title", albumTrack.AlbumId);
            ViewBag.SongId = new SelectList(_db.Songs, "Id", "Title", albumTrack.SongId);
            return View(albumTrack);
        }

        // GET: AlbumTracks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlbumTrack albumTrack = await _db.AlbumTracks.FindAsync(id);
            if (albumTrack == null)
            {
                return HttpNotFound();
            }
            ViewBag.AlbumId = new SelectList(_db.Albums, "Id", "Title", albumTrack.AlbumId);
            ViewBag.SongId = new SelectList(_db.Songs, "Id", "Title", albumTrack.SongId);
            return View(albumTrack);
        }

        // POST: AlbumTracks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AlbumId,AlbumTrackNumber,SongId")] AlbumTrack albumTrack)
        {
            if (ModelState.IsValid)
            {
                _db.SetModified(albumTrack); // This replaces the default <_db.Entry(albumTrack).State = EntityState.Modified;> which cannot otherwise be mocked for unit testing
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AlbumId = new SelectList(_db.Albums, "Id", "Title", albumTrack.AlbumId);
            ViewBag.SongId = new SelectList(_db.Songs, "Id", "Title", albumTrack.SongId);
            return View(albumTrack);
        }

        // GET: AlbumTracks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlbumTrack albumTrack = await _db.AlbumTracks.FindAsync(id);
            if (albumTrack == null)
            {
                return HttpNotFound();
            }
            return View(albumTrack);
        }

        // POST: AlbumTracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AlbumTrack albumTrack = await _db.AlbumTracks.FindAsync(id);
            _db.AlbumTracks.Remove(albumTrack);
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
