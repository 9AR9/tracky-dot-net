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
    public class GenresControllerTests
    {
        private GenresController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new EfDatabaseInitializer());           // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                             // Create data context
            context.Database.Initialize(true);                              // Initialize database on context

            var container = IoC.Initialize();                               // Initialize the IoC container
            _controller = container.GetInstance<GenresController>();        // Ask container for a controller, built with all dependencies
        }

        [Test]
        public async Task IndexGet_ShouldGetAllGenresOrderedByName()
        {
            // Arrange
            const int expectedTotalGenres = 10;
            const string expectedGenreNameFirstAlphabetically = "Children";
            const string expectedGenreNameSecondAlphabetically = "Comedy";
            const string expectedGenreNameNextToLastAlphabetically = "R&B";
            const string expectedGenreNameLastAlphabetically = "Rock";

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<Genre>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalGenres, model.Count);
            Assert.AreEqual(expectedGenreNameFirstAlphabetically, model.First().Name);
            Assert.AreEqual(expectedGenreNameSecondAlphabetically, model[1].Name);
            Assert.AreEqual(expectedGenreNameNextToLastAlphabetically, model[model.Count - 2].Name);
            Assert.AreEqual(expectedGenreNameLastAlphabetically, model.Last().Name);
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingGenreId()
        {
            // Arrange
            var existingGenre = new Genre() { Id = 2, Name = "Hip-Hop" };

            // Act
            var result = await _controller.Details(existingGenre.Id);
            var viewResult = (ViewResult)result;
            var model = (Genre)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingGenre.Id, model.Id);
            Assert.AreEqual(existingGenre.Name, model.Name);
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
        public async Task DetailsGet_ShouldReturnNotFoundResultForNonExistentGenre()
        {
            // Arrange
            var nonExistentGenre = new Genre() { Id = 99, Name = "Eee dee emm" };

            // Act
            var result = await _controller.Details(nonExistentGenre.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewGenre()
        {
            // Arrange
            const int expectedTotalGenresAfterCreate = 11;
            const int expectedIdForNewGenre = 11;
            var newGenre = new Genre() { Name = "Trance" };

            // Act
            var result1 = await _controller.Create(newGenre);
            var redirectToRouteResult = (RedirectToRouteResult) result1;
            var result2 = await _controller.Index();
            var viewResult = (ViewResult)result2;
            var model = (List<Genre>)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalGenresAfterCreate, model.Count);
            Assert.AreEqual(expectedIdForNewGenre, model.Last().Id);
            Assert.AreEqual(newGenre.Name, model.Last().Name);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSameGenreWhenModelStateIsNotValid()
        {
            // Arrange
            var newGenre = new Genre() { Name = "Mariachi" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newGenre);
            var viewResult = (ViewResult)result;
            var model = (Genre)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newGenre, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingGenre()
        {
            // Arrange
            var existingGenre = new Genre() { Id = 2, Name = "Hip-Hop" };

            // Act
            var result = await _controller.Edit(existingGenre.Id);
            var viewResult = (ViewResult)result;
            var model = (Genre)viewResult.Model;

            // Assert
            Assert.AreEqual(existingGenre.Id, model.Id);
            Assert.AreEqual(existingGenre.Name, model.Name);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentGenre()
        {
            // Arrange
            var nonExistentGenre = new Genre() { Id = 99, Name = "Eee dee emm" };

            // Act
            var result = await _controller.Edit(nonExistentGenre.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedGenre()
        {
            // Arrange
            var existingGenreWithModifiedName = new Genre() { Id = 2, Name = "Hip-Hop And Ya Don't Stop" };

            // Act
            var result1 = await _controller.Edit(existingGenreWithModifiedName);
            var redirectToRouteResult = (RedirectToRouteResult)result1;
            var result2 = await _controller.Details(existingGenreWithModifiedName.Id);
            var viewResult = (ViewResult)result2;
            var model = (Genre)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingGenreWithModifiedName.Id, model.Id);
            Assert.AreEqual(existingGenreWithModifiedName.Name, model.Name);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSameGenreWhenModelStateIsNotValid()
        {
            // Arrange
            var existingGenre = new Genre() { Id = 2, Name = "Hip-Hop" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingGenre);
            var viewResult = (ViewResult)result;
            var model = (Genre)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingGenre, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingGenre()
        {
            // Arrange
            var existingGenre = new Genre() { Id = 2, Name = "Hip-Hop" };

            // Act
            var result = await _controller.Delete(existingGenre.Id);
            var viewResult = (ViewResult)result;
            var model = (Genre)viewResult.Model;

            // Assert
            Assert.AreEqual(existingGenre.Id, model.Id);
            Assert.AreEqual(existingGenre.Name, model.Name);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentGenre()
        {
            // Arrange
            var nonExistentGenre = new Genre() { Id = 99, Name = "Eee dee emm" };

            // Act
            var result = await _controller.Delete(nonExistentGenre.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingGenreWhenNotLinkedToAnySongsOrAlbums()
        {
            // Arrange
            const int expectedGenreCountAfterDeletion = 9;
            const int existingGenreId = 10;

            // Act
            var result = await _controller.DeleteConfirmed(existingGenreId);
            var redirectToRouteResult = (RedirectToRouteResult)result;
            var result2 = await _controller.Details(existingGenreId);
            var result3 = await _controller.Index();
            var viewResult = (ViewResult)result3;
            var model = (List<Genre>)viewResult.Model;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.IsInstanceOf<HttpNotFoundResult>(result2);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedGenreCountAfterDeletion, model.Count);
        }
    }
}