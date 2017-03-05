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
    class PlaylistTracksControllerTests
    {
        private Mock<DbSet<Song>> _mockSongSet;
        private Mock<DbSet<Playlist>> _mockPlaylistSet;
        private Mock<DbSet<PlaylistTrack>> _mockPlaylistTrackSet;
        private Mock<LibraryContext> _mockContext;
        private PlaylistTracksController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _mockSongSet = new Mock<DbSet<Song>>();
            _mockPlaylistSet = new Mock<DbSet<Playlist>>();
            _mockPlaylistTrackSet = new Mock<DbSet<PlaylistTrack>>();
            _mockContext = new Mock<LibraryContext>();
        }

        [Test]
        public void IndexGet_ShouldGetAllPlaylistTracksOrderedByPlaylistTrackNumber()
        {
            // SongControllerTests attempted mocking context for data with related entities, and it exposed mocking challenges for the Include method of the DbSet,
            // which returns null at runtime. Trying to mock Include feels painful and has not yeilded good results. Following the advice of others, these relational
            // data querying tests will be best handled only as integration tests.
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingPlaylistTrackId()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 1 };
            _mockPlaylistTrackSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingPlaylistTrack));
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Details(1);
            var viewResult = (ViewResult)result;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            _mockPlaylistTrackSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylistTrack, model);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new PlaylistTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnNotFoundResultForNonExistentPlaylistTrack()
        {
            // Arrange
            var nonExistentPlaylistTrack = new PlaylistTrack() { Id = 77, PlaylistId = 77, PlaylistTrackNumber = 77, SongId = 77 };
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentPlaylistTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public void CreateGet_ShouldReturnView()
        {
            // Arrange
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _mockContext.Setup(c => c.Playlists).Returns(_mockPlaylistSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            // Act
            var result = _controller.Create();
            var viewResult = (ViewResult)result;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.IsNotNull(viewResult.ViewBag.PlaylistId);
            Assert.IsNotNull(viewResult.ViewBag.SongId);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewPlaylistTrack()
        {
            // Arrange
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            //Act
            var result = await _controller.Create(new PlaylistTrack() { PlaylistId = 9, PlaylistTrackNumber = 9, SongId = 9 });
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockPlaylistTrackSet.Verify(s => s.Add(It.IsAny<PlaylistTrack>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSamePlaylistTrackWhenModelStateIsNotValid()
        {
            // Arrange
            var newPlaylistTrack = new PlaylistTrack() { Id = 9, PlaylistId = 9, PlaylistTrackNumber = 9, SongId = 9 };
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _mockContext.Setup(c => c.Playlists).Returns(_mockPlaylistSet.Object);
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newPlaylistTrack);
            var viewResult = (ViewResult)result;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newPlaylistTrack, model);
            Assert.IsNotNull(viewResult.ViewBag.PlaylistId);
            Assert.IsNotNull(viewResult.ViewBag.SongId);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingPlaylistTrack()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 7, PlaylistId = 7, PlaylistTrackNumber = 7, SongId = 7 };
            _mockPlaylistTrackSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingPlaylistTrack));
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _mockContext.Setup(c => c.Playlists).Returns(_mockPlaylistSet.Object);
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(existingPlaylistTrack.Id);
            var viewResult = (ViewResult)result;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            _mockPlaylistTrackSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylistTrack, model);
            Assert.IsNotNull(viewResult.ViewBag.PlaylistId);
            Assert.IsNotNull(viewResult.ViewBag.SongId);
        }

        [Test]
        public async Task EditGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new PlaylistTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Edit((int?)null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentPlaylistTrack()
        {
            // Arrange
            var nonExistentPlaylistTrack = new PlaylistTrack() { Id = 77, PlaylistId = 77, PlaylistTrackNumber = 77, SongId = 77 };
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(nonExistentPlaylistTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedPlaylistTrack()
        {
            // Arrange
            var updatedPlaylistTrack = new PlaylistTrack() { Id = 7, PlaylistId = 7, PlaylistTrackNumber = 7, SongId = 8 };
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            //Act
            var result = await _controller.Edit(updatedPlaylistTrack);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockContext.Verify(c => c.SetModified(It.IsAny<object>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSamePlaylistTrackWhenModelStateIsNotValid()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 7, PlaylistId = 7, PlaylistTrackNumber = 7, SongId = 7 };
            _mockContext.Setup(c => c.Songs).Returns(_mockSongSet.Object);
            _mockContext.Setup(c => c.Playlists).Returns(_mockPlaylistSet.Object);
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingPlaylistTrack);
            var viewResult = (ViewResult)result;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylistTrack, model);
            Assert.IsNotNull(viewResult.ViewBag.PlaylistId);
            Assert.IsNotNull(viewResult.ViewBag.SongId);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingPlaylistTrack()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 7, PlaylistId = 7, PlaylistTrackNumber = 7, SongId = 7 };
            _mockPlaylistTrackSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingPlaylistTrack));
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(existingPlaylistTrack.Id);
            var viewResult = (ViewResult)result;
            var model = (PlaylistTrack)viewResult.Model;

            // Assert
            _mockPlaylistTrackSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylistTrack, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentPlaylistTrack()
        {
            // Arrange
            var nonExistentPlaylistTrack = new PlaylistTrack() { Id = 77, PlaylistId = 77, PlaylistTrackNumber = 77, SongId = 77 };
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentPlaylistTrack.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingPlaylistTrack()
        {
            // Arrange
            var existingPlaylistTrack = new PlaylistTrack() { Id = 77, PlaylistId = 77, PlaylistTrackNumber = 77, SongId = 77 };
            _mockContext.Setup(c => c.PlaylistTracks).Returns(_mockPlaylistTrackSet.Object);
            _controller = new PlaylistTracksController(_mockContext.Object);

            //Act
            var result = await _controller.DeleteConfirmed(existingPlaylistTrack.Id);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockPlaylistTrackSet.Verify(s => s.Remove(It.IsAny<PlaylistTrack>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}
