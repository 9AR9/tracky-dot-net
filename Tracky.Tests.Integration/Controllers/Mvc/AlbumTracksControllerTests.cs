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
    public class AlbumTracksControllerTests
    {
        private AlbumTracksController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new EfDatabaseInitializer());           // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                             // Create data context
            context.Database.Initialize(true);                              // Initialize database on context

            var container = IoC.Initialize();                               // Initialize the IoC container
            _controller = container.GetInstance<AlbumTracksController>();   // Ask container for a controller, built with all dependencies
        }

        [Test] public async Task IndexGet_ShouldGetAllAlbumTracksOrderedByAlbumTitleThenAlbumTrackNumber()
        {
            // Arrange
            const int expectedTotalAlbumTracks = 19;
            const string expectedAlbumTitleFirstAlphabetically = "3 Feet High And Rising";
            const string expectedAlbumTitleNextToLastAlphabetically = "Shape Shift With Me";
            const string expectedAlbumTitleLastAlphabetically = "Stranger Than Fiction";
            const int expectedAlbumTrackNumberFirst = 2;
            const int expectedAlbumTrackNumberSecond = 7;
            const int expectedAlbumTrackNumberNextToLast = 8;
            const int expectedAlbumTrackNumberLast = 10;

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<AlbumTrack>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalAlbumTracks, model.Count);
            Assert.AreEqual(expectedAlbumTrackNumberFirst, model.First().AlbumTrackNumber);
            Assert.AreEqual(expectedAlbumTitleFirstAlphabetically, model.First().Album.Title);
            Assert.AreEqual(expectedAlbumTrackNumberSecond, model[1].AlbumTrackNumber);
            Assert.AreEqual(expectedAlbumTitleFirstAlphabetically, model[1].Album.Title);
            Assert.AreEqual(expectedAlbumTrackNumberNextToLast, model[model.Count - 2].AlbumTrackNumber);
            Assert.AreEqual(expectedAlbumTitleNextToLastAlphabetically, model[model.Count - 2].Album.Title);
            Assert.AreEqual(expectedAlbumTrackNumberLast, model.Last().AlbumTrackNumber);
            Assert.AreEqual(expectedAlbumTitleLastAlphabetically, model.Last().Album.Title);
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingAlbumTrackId()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 1, AlbumId = 1, AlbumTrackNumber = 2, SongId = 1 };

            // Act
            var result = await _controller.Details(existingAlbumTrack.Id);
            var viewResult = (ViewResult)result;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbumTrack.Id, model.Id);
            Assert.AreEqual(existingAlbumTrack.AlbumId, model.AlbumId);
            Assert.AreEqual(existingAlbumTrack.AlbumTrackNumber, model.AlbumTrackNumber);
            Assert.AreEqual(existingAlbumTrack.SongId, model.SongId);
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
        public async Task DetailsGet_ShouldReturnNotFoundResultForNonExistentAlbumTrack()
        {
            // Arrange
            var nonExistentAlbumTrack = new AlbumTrack() { Id = 99, AlbumId = 1, AlbumTrackNumber = 77, SongId = 1 };

            // Act
            var result = await _controller.Details(nonExistentAlbumTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewAlbumTrack()
        {
            // Arrange
            const int expectedTotalAlbumTracksAfterCreate = 20;
            const int expectedIdForNewAlbumTrack = 20;
            var newAlbumTrack = new AlbumTrack() { AlbumId = 1, AlbumTrackNumber = 77, SongId = 1 };

            // Act
            var result1 = await _controller.Create(newAlbumTrack);
            var redirectToRouteResult = (RedirectToRouteResult) result1;
            var result2 = await _controller.Index();
            var viewResult = (ViewResult)result2;
            var model = (List<AlbumTrack>)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalAlbumTracksAfterCreate, model.Count);
            Assert.AreEqual(expectedIdForNewAlbumTrack, model.Find(at => at.AlbumId == newAlbumTrack.AlbumId && at.AlbumTrackNumber == newAlbumTrack.AlbumTrackNumber).Id);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSameAlbumTrackWhenModelStateIsNotValid()
        {
            // Arrange
            var newAlbumTrack = new AlbumTrack() { AlbumId = 1, AlbumTrackNumber = 77, SongId = 1 };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newAlbumTrack);
            var viewResult = (ViewResult)result;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newAlbumTrack, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingAlbumTrack()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 1, AlbumId = 1, AlbumTrackNumber = 2, SongId = 1 };

            // Act
            var result = await _controller.Edit(existingAlbumTrack.Id);
            var viewResult = (ViewResult)result;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(existingAlbumTrack.Id, model.Id);
            Assert.AreEqual(existingAlbumTrack.AlbumId, model.AlbumId);
            Assert.AreEqual(existingAlbumTrack.AlbumTrackNumber, model.AlbumTrackNumber);
            Assert.AreEqual(existingAlbumTrack.SongId, model.SongId);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentAlbumTrack()
        {
            // Arrange
            var nonExistentAlbumTrack = new AlbumTrack() { Id = 99, AlbumId = 1, AlbumTrackNumber = 77, SongId = 1 };

            // Act
            var result = await _controller.Edit(nonExistentAlbumTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedAlbumTrack()
        {
            // Arrange
            var existingAlbumTrackWithModifiedSongId = new AlbumTrack() { Id = 1, AlbumId = 1, AlbumTrackNumber = 2, SongId = 11 };

            // Act
            var result1 = await _controller.Edit(existingAlbumTrackWithModifiedSongId);
            var redirectToRouteResult = (RedirectToRouteResult)result1;
            var result2 = await _controller.Details(existingAlbumTrackWithModifiedSongId.Id);
            var viewResult = (ViewResult)result2;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbumTrackWithModifiedSongId.Id, model.Id);
            Assert.AreEqual(existingAlbumTrackWithModifiedSongId.SongId, model.SongId);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSameAlbumTrackWhenModelStateIsNotValid()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 1, AlbumId = 1, AlbumTrackNumber = 2, SongId = 1 };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingAlbumTrack);
            var viewResult = (ViewResult)result;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbumTrack, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingAlbumTrack()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 1, AlbumId = 1, AlbumTrackNumber = 2, SongId = 1 };

            // Act
            var result = await _controller.Delete(existingAlbumTrack.Id);
            var viewResult = (ViewResult)result;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(existingAlbumTrack.Id, model.Id);
            Assert.AreEqual(existingAlbumTrack.AlbumId, model.AlbumId);
            Assert.AreEqual(existingAlbumTrack.AlbumTrackNumber, model.AlbumTrackNumber);
            Assert.AreEqual(existingAlbumTrack.SongId, model.SongId);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentAlbumTrack()
        {
            // Arrange
            var nonExistentAlbumTrack = new AlbumTrack() { Id = 99, AlbumId = 1, AlbumTrackNumber = 77, SongId = 1 };

            // Act
            var result = await _controller.Delete(nonExistentAlbumTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingAlbumTrack()
        {
            // Arrange
            const int expectedAlbumTrackCountAfterDeletion = 18;
            const int existingAlbumTrack = 10;

            // Act
            var result = await _controller.DeleteConfirmed(existingAlbumTrack);
            var redirectToRouteResult = (RedirectToRouteResult)result;
            var result2 = await _controller.Details(existingAlbumTrack);
            var result3 = await _controller.Index();
            var viewResult = (ViewResult)result3;
            var model = (List<AlbumTrack>)viewResult.Model;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.IsInstanceOf<HttpNotFoundResult>(result2);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedAlbumTrackCountAfterDeletion, model.Count);
        }
    }
}