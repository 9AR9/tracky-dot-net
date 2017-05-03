using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;

namespace Tracky.Controllers.Mvc
{
    public class SongsController : Controller
    {
        private readonly LibraryContext _db;

        public SongsController(LibraryContext context)
        {
            _db = context;
        }

        // GET: Songs
        public async Task<ActionResult> Index()
        {
            var songs = _db.Songs.Include(s => s.Artist).Include(s => s.Genre).OrderBy(s => s.Artist.Name).ThenBy(s => s.Title);
            return View(await songs.ToListAsync());
        }

        // GET: Songs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = await _db.Songs.FindAsync(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // GET: Songs/Create
        public ActionResult Create()
        {
            ViewBag.ArtistId = new SelectList(_db.Artists, "Id", "Name");
            ViewBag.GenreId = new SelectList(_db.Genres, "Id", "Name");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,ArtistId,GenreId")] Song song)
        {
            if (ModelState.IsValid)
            {
                _db.Songs.Add(song);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ArtistId = new SelectList(_db.Artists, "Id", "Name", song.ArtistId);
            ViewBag.GenreId = new SelectList(_db.Genres, "Id", "Name", song.GenreId);
            return View(song);
        }

        // GET: Songs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = await _db.Songs.FindAsync(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtistId = new SelectList(_db.Artists, "Id", "Name", song.ArtistId);
            ViewBag.GenreId = new SelectList(_db.Genres, "Id", "Name", song.GenreId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,ArtistId,GenreId")] Song song)
        {
            if (ModelState.IsValid)
            {
                _db.SetModified(song); // This replaces the default <_db.Entry(song).State = EntityState.Modified;> which cannot otherwise be mocked for unit testing
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ArtistId = new SelectList(_db.Artists, "Id", "Name", song.ArtistId);
            ViewBag.GenreId = new SelectList(_db.Genres, "Id", "Name", song.GenreId);
            return View(song);
        }

        // GET: Songs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = await _db.Songs.FindAsync(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Song song = await _db.Songs.FindAsync(id);
            _db.Songs.Remove(song);
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
