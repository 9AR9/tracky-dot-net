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
    public class SongsControllerTests
    {
        private SongsController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new DatabaseInitializer());             // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                             // Create data context
            context.Database.Initialize(true);                              // Initialize database on context

            var container = IoC.Initialize();                               // Initialize the IoC container
            _controller = container.GetInstance<SongsController>();         // Ask container for a controller, built with all dependencies
        }

        [Test]
        public async Task IndexGet_ShouldGetAllSongsOrderedByArtistThenTitle()
        {
            // Arrange
            const int expectedTotalSongs = 19;
            const string expectedArtistFirstAlphabetically = "Against Me!";
            const string expectedArtistNextToLastAlphabetically = "Talking Heads";
            const string expectedArtistLastAlphabetically = "The Dead Milkmen";
            const string expectedSongTitleFirst = "333";
            const string expectedSongTitleSecond = "Dead Rats";
            const string expectedSongTitleNextToLast = "Crosseyed & Painless";
            const string expectedSongTitleLast = "Takin' Retards To The Zoo";

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<Song>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalSongs, model.Count);
            Assert.AreEqual(expectedSongTitleFirst, model.First().Title);
            Assert.AreEqual(expectedArtistFirstAlphabetically, model.First().Artist.Name);
            Assert.AreEqual(expectedSongTitleSecond, model[1].Title);
            Assert.AreEqual(expectedArtistFirstAlphabetically, model[1].Artist.Name);
            Assert.AreEqual(expectedSongTitleNextToLast, model[model.Count - 2].Title);
            Assert.AreEqual(expectedArtistNextToLastAlphabetically, model[model.Count - 2].Artist.Name);
            Assert.AreEqual(expectedSongTitleLast, model.Last().Title);
            Assert.AreEqual(expectedArtistLastAlphabetically, model.Last().Artist.Name);
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingSongId()
        {
            // Arrange
            var existingSong = new Song() { Id = 1, Title = "Apache", ArtistId = 1, GenreId = 1 };

            // Act
            var result = await _controller.Details(existingSong.Id);
            var viewResult = (ViewResult)result;
            var model = (Song)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingSong.Id, model.Id);
            Assert.AreEqual(existingSong.Title, model.Title);
            Assert.AreEqual(existingSong.ArtistId, model.ArtistId);
            Assert.AreEqual(existingSong.GenreId, model.GenreId);
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
        public async Task DetailsGet_ShouldReturnNotFoundResultForNonExistentSong()
        {
            // Arrange
            var nonExistentSong = new Song() { Id = 99, Title = "The Song That Didn't Exist", ArtistId = 1, GenreId = 1 };

            // Act
            var result = await _controller.Details(nonExistentSong.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewSong()
        {
            // Arrange
            const int expectedTotalSongsAfterCreate = 20;
            const int expectedIdForNewSong = 20;
            var newSong = new Song() { Title = "Cool It Now", ArtistId = 6, GenreId = 7 };

            // Act
            var result1 = await _controller.Create(newSong);
            var redirectToRouteResult = (RedirectToRouteResult) result1;
            var result2 = await _controller.Index();
            var viewResult = (ViewResult)result2;
            var model = (List<Song>)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedTotalSongsAfterCreate, model.Count);
            Assert.AreEqual(expectedIdForNewSong, model.Find(s => s.Title == "Cool It Now" && s.ArtistId == 6).Id);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSameSongWhenModelStateIsNotValid()
        {
            // Arrange
            var newSong = new Song() { Title = "Cool It Now", ArtistId = 6, GenreId = 7 };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newSong);
            var viewResult = (ViewResult)result;
            var model = (Song)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newSong, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingSong()
        {
            // Arrange
            var existingSong = new Song() { Id = 1, Title = "Apache", ArtistId = 1, GenreId = 1 };

            // Act
            var result = await _controller.Edit(existingSong.Id);
            var viewResult = (ViewResult)result;
            var model = (Song)viewResult.Model;

            // Assert
            Assert.AreEqual(existingSong.Id, model.Id);
            Assert.AreEqual(existingSong.Title, model.Title);
            Assert.AreEqual(existingSong.ArtistId, model.ArtistId);
            Assert.AreEqual(existingSong.GenreId, model.GenreId);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentSong()
        {
            // Arrange
            var nonExistentSong = new Song() { Id = 99, Title = "Dueling Bongos", ArtistId = 1, GenreId = 1 };

            // Act
            var result = await _controller.Edit(nonExistentSong.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedSong()
        {
            // Arrange
            var existingSongWithModifiedTitle = new Song() { Id = 1, Title = "A-patchey", ArtistId = 1, GenreId = 1 };

            // Act
            var result1 = await _controller.Edit(existingSongWithModifiedTitle);
            var redirectToRouteResult = (RedirectToRouteResult)result1;
            var result2 = await _controller.Details(existingSongWithModifiedTitle.Id);
            var viewResult = (ViewResult)result2;
            var model = (Song)viewResult.Model;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingSongWithModifiedTitle.Id, model.Id);
            Assert.AreEqual(existingSongWithModifiedTitle.Title, model.Title);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSameSongWhenModelStateIsNotValid()
        {
            // Arrange
            var existingSong = new Song() { Id = 1, Title = "Apache", ArtistId = 1, GenreId = 1 };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingSong);
            var viewResult = (ViewResult)result;
            var model = (Song)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingSong, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingSong()
        {
            // Arrange
            var existingSong = new Song() { Id = 1, Title = "Apache", ArtistId = 1, GenreId = 1 };

            // Act
            var result = await _controller.Delete(existingSong.Id);
            var viewResult = (ViewResult)result;
            var model = (Song)viewResult.Model;

            // Assert
            Assert.AreEqual(existingSong.Id, model.Id);
            Assert.AreEqual(existingSong.Title, model.Title);
            Assert.AreEqual(existingSong.ArtistId, model.ArtistId);
            Assert.AreEqual(existingSong.GenreId, model.GenreId);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentSong()
        {
            // Arrange
            var nonExistentSong = new Song() { Id = 99, Title = "Dueling Bongos", ArtistId = 1, GenreId = 1 };

            // Act
            var result = await _controller.Delete(nonExistentSong.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingSong()
        {
            // Arrange
            const int expectedSongCountAfterDeletion = 18;
            const int existingSongId = 10;

            // Act
            var result = await _controller.DeleteConfirmed(existingSongId);
            var redirectToRouteResult = (RedirectToRouteResult)result;
            var result2 = await _controller.Details(existingSongId);
            var result3 = await _controller.Index();
            var viewResult = (ViewResult)result3;
            var model = (List<Song>)viewResult.Model;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.IsInstanceOf<HttpNotFoundResult>(result2);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(expectedSongCountAfterDeletion, model.Count);
        }
    }
}