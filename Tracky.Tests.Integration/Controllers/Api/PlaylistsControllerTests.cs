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
    class PlaylistsControllerTests
    {
        private PlaylistsController _controller;

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
        public void GetPlaylists_ShouldGetAllPlaylistsOrderedByName()
        {
            // Arrange
            const int expectedTotalPlaylists = 4;
            const string expectedPlaylistNameFirstAlphabetically = "Happy Happy Tracks";
            const string expectedPlaylistNameSecondAlphabetically = "Luncheonette Soul Jazz";
            const string expectedPlaylistNameNextToLastAlphabetically = "Mellow Pimpin' Tracks";
            const string expectedPlaylistNameLastAlphabetically = "Zelda Jams";

            // Act
            var playlists = _controller.GetPlaylists().ToList();

            // Assert
            Assert.AreEqual(expectedTotalPlaylists, playlists.Count);
            Assert.AreEqual(expectedPlaylistNameFirstAlphabetically, playlists.First().Name);
            Assert.AreEqual(expectedPlaylistNameSecondAlphabetically, playlists[1].Name);
            Assert.AreEqual(expectedPlaylistNameNextToLastAlphabetically, playlists[playlists.Count - 2].Name);
            Assert.AreEqual(expectedPlaylistNameLastAlphabetically, playlists.Last().Name);
        }

        [Test]
        public async Task GetPlaylist_ShouldGetExistingPlaylist()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 2, Name = "Mellow Pimpin' Tracks" };

            // Act
            var actionResult = await _controller.GetPlaylist(existingPlaylist.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Playlist>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingPlaylist.Name, contentResult.Content.Name);
        }

        [Test]
        public async Task GetPlaylist_ShouldReturnNotFoundForNonExistentPlaylist()
        {
            // Arrange
            var nonExistentPlaylist = new Playlist() { Id = 99, Name = "Fake Tracks. Sad!" };

            // Act
            var actionResult = await _controller.GetPlaylist(nonExistentPlaylist.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PutPlaylist_ShouldUpdateAnExistingPlaylist()
        {
            // Arrange
            var existingPlaylistWithModifiedName = new Playlist() { Id = 2, Name = "Mellow Huggin' Tracks" }; ;
            const int expectedPlaylistCountAfterPut = 4;

            // Act
            var actionResult = await _controller.PutPlaylist(existingPlaylistWithModifiedName.Id, existingPlaylistWithModifiedName);
            var playlists = _controller.GetPlaylists().ToList();

            // Assert
            Assert.IsInstanceOf<StatusCodeResult>(actionResult);
            Assert.AreEqual(HttpStatusCode.NoContent, ((StatusCodeResult)actionResult).StatusCode);
            Assert.AreEqual(expectedPlaylistCountAfterPut, playlists.Count);
            Assert.AreEqual(existingPlaylistWithModifiedName.Name, playlists.Find(a => a.Id == existingPlaylistWithModifiedName.Id).Name);
        }

        [Test]
        public async Task PutPlaylist_ShouldReturnBadRequestWhenIdsInParameterDataDoNotMatch()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 2, Name = "Mellow Pimpin' Tracks" };
            const int nonMatchingPlaylistId = 99;

            // Act
            var actionResult = await _controller.PutPlaylist(nonMatchingPlaylistId, existingPlaylist);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public async Task PutPlaylist_ShouldReturnNotFoundForNonExistentPlaylist()
        {
            // Arrange
            var nonExistentPlaylist = new Playlist() { Id = 99, Name = "Fake Tracks. Sad!" };

            // Act
            var actionResult = await _controller.PutPlaylist(nonExistentPlaylist.Id, nonExistentPlaylist);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PostPlaylist_ShouldAddAnPlaylistAsNextInSequence()
        {
            // Arrange
            var newPlaylist = new Playlist() { Name = "ZZ Top's Worst Songs" };
            const int expectedIdForNewPlaylist = 5;
            const int expectedPlaylistCountAfterAdd = 5;

            // Act
            var actionResult = await _controller.PostPlaylist(newPlaylist);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Playlist>;
            var playlists = _controller.GetPlaylists().ToList();

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(expectedIdForNewPlaylist, createdResult.RouteValues["id"]);
            Assert.AreEqual(newPlaylist.Name, playlists.Last().Name);
            Assert.AreEqual(expectedIdForNewPlaylist, playlists.Last().Id);
            Assert.AreEqual(expectedPlaylistCountAfterAdd, playlists.Count);
        }

        [Test]
        public async Task PostPlaylist_ShouldReturnBadRequestWhenModelStateIsInvalid()
        {
            // Arrange
            var newPlaylist = new Playlist() { Name = "ZZ Top's Worst Songs" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var actionResult = await _controller.PostPlaylist(newPlaylist);

            // Assert
            Assert.IsInstanceOf<InvalidModelStateResult>(actionResult);
        }

        [Test]
        public async Task DeletePlaylist_ShouldDeleteAnExistingPlaylist()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 2, Name = "Mellow Pimpin' Tracks" };
            const int expectedPlaylistCountAfterDelete = 3;

            // Act
            var actionResult = await _controller.DeletePlaylist(existingPlaylist.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Playlist>;
            var playlists = _controller.GetPlaylists().ToList();

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingPlaylist.Id, contentResult.Content.Id);
            Assert.AreEqual(existingPlaylist.Name, contentResult.Content.Name);
            Assert.AreEqual(expectedPlaylistCountAfterDelete, playlists.Count);
        }

        [Test]
        public async Task DeletePlaylist_ShouldReturnNotFoundForNonExistentPlaylist()
        {
            // Arrange
            var nonExistentPlaylist = new Playlist() { Id = 99, Name = "Fake Tracks. Sad!" };

            // Act
            var actionResult = await _controller.DeletePlaylist(nonExistentPlaylist.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}
