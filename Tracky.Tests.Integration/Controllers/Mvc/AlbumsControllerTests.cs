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
    public class AlbumsControllerTests
    {
        private AlbumsController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

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
        public async Task IndexGet_ShouldGetAllAlbumsOrderedByTitle()
        {
            // Arrange
            const int expectedTotalAlbums = 17;
            const string expectedAlbumTitleFirstAlphabetically = "3 Feet High And Rising";
            const string expectedAlbumTitleSecondAlphabetically = "Apocalypse 91...The Enemy Strikes Black";
            const string expectedAlbumTitleNextToLastAlphabetically = "Shape Shift With Me";
            const string expectedAlbumTitleLastAlphabetically = "Stranger Than Fiction";

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<Album>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalAlbums, model.Count);
            Assert.AreEqual(expectedAlbumTitleFirstAlphabetically, model.First().Title);
            Assert.AreEqual(expectedAlbumTitleSecondAlphabetically, model[1].Title);
            Assert.AreEqual(expectedAlbumTitleNextToLastAlphabetically, model[model.Count - 2].Title);
            Assert.AreEqual(expectedAlbumTitleLastAlphabetically, model.Last().Title);
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingAlbumId()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 1, Title = "Bongo Rock", ArtistId = 1, GenreId = 1, Year = 1973, Label = "Pride" };

            // Act
            var result = await _controller.Details(existingAlbum.Id);
            var viewResult = (ViewResult)result;
            var model = (Album)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbum.Id, model.Id);
            Assert.AreEqual(existingAlbum.Title, model.Title);
            Assert.AreEqual(existingAlbum.ArtistId, model.ArtistId);
            Assert.AreEqual(existingAlbum.GenreId, model.GenreId);
            Assert.AreEqual(existingAlbum.Year, model.Year);
            Assert.AreEqual(existingAlbum.Label, model.Label);
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
        public async Task DetailsGet_ShouldReturnNotFoundResultForNonExistentAlbum()
        {
            // Arrange
            var nonExistentAlbum = new Album() { Id = 99, Title = "The Lame Album", ArtistId = 1, GenreId = 1, Year = 2017, Label = "Wack" };

            // Act
            var result = await _controller.Details(nonExistentAlbum.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewAlbum()
        {
            // Arrange
            const int expectedTotalAlbumsAfterCreate = 18;
            const int expectedIdForNewAlbum = 18;
            var newAlbum = new Album() { Title = "Man Plans God Laughs", ArtistId = 2, GenreId = 2, Year = 2015, Label = "RCS" };

            // Act
            var result1 = await _controller.Create(newAlbum);
            var redirectToRouteResult = (RedirectToRouteResult) result1;
            var result2 = await _controller.Index();
            var viewResult = (ViewResult)result2;
            var model = (List<Album>)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalAlbumsAfterCreate, model.Count);
            Assert.AreEqual(expectedIdForNewAlbum, model.Find(a => a.Title == newAlbum.Title).Id);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSameAlbumWhenModelStateIsNotValid()
        {
            // Arrange
            var newAlbum = new Album() { Title = "Man Plans God Laughs", ArtistId = 2, GenreId = 2, Year = 2015, Label = "RCS" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newAlbum);
            var viewResult = (ViewResult)result;
            var model = (Album)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newAlbum, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingAlbum()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 1, Title = "Bongo Rock", ArtistId = 1, GenreId = 1, Year = 1973, Label = "Pride" };

            // Act
            var result = await _controller.Edit(existingAlbum.Id);
            var viewResult = (ViewResult)result;
            var model = (Album)viewResult.Model;

            // Assert
            Assert.AreEqual(existingAlbum.Id, model.Id);
            Assert.AreEqual(existingAlbum.Title, model.Title);
            Assert.AreEqual(existingAlbum.ArtistId, model.ArtistId);
            Assert.AreEqual(existingAlbum.GenreId, model.GenreId);
            Assert.AreEqual(existingAlbum.Year, model.Year);
            Assert.AreEqual(existingAlbum.Label, model.Label);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentAlbum()
        {
            // Arrange
            var nonExistentAlbum = new Album() { Id = 99, Title = "The Lame Album", ArtistId = 1, GenreId = 1, Year = 2017, Label = "Wack" };

            // Act
            var result = await _controller.Edit(nonExistentAlbum.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedAlbum()
        {
            // Arrange
            var existingAlbumWithModifiedTitle = new Album() { Id = 1, Title = "Congo Schlock", ArtistId = 1, GenreId = 1, Year = 1973, Label = "Pride" };

            // Act
            var result1 = await _controller.Edit(existingAlbumWithModifiedTitle);
            var redirectToRouteResult = (RedirectToRouteResult)result1;
            var result2 = await _controller.Details(existingAlbumWithModifiedTitle.Id);
            var viewResult = (ViewResult)result2;
            var model = (Album)viewResult.Model;

            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbumWithModifiedTitle.Id, model.Id);
            Assert.AreEqual(existingAlbumWithModifiedTitle.Title, model.Title);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSameAlbumWhenModelStateIsNotValid()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 1, Title = "Bongo Rock", ArtistId = 1, GenreId = 1, Year = 1973, Label = "Pride" };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingAlbum);
            var viewResult = (ViewResult)result;
            var model = (Album)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbum, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingAlbum()
        {
            var existingAlbum = new Album() { Id = 1, Title = "Bongo Rock", ArtistId = 1, GenreId = 1, Year = 1973, Label = "Pride" };

            var result = await _controller.Delete(existingAlbum.Id);
            var viewResult = (ViewResult)result;
            var model = (Album)viewResult.Model;

            Assert.AreEqual(existingAlbum.Id, model.Id);
            Assert.AreEqual(existingAlbum.Title, model.Title);
            Assert.AreEqual(existingAlbum.ArtistId, model.ArtistId);
            Assert.AreEqual(existingAlbum.GenreId, model.GenreId);
            Assert.AreEqual(existingAlbum.Year, model.Year);
            Assert.AreEqual(existingAlbum.Label, model.Label);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentAlbum()
        {
            // Arrange
            var nonExistentAlbum = new Album() { Id = 99, Title = "The Lame Album", ArtistId = 1, GenreId = 1, Year = 2017, Label = "Wack" };

            // Act
            var result = await _controller.Delete(nonExistentAlbum.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingAlbum()
        {
            // Arrange
            const int expectedAlbumCountAfterDeletion = 16;
            const int existingAlbumIdWithNoAlbumTracks = 4;

            // Act
            var result = await _controller.DeleteConfirmed(existingAlbumIdWithNoAlbumTracks);
            var redirectToRouteResult = (RedirectToRouteResult)result;
            var result2 = await _controller.Details(existingAlbumIdWithNoAlbumTracks);
            var result3 = await _controller.Index();
            var viewResult = (ViewResult)result3;
            var model = (List<Album>)viewResult.Model;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.IsInstanceOf<HttpNotFoundResult>(result2);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedAlbumCountAfterDeletion, model.Count);
        }
    }
}