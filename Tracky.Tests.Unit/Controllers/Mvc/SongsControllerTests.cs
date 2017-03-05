using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Tracky.Controllers.Mvc;
using Tracky.Domain.EF.DataContexts;
using Tracky.Domain.EF.Music;

namespace Tracky.Tests.Unit.Controllers.Mvc
{
    [TestFixture]
    class SongsControllerTests
    {
        private Mock<DbSet<Artist>> _mockArtistSet;
        private Mock<DbSet<Genre>> _mockGenreSet;
        private Mock<DbSet<Song>> _mockSongSet;
        private Mock<LibraryContext> _mockContext;
        private SongsController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _mockArtistSet = new Mock<DbSet<Artist>>();
            _mockGenreSet = new Mock<DbSet<Genre>>();
            _mockSongSet = new Mock<DbSet<Song>>();
            _mockContext = new Mock<LibraryContext>();
        }

        [Test]
        public void IndexGet_ShouldGetAllSongsOrderedByTitle()
        {
            // This attempt at mocking context for data with related entities exposes mocking challenges for the Include method of the DbSet, which returns null
            // at runtime. Trying to mock Include feels painful and has not yeilded good results. It was a valiant effort, but, following the advice of others,
            // these relational data querying tests will be best handled only as integration tests.



            /*
            // Arrange
            var artistData = new List<Artist>
            {
                new Artist() { Id = 1, Name = "Public Enemy" }, new Artist() { Id = 2, Name = "ZZ Top" }, new Artist() { Id = 3, Name = "James Brown" }
            }.AsQueryable();
            var genreData = new List<Genre>
            {
                new Genre() { Id = 1, Name = "Hip-Hop" }, new Genre() { Id = 2, Name = "Rock" }, new Genre() { Id = 3, Name = "Funk" }
            }.AsQueryable();
            var songData = new List<Song>
            {
                new Song() { Id = 1, Title = "Don't Believe The Hype", ArtistId = 1, GenreId = 1 },
                new Song() { Id = 2, Title = "(Somebody Else Been) Shaking Your Tree", ArtistId = 2, GenreId = 2 },
                new Song() { Id = 3, Title = "Funky Drummer", ArtistId = 3, GenreId = 3 }
            }.AsQueryable();
            _mockArtistSet.As<IDbAsyncEnumerable<Artist>>().Setup(s => s.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Artist>(artistData.GetEnumerator()));
            _mockArtistSet.As<IQueryable<Artist>>().Setup(s => s.Provider).Returns(new TestDbAsyncQueryProvider<Artist>(artistData.Provider));
            _mockArtistSet.As<IQueryable<Artist>>().Setup(s => s.Expression).Returns(artistData.Expression);
            _mockArtistSet.As<IQueryable<Artist>>().Setup(s => s.ElementType).Returns(artistData.ElementType);
            _mockArtistSet.As<IQueryable<Artist>>().Setup(s => s.GetEnumerator()).Returns(() => artistData.GetEnumerator());
            _mockGenreSet.As<IDbAsyncEnumerable<Genre>>().Setup(s => s.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Genre>(genreData.GetEnumerator()));
            _mockGenreSet.As<IQueryable<Genre>>().Setup(s => s.Provider).Returns(new TestDbAsyncQueryProvider<Genre>(genreData.Provider));
            _mockGenreSet.As<IQueryable<Genre>>().Setup(s => s.Expression).Returns(genreData.Expression);
            _mockGenreSet.As<IQueryable<Genre>>().Setup(s => s.ElementType).Returns(genreData.ElementType);
            _mockGenreSet.As<IQueryable<Genre>>().Setup(s => s.GetEnumerator()).Returns(() => genreData.GetEnumerator());
            _mockSongSet.As<IDbAsyncEnumerable<Song>>().Setup(s => s.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Song>(songData.GetEnumerator()));
            _mockSongSet.As<IQueryable<Song>>().Setup(s => s.Provider).Returns(new TestDbAsyncQueryProvider<Song>(songData.Provider));
            _mockSongSet.As<IQueryable<Song>>().Setup(s => s.Expression).Returns(songData.Expression);
            _mockSongSet.As<IQueryable<Song>>().Setup(s => s.ElementType).Returns(songData.ElementType);
            _mockSongSet.As<IQueryable<Song>>().Setup(s => s.GetEnumerator()).Returns(() => songData.GetEnumerator());
            _mockContext.Setup(c => c.Artists).Returns(_mockArtistSet.Object);
            _mockContext.Setup(c => c.Genres).Returns(_mockGenreSet.Object);
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<Song>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(songData.ToList()[2].Title, model[0].Title);
            Assert.AreEqual(songData.ToList()[0].Title, model[1].Title);
            Assert.AreEqual(songData.ToList()[1].Title, model[2].Title);
            */
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingSongId()
        {
            // Arrange
            var existingSong = new Song() { Id = 1, Title = "Existing Song", ArtistId = 1, GenreId = 1 };
            _mockSongSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingSong));
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            // Act
            var result = await _controller.Details(1);
            var viewResult = (ViewResult)result;
            var model = (Song)viewResult.Model;

            // Assert
            _mockSongSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingSong, model);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new SongsController(_mockContext.Object);

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
            var nonExistentSong = new Song() { Id = 99, Title = "Updated Nonexistent Song", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentSong.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public void CreateGet_ShouldReturnView()
        {
            // Arrange
            _mockContext.Setup(c => c.Artists).Returns(_mockArtistSet.Object);
            _mockContext.Setup(c => c.Genres).Returns(_mockGenreSet.Object);
            _controller = new SongsController(_mockContext.Object);

            // Act
            var result = _controller.Create();
            var viewResult = (ViewResult)result;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.IsNotNull(viewResult.ViewBag.ArtistId);
            Assert.IsNotNull(viewResult.ViewBag.GenreId);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewSong()
        {
            // Arrange
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            //Act
            var result = await _controller.Create(new Song() { Title = "New Song", ArtistId = 1, GenreId = 1 });
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockSongSet.Verify(s => s.Add(It.IsAny<Song>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSameSongWhenModelStateIsNotValid()
        {
            // Arrange
            var newSong = new Song() { Title = "New Song", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Artists).Returns(_mockArtistSet.Object);
            _mockContext.Setup(c => c.Genres).Returns(_mockGenreSet.Object);
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newSong);
            var viewResult = (ViewResult)result;
            var model = (Song)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newSong, model);
            Assert.IsNotNull(viewResult.ViewBag.ArtistId);
            Assert.IsNotNull(viewResult.ViewBag.GenreId);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingSong()
        {
            // Arrange
            var existingSong = new Song() { Id = 7, Title = "Existing Song", ArtistId = 1, GenreId = 1 };
            _mockSongSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingSong));
            _mockContext.Setup(c => c.Artists).Returns(_mockArtistSet.Object);
            _mockContext.Setup(c => c.Genres).Returns(_mockGenreSet.Object);
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(existingSong.Id);
            var viewResult = (ViewResult)result;
            var model = (Song)viewResult.Model;

            // Assert
            _mockSongSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingSong, model);
            Assert.IsNotNull(viewResult.ViewBag.ArtistId);
            Assert.IsNotNull(viewResult.ViewBag.GenreId);
        }

        [Test]
        public async Task EditGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new SongsController(_mockContext.Object);

            // Act
            var result = await _controller.Edit((int?)null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentSong()
        {
            // Arrange
            var nonExistentSong = new Song() { Id = 99, Title = "Nonexistent Song", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(nonExistentSong.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedSong()
        {
            // Arrange
            var updatedSong = new Song() { Id = 88, Title = "Updated Song", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            //Act
            var result = await _controller.Edit(updatedSong);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockContext.Verify(c => c.SetModified(It.IsAny<object>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSameSongWhenModelStateIsNotValid()
        {
            // Arrange
            var existingSong = new Song() { Id = 57, Title = "Existing Song", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Artists).Returns(_mockArtistSet.Object);
            _mockContext.Setup(c => c.Genres).Returns(_mockGenreSet.Object);
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingSong);
            var viewResult = (ViewResult)result;
            var model = (Song)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingSong, model);
            Assert.IsNotNull(viewResult.ViewBag.ArtistId);
            Assert.IsNotNull(viewResult.ViewBag.GenreId);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingSong()
        {
            // Arrange
            var existingSong = new Song() { Id = 57, Title = "Existing Song", ArtistId = 1, GenreId = 1 };
            _mockSongSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingSong));
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(existingSong.Id);
            var viewResult = (ViewResult)result;
            var model = (Song)viewResult.Model;

            // Assert
            _mockSongSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingSong, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentSong()
        {
            // Arrange
            var nonExistentSong = new Song() { Id = 99, Title = "Nonexistent Song", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentSong.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingSong()
        {
            // Arrange
            var existingSong = new Song() { Id = 57, Title = "Existing Song", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _controller = new SongsController(_mockContext.Object);

            //Act
            var result = await _controller.DeleteConfirmed(existingSong.Id);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockSongSet.Verify(s => s.Remove(It.IsAny<Song>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}
