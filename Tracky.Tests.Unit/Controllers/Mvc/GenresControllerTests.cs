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
    public class GenresControllerTests
    {
        private Mock<DbSet<Genre>> _mockSet;
        private Mock<LibraryContext> _mockContext;
        private GenresController _controller;
        private static readonly string ConventionallyEmptyViewName = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _mockSet = new Mock<DbSet<Genre>>();
            _mockContext = new Mock<LibraryContext>();
        }

        [Test]
        public async Task IndexGet_ShouldGetAllGenresOrderedByName()
        {
            // Arrange
            var data = new List<Genre>
            {
                new Genre() { Name = "Hip-Hop" }, new Genre() { Name = "Ska" }, new Genre() { Name = "Calypso" }
            }.AsQueryable();
            _mockSet.As<IDbAsyncEnumerable<Genre>>().Setup(s => s.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Genre>(data.GetEnumerator()));
            _mockSet.As<IQueryable<Genre>>().Setup(s => s.Provider).Returns(new TestDbAsyncQueryProvider<Genre>(data.Provider));
            _mockSet.As<IQueryable<Genre>>().Setup(s => s.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<Genre>>().Setup(s => s.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<Genre>>().Setup(s => s.GetEnumerator()).Returns(() => data.GetEnumerator());
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            // Act
            var result = await _controller.Index();
            var viewResult = (ViewResult)result;
            var model = (List<Genre>)viewResult.Model;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(data.ToList()[2].Name, model[0].Name);
            Assert.AreEqual(data.ToList()[0].Name, model[1].Name);
            Assert.AreEqual(data.ToList()[1].Name, model[2].Name);
        }

        [Test]
        public async Task DetailsGet_ShouldGetDetailsForExistingGenreId()
        {
            // Arrange
            var existingGenre = new Genre() { Id = 1, Name = "Existing Genre" };
            _mockSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingGenre));
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            // Act
            var result = await _controller.Details(1);
            var viewResult = (ViewResult)result;
            var model = (Genre)viewResult.Model;

            // Assert
            _mockSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingGenre, model);
        }

        [Test]
        public async Task DetailsGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new GenresController(_mockContext.Object);

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
            var nonExistentGenre = new Genre() { Id = 99, Name = "Updated Nonexistent Genre" };
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentGenre.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public void CreateGet_ShouldReturnView()
        {
            // Arrange
            _controller = new GenresController(_mockContext.Object);

            // Act
            var result = _controller.Create();
            var viewResult = (ViewResult)result;

            // Assert
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
        }

        [Test]
        public async Task CreatePost_ShouldAddNewGenre()
        {
            // Arrange
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            //Act
            var result = await _controller.Create(new Genre() { Name = "New Genre" });
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockSet.Verify(s => s.Add(It.IsAny<Genre>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task CreatePost_ShouldOnlyReturnViewWithSameGenreWhenModelStateIsNotValid()
        {
            // Arrange
            var newGenre = new Genre() { Name = "New Genre" };
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);
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
            var existingGenre = new Genre() { Id = 7, Name = "Existing Genre" };
            _mockSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingGenre));
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(existingGenre.Id);
            var viewResult = (ViewResult)result;
            var model = (Genre)viewResult.Model;

            // Assert
            _mockSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingGenre, model);
        }

        [Test]
        public async Task EditGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {
            // Arrange
            _controller = new GenresController(_mockContext.Object);

            // Act
            var result = await _controller.Edit((int?)null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task EditGet_ShouldReturnNotFoundResultForNonExistentGenre()
        {
            // Arrange
            var nonExistentGenre = new Genre() { Id = 99, Name = "Nonexistent Genre" };
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(nonExistentGenre.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_ShouldSaveChangesToModifiedGenre()
        {
            // Arrange
            var updatedGenre = new Genre() { Id = 88, Name = "Updated Genre" };
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            //Act
            var result = await _controller.Edit(updatedGenre);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockContext.Verify(c => c.SetModified(It.IsAny<object>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public async Task EditPost_ShouldOnlyReturnViewWithSameGenreWhenModelStateIsNotValid()
        {
            // Arrange
            var existingGenre = new Genre() { Id = 57, Name = "Existing Genre" };
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);
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
            var existingGenre = new Genre() { Id = 57, Name = "Existing Genre" };
            _mockSet.Setup(s => s.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(existingGenre));
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(existingGenre.Id);
            var viewResult = (ViewResult)result;
            var model = (Genre)viewResult.Model;

            // Assert
            _mockSet.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(ConventionallyEmptyViewName, viewResult.ViewName);
            Assert.AreEqual(existingGenre, model);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnNotFoundResultForNonExistentGenre()
        {
            // Arrange
            var nonExistentGenre = new Genre() { Id = 99, Name = "Nonexistent Genre" };
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(nonExistentGenre.Id);

            // Assert
            Assert.IsInstanceOf<HttpNotFoundResult>(result);
        }

        [Test]
        public async Task DeleteGet_ShouldReturnBadRequestWhenCalledWithNullId()
        {   
            // Arrange
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsInstanceOf<HttpStatusCodeResult>(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public async Task  DeleteConfirmedPost_ShouldDeleteExistingGenre()
        {
            // Arrange
            var existingGenre = new Genre() { Id = 57, Name = "Existing Genre" };
            _mockContext.Setup(c => c.Genres).Returns(_mockSet.Object);
            _controller = new GenresController(_mockContext.Object);

            //Act
            var result = await _controller.DeleteConfirmed(existingGenre.Id);
            var redirectToRouteResult = (RedirectToRouteResult)result;

            //Assert
            _mockSet.Verify(s => s.Remove(It.IsAny<Genre>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}
