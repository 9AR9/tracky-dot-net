using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories;
using Tracky.Domain.Repositories.NH;

namespace Tracky.Tests.Integration.Repositories.NH
{
    [TestFixture]
    class GenreRepositoryTests : NhDatabaseIntegrationTestBase
    {
        private IGenreRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new GenreRepository(UnitOfWork);
        }

        [Test]
        public void ShouldAddNewGenreAndRetrieveItFromDatabase()
        {
            var genre = new Genre() { Name = "Rock" };

            _repository.Save(genre);

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.Get(genre.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(genre));
            Assert.That(fromDb.Name, Is.EqualTo(genre.Name));
        }

        [Test]
        public void ShouldGetAllGenresFromDatabase()
        {
            _repository.Save(new Genre() {Name = "A"});
            _repository.Save(new Genre() {Name = "B"});
            _repository.Save(new Genre() {Name = "C"});
            _repository.Save(new Genre() {Name = "D"});

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.GetAll();

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb.Count, Is.EqualTo(4));
        }

        [Test]
        public void ShouldDeleteGenreFromDatabase()
        {
            var genre = new Genre() { Name = "Ska" };

            _repository.Save(genre);

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.Get(genre.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(genre));
            Assert.That(fromDb.Name, Is.EqualTo(genre.Name));

            UnitOfWork.FlushAndClear();
            _repository.Delete(genre);
            var fromDb2 = _repository.Get(genre.Id);

            Assert.That(fromDb2, Is.Null);
        }
    }
}
