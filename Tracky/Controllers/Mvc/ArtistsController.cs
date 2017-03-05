﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tracky.Domain.EF.DataContexts;
using Tracky.Domain.EF.Music;
using Tracky.Domain.Services.EF.Music;

namespace Tracky.Controllers.Mvc
{
    public class ArtistsController : Controller
    {
        private readonly LibraryContext _db;
        private readonly IArtistService _artistService;

        public ArtistsController(LibraryContext libraryContext, IArtistService artistService)
        {
            _artistService = artistService;
            _db = libraryContext;
        }

        // GET: Artists
        public async Task<ActionResult> Index()
        {
            return View(await _db.Artists.OrderBy(a => a.Name).ToListAsync());
        }

        // GET: Artists/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = await _db.Artists.FindAsync(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // GET: Artists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                _db.Artists.Add(artist);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(artist);
        }

        // GET: Artists/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = await _db.Artists.FindAsync(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                _db.SetModified(artist); // This replaces the default <_db.Entry(artist).State = EntityState.Modified;> which cannot otherwise be mocked for unit testing
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(artist);
        }

        // GET: Artists/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = await _db.Artists.FindAsync(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Artist artist = await _db.Artists.FindAsync(id);
            _db.Artists.Remove(artist);
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




        /// <summary>
        /// This method temporarily and indifferently exposes usage of the injected ArtistService, for integration testing verification.
        /// It should be removed once service usage is appropriately woven into the other action methods of this controller.
        /// </summary>
        /// <returns>Index view</returns>
        public ActionResult Zang()
        {
            var newArtist = new Artist() { Id = _artistService.ReturnSomethingDumb(), Name = "Wang Chung" };
            var artists = new List<Artist> { newArtist };
            return View("Index", artists);
        }
    }
}
