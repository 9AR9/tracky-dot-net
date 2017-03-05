using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tracky.Domain.EF.DataContexts;
using Tracky.Domain.EF.Music;

namespace Tracky.Controllers.Mvc
{
    public class PlaylistsController : Controller
    {
        private readonly LibraryContext _db;

        public PlaylistsController(LibraryContext context)
        {
            _db = context;
        }

        // GET: Playlists
        public async Task<ActionResult> Index()
        {
            return View(await _db.Playlists.OrderBy(p => p.Name).ToListAsync());
        }

        // GET: Playlists/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlist playlist = await _db.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return HttpNotFound();
            }
            return View(playlist);
        }

        // GET: Playlists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Playlists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] Playlist playlist)
        {
            if (ModelState.IsValid)
            {
                _db.Playlists.Add(playlist);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(playlist);
        }

        // GET: Playlists/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlist playlist = await _db.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return HttpNotFound();
            }
            return View(playlist);
        }

        // POST: Playlists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] Playlist playlist)
        {
            if (ModelState.IsValid)
            {
                _db.SetModified(playlist); // This replaces the default <_db.Entry(playlist).State = EntityState.Modified;> which cannot otherwise be mocked for unit testing
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(playlist);
        }

        // GET: Playlists/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Playlist playlist = await _db.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return HttpNotFound();
            }
            return View(playlist);
        }

        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Playlist playlist = await _db.Playlists.FindAsync(id);
            _db.Playlists.Remove(playlist);
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
