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
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;

namespace Tracky.Tests.Unit.Controllers.Mvc
{
    [TestFixture]
    class PlaylistsControllerTests
    {
        private Mock<DbSet<Playlist>> _mockSet;
        private Mock<LibraryContext> _mockContext;
        private PlaylistsController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _mockSet = new Mock<DbSet<Playlist>>();
            _mockContext = new Mock<LibraryContext>();
        }

        [Test]
        public async Task IndexGet_ShouldGetAllPlaylistsOrderedByName()
        {
            // Arrange
            var data = new List<Playlist>
            {
                new Playlist() { Name = "Wedding: Kerouac 3/10/2099" }, new Playlist() { Name = "Wedding: Lebowski 3/9/2099" }, new Playlist() { Name = "Funk Dance Party Master" }
            }.AsQueryable();
            _mockSet.As<IDbAsyncEnumerable<Playlist>>().Setup(s => s.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Playlist>(data.GetEnumerator()));
            _mockSet.As<IQueryable<Playlist>>().Setup(s => s.Provider).Returns(new TestDbAsyncQueryProvider<Playlist>(data.Provider));
            _mockSet.As<IQueryable<Playlist>>().Setup(s => s.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<Playlist>>().Setup(s => s.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<Playlist>>().Setup(s => s.GetEnumerator()).Returns(() => data.GetEnumerator());
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<Playlist>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(data.ToList()[2].Name, model[0].Name);
            Assert.AreEqual(data.ToList()[0].Name, model[1].Name);
            Assert.AreEqual(data.ToList()[1].Name, model[2].Name);
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingPlaylistId()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 1, Name = "Existing Playlist" };
            _mockSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingPlaylist));
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = await _controller.Details(1);
            var viewResult = (ViewResult)result;
            var model = (Playlist)viewResult.Model;

            // Assert
            _mockSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylist, model);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnNotFoundResultForNonExistentPlaylist()
        {
            // Arrange
            var nonExistentPlaylist = new Playlist() { Id = 99, Name = "Updated Nonexistent Playlist" };
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentPlaylist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public void CreateGet_ShouldReturnView()
        {
            // Arrange
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = _controller.Create();
            var viewResult = (ViewResult)result;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewPlaylist()
        {
            // Arrange
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            //Act
            var result = await _controller.Create(new Playlist() { Name = "New Playlist" });
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockSet.Verify(s => s.Add(It.IsAny<Playlist>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSamePlaylistWhenModelStateIsNotValid()
        {
            // Arrange
            var newPlaylist = new Playlist() { Name = "New Playlist" };
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Create(newPlaylist);
            var viewResult = (ViewResult)result;
            var model = (Playlist)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(newPlaylist, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnExistingPlaylist()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 7, Name = "Existing Playlist" };
            _mockSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingPlaylist));
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(existingPlaylist.Id);
            var viewResult = (ViewResult)result;
            var model = (Playlist)viewResult.Model;

            // Assert
            _mockSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylist, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = await _controller.Edit((int?)null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentPlaylist()
        {
            // Arrange
            var nonExistentPlaylist = new Playlist() { Id = 99, Name = "Nonexistent Playlist" };
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(nonExistentPlaylist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedPlaylist()
        {
            // Arrange
            var updatedPlaylist = new Playlist() { Id = 88, Name = "Updated Playlist" };
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            //Act
            var result = await _controller.Edit(updatedPlaylist);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockContext.Verify(c => c.SetModified(It.IsAny<object>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSamePlaylistWhenModelStateIsNotValid()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 57, Name = "Existing Playlist" };
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = await _controller.Edit(existingPlaylist);
            var viewResult = (ViewResult)result;
            var model = (Playlist)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylist, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnExistingPlaylist()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 57, Name = "Existing Playlist" };
            _mockSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingPlaylist));
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(existingPlaylist.Id);
            var viewResult = (ViewResult)result;
            var model = (Playlist)viewResult.Model;

            // Assert
            _mockSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingPlaylist, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentPlaylist()
        {
            // Arrange
            var nonExistentPlaylist = new Playlist() { Id = 99, Name = "Nonexistent Playlist" };
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentPlaylist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingPlaylist()
        {
            // Arrange
            var existingPlaylist = new Playlist() { Id = 57, Name = "Existing Playlist" };
            _mockContext.Setup(c => c.Playlists).Returns(_mockSet.Object);
            _controller = new PlaylistsController(_mockContext.Object);

            //Act
            var result = await _controller.DeleteConfirmed(existingPlaylist.Id);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockSet.Verify(s => s.Remove(It.IsAny<Playlist>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}
