using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using Tracky.Controllers.Mvc;
using Tracky.DependencyResolution;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;

namespace Tracky.Tests.Integration.Controllers.Mvc
{
    [TestFixture]
    public class PlaylistsControllerTests
    {
        private PlaylistsController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new EFDatabaseInitializer());           // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                             // Create data context
            context.Database.Initialize(true);                              // Initialize database on context

            var container = IoC.Initialize();                               // Initialize the IoC container
            _controller = container.GetInstance<PlaylistsController>();     // Ask container for a controller, built with all dependencies
        }

        [Test]
        public async Task IndexGet_ShouldGetAllPlaylistsOrderedByName()
        {
            // Arrange
            const int expectedTotalPlaylists = 4;
            const string expectedPlaylistNameFirstAlphabetically = "Happy Happy Tracks";
            const string expectedPlaylistNameSecondAlphabetically = "Luncheonette Soul Jazz";
            const string expectedPlaylistNameNextToLastAlphabetically = "Mellow Pimpin' Tracks";
            const string expectedPlaylistNameLastAlphabetically = "Zelda Jams";

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<Playlist>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalPlaylists, model.Count);
            Assert.AreEqual(expectedPlaylistNameFirstAlphabetically, model.First().Name);
            Assert.AreEqual(expectedPlaylistNameSecondAlphabetically, model[1].Name);
            Assert.AreEqual(expectedPlaylistNameNextToLastAlphabetically, model[model.Count - 2].Name);
            Assert.AreEqual(expectedPlaylistNameLastAlphabetically, model.Last().Name);
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingPlaylistId()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 2, Name = "Mellow Pimpin' Tracks" };

            // Act
            var result = await _controller.Details(existingPlaylist.Id);
            var viewResult = (ViewResult)result;
            var model = (Playlist)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylist.Id, model.Id);
            Assert.AreEqual(existingPlaylist.Name, model.Name);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnNotFoundResultForNonExistentPlaylist()
        {
            // Arrange
            var nonExistentPlaylist = new Playlist() { Id = 99, Name = "Fake Tracks. Sad!" };

            // Act
            var result = await _controller.Details(nonExistentPlaylist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewPlaylist()
        {
            // Arrange
            const int expectedTotalPlaylistsAfterCreate = 5;
            const int expectedIdForNewPlaylist = 5;
            var newPlaylist = new Playlist() { Name = "ZZ Top's Worst Songs" };

            // Act
            var result1 = await _controller.Create(newPlaylist);
            var redirectToRouteResult = (RedirectToRouteResult) result1;
            var result2 = await _controller.Index();
            var viewResult = (ViewResult)result2;
            var model = (List<Playlist>)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalPlaylistsAfterCreate, model.Count);
            Assert.AreEqual(expectedIdForNewPlaylist, model.Last().Id);
            Assert.AreEqual(newPlaylist.Name, model.Last().Name);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSamePlaylistWhenModelStateIsNotValid()
        {
            // Arrange
            var newPlaylist = new Playlist() { Name = "ZZ Top's Worst Songs" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newPlaylist);
            var viewResult = (ViewResult)result;
            var model = (Playlist)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newPlaylist, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingPlaylist()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 2, Name = "Mellow Pimpin' Tracks" };

            // Act
            var result = await _controller.Edit(existingPlaylist.Id);
            var viewResult = (ViewResult)result;
            var model = (Playlist)viewResult.Model;

            // Assert
            Assert.AreEqual(existingPlaylist.Id, model.Id);
            Assert.AreEqual(existingPlaylist.Name, model.Name);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentPlaylist()
        {
            // Arrange
            var nonExistentPlaylist = new Playlist() { Id = 99, Name = "Fake Tracks. Sad!" };

            // Act
            var result = await _controller.Edit(nonExistentPlaylist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedPlaylist()
        {
            // Arrange
            var existingPlaylistWithModifiedName = new Playlist() { Id = 2, Name = "Mellow Huggin' Tracks" };

            // Act
            var result1 = await _controller.Edit(existingPlaylistWithModifiedName);
            var redirectToRouteResult = (RedirectToRouteResult)result1;
            var result2 = await _controller.Details(existingPlaylistWithModifiedName.Id);
            var viewResult = (ViewResult)result2;
            var model = (Playlist)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylistWithModifiedName.Id, model.Id);
            Assert.AreEqual(existingPlaylistWithModifiedName.Name, model.Name);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSamePlaylistWhenModelStateIsNotValid()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 2, Name = "Mellow Pimpin' Tracks" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingPlaylist);
            var viewResult = (ViewResult)result;
            var model = (Playlist)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylist, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingPlaylist()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 2, Name = "Mellow Pimpin' Tracks" };

            // Act
            var result = await _controller.Delete(existingPlaylist.Id);
            var viewResult = (ViewResult)result;
            var model = (Playlist)viewResult.Model;

            // Assert
            Assert.AreEqual(existingPlaylist.Id, model.Id);
            Assert.AreEqual(existingPlaylist.Name, model.Name);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentPlaylist()
        {
            // Arrange
            var nonExistentPlaylist = new Playlist() { Id = 99, Name = "Fake Tracks. Sad!" };

            // Act
            var result = await _controller.Delete(nonExistentPlaylist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingPlaylist()
        {
            // Arrange
            const int expectedPlaylistCountAfterDeletion = 3;
            const int existingPlaylistIdWithNoPlaylistTracks = 1;

            // Act
            var result = await _controller.DeleteConfirmed(existingPlaylistIdWithNoPlaylistTracks);
            var redirectToRouteResult = (RedirectToRouteResult)result;
            var result2 = await _controller.Details(existingPlaylistIdWithNoPlaylistTracks);
            var result3 = await _controller.Index();
            var viewResult = (ViewResult)result3;
            var model = (List<Playlist>)viewResult.Model;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.IsInstanceOf<HttpNotFoundResult>(result2);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedPlaylistCountAfterDeletion, model.Count);
        }
    }
}