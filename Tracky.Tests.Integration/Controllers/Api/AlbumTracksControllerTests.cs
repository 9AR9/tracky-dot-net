using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;
using NUnit.Framework;
using Tracky.Controllers.Api;
using Tracky.DependencyResolution;
using Tracky.Domain.EF.DataContexts;
using Tracky.Domain.EF.Music;

namespace Tracky.Tests.Integration.Controllers.Api
{
    class AlbumTracksControllerTests
    {
        private AlbumTracksController _controller;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new DatabaseInitializer());             // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                             // Create data context
            context.Database.Initialize(true);                              // Initialize database on context

            var container = IoC.Initialize();                               // Initialize the IoC container
            _controller = container.GetInstance<AlbumTracksController>();   // Ask container for a controller, built with all dependencies
        }

        [Test]
        public void GetAlbumTracks_ShouldGetAllAlbumTracksOrderedByAlbumTitleThenAlbumTrackNumber()
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
            var albumTracks = _controller.GetAlbumTracks().ToList();

            // Assert
            Assert.AreEqual(expectedTotalAlbumTracks, albumTracks.Count);
            Assert.AreEqual(expectedAlbumTrackNumberFirst, albumTracks.First().AlbumTrackNumber);
            Assert.AreEqual(expectedAlbumTitleFirstAlphabetically, albumTracks.First().Album.Title);
            Assert.AreEqual(expectedAlbumTrackNumberSecond, albumTracks[1].AlbumTrackNumber);
            Assert.AreEqual(expectedAlbumTitleFirstAlphabetically, albumTracks[1].Album.Title);
            Assert.AreEqual(expectedAlbumTrackNumberNextToLast, albumTracks[albumTracks.Count - 2].AlbumTrackNumber);
            Assert.AreEqual(expectedAlbumTitleNextToLastAlphabetically, albumTracks[albumTracks.Count - 2].Album.Title);
            Assert.AreEqual(expectedAlbumTrackNumberLast, albumTracks.Last().AlbumTrackNumber);
            Assert.AreEqual(expectedAlbumTitleLastAlphabetically, albumTracks.Last().Album.Title);
        }

        [Test]
        public async Task GetAlbumTrack_ShouldGetExistingAlbumTrack()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 1, AlbumId = 1, AlbumTrackNumber = 2, SongId = 1 };

            // Act
            var actionResult = await _controller.GetAlbumTrack(existingAlbumTrack.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<AlbumTrack>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingAlbumTrack.AlbumId, contentResult.Content.AlbumId);
            Assert.AreEqual(existingAlbumTrack.AlbumTrackNumber, contentResult.Content.AlbumTrackNumber);
            Assert.AreEqual(existingAlbumTrack.SongId, contentResult.Content.SongId);
        }

        [Test]
        public async Task GetAlbumTrack_ShouldReturnNotFoundForNonExistentAlbumTrack()
        {
            // Arrange
            var nonExistentAlbumTrack = new AlbumTrack() { Id = 99, AlbumId = 1, AlbumTrackNumber = 77, SongId = 1 };

            // Act
            var actionResult = await _controller.GetAlbumTrack(nonExistentAlbumTrack.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PutAlbumTrack_ShouldUpdateAnExistingAlbumTrack()
        {
            // Arrange
            var existingAlbumTrackWithModifiedSongId = new AlbumTrack() { Id = 1, AlbumId = 1, AlbumTrackNumber = 2, SongId = 11 };
            const int expectedAlbumTrackCountAfterPut = 19;

            // Act
            var actionResult = await _controller.PutAlbumTrack(existingAlbumTrackWithModifiedSongId.Id, existingAlbumTrackWithModifiedSongId);
            var albumTracks = _controller.GetAlbumTracks().ToList();

            // Assert
            Assert.IsInstanceOf<StatusCodeResult>(actionResult);
            Assert.AreEqual(HttpStatusCode.NoContent, ((StatusCodeResult)actionResult).StatusCode);
            Assert.AreEqual(expectedAlbumTrackCountAfterPut, albumTracks.Count);
            Assert.AreEqual(existingAlbumTrackWithModifiedSongId.SongId, albumTracks.Find(at => at.Id == existingAlbumTrackWithModifiedSongId.Id).SongId);
        }

        [Test]
        public async Task PutAlbumTrack_ShouldReturnBadRequestWhenIdsInParameterDataDoNotMatch()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 1, AlbumId = 1, AlbumTrackNumber = 2, SongId = 1 };
            const int nonMatchingAlbumTrackId = 99;

            // Act
            var actionResult = await _controller.PutAlbumTrack(nonMatchingAlbumTrackId, existingAlbumTrack);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public async Task PutAlbumTrack_ShouldReturnNotFoundForNonExistentAlbumTrack()
        {
            // Arrange
            var nonExistentAlbumTrack = new AlbumTrack() { Id = 99, AlbumId = 1, AlbumTrackNumber = 77, SongId = 1 };

            // Act
            var actionResult = await _controller.PutAlbumTrack(nonExistentAlbumTrack.Id, nonExistentAlbumTrack);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PostAlbumTrack_ShouldAddAnAlbumTrack()
        {
            // Arrange
            var newAlbumTrack = new AlbumTrack() { AlbumId = 1, AlbumTrackNumber = 77, SongId = 1 };
            const int expectedIdForNewAlbumTrack = 20;
            const int expectedAlbumTrackCountAfterAdd = 20;

            // Act
            var actionResult = await _controller.PostAlbumTrack(newAlbumTrack);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<AlbumTrack>;
            var albumTracks = _controller.GetAlbumTracks().ToList();
            var postedAlbumTrack = albumTracks.Find(at => at.AlbumId == newAlbumTrack.AlbumId && at.AlbumTrackNumber == newAlbumTrack.AlbumTrackNumber);

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(expectedIdForNewAlbumTrack, createdResult.RouteValues["id"]);
            Assert.AreEqual(newAlbumTrack.SongId, postedAlbumTrack.SongId);
            Assert.AreEqual(expectedIdForNewAlbumTrack, postedAlbumTrack.Id);
            Assert.AreEqual(expectedAlbumTrackCountAfterAdd, albumTracks.Count);
        }

        [Test]
        public async Task PostAlbumTrack_ShouldReturnBadRequestWhenModelStateIsInvalid()
        {
            // Arrange
            var newAlbumTrack = new AlbumTrack() { AlbumId = 1, AlbumTrackNumber = 77, SongId = 1 };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var actionResult = await _controller.PostAlbumTrack(newAlbumTrack);

            // Assert
            Assert.IsInstanceOf<InvalidModelStateResult>(actionResult);
        }

        [Test]
        public async Task DeleteAlbumTrack_ShouldDeleteAnExistingAlbumTrack()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 1, AlbumId = 1, AlbumTrackNumber = 2, SongId = 1 };
            const int expectedAlbumTrackCountAfterDelete = 18;

            // Act
            var actionResult = await _controller.DeleteAlbumTrack(existingAlbumTrack.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<AlbumTrack>;
            var albumTracks = _controller.GetAlbumTracks().ToList();

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingAlbumTrack.Id, contentResult.Content.Id);
            Assert.AreEqual(existingAlbumTrack.AlbumId, contentResult.Content.AlbumId);
            Assert.AreEqual(existingAlbumTrack.AlbumTrackNumber, contentResult.Content.AlbumTrackNumber);
            Assert.AreEqual(existingAlbumTrack.SongId, contentResult.Content.SongId);
            Assert.AreEqual(expectedAlbumTrackCountAfterDelete, albumTracks.Count);
        }

        [Test]
        public async Task DeleteAlbumTrack_ShouldReturnNotFoundForNonExistentAlbumTrack()
        {
            // Arrange
            var nonExistentAlbumTrack = new AlbumTrack() { Id = 99, AlbumId = 1, AlbumTrackNumber = 77, SongId = 1 };

            // Act
            var actionResult = await _controller.DeleteAlbumTrack(nonExistentAlbumTrack.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}
