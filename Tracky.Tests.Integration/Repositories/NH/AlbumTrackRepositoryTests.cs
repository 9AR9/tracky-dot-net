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
    class AlbumTrackRepositoryTests : NHDatabaseIntegrationTestBase
    {
        private IAlbumTrackRepository _repository;
        private IArtistRepository _artistRepository;
        private IGenreRepository _genreRepository;
        private ISongRepository _songRepository;
        private IAlbumRepository _albumRepository;

        [SetUp]
        public void SetUp()
        {
            _repository = new AlbumTrackRepository(UnitOfWork);
            _artistRepository = new ArtistRepository(UnitOfWork);
            _genreRepository = new GenreRepository(UnitOfWork);
            _songRepository = new SongRepository(UnitOfWork);
            _albumRepository = new AlbumRepository(UnitOfWork);
        }

        [Test]
        public void ShouldAddNewAlbumTrackAndRetrieveItFromDatabase()
        {
            var artist = new Artist() { Name = "Eddie Money" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Rock" };
            _genreRepository.Save(genre);
            var album = new Album() { Artist = artist, Genre = genre, Label = "Columbia", Title = "Can't Hold Back", Year = 1986 };
            _albumRepository.Save(album);
            var song = new Song() { Artist = artist, Genre = genre, Title = "Take Me Home Tonight" };
            _songRepository.Save(song);
            UnitOfWork.FlushAndClear();

            var albumTrack = new AlbumTrack() { Album = album, AlbumTrackNumber = 1, Song = song };
            _repository.Save(albumTrack);
            UnitOfWork.FlushAndClear();

            var fromDb = _repository.Get(albumTrack.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(albumTrack));
            Assert.That(fromDb.AlbumId, Is.EqualTo(album.Id));
            Assert.That(fromDb.SongId, Is.EqualTo(song.Id));
            Assert.That(fromDb.AlbumTrackNumber, Is.EqualTo(albumTrack.AlbumTrackNumber));
        }

        [Test]
        public void ShouldGetAllAlbumTracksFromDatabase()
        {
            var artist = new Artist() { Name = "Public Enemy" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Hip-Hop" };
            _genreRepository.Save(genre);
            var album = new Album() { Artist = artist, Genre = genre, Label = "Def Jam", Title = "Apocalypse '91...The Enemy Strikes Black", Year = 1991 };
            _albumRepository.Save(album);
            var song1 = new Song() { Artist = artist, Genre = genre, Title = "Lost At Birth" };
            _songRepository.Save(song1);
            var song2 = new Song() { Artist = artist, Genre = genre, Title = "Rebirth" };
            _songRepository.Save(song2);
            var song3 = new Song() { Artist = artist, Genre = genre, Title = "Nighttrain" };
            _songRepository.Save(song3);
            UnitOfWork.FlushAndClear();

            _repository.Save(new AlbumTrack() { Album = album, AlbumTrackNumber = 1, Song = song1 });
            _repository.Save(new AlbumTrack() { Album = album, AlbumTrackNumber = 2, Song = song2 });
            _repository.Save(new AlbumTrack() { Album = album, AlbumTrackNumber = 3, Song = song3 });

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.GetAll();

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb.Count, Is.EqualTo(3));
        }

        [Test]
        public void ShouldDeleteAlbumTrackFromDatabaseButNotDeleteItsAlbumOrSong()
        {
            var artist = new Artist() { Name = "Eddie Money" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Rock" };
            _genreRepository.Save(genre);
            var album = new Album() { Artist = artist, Genre = genre, Label = "Columbia", Title = "Can't Hold Back", Year = 1986 };
            _albumRepository.Save(album);
            var song = new Song() { Artist = artist, Genre = genre, Title = "Take Me Home Tonight" };
            _songRepository.Save(song);
            UnitOfWork.FlushAndClear();

            var albumTrack = new AlbumTrack() { Album = album, AlbumTrackNumber = 1, Song = song };
            _repository.Save(albumTrack);
            UnitOfWork.FlushAndClear();

            var fromDb = _repository.Get(albumTrack.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(albumTrack));
            Assert.That(fromDb.AlbumId, Is.EqualTo(album.Id));
            Assert.That(fromDb.SongId, Is.EqualTo(song.Id));
            Assert.That(fromDb.AlbumTrackNumber, Is.EqualTo(albumTrack.AlbumTrackNumber));

            UnitOfWork.FlushAndClear();
            _repository.Delete(albumTrack);
            var albumTrackFromDbAfterDelete = _repository.Get(albumTrack.Id);

            Assert.That(albumTrackFromDbAfterDelete, Is.Null);

            UnitOfWork.FlushAndClear();
            var albumFromDb = _albumRepository.Get(album.Id);

            Assert.That(albumFromDb, Is.Not.Null);
            Assert.That(albumFromDb.Title, Is.EqualTo(album.Title));

            var songFromDb = _songRepository.Get(song.Id);

            Assert.That(songFromDb, Is.Not.Null);
            Assert.That(songFromDb.Title, Is.EqualTo(song.Title));
        }
    }
}
