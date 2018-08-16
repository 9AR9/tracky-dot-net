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
    class ArtistRepositoryTests : NhDatabaseIntegrationTestBase
    {
        private IArtistRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new ArtistRepository(UnitOfWork);
        }

        [Test]
        public void ShouldAddNewArtistAndRetrieveItFromDatabase()
        {
            var artist = new Artist() { Name = "Funkadelic" };

            _repository.Save(artist);

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.Get(artist.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(artist));
            Assert.That(fromDb.Name, Is.EqualTo(artist.Name));
        }

        [Test]
        public void ShouldGetAllArtistsFromDatabase()
        {
            _repository.Save(new Artist() {Name = "A"});
            _repository.Save(new Artist() {Name = "B"});
            _repository.Save(new Artist() {Name = "C"});
            _repository.Save(new Artist() {Name = "D"});

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.GetAll();

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb.Count, Is.EqualTo(4));
        }

        [Test]
        public void ShouldDeleteArtistFromDatabase()
        {
            var artist = new Artist() { Name = "Little Dragon" };

            _repository.Save(artist);

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.Get(artist.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(artist));
            Assert.That(fromDb.Name, Is.EqualTo(artist.Name));

            UnitOfWork.FlushAndClear();
            _repository.Delete(artist);
            var fromDb2 = _repository.Get(artist.Id);

            Assert.That(fromDb2, Is.Null);
        }
    }
}
