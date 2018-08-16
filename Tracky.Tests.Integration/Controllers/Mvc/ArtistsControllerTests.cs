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
    public class ArtistsControllerTests
    {
        private ArtistsController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new EfDatabaseInitializer());           // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                             // Create data context
            context.Database.Initialize(true);                              // Initialize database on context

            var container = IoC.Initialize();                               // Initialize the IoC container
            _controller = container.GetInstance<ArtistsController>();       // Ask container for a controller, built with all dependencies
        }

        [Test]
        public async Task IndexGet_ShouldGetAllArtistsOrderedByName()
        {
            // Arrange
            const int expectedTotalArtists = 17;
            const string expectedArtistNameFirstAlphabetically = "Against Me!";
            const string expectedArtistNameSecondAlphabetically = "Bad Religion";
            const string expectedArtistNameNextToLastAlphabetically = "Talking Heads";
            const string expectedArtistNameLastAlphabetically = "The Dead Milkmen";

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<Artist>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalArtists, model.Count);
            Assert.AreEqual(expectedArtistNameFirstAlphabetically, model.First().Name);
            Assert.AreEqual(expectedArtistNameSecondAlphabetically, model[1].Name);
            Assert.AreEqual(expectedArtistNameNextToLastAlphabetically, model[model.Count - 2].Name);
            Assert.AreEqual(expectedArtistNameLastAlphabetically, model.Last().Name);
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingArtistId()
        {
            // Arrange
            var existingArtist = new Artist() { Id = 2, Name = "Public Enemy" };

            // Act
            var result = await _controller.Details(existingArtist.Id);
            var viewResult = (ViewResult)result;
            var model = (Artist)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingArtist.Id, model.Id);
            Assert.AreEqual(existingArtist.Name, model.Name);
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
        public async Task DetailsGet_ShouldReturnNotFoundResultForNonExistentArtist()
        {
            // Arrange
            var nonExistentArtist = new Artist() { Id = 99, Name = "Jimmy Buffett" };

            // Act
            var result = await _controller.Details(nonExistentArtist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewArtist()
        {
            // Arrange
            const int expectedTotalArtistsAfterCreate = 18;
            const int expectedIdForNewArtist = 18;
            var newArtist = new Artist() { Name = "Unknown Hinson" };

            // Act
            var result1 = await _controller.Create(newArtist);
            var redirectToRouteResult = (RedirectToRouteResult) result1;
            var result2 = await _controller.Index();
            var viewResult = (ViewResult)result2;
            var model = (List<Artist>)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalArtistsAfterCreate, model.Count);
            Assert.AreEqual(expectedIdForNewArtist, model.Last().Id);
            Assert.AreEqual(newArtist.Name, model.Last().Name);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSameArtistWhenModelStateIsNotValid()
        {
            // Arrange
            var newArtist = new Artist() { Name = "Unknown Hinson" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newArtist);
            var viewResult = (ViewResult)result;
            var model = (Artist)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newArtist, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingArtist()
        {
            // Arrange
            var existingArtist = new Artist() { Id = 2, Name = "Public Enemy" };

            // Act
            var result = await _controller.Edit(existingArtist.Id);
            var viewResult = (ViewResult)result;
            var model = (Artist)viewResult.Model;

            // Assert
            Assert.AreEqual(existingArtist.Id, model.Id);
            Assert.AreEqual(existingArtist.Name, model.Name);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentArtist()
        {
            // Arrange
            var nonExistentArtist = new Artist() { Id = 99, Name = "Jimmy Buffett" };

            // Act
            var result = await _controller.Edit(nonExistentArtist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedArtist()
        {
            // Arrange
            var existingArtistWithModifiedName = new Artist() { Id = 2, Name = "Private Friend" };

            // Act
            var result1 = await _controller.Edit(existingArtistWithModifiedName);
            var redirectToRouteResult = (RedirectToRouteResult)result1;
            var result2 = await _controller.Details(existingArtistWithModifiedName.Id);
            var viewResult = (ViewResult)result2;
            var model = (Artist)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingArtistWithModifiedName.Id, model.Id);
            Assert.AreEqual(existingArtistWithModifiedName.Name, model.Name);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSameArtistWhenModelStateIsNotValid()
        {
            // Arrange
            var existingArtist = new Artist() { Id = 2, Name = "Public Enemy" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingArtist);
            var viewResult = (ViewResult)result;
            var model = (Artist)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingArtist, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingArtist()
        {
            // Arrange
            var existingArtist = new Artist() { Id = 2, Name = "Public Enemy" };

            // Act
            var result = await _controller.Delete(existingArtist.Id);
            var viewResult = (ViewResult)result;
            var model = (Artist)viewResult.Model;

            // Assert
            Assert.AreEqual(existingArtist.Id, model.Id);
            Assert.AreEqual(existingArtist.Name, model.Name);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentArtist()
        {
            // Arrange
            var nonExistentArtist = new Artist() { Id = 99, Name = "Jimmy Buffett" };

            // Act
            var result = await _controller.Delete(nonExistentArtist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingArtist()
        {
            // Arrange
            const int expectedArtistCountAfterDeletion = 16;
            const int existingArtistId = 2;

            // Act
            var result = await _controller.DeleteConfirmed(existingArtistId);
            var redirectToRouteResult = (RedirectToRouteResult)result;
            var result2 = await _controller.Details(existingArtistId);
            var result3 = await _controller.Index();
            var viewResult = (ViewResult)result3;
            var model = (List<Artist>)viewResult.Model;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.IsInstanceOf<HttpNotFoundResult>(result2);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedArtistCountAfterDeletion, model.Count);
        }




        /// <summary>
        /// This is an example of testing dependencies injected via StructureMap - an important thing
        /// to test in an integration test suite. This is a trivial example, where the method on
        /// the service is really silly and simple, but it does demonstrate validation that the
        /// service was injected and is available for use when the Controller is created.
        /// </summary>
        [Test]
        public void ShouldFindServiceOnControllerInjectedViaStructureMap()
        {
            var result = _controller.Zang();
            var viewResult = (ViewResult)result;
            var model = (List<Artist>)viewResult.Model;

            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.AreEqual(77, model[0].Id);
        }
    }
}