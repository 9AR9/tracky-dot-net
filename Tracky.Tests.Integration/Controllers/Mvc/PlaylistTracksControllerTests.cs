using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using Tracky.Controllers.Mvc;
using Tracky.DependencyResolution;
using Tracky.Domain.EF.DataContexts;
using Tracky.Domain.EF.Music;

namespace Tracky.Tests.Integration.Controllers.Mvc
{
    [TestFixture]
    public class PlaylistTracksControllerTests
    {
        private PlaylistTracksController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new DatabaseInitializer());                 // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                                 // Create data context
            context.Database.Initialize(true);                                  // Initialize database on context

            var container = IoC.Initialize();                                   // Initialize the IoC container
            _controller = container.GetInstance<PlaylistTracksController>();    // Ask container for a controller, built with all dependencies
        }

        [Test] public async Task IndexGet_ShouldGetAllPlaylistTracksOrderedByPlaylistNameThenPlaylistTrackNumber()
        {
            // Arrange
            const int expectedTotalPlaylistTracks = 17;
            const string expectedPlaylistNameFirstAlphabetically = "Happy Happy Tracks";
            const string expectedAlbumTitleLastAlphabetically = "Mellow Pimpin' Tracks";
            const int expectedPlaylistTrackNumberFirst = 1;
            const int expectedPlaylistTrackNumberSecond = 2;
            const int expectedPlaylistTrackNumberNextToLast = 1;
            const int expectedPlaylistTrackNumberLast = 2;

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<PlaylistTrack>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalPlaylistTracks, model.Count);
            Assert.AreEqual(expectedPlaylistTrackNumberFirst, model.First().PlaylistTrackNumber);
            Assert.AreEqual(expectedPlaylistNameFirstAlphabetically, model.First().Playlist.Name);
            Assert.AreEqual(expectedPlaylistTrackNumberSecond, model[1].PlaylistTrackNumber);
            Assert.AreEqual(expectedPlaylistNameFirstAlphabetically, model[1].Playlist.Name);
            Assert.AreEqual(expectedPlaylistTrackNumberNextToLast, model[model.Count - 2].PlaylistTrackNumber);
            Assert.AreEqual(expectedAlbumTitleLastAlphabetically, model[model.Count - 2].Playlist.Name);
            Assert.AreEqual(expectedPlaylistTrackNumberLast, model.Last().PlaylistTrackNumber);
            Assert.AreEqual(expectedAlbumTitleLastAlphabetically, model.Last().Playlist.Name);
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingPlaylistTrackId()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 1 };

            // Act
            var result = await _controller.Details(existingPlaylistTrack.Id);
            var viewResult = (ViewResult)result;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylistTrack.Id, model.Id);
            Assert.AreEqual(existingPlaylistTrack.PlaylistId, model.PlaylistId);
            Assert.AreEqual(existingPlaylistTrack.PlaylistTrackNumber, model.PlaylistTrackNumber);
            Assert.AreEqual(existingPlaylistTrack.SongId, model.SongId);
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
        public async Task DetailsGet_ShouldReturnNotFoundResultForNonExistentPlaylistTrack()
        {
            // Arrange
            var nonExistentPlaylistTrack = new PlaylistTrack() { Id = 99, PlaylistId = 1, PlaylistTrackNumber = 77, SongId = 1 };

            // Act
            var result = await _controller.Details(nonExistentPlaylistTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewPlaylistTrack()
        {
            // Arrange
            const int expectedTotalPlaylistTracksAfterCreate = 18;
            const int expectedIdForNewPlaylistTrack = 18;
            var newPlaylistTrack = new PlaylistTrack() { PlaylistId = 1, PlaylistTrackNumber = 77, SongId = 1 };

            // Act
            var result1 = await _controller.Create(newPlaylistTrack);
            var redirectToRouteResult = (RedirectToRouteResult) result1;
            var result2 = await _controller.Index();
            var viewResult = (ViewResult)result2;
            var model = (List<PlaylistTrack>)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalPlaylistTracksAfterCreate, model.Count);
            Assert.AreEqual(expectedIdForNewPlaylistTrack, model.Find(pt => pt.PlaylistId == newPlaylistTrack.PlaylistId && pt.PlaylistTrackNumber == newPlaylistTrack.PlaylistTrackNumber).Id);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSamePlaylistTrackWhenModelStateIsNotValid()
        {
            // Arrange
            var newPlaylistTrack = new PlaylistTrack() { PlaylistId = 1, PlaylistTrackNumber = 77, SongId = 1 };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newPlaylistTrack);
            var viewResult = (ViewResult)result;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newPlaylistTrack, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingPlaylistTrack()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 1 };

            // Act
            var result = await _controller.Edit(existingPlaylistTrack.Id);
            var viewResult = (ViewResult)result;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(existingPlaylistTrack.Id, model.Id);
            Assert.AreEqual(existingPlaylistTrack.PlaylistId, model.PlaylistId);
            Assert.AreEqual(existingPlaylistTrack.PlaylistTrackNumber, model.PlaylistTrackNumber);
            Assert.AreEqual(existingPlaylistTrack.SongId, model.SongId);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentPlaylistTrack()
        {
            // Arrange
            var nonExistentPlaylistTrack = new PlaylistTrack() { Id = 99, PlaylistId = 1, PlaylistTrackNumber = 77, SongId = 1 };

            // Act
            var result = await _controller.Edit(nonExistentPlaylistTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedPlaylistTrack()
        {
            // Arrange
            var existingPlaylistTrackWithModifiedSongId = new PlaylistTrack() { Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 11 };

            // Act
            var result1 = await _controller.Edit(existingPlaylistTrackWithModifiedSongId);
            var redirectToRouteResult = (RedirectToRouteResult)result1;
            var result2 = await _controller.Details(existingPlaylistTrackWithModifiedSongId.Id);
            var viewResult = (ViewResult)result2;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylistTrackWithModifiedSongId.Id, model.Id);
            Assert.AreEqual(existingPlaylistTrackWithModifiedSongId.SongId, model.SongId);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSamePlaylistTrackWhenModelStateIsNotValid()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 1 };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingPlaylistTrack);
            var viewResult = (ViewResult)result;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylistTrack, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingPlaylistTrack()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 1 };

            // Act
            var result = await _controller.Delete(existingPlaylistTrack.Id);
            var viewResult = (ViewResult)result;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(existingPlaylistTrack.Id, model.Id);
            Assert.AreEqual(existingPlaylistTrack.PlaylistId, model.PlaylistId);
            Assert.AreEqual(existingPlaylistTrack.PlaylistTrackNumber, model.PlaylistTrackNumber);
            Assert.AreEqual(existingPlaylistTrack.SongId, model.SongId);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentPlaylistTrack()
        {
            // Arrange
            var nonExistentPlaylistTrack = new PlaylistTrack() { Id = 99, PlaylistId = 1, PlaylistTrackNumber = 77, SongId = 1 };

            // Act
            var result = await _controller.Delete(nonExistentPlaylistTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingPlaylistTrack()
        {
            // Arrange
            const int expectedPlaylistTrackCountAfterDeletion = 16;
            const int existingPlaylistTrack = 10;

            // Act
            var result = await _controller.DeleteConfirmed(existingPlaylistTrack);
            var redirectToRouteResult = (RedirectToRouteResult)result;
            var result2 = await _controller.Details(existingPlaylistTrack);
            var result3 = await _controller.Index();
            var viewResult = (ViewResult)result3;
            var model = (List<PlaylistTrack>)viewResult.Model;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.IsInstanceOf<HttpNotFoundResult>(result2);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedPlaylistTrackCountAfterDeletion, model.Count);
        }
    }
}