using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.NH;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;
using Tracky.Domain.Repositories.Orm.NH.Tracky;

namespace Tracky.Controllers.Mvc
{
    public class AlbumsController : Controller
    {
        private readonly LibraryContext _db;

        public AlbumsController(LibraryContext context)
        {
            _db = context;
        }

        // GET: Albums
        public async Task<ActionResult> Index()
        {
            var albums = _db.Albums.OrderBy(a => a.Title).Include(a => a.Artist).Include(a => a.Genre);
            return View(await albums.ToListAsync());

            /*
             * TEST-ONLY BLOCK BELOW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
             * The following block uses NHibernate to get the Albums.
             *
             * This represents the interaction flow to be used by services
             * that need to interact with the database. The Unit of Work
             * object gets wrapped in a using statement, which ensure its
             * Dispose method will be called when finished, guaranteeing
             * the session that it uses will be closed and disposed of.
             *
             * This simple approach should NOT be used in live production code,
             * but can be used in testing to observe the basic NHibernate
             * database interaction process.
             */
            //var uow = new UnitOfWork(false);
            //using (uow)
            //{
            //    uow.BeginTransaction();
            //    var repo = new AlbumRepository(new UnitOfWork(false));
            //    IList<Album> albums = repo.GetAll().OrderBy(a => a.Title).ToList();
            //    uow.FinishTransaction(true);
            //    return View(albums);
            //}
        }

        // GET: Albums/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = await _db.Albums.FindAsync(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // GET: Albums/Create
        public ActionResult Create()
        {
            ViewBag.ArtistId = new SelectList(_db.Artists, "Id", "Name");
            ViewBag.GenreId = new SelectList(_db.Genres, "Id", "Name");
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,ArtistId,GenreId,Year,Label")] Album album)
        {
            if (ModelState.IsValid)
            {
                _db.Albums.Add(album);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ArtistId = new SelectList(_db.Artists, "Id", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(_db.Genres, "Id", "Name");
            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = await _db.Albums.FindAsync(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtistId = new SelectList(_db.Artists, "Id", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(_db.Genres, "Id", "Name");
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,ArtistId,GenreId,Year,Label")] Album album)
        {
            if (ModelState.IsValid)
            {
                _db.SetModified(album); // This replaces the default <_db.Entry(album).State = EntityState.Modified;> which cannot otherwise be mocked for unit testing
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ArtistId = new SelectList(_db.Artists, "Id", "Name", album.ArtistId);
            ViewBag.GenreId = new SelectList(_db.Genres, "Id", "Name");
            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = await _db.Albums.FindAsync(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Album album = await _db.Albums.FindAsync(id);
            var tracks = _db.AlbumTracks.Where(at => at.AlbumId == id);
            foreach (AlbumTrack track in tracks)
            {
                _db.AlbumTracks.Remove(track);
            }
            _db.Albums.Remove(album);
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
