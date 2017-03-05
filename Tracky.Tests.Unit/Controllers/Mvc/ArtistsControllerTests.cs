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
using Tracky.Domain.Services.EF.Music;

namespace Tracky.Tests.Unit.Controllers.Mvc
{
    [TestFixture]
    public class ArtistsControllerTests
    {
        private Mock<DbSet<Artist>> _mockSet;
        private Mock<LibraryContext> _mockContext;
        private Mock<ArtistService> _mockService;
        private ArtistsController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _mockSet = new Mock<DbSet<Artist>>();
            _mockContext = new Mock<LibraryContext>();
            _mockService = new Mock<ArtistService>();
            _mockService.Setup(s => s.ReturnSomethingDumb()).Returns(77);
        }

        [Test]
        public async Task IndexGet_ShouldGetAllArtistsOrderedByName()
        {
            // Arrange
            var data = new List<Artist>
            {
                new Artist() { Name = "Public Enemy" }, new Artist() { Name = "ZZ Top" }, new Artist() { Name = "James Brown" }
            }.AsQueryable();
            _mockSet.As<IDbAsyncEnumerable<Artist>>().Setup(s => s.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Artist>(data.GetEnumerator()));
            _mockSet.As<IQueryable<Artist>>().Setup(s => s.Provider).Returns(new TestDbAsyncQueryProvider<Artist>(data.Provider));
            _mockSet.As<IQueryable<Artist>>().Setup(s => s.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<Artist>>().Setup(s => s.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<Artist>>().Setup(s => s.GetEnumerator()).Returns(() => data.GetEnumerator());
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<Artist>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(data.ToList()[2].Name, model[0].Name);
            Assert.AreEqual(data.ToList()[0].Name, model[1].Name);
            Assert.AreEqual(data.ToList()[1].Name, model[2].Name);
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingArtistId()
        {
            // Arrange
            var existingArtist = new Artist() { Id = 1, Name = "Existing Artist" };
            _mockSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingArtist));
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            // Act
            var result = await _controller.Details(1);
            var viewResult = (ViewResult)result;
            var model = (Artist)viewResult.Model;

            // Assert
            _mockSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingArtist, model);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

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
            var nonExistentArtist = new Artist() { Id = 99, Name = "Updated Nonexistent Artist" };
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            // Act
            var result = await _controller.Delete(nonExistentArtist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public void CreateGet_ShouldReturnView()
        {
            // Arrange
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            // Act
            var result = _controller.Create();
            var viewResult = (ViewResult)result;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewArtist()
        {
            // Arrange
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            //Act
            var result = await _controller.Create(new Artist() { Name = "New Artist" });
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockSet.Verify(s => s.Add(It.IsAny<Artist>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSameArtistWhenModelStateIsNotValid()
        {
            // Arrange
            var newArtist = new Artist() { Name = "New Artist" };
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);
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
            var existingArtist = new Artist() { Id = 7, Name = "Existing Artist" };
            _mockSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingArtist));
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            // Act
            var result = await _controller.Edit(existingArtist.Id);
            var viewResult = (ViewResult)result;
            var model = (Artist)viewResult.Model;

            // Assert
            _mockSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingArtist, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            // Act
            var result = await _controller.Edit((int?)null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentArtist()
        {
            // Arrange
            var nonExistentArtist = new Artist() { Id = 99, Name = "Nonexistent Artist" };
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            // Act
            var result = await _controller.Edit(nonExistentArtist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedArtist()
        {
            // Arrange
            var updatedArtist = new Artist() { Id = 88, Name = "Updated Artist" };
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            //Act
            var result = await _controller.Edit(updatedArtist);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockContext.Verify(c => c.SetModified(It.IsAny<object>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSameArtistWhenModelStateIsNotValid()
        {
            // Arrange
            var existingArtist = new Artist() { Id = 57, Name = "Existing Artist" };
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);
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
            var existingArtist = new Artist() { Id = 57, Name = "Existing Artist" };
            _mockSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingArtist));
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            // Act
            var result = await _controller.Delete(existingArtist.Id);
            var viewResult = (ViewResult)result;
            var model = (Artist)viewResult.Model;

            // Assert
            _mockSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingArtist, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentArtist()
        {
            // Arrange
            var nonExistentArtist = new Artist() { Id = 99, Name = "Nonexistent Artist" };
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            // Act
            var result = await _controller.Delete(nonExistentArtist.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task DeleteConfirmedPost_ShouldDeleteExistingArtist()
        {
            // Arrange
            var existingArtist = new Artist() { Id = 57, Name = "Existing Artist" };
            _mockContext.Setup(c => c.Artists).Returns(_mockSet.Object);
            _controller = new ArtistsController(_mockContext.Object, _mockService.Object);

            //Act
            var result = await _controller.DeleteConfirmed(existingArtist.Id);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockSet.Verify(s => s.Remove(It.IsAny<Artist>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}
