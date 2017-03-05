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
    class GenresControllerTests
    {
        private GenresController _controller;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new DatabaseInitializer());         // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                         // Create data context
            context.Database.Initialize(true);                          // Initialize database on context

            var container = IoC.Initialize();                           // Initialize the IoC container
            _controller = container.GetInstance<GenresController>();    // Ask container for a controller, built with all dependencies
        }

        [Test]
        public void GetGenres_ShouldGetAllGenresOrderedByName()
        {
            // Arrange
            const int expectedTotalGenres = 10;
            const string expectedGenreNameFirstAlphabetically = "Children";
            const string expectedGenreNameSecondAlphabetically = "Comedy";
            const string expectedGenreNameNextToLastAlphabetically = "R&B";
            const string expectedGenreNameLastAlphabetically = "Rock";

            // Act
            var genres = _controller.GetGenres().ToList();

            // Assert
            Assert.AreEqual(expectedTotalGenres, genres.Count);
            Assert.AreEqual(expectedGenreNameFirstAlphabetically, genres.First().Name);
            Assert.AreEqual(expectedGenreNameSecondAlphabetically, genres[1].Name);
            Assert.AreEqual(expectedGenreNameNextToLastAlphabetically, genres[genres.Count - 2].Name);
            Assert.AreEqual(expectedGenreNameLastAlphabetically, genres.Last().Name);
        }

        [Test]
        public async Task GetGenre_ShouldGetExistingGenre()
        {
            // Arrange
            var existingGenre = new Genre() { Id = 2, Name = "Hip-Hop" };

            // Act
            var actionResult = await _controller.GetGenre(existingGenre.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Genre>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingGenre.Name, contentResult.Content.Name);
        }

        [Test]
        public async Task GetGenre_ShouldReturnNotFoundForNonExistentGenre()
        {
            // Arrange
            var nonExistentGenre = new Genre() { Id = 99, Name = "Eee dee emm" };

            // Act
            var actionResult = await _controller.GetGenre(nonExistentGenre.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PutGenre_ShouldUpdateAnExistingGenre()
        {
            // Arrange
            var existingGenreWithModifiedName = new Genre() { Id = 2, Name = "Hip-Hop And Ya Don't Stop" };
            const int expectedGenreCountAfterPut = 10;

            // Act
            var actionResult = await _controller.PutGenre(existingGenreWithModifiedName.Id, existingGenreWithModifiedName);
            var genres = _controller.GetGenres().ToList();

            // Assert
            Assert.IsInstanceOf<StatusCodeResult>(actionResult);
            Assert.AreEqual(HttpStatusCode.NoContent, ((StatusCodeResult)actionResult).StatusCode);
            Assert.AreEqual(expectedGenreCountAfterPut, genres.Count);
            Assert.AreEqual(existingGenreWithModifiedName.Name, genres.Find(a => a.Id == existingGenreWithModifiedName.Id).Name);
        }

        [Test]
        public async Task PutGenre_ShouldReturnBadRequestWhenIdsInParameterDataDoNotMatch()
        {
            // Arrange
            var existingGenre = new Genre() { Id = 2, Name = "Hip-Hop" };
            const int nonMatchingGenreId = 99;

            // Act
            var actionResult = await _controller.PutGenre(nonMatchingGenreId, existingGenre);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public async Task PutGenre_ShouldReturnNotFoundForNonExistentGenre()
        {
            // Arrange
            var nonExistentGenre = new Genre() { Id = 99, Name = "Eee dee emm" };

            // Act
            var actionResult = await _controller.PutGenre(nonExistentGenre.Id, nonExistentGenre);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PostGenre_ShouldAddAnGenreAsNextInSequence()
        {
            // Arrange
            var newGenre = new Genre() { Name = "Mariachi" };
            const int expectedIdForNewGenre = 11;
            const int expectedGenreCountAfterAdd = 11;

            // Act
            var actionResult = await _controller.PostGenre(newGenre);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Genre>;
            var genres = _controller.GetGenres().ToList();

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(expectedIdForNewGenre, createdResult.RouteValues["id"]);
            Assert.AreEqual(newGenre.Name, genres.Find(g => g.Id == expectedIdForNewGenre).Name);
            Assert.AreEqual(expectedGenreCountAfterAdd, genres.Count);
        }

        [Test]
        public async Task PostGenre_ShouldReturnBadRequestWhenModelStateIsInvalid()
        {
            // Arrange
            var newGenre = new Genre() { Name = "Mariachi" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var actionResult = await _controller.PostGenre(newGenre);

            // Assert
            Assert.IsInstanceOf<InvalidModelStateResult>(actionResult);
        }

        [Test]
        public async Task DeleteGenre_ShouldDeleteExistingGenreWhenNotLinkedToAnySongsOrAlbums()
        {
            // Arrange
            const int expectedGenreCountAfterDeletion = 9;
            var existingGenreWithNoCurrentUsageInSongsOrAlbums = new Genre() { Id = 10, Name = "Country" };

            // Act
            var actionResult = await _controller.DeleteGenre(existingGenreWithNoCurrentUsageInSongsOrAlbums.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Genre>;
            var genres = _controller.GetGenres().ToList();
            var getResult = await _controller.GetGenre(existingGenreWithNoCurrentUsageInSongsOrAlbums.Id);

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingGenreWithNoCurrentUsageInSongsOrAlbums.Id, contentResult.Content.Id);
            Assert.AreEqual(existingGenreWithNoCurrentUsageInSongsOrAlbums.Name, contentResult.Content.Name);
            Assert.AreEqual(expectedGenreCountAfterDeletion, genres.Count);
            Assert.IsInstanceOf<NotFoundResult>(getResult);
        }

        [Test]
        public async Task DeleteGenre_ShouldReturnNotFoundForNonExistentGenre()
        {
            // Arrange
            var nonExistentGenre = new Genre() { Id = 99, Name = "Eee dee emm" };

            // Act
            var actionResult = await _controller.DeleteGenre(nonExistentGenre.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}
