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
    class AlbumsControllerTests
    {
        private AlbumsController _controller;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new DatabaseInitializer());             // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                             // Create data context
            context.Database.Initialize(true);                              // Initialize database on context

            var container = IoC.Initialize();                               // Initialize the IoC container
            _controller = container.GetInstance<AlbumsController>();        // Ask container for a controller, built with all dependencies
        }

        [Test]
        public void GetAlbums_ShouldGetAllAlbumsOrderedByTitle()
        {
            // Arrange
            const int expectedTotalAlbums = 17;
            const string expectedAlbumTitleFirstAlphabetically = "3 Feet High And Rising";
            const string expectedAlbumTitleSecondAlphabetically = "Apocalypse 91...The Enemy Strikes Black";
            const string expectedAlbumTitleNextToLastAlphabetically = "Shape Shift With Me";
            const string expectedAlbumTitleLastAlphabetically = "Stranger Than Fiction";

            // Act
            var albums = _controller.GetAlbums().ToList();

            // Assert
            Assert.AreEqual(expectedTotalAlbums, albums.Count);
            Assert.AreEqual(expectedTotalAlbums, albums.Count);
            Assert.AreEqual(expectedAlbumTitleFirstAlphabetically, albums.First().Title);
            Assert.AreEqual(expectedAlbumTitleSecondAlphabetically, albums[1].Title);
            Assert.AreEqual(expectedAlbumTitleNextToLastAlphabetically, albums[albums.Count - 2].Title);
            Assert.AreEqual(expectedAlbumTitleLastAlphabetically, albums.Last().Title);
        }

        [Test]
        public async Task GetAlbum_ShouldGetExistingAlbum()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 1, Title = "Bongo Rock", ArtistId = 1, GenreId = 1, Year = 1973, Label = "Pride" };

            // Act
            var actionResult = await _controller.GetAlbum(existingAlbum.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Album>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingAlbum.Title, contentResult.Content.Title);
            Assert.AreEqual(existingAlbum.ArtistId, contentResult.Content.ArtistId);
            Assert.AreEqual(existingAlbum.GenreId, contentResult.Content.GenreId);
            Assert.AreEqual(existingAlbum.Year, contentResult.Content.Year);
            Assert.AreEqual(existingAlbum.Label, contentResult.Content.Label);
        }

        [Test]
        public async Task GetAlbum_ShouldReturnNotFoundForNonExistentAlbum()
        {
            // Arrange
            var nonExistentAlbum = new Album() { Id = 99, Title = "The Lame Album", ArtistId = 1, GenreId = 1, Year = 2017, Label = "Wack" };

            // Act
            var actionResult = await _controller.GetAlbum(nonExistentAlbum.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PutAlbum_ShouldUpdateAnExistingAlbum()
        {
            // Arrange
            var existingAlbumWithModifiedName = new Album() { Id = 1, Title = "Congo Schlock", ArtistId = 1, GenreId = 1, Year = 1973, Label = "Pride" };
            const int expectedAlbumCountAfterPut = 17;

            // Act
            var actionResult = await _controller.PutAlbum(existingAlbumWithModifiedName.Id, existingAlbumWithModifiedName);
            var albums = _controller.GetAlbums().ToList();

            // Assert
            Assert.IsInstanceOf<StatusCodeResult>(actionResult);
            Assert.AreEqual(HttpStatusCode.NoContent, ((StatusCodeResult)actionResult).StatusCode);
            Assert.AreEqual(expectedAlbumCountAfterPut, albums.Count);
            Assert.AreEqual(existingAlbumWithModifiedName.Title, albums.Find(a => a.Id == existingAlbumWithModifiedName.Id).Title);
        }

        [Test]
        public async Task PutAlbum_ShouldReturnBadRequestWhenIdsInParameterDataDoNotMatch()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 1, Title = "Bongo Rock", ArtistId = 1, GenreId = 1, Year = 1973, Label = "Pride" };
            const int nonMatchingAlbumId = 99;

            // Act
            var actionResult = await _controller.PutAlbum(nonMatchingAlbumId, existingAlbum);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public async Task PutAlbum_ShouldReturnNotFoundForNonExistentAlbum()
        {
            // Arrange
            var nonExistentAlbum = new Album() { Id = 99, Title = "The Lame Album", ArtistId = 1, GenreId = 1, Year = 2017, Label = "Wack" };

            // Act
            var actionResult = await _controller.PutAlbum(nonExistentAlbum.Id, nonExistentAlbum);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PostAlbum_ShouldAddAnAlbum()
        {
            // Arrange
            var newAlbum = new Album() { Title = "Man Plans God Laughs", ArtistId = 2, GenreId = 2, Year = 2015, Label = "RCS" };
            const int expectedIdForNewAlbum = 18;
            const int expectedAlbumCountAfterAdd = 18;

            // Act
            var actionResult = await _controller.PostAlbum(newAlbum);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Album>;
            var albums = _controller.GetAlbums().ToList();
            var postedAlbum = albums.Find(a => a.Title == newAlbum.Title && a.ArtistId == newAlbum.ArtistId);

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(expectedIdForNewAlbum, createdResult.RouteValues["id"]);
            Assert.AreEqual(newAlbum.Title, postedAlbum.Title);
            Assert.AreEqual(newAlbum.ArtistId, postedAlbum.ArtistId);
            Assert.AreEqual(newAlbum.GenreId, postedAlbum.GenreId);
            Assert.AreEqual(newAlbum.Year, postedAlbum.Year);
            Assert.AreEqual(newAlbum.Label, postedAlbum.Label);
            Assert.AreEqual(expectedIdForNewAlbum, postedAlbum.Id);
            Assert.AreEqual(expectedAlbumCountAfterAdd, albums.Count);
        }

        [Test]
        public async Task PostAlbum_ShouldReturnBadRequestWhenModelStateIsInvalid()
        {
            // Arrange
            var newAlbum = new Album() { Title = "Man Plans God Laughs", ArtistId = 2, GenreId = 2, Year = 2015, Label = "RCS" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var actionResult = await _controller.PostAlbum(newAlbum);

            // Assert
            Assert.IsInstanceOf<InvalidModelStateResult>(actionResult);
        }

        [Test]
        public async Task DeleteAlbum_ShouldDeleteAnExistingAlbum()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 1, Title = "Bongo Rock", ArtistId = 1, GenreId = 1, Year = 1973, Label = "Pride" };
            const int expectedAlbumCountAfterDelete = 16;

            // Act
            var actionResult = await _controller.DeleteAlbum(existingAlbum.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Album>;
            var albums = _controller.GetAlbums().ToList();

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingAlbum.Id, contentResult.Content.Id);
            Assert.AreEqual(existingAlbum.Title, contentResult.Content.Title);
            Assert.AreEqual(existingAlbum.ArtistId, contentResult.Content.ArtistId);
            Assert.AreEqual(existingAlbum.GenreId, contentResult.Content.GenreId);
            Assert.AreEqual(existingAlbum.Year, contentResult.Content.Year);
            Assert.AreEqual(existingAlbum.Label, contentResult.Content.Label);
            Assert.AreEqual(expectedAlbumCountAfterDelete, albums.Count);
        }

        [Test]
        public async Task DeleteAlbum_ShouldReturnNotFoundForNonExistentAlbum()
        {
            // Arrange
            var nonExistentAlbum = new Album() { Id = 99, Title = "The Lame Album", ArtistId = 1, GenreId = 1, Year = 2017, Label = "Wack" };

            // Act
            var actionResult = await _controller.DeleteAlbum(nonExistentAlbum.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}
