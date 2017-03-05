using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using NUnit.Framework;
using Tracky.Controllers.Api;
using Tracky.Domain.EF.Books;
using Tracky.Domain.EF.DataContexts;
using Tracky.Domain.Services.EF.Books;

namespace Tracky.Tests.Integration.Controllers.Api
{
    [TestFixture]
    public class AuthorsControllerTests
    {
        private AuthorService _authorService;
        private AuthorsController _authorsController;

        [SetUp]
        public void SetUp()
        {
            Database.SetInitializer(new DatabaseInitializer());             // Set initializer for a fresh in-memory database for each test run
            var context = new LibraryContext();                             // Create data context
            context.Database.Initialize(true);                              // Initialize database on context

            _authorService = new AuthorService();                           // Initialize service dependency for controller (manually, as opposed to asking IoC container for it)
            _authorsController = new AuthorsController(_authorService);     // Initialize controller
        }

        [Test]
        public void ShouldReturnAllAuthors()
        {
            var result = _authorsController.GetAuthors();
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual("Jane Austen3", result.ToList()[0].Name);          // Evidence of service usage
        }

        /// <summary>
        /// EXAMPLE: An integration test against an asynchronous method - a controller POST method that creates
        /// a new Author object in the database, with the next Id value in the identity succession. In order to
        /// follow NUnit's implementation standards for asynchronous testing, the method (like many others in this
        /// test fixture) is marked async with return type Task, and employs the await keyword so that the test 
        /// awaits response from the asynchronous method in the same way that production code would.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ShouldFindExistingAuthor()
        {
            var existingAuthor = new Author() { Id = 1, Name = "Jane Austen" };

            IHttpActionResult actionResult = await _authorsController.GetAuthor(existingAuthor.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Author>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingAuthor.Name, contentResult.Content.Name);
        }

        [Test]
        public async Task ShouldReturnNotFoundForNonExistentAuthorGet()
        {
            var nonExistentAuthor = new Author() { Id = 111, Name = "Martin Fowler" };

            IHttpActionResult actionResult = await _authorsController.GetAuthor(nonExistentAuthor.Id);

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task ShouldUpdateAnExistingAuthorWhenUsingPut()
        {
            var modifiedAuthor = new Author() { Id = 4, Name = "Chuck Woolery" };
            var expectedAuthorCountAfterAdd = 4;

            await _authorsController.PutAuthor(modifiedAuthor.Id, modifiedAuthor);
            var authors = _authorsController.GetAuthors();

            Assert.AreEqual(modifiedAuthor.Name, authors.ToList().Last().Name);
            Assert.AreEqual(modifiedAuthor.Id, authors.ToList().Last().Id);
            Assert.AreEqual(expectedAuthorCountAfterAdd, authors.Count());
        }

        [Test]
        public async Task ShouldReturnBadRequestWhenIdsInParameterDataDoNotMatchWhenUsingPut()
        {
            var existingAuthor = new Author() { Id = 2, Name = "Charles Dickens" };
            var nonMatchingAuthorId = 12;

            IHttpActionResult actionResult = await _authorsController.PutAuthor(nonMatchingAuthorId, existingAuthor);

            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        public async Task ShouldReturnNotFoundWhenNonExistentAuthorUsedForPut()
        {
            var nonExistentAuthor = new Author() { Id = 22, Name = "Tim Heidecker" };

            IHttpActionResult actionResult = await _authorsController.PutAuthor(nonExistentAuthor.Id, nonExistentAuthor);

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task ShouldAddAnAuthorAsNextInSequenceWhenUsingPost()
        {
            var newAuthor = new Author() { Name = "George Carlin" };
            const int expectedIdForNewAuthor = 5;
            const int expectedAuthorCountAfterAdd = 5;

            IHttpActionResult actionResult = await _authorsController.PostAuthor(newAuthor);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Author>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(5, createdResult.RouteValues["id"]);

            var authors = _authorsController.GetAuthors();

            Assert.AreEqual(newAuthor.Name, authors.ToList().Last().Name);
            Assert.AreEqual(expectedIdForNewAuthor, authors.ToList().Last().Id);
            Assert.AreEqual(expectedAuthorCountAfterAdd, authors.Count());
        }

        [Test]
        public async Task ShouldDeleteAnExistingAuthor()
        {
            var existingAuthor = new Author() { Id = 1, Name = "Jane Austen" };
            var expectedAuthorCountAfterDelete = 3;

            IHttpActionResult actionResult = await _authorsController.DeleteAuthor(existingAuthor.Id);
            var contentResult = actionResult as OkNegotiatedContentResult<Author>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(existingAuthor.Name, contentResult.Content.Name);

            var authors = _authorsController.GetAuthors();

            Assert.AreEqual(expectedAuthorCountAfterDelete, authors.Count());
        }

        [Test]
        public async Task ShouldReturnNotFoundWhenNonExistentAuthorUsedForDelete()
        {
            var nonExistentAuthor = new Author() { Id = 23, Name = "Eric Wareheim" };

            IHttpActionResult actionResult = await _authorsController.DeleteAuthor(nonExistentAuthor.Id);

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}
