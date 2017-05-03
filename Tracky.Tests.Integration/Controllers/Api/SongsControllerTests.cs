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
    class SongsControllerTests
    {
        private SongsController _controller;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new EFDatabaseInitializer());       // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                         // Create data context
            context.Database.Initialize(true);                          // Initialize database on context

            var container = IoC.Initialize();                           // Initialize the IoC container
            _controller = container.GetInstance<SongsController>();     // Ask container for a controller, built with all dependencies
        }

        [Test]
        public void GetSongs_ShouldGetAllSongsOrderedByArtistThenTitle()
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
            var songs = _controller.GetSongs().ToList();

            // Assert
            Assert.AreEqual(expectedTotalSongs, songs.Count);
            Assert.AreEqual(expectedSongTitleFirst, songs.First().Title);
            Assert.AreEqual(expectedArtistFirstAlphabetically, songs.First().Artist.Name);
            Assert.AreEqual(expectedSongTitleSecond, songs[1].Title);
            Assert.AreEqual(expectedArtistFirstAlphabetically, songs[1].Artist.Name);
            Assert.AreEqual(expectedSongTitleNextToLast, songs[songs.Count - 2].Title);
            Assert.AreEqual(expectedArtistNextToLastAlphabetically, songs[songs.Count - 2].Artist.Name);
            Assert.AreEqual(expectedSongTitleLast, songs.Last().Title);
            Assert.AreEqual(expectedArtistLastAlphabetically, songs.Last().Artist.Name);
        }

        [Test]
        public async Task GetSong_ShouldGetExistingSong()
        {
            // Arrange
            var existingSong = new Song() { Id = 1, Title = "Apache", ArtistId = 1, GenreId = 1 };

            // Act
            var actionResult = await _controller.GetSong(existingSong.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Song>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingSong.Title, contentResult.Content.Title);
            Assert.AreEqual(existingSong.ArtistId, contentResult.Content.ArtistId);
            Assert.AreEqual(existingSong.GenreId, contentResult.Content.GenreId);
        }

        [Test]
        public async Task GetSong_ShouldReturnNotFoundForNonExistentSong()
        {
            // Arrange
            var nonExistentSong = new Song() { Id = 99, Title = "The Song That Didn't Exist", ArtistId = 1, GenreId = 1 }; ;

            // Act
            var actionResult = await _controller.GetSong(nonExistentSong.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PutSong_ShouldUpdateAnExistingSong()
        {
            // Arrange
            var existingSongWithModifiedTitle = new Song() { Id = 1, Title = "A-patchey", ArtistId = 1, GenreId = 1 };
            const int expectedSongCountAfterPut = 19;

            // Act
            var actionResult = await _controller.PutSong(existingSongWithModifiedTitle.Id, existingSongWithModifiedTitle);
            var songs = _controller.GetSongs().ToList();

            // Assert
            Assert.IsInstanceOf<StatusCodeResult>(actionResult);
            Assert.AreEqual(HttpStatusCode.NoContent, ((StatusCodeResult)actionResult).StatusCode);
            Assert.AreEqual(expectedSongCountAfterPut, songs.Count);
            Assert.AreEqual(existingSongWithModifiedTitle.Title, songs.Find(a => a.Id == existingSongWithModifiedTitle.Id).Title);
        }

        [Test]
        public async Task PutSong_ShouldReturnBadRequestWhenIdsInParameterDataDoNotMatch()
        {
            // Arrange
            var existingSong = new Song() { Id = 1, Title = "Apache", ArtistId = 1, GenreId = 1 };
            const int nonMatchingSongId = 99;

            // Act
            var actionResult = await _controller.PutSong(nonMatchingSongId, existingSong);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public async Task PutSong_ShouldReturnNotFoundForNonExistentSong()
        {
            // Arrange
            var nonExistentSong = new Song() { Id = 99, Title = "The Song That Didn't Exist", ArtistId = 1, GenreId = 1 };

            // Act
            var actionResult = await _controller.PutSong(nonExistentSong.Id, nonExistentSong);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task PostSong_ShouldAddAnSongAsNextInSequence()
        {
            // Arrange
            var newSong = new Song() { Title = "Cool It Now", ArtistId = 6, GenreId = 7 };
            const int expectedIdForNewSong = 20;
            const int expectedSongCountAfterAdd = 20;

            // Act
            var actionResult = await _controller.PostSong(newSong);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Song>;
            var songs = _controller.GetSongs().ToList();

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(expectedIdForNewSong, createdResult.RouteValues["id"]);
            Assert.AreEqual(newSong.Title, songs.Find(s => s.Id == expectedIdForNewSong).Title);
            Assert.AreEqual(expectedSongCountAfterAdd, songs.Count);
        }

        [Test]
        public async Task PostSong_ShouldReturnBadRequestWhenModelStateIsInvalid()
        {
            // Arrange
            var newSong = new Song() { Title = "Cool It Now", ArtistId = 6, GenreId = 7 };
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var actionResult = await _controller.PostSong(newSong);

            // Assert
            Assert.IsInstanceOf<InvalidModelStateResult>(actionResult);
        }

        [Test]
        public async Task DeleteSong_ShouldDeleteAnExistingSong()
        {
            // Arrange
            var existingSong = new Song() { Id = 1, Title = "Apache", ArtistId = 1, GenreId = 1 };
            const int expectedSongCountAfterDelete = 18;

            // Act
            var actionResult = await _controller.DeleteSong(existingSong.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Song>;
            var songs = _controller.GetSongs().ToList();

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingSong.Id, contentResult.Content.Id);
            Assert.AreEqual(existingSong.Title, contentResult.Content.Title);
            Assert.AreEqual(existingSong.ArtistId, contentResult.Content.ArtistId);
            Assert.AreEqual(existingSong.GenreId, contentResult.Content.GenreId);
            Assert.AreEqual(expectedSongCountAfterDelete, songs.Count);
        }

        [Test]
        public async Task DeleteSong_ShouldReturnNotFoundForNonExistentSong()
        {
            // Arrange
            var nonExistentSong = new Song() { Id = 99, Title = "The Song That Didn't Exist", ArtistId = 1, GenreId = 1 };

            // Act
            var actionResult = await _controller.DeleteSong(nonExistentSong.Id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}
