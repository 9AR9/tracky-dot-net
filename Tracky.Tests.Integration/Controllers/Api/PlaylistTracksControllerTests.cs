using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;
using NUnit.Framework;
using Tracky.Controllers.Api;
using Tracky.DependencyResolution;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;

namespace Tracky.Tests.Integration.Controllers.Api
{
    class PlaylistTracksControllerTests
    {
        private PlaylistTracksController _controller;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new EfDatabaseInitializer());               // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                                 // Create data context
            context.Database.Initialize(true);                                  // Initialize database on context

            var container = IoC.Initialize();                                   // Initialize the IoC container
            _controller = container.GetInstance<PlaylistTracksController>();    // Ask container for a controller, built with all dependencies
        }

        [Test]
        public void GetPlaylistTracks_ShouldGetAllPlaylistTracksOrderedByPlaylistNameThenPlaylistTrackNumber()
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
            var playlistTracks = _controller.GetPlaylistTracks().ToList();

            // Assert
            Assert.AreEqual(expectedTotalPlaylistTracks, playlistTracks.Count);
            Assert.AreEqual(expectedPlaylistTrackNumberFirst, playlistTracks.First().PlaylistTrackNumber);
            Assert.AreEqual(expectedPlaylistNameFirstAlphabetically, playlistTracks.First().Playlist.Name);
            Assert.AreEqual(expectedPlaylistTrackNumberSecond, playlistTracks[1].PlaylistTrackNumber);
            Assert.AreEqual(expectedPlaylistNameFirstAlphabetically, playlistTracks[1].Playlist.Name);
            Assert.AreEqual(expectedPlaylistTrackNumberNextToLast, playlistTracks[playlistTracks.Count - 2].PlaylistTrackNumber);
            Assert.AreEqual(expectedAlbumTitleLastAlphabetically, playlistTracks[playlistTracks.Count - 2].Playlist.Name);
            Assert.AreEqual(expectedPlaylistTrackNumberLast, playlistTracks.Last().PlaylistTrackNumber);
            Assert.AreEqual(expectedAlbumTitleLastAlphabetically, playlistTracks.Last().Playlist.Name);
        }

        [Test]
        public async Task GetPlaylistTrack_ShouldGetExistingPlaylistTrack()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 1 };

            // Act
            var actionResult = await _controller.GetPlaylistTrack(existingPlaylistTrack.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<PlaylistTrack>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingPlaylistTrack.PlaylistId, contentResult.Content.PlaylistId);
            Assert.AreEqual(existingPlaylistTrack.PlaylistTrackNumber, contentResult.Content.PlaylistTrackNumber);
            Assert.AreEqual(existingPlaylistTrack.SongId, contentResult.Content.SongId);
        }

        [Test]
        public async Task GetPlaylistTrack_ShouldReturnNotFoundForNonExistentPlaylistTrack()
        {
            // Arrange
            var nonExistentPlaylistTrack = new PlaylistTrack() { Id = 99, PlaylistId = 1, PlaylistTrackNumber = 77, SongId = 1 };

            // Act
            var actionResult = await _controller.GetPlaylistTrack(nonExistentPlaylistTrack.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PutPlaylistTrack_ShouldUpdateAnExistingPlaylistTrack()
        {
            // Arrange
            var existingPlaylistTrackWithModifiedSongId = new PlaylistTrack() { Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 11 };
            const int expectedPlaylistTrackCountAfterPut = 17;

            // Act
            var actionResult = await _controller.PutPlaylistTrack(existingPlaylistTrackWithModifiedSongId.Id, existingPlaylistTrackWithModifiedSongId);
            var playlistTracks = _controller.GetPlaylistTracks().ToList();

            // Assert
            Assert.IsInstanceOf<StatusCodeResult>(actionResult);
            Assert.AreEqual(HttpStatusCode.NoContent, ((StatusCodeResult)actionResult).StatusCode);
            Assert.AreEqual(expectedPlaylistTrackCountAfterPut, playlistTracks.Count);
            Assert.AreEqual(existingPlaylistTrackWithModifiedSongId.SongId, playlistTracks.Find(at => at.Id == existingPlaylistTrackWithModifiedSongId.Id).SongId);
        }

        [Test]
        public async Task PutPlaylistTrack_ShouldReturnBadRequestWhenIdsInParameterDataDoNotMatch()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 1 };
            const int nonMatchingPlaylistTrackId = 99;

            // Act
            var actionResult = await _controller.PutPlaylistTrack(nonMatchingPlaylistTrackId, existingPlaylistTrack);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public async Task PutPlaylistTrack_ShouldReturnNotFoundForNonExistentPlaylistTrack()
        {
            // Arrange
            var nonExistentPlaylistTrack = new PlaylistTrack() { Id = 99, PlaylistId = 1, PlaylistTrackNumber = 77, SongId = 1 };

            // Act
            var actionResult = await _controller.PutPlaylistTrack(nonExistentPlaylistTrack.Id, nonExistentPlaylistTrack);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PostPlaylistTrack_ShouldAddAnPlaylistTrack()
        {
            // Arrange
            var newPlaylistTrack = new PlaylistTrack() { PlaylistId = 1, PlaylistTrackNumber = 77, SongId = 1 };
            const int expectedIdForNewPlaylistTrack = 18;
            const int expectedPlaylistTrackCountAfterAdd = 18;

            // Act
            var actionResult = await _controller.PostPlaylistTrack(newPlaylistTrack);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<PlaylistTrack>;
            var playlistTracks = _controller.GetPlaylistTracks().ToList();
            var postedPlaylistTrack = playlistTracks.Find(at => at.PlaylistId == newPlaylistTrack.PlaylistId && at.PlaylistTrackNumber == newPlaylistTrack.PlaylistTrackNumber);

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(expectedIdForNewPlaylistTrack, createdResult.RouteValues["id"]);
            Assert.AreEqual(newPlaylistTrack.SongId, postedPlaylistTrack.SongId);
            Assert.AreEqual(expectedIdForNewPlaylistTrack, postedPlaylistTrack.Id);
            Assert.AreEqual(expectedPlaylistTrackCountAfterAdd, playlistTracks.Count);
        }

        [Test]
        public async Task PostPlaylistTrack_ShouldReturnBadRequestWhenModelStateIsInvalid()
        {
            // Arrange
            var newPlaylistTrack = new PlaylistTrack() { PlaylistId = 1, PlaylistTrackNumber = 77, SongId = 1 };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var actionResult = await _controller.PostPlaylistTrack(newPlaylistTrack);

            // Assert
            Assert.IsInstanceOf<InvalidModelStateResult>(actionResult);
        }

        [Test]
        public async Task DeletePlaylistTrack_ShouldDeleteAnExistingPlaylistTrack()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 1 };
            const int expectedPlaylistTrackCountAfterDelete = 16;

            // Act
            var actionResult = await _controller.DeletePlaylistTrack(existingPlaylistTrack.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<PlaylistTrack>;
            var playlistTracks = _controller.GetPlaylistTracks().ToList();

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingPlaylistTrack.Id, contentResult.Content.Id);
            Assert.AreEqual(existingPlaylistTrack.PlaylistId, contentResult.Content.PlaylistId);
            Assert.AreEqual(existingPlaylistTrack.PlaylistTrackNumber, contentResult.Content.PlaylistTrackNumber);
            Assert.AreEqual(existingPlaylistTrack.SongId, contentResult.Content.SongId);
            Assert.AreEqual(expectedPlaylistTrackCountAfterDelete, playlistTracks.Count);
        }

        [Test]
        public async Task DeletePlaylistTrack_ShouldReturnNotFoundForNonExistentPlaylistTrack()
        {
            // Arrange
            var nonExistentPlaylistTrack = new PlaylistTrack() { Id = 99, PlaylistId = 1, PlaylistTrackNumber = 77, SongId = 1 };

            // Act
            var actionResult = await _controller.DeletePlaylistTrack(nonExistentPlaylistTrack.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}
