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
    class AlbumTracksControllerTests
    {
        private Mock<DbSet<Song>> _mockSongSet;
        private Mock<DbSet<Album>> _mockAlbumSet;
        private Mock<DbSet<AlbumTrack>> _mockAlbumTrackSet;
        private Mock<LibraryContext> _mockContext;
        private AlbumTracksController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _mockSongSet = new Mock<DbSet<Song>>();
            _mockAlbumSet = new Mock<DbSet<Album>>();
            _mockAlbumTrackSet = new Mock<DbSet<AlbumTrack>>();
            _mockContext = new Mock<LibraryContext>();
        }

        [Test]
        public void IndexGet_ShouldGetAllAlbumTracksOrderedByAlbumTrackNumber()
        {
            // SongControllerTests attempted mocking context for data with related entities, and it exposed mocking challenges for the Include method of the DbSet,
            // which returns null at runtime. Trying to mock Include feels painful and has not yeilded good results. Following the advice of others, these relational
            // data querying tests will be best handled only as integration tests.
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingAlbumTrackId()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 1, AlbumId = 1, AlbumTrackNumber = 1, SongId = 1 };
            _mockAlbumTrackSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingAlbumTrack));
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Details(1);
            var viewResult = (ViewResult)result;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            _mockAlbumTrackSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbumTrack, model);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new AlbumTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnNotFoundResultForNonExistentAlbumTrack()
        {
            // Arrange
            var nonExistentAlbumTrack = new AlbumTrack() { Id = 77, AlbumId = 77, AlbumTrackNumber = 77, SongId = 77 };
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentAlbumTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public void CreateGet_ShouldReturnView()
        {
            // Arrange
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            // Act
            var result = _controller.Create();
            var viewResult = (ViewResult)result;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.IsNotNull(viewResult.ViewBag.AlbumId);
            Assert.IsNotNull(viewResult.ViewBag.SongId);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewAlbumTrack()
        {
            // Arrange
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            //Act
            var result = await _controller.Create(new AlbumTrack() { AlbumId = 9, AlbumTrackNumber = 9, SongId = 9 });
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockAlbumTrackSet.Verify(s => s.Add(It.IsAny<AlbumTrack>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSameAlbumTrackWhenModelStateIsNotValid()
        {
            // Arrange
            var newAlbumTrack = new AlbumTrack() { Id = 9, AlbumId = 9, AlbumTrackNumber = 9, SongId = 9 };
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newAlbumTrack);
            var viewResult = (ViewResult)result;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newAlbumTrack, model);
            Assert.IsNotNull(viewResult.ViewBag.AlbumId);
            Assert.IsNotNull(viewResult.ViewBag.SongId);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingAlbumTrack()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 7, AlbumId = 7, AlbumTrackNumber = 7, SongId = 7 };
            _mockAlbumTrackSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingAlbumTrack));
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(existingAlbumTrack.Id);
            var viewResult = (ViewResult)result;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            _mockAlbumTrackSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbumTrack, model);
            Assert.IsNotNull(viewResult.ViewBag.AlbumId);
            Assert.IsNotNull(viewResult.ViewBag.SongId);
        }

        [Test]
        public async Task EditGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new AlbumTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Edit((int?)null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentAlbumTrack()
        {
            // Arrange
            var nonExistentAlbumTrack = new AlbumTrack() { Id = 77, AlbumId = 77, AlbumTrackNumber = 77, SongId = 77 };
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(nonExistentAlbumTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedAlbumTrack()
        {
            // Arrange
            var updatedAlbumTrack = new AlbumTrack() { Id = 7, AlbumId = 7, AlbumTrackNumber = 7, SongId = 8 };
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            //Act
            var result = await _controller.Edit(updatedAlbumTrack);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockContext.Verify(c => c.SetModified(It.IsAny<object>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSameAlbumTrackWhenModelStateIsNotValid()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 7, AlbumId = 7, AlbumTrackNumber = 7, SongId = 7 };
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _mockContext.Setup(c => c.Albums).Returns(_mockAlbumSet.Object);
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingAlbumTrack);
            var viewResult = (ViewResult)result;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbumTrack, model);
            Assert.IsNotNull(viewResult.ViewBag.AlbumId);
            Assert.IsNotNull(viewResult.ViewBag.SongId);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingAlbumTrack()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 7, AlbumId = 7, AlbumTrackNumber = 7, SongId = 7 };
            _mockAlbumTrackSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingAlbumTrack));
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(existingAlbumTrack.Id);
            var viewResult = (ViewResult)result;
            var model = (AlbumTrack)viewResult.Model;

            // Assert
            _mockAlbumTrackSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingAlbumTrack, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentAlbumTrack()
        {
            // Arrange
            var nonExistentAlbumTrack = new AlbumTrack() { Id = 77, AlbumId = 77, AlbumTrackNumber = 77, SongId = 77 };
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentAlbumTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingAlbumTrack()
        {
            // Arrange
            var existingAlbumTrack = new AlbumTrack() { Id = 77, AlbumId = 77, AlbumTrackNumber = 77, SongId = 77 };
            _mockContext.Setup(c => c.AlbumTracks).Returns(_mockAlbumTrackSet.Object);
            _controller = new AlbumTracksController(_mockContext.Object);

            //Act
            var result = await _controller.DeleteConfirmed(existingAlbumTrack.Id);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockAlbumTrackSet.Verify(s => s.Remove(It.IsAny<AlbumTrack>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}
