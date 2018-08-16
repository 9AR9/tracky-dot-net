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
    class SongRepositoryTests : NhDatabaseIntegrationTestBase
    {
        private ISongRepository _repository;
        private IArtistRepository _artistRepository;
        private IGenreRepository _genreRepository;

        [SetUp]
        public void SetUp()
        {
            _repository = new SongRepository(UnitOfWork);
            _artistRepository = new ArtistRepository(UnitOfWork);
            _genreRepository = new GenreRepository(UnitOfWork);
        }

        [Test]
        public void ShouldAddNewSongAndRetrieveItFromDatabase()
        {
            var artist = new Artist() { Name = "Eddie Money" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Rock" };
            _genreRepository.Save(genre);
            UnitOfWork.FlushAndClear();

            var song = new Song() { Artist = artist, Genre = genre, Title = "Two Tickets To Paradise" };
            _repository.Save(song);
            UnitOfWork.FlushAndClear();

            var fromDb = _repository.Get(song.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(song));
            Assert.That(fromDb.Title, Is.EqualTo(song.Title));
            Assert.That(fromDb.ArtistId, Is.EqualTo(artist.Id));
            Assert.That(fromDb.GenreId, Is.EqualTo(genre.Id));
        }

        [Test]
        public void ShouldGetAllSongsFromDatabase()
        {
            var artist = new Artist() { Name = "Public Enemy" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Hip-Hop" };
            _genreRepository.Save(genre);
            UnitOfWork.FlushAndClear();

            _repository.Save(new Song() { Artist = artist, Genre = genre, Title = "You're Gonna Get Yours" });
            _repository.Save(new Song() { Artist = artist, Genre = genre, Title = "Public Enemy No. 1" });
            _repository.Save(new Song() { Artist = artist, Genre = genre, Title = "Bring The Noise" });
            _repository.Save(new Song() { Artist = artist, Genre = genre, Title = "Night Of The Living Baseheads" });

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.GetAll();

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb.Count, Is.EqualTo(4));
        }

        [Test]
        public void ShouldDeleteSongFromDatabaseButNotDeleteItsArtistOrGenre()
        {
            var artist = new Artist() { Name = "Public Enemy" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Hip-Hop" };
            _genreRepository.Save(genre);
            UnitOfWork.FlushAndClear();

            var song = new Song() { Artist = artist, Genre = genre, Title = "You're Gonna Get Yours" };
            _repository.Save(song);

            UnitOfWork.FlushAndClear();
            var songFromDb = _repository.Get(song.Id);

            Assert.That(songFromDb, Is.Not.Null);
            Assert.That(songFromDb, Is.Not.SameAs(song));
            Assert.That(songFromDb.Title, Is.EqualTo(song.Title));
            Assert.That(songFromDb.ArtistId, Is.EqualTo(artist.Id));
            Assert.That(songFromDb.GenreId, Is.EqualTo(genre.Id));

            UnitOfWork.FlushAndClear();
            _repository.Delete(song);
            var songFromDbAfterDelete = _repository.Get(song.Id);

            Assert.That(songFromDbAfterDelete, Is.Null);

            UnitOfWork.FlushAndClear();
            var artistFromDb = _artistRepository.Get(artist.Id);

            Assert.That(artistFromDb, Is.Not.Null);
            Assert.That(artistFromDb.Name, Is.EqualTo(artist.Name));

            var genreFromDb = _genreRepository.Get(genre.Id);

            Assert.That(genreFromDb, Is.Not.Null);
            Assert.That(genreFromDb.Name, Is.EqualTo(genre.Name));
        }
    }
}
