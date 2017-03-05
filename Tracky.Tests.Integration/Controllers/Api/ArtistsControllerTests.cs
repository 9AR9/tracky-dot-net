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
    class ArtistsControllerTests
    {
        private ArtistsController _controller;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new DatabaseInitializer());             // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                             // Create data context
            context.Database.Initialize(true);                              // Initialize database on context

            var container = IoC.Initialize();                               // Initialize the IoC container
            _controller = container.GetInstance<ArtistsController>();       // Ask container for a controller, built with all dependencies
        }

        [Test]
        public void GetArtists_ShouldGetAllArtistsOrderedByName()
        {
            // Arrange
            const int expectedTotalArtists = 17;
            const string expectedArtistNameFirstAlphabetically = "Against Me!";
            const string expectedArtistNameSecondAlphabetically = "Bad Religion";
            const string expectedArtistNameNextToLastAlphabetically = "Talking Heads";
            const string expectedArtistNameLastAlphabetically = "The Dead Milkmen";

            // Act
            var artists = _controller.GetArtists().ToList();

            // Assert
            Assert.AreEqual(expectedTotalArtists, artists.Count);
            Assert.AreEqual(expectedArtistNameFirstAlphabetically, artists.First().Name);
            Assert.AreEqual(expectedArtistNameSecondAlphabetically, artists[1].Name);
            Assert.AreEqual(expectedArtistNameNextToLastAlphabetically, artists[artists.Count - 2].Name);
            Assert.AreEqual(expectedArtistNameLastAlphabetically, artists.Last().Name);
        }

        [Test]
        public async Task GetArtist_ShouldGetExistingArtist()
        {
            // Arrange
            var existingArtist = new Artist() { Id = 2, Name = "Public Enemy" };

            // Act
            var actionResult = await _controller.GetArtist(existingArtist.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Artist>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingArtist.Name, contentResult.Content.Name);
        }

        [Test]
        public async Task GetArtist_ShouldReturnNotFoundForNonExistentArtist()
        {
            // Arrange
            var nonExistentArtist = new Artist() { Id = 99, Name = "Jimmy Buffett" };

            // Act
            var actionResult = await _controller.GetArtist(nonExistentArtist.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PutArtist_ShouldUpdateAnExistingArtist()
        {
            // Arrange
            var existingArtistWithModifiedName = new Artist() { Id = 2, Name = "Private Friend" };
            const int expectedArtistCountAfterPut = 17;

            // Act
            var actionResult = await _controller.PutArtist(existingArtistWithModifiedName.Id, existingArtistWithModifiedName);
            var artists = _controller.GetArtists().ToList();

            // Assert
            Assert.IsInstanceOf<StatusCodeResult>(actionResult);
            Assert.AreEqual(HttpStatusCode.NoContent, ((StatusCodeResult)actionResult).StatusCode);
            Assert.AreEqual(expectedArtistCountAfterPut, artists.Count);
            Assert.AreEqual(existingArtistWithModifiedName.Name, artists.Find(a => a.Id == existingArtistWithModifiedName.Id).Name);
        }

        [Test]
        public async Task PutArtist_ShouldReturnBadRequestWhenIdsInParameterDataDoNotMatch()
        {
            // Arrange
            var existingArtist = new Artist() { Id = 2, Name = "Public Enemy" };
            const int nonMatchingArtistId = 99;

            // Act
            var actionResult = await _controller.PutArtist(nonMatchingArtistId, existingArtist);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public async Task PutArtist_ShouldReturnNotFoundForNonExistentArtist()
        {
            // Arrange
            var nonExistentArtist = new Artist() { Id = 99, Name = "Jimmy Buffett" };

            // Act
            var actionResult = await _controller.PutArtist(nonExistentArtist.Id, nonExistentArtist);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PostArtist_ShouldAddAnArtistAsNextInSequence()
        {
            // Arrange
            var newArtist = new Artist() { Name = "Unknown Hinson" };
            const int expectedIdForNewArtist = 18;
            const int expectedArtistCountAfterAdd = 18;

            // Act
            var actionResult = await _controller.PostArtist(newArtist);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Artist>;
            var artists = _controller.GetArtists().ToList();

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(expectedIdForNewArtist, createdResult.RouteValues["id"]);
            Assert.AreEqual(newArtist.Name, artists.Last().Name);
            Assert.AreEqual(expectedIdForNewArtist, artists.Last().Id);
            Assert.AreEqual(expectedArtistCountAfterAdd, artists.Count);
        }

        [Test]
        public async Task PostArtist_ShouldReturnBadRequestWhenModelStateIsInvalid()
        {
            // Arrange
            var newArtist = new Artist() { Name = "Unknown Hinson" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var actionResult = await _controller.PostArtist(newArtist);

            // Assert
            Assert.IsInstanceOf<InvalidModelStateResult>(actionResult);
        }

        [Test]
        public async Task DeleteArtist_ShouldDeleteAnExistingArtist()
        {
            // Arrange
            var existingArtist = new Artist() { Id = 2, Name = "Public Enemy" };
            const int expectedArtistCountAfterDelete = 16;

            // Act
            var actionResult = await _controller.DeleteArtist(existingArtist.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Artist>;
            var artists = _controller.GetArtists().ToList();

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingArtist.Id, contentResult.Content.Id);
            Assert.AreEqual(existingArtist.Name, contentResult.Content.Name);
            Assert.AreEqual(expectedArtistCountAfterDelete, artists.Count);
        }

        [Test]
        public async Task DeleteArtist_ShouldReturnNotFoundForNonExistentArtist()
        {
            // Arrange
            var nonExistentArtist = new Artist() { Id = 99, Name = "Jimmy Buffett" };

            // Act
            var actionResult = await _controller.DeleteArtist(nonExistentArtist.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}
