using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
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
    class AlbumsControllerTests
    {
        private Mock<DbSet<Artist>> _mockArtistSet;
        private Mock<DbSet<Genre>> _mockGenreSet;
        private Mock<DbSet<Album>> _mockAlbumSet;
        private Mock<DbSet<AlbumTrack>> _mockAlbumTrackSet;
        private Mock<LibraryContext> _mockContext;
        private AlbumsController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _mockArtistSet = new Mock<DbSet<Artist>>();
            _mockAlbumSet = new Mock<DbSet<Album>>();
            _mockAlbumTrackSet = new Mock<DbSet<AlbumTrack>>();
            _mockGenreSet = new Mock<DbSet<Genre>>();
            _mockContext = new Mock<LibraryContext>();
        }

        [Test]
        public void IndexGet_ShouldGetAllAlbumsOrderedByTitle()
        {
            // SongControllerTests attempted mocking context for data with related entities, and it exposed mocking challenges for the Include method of the DbSet,
            // which returns null at runtime. Trying to mock Include feels painful and has not yeilded good results. Following the advice of others, these relational
            // data querying tests will be best handled only as integration tests.
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingAlbum()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 1, Title = "Existing Album", ArtistId = 1, GenreId = 1 };
            _mockAlbumSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingAlbum));
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumsController(_mockContext.Object);

            // Act
            var result = await _controller.Details(1);
            var viewResult = (ViewResult)result;
            var model = (Album)viewResult.Model;

            // Assert
            _mockAlbumSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbum, model);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new AlbumsController(_mockContext.Object);

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
            var nonExistentAlbum = new Album() { Id = 99, Title = "Updated Nonexistent Album", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentAlbum.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public void CreateGet_ShouldReturnView()
        {
            // Arrange
            _mockContext.Setup(c => c.Artists).Returns(_mockArtistSet.Object);
            _mockContext.Setup(c => c.Genres).Returns(_mockGenreSet.Object);
            _controller = new AlbumsController(_mockContext.Object);

            // Act
            var result = _controller.Create();
            var viewResult = (ViewResult)result;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.IsNotNull(viewResult.ViewBag.ArtistId);
            Assert.IsNotNull(viewResult.ViewBag.GenreId);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewAlbum()
        {
            // Arrange
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumsController(_mockContext.Object);

            //Act
            var result = await _controller.Create(new Album() { Title = "New Album", ArtistId = 1, GenreId = 1 });
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockAlbumSet.Verify(s => s.Add(It.IsAny<Album>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSameAlbumWhenModelStateIsNotValid()
        {
            // Arrange
            var newAlbum = new Album() { Title = "New Album", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Artists).Returns(_mockArtistSet.Object);
            _mockContext.Setup(c => c.Genres).Returns(_mockGenreSet.Object);
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumsController(_mockContext.Object);
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newAlbum);
            var viewResult = (ViewResult)result;
            var model = (Album)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newAlbum, model);
            Assert.IsNotNull(viewResult.ViewBag.ArtistId);
            Assert.IsNotNull(viewResult.ViewBag.GenreId);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingAlbum()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 7, Title = "Existing Album", ArtistId = 1, GenreId = 1 };
            _mockAlbumSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingAlbum));
            _mockContext.Setup(c => c.Artists).Returns(_mockArtistSet.Object);
            _mockContext.Setup(c => c.Genres).Returns(_mockGenreSet.Object);
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumsController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(existingAlbum.Id);
            var viewResult = (ViewResult)result;
            var model = (Album)viewResult.Model;

            // Assert
            _mockAlbumSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbum, model);
            Assert.IsNotNull(viewResult.ViewBag.ArtistId);
            Assert.IsNotNull(viewResult.ViewBag.GenreId);
        }

        [Test]
        public async Task EditGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new AlbumsController(_mockContext.Object);

            // Act
            var result = await _controller.Edit((int?)null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentAlbum()
        {
            // Arrange
            var nonExistentAlbum = new Album() { Id = 99, Title = "Nonexistent Album" };
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumsController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(nonExistentAlbum.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedAlbum()
        {
            // Arrange
            var updatedAlbum = new Album() { Id = 88, Title = "Updated Album", ArtistId = 1, GenreId = 1 };
            var existingAlbum = new Album() { Id = 88, Title = "Original Album", ArtistId = 1, GenreId = 1 };
            var data = new List<Album> { existingAlbum }.AsQueryable();
            _mockAlbumSet.As<IDbAsyncEnumerable<Album>>().Setup(s => s.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Album>(data.GetEnumerator()));
            _mockAlbumSet.As<IQueryable<Album>>().Setup(s => s.Provider).Returns(new TestDbAsyncQueryProvider<Album>(data.Provider));
            _mockAlbumSet.As<IQueryable<Album>>().Setup(s => s.Expression).Returns(data.Expression);
            _mockAlbumSet.As<IQueryable<Album>>().Setup(s => s.ElementType).Returns(data.ElementType);
            _mockAlbumSet.As<IQueryable<Album>>().Setup(s => s.GetEnumerator()).Returns(() => data.GetEnumerator());
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _mockContext.Setup(c => c.SetModified(It.IsAny<object>())).Verifiable();
            _controller = new AlbumsController(_mockContext.Object);

            //Act
            var result = await _controller.Edit(updatedAlbum);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockContext.Verify(c => c.SetModified(It.IsAny<object>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSameAlbumWhenModelStateIsNotValid()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 57, Title = "Existing Album", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Artists).Returns(_mockArtistSet.Object);
            _mockContext.Setup(c => c.Genres).Returns(_mockGenreSet.Object);
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumsController(_mockContext.Object);
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingAlbum);
            var viewResult = (ViewResult)result;
            var model = (Album)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbum, model);
            Assert.IsNotNull(viewResult.ViewBag.ArtistId);
            Assert.IsNotNull(viewResult.ViewBag.GenreId);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingAlbum()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 57, Title = "Existing Album", ArtistId = 1, GenreId = 1 };
            _mockAlbumSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingAlbum));
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(existingAlbum.Id);
            var viewResult = (ViewResult)result;
            var model = (Album)viewResult.Model;

            // Assert
            _mockAlbumSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbum, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentAlbum()
        {
            // Arrange
            var nonExistentAlbum = new Album() { Id = 99, Title = "Nonexistent Album", ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentAlbum.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingAlbumAndRelatedAlbumTracks()
        {
            // Arrange
            var existingAlbum = new Album() { Id = 57, Title = "Existing Album", ArtistId = 1, GenreId = 1 };
            var existingAlbumTrack = new AlbumTrack() { Id = 104, AlbumId = 57, AlbumTrackNumber = 1, SongId = 555 };
            var albumTrackData = new List<AlbumTrack> { existingAlbumTrack }.AsQueryable();
            _mockAlbumTrackSet.As<IDbAsyncEnumerable<AlbumTrack>>().Setup(s => s.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<AlbumTrack>(albumTrackData.GetEnumerator()));
            _mockAlbumTrackSet.As<IQueryable<AlbumTrack>>().Setup(s => s.Provider).Returns(new TestDbAsyncQueryProvider<AlbumTrack>(albumTrackData.Provider));
            _mockAlbumTrackSet.As<IQueryable<AlbumTrack>>().Setup(s => s.Expression).Returns(albumTrackData.Expression);
            _mockAlbumTrackSet.As<IQueryable<AlbumTrack>>().Setup(s => s.ElementType).Returns(albumTrackData.ElementType);
            _mockAlbumTrackSet.As<IQueryable<AlbumTrack>>().Setup(s => s.GetEnumerator()).Returns(() => albumTrackData.GetEnumerator());
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumsController(_mockContext.Object);

            //Act
            var result = await _controller.DeleteConfirmed(existingAlbum.Id);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockAlbumSet.Verify(s => s.Remove(It.IsAny<Album>()), Times.Once);
            _mockAlbumTrackSet.Verify(s => s.Remove(It.IsAny<AlbumTrack>()), Times.AtLeastOnce);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}
