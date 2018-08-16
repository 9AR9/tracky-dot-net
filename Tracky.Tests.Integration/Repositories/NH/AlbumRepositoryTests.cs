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
    class AlbumRepositoryTests : NhDatabaseIntegrationTestBase
    {
        private IAlbumRepository _repository;
        private ISongRepository _songRepository;
        private IArtistRepository _artistRepository;
        private IGenreRepository _genreRepository;
        private IAlbumTrackRepository _albumTrackRepository;

        [SetUp]
        public void SetUp()
        {
            _repository = new AlbumRepository(UnitOfWork);
            _songRepository = new SongRepository(UnitOfWork);
            _artistRepository = new ArtistRepository(UnitOfWork);
            _genreRepository = new GenreRepository(UnitOfWork);
            _albumTrackRepository = new AlbumTrackRepository(UnitOfWork);
        }

        [Test]
        public void ShouldAddNewAlbumAndRetrieveItFromDatabase()
        {
            var artist = new Artist() { Name = "Eddie Money" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Rock" };
            _genreRepository.Save(genre);
            var song1 = new Song() { Artist = artist, Genre = genre, Title = "Take Me Home Tonight" };
            _songRepository.Save(song1);
            var song2 = new Song() { Artist = artist, Genre = genre, Title = "One Love" };
            _songRepository.Save(song2);
            UnitOfWork.FlushAndClear();

            var album = new Album() { Artist = artist, Genre = genre, Label = "Columbia", Title = "Can't Hold Back", Year = 1986 };
            var albumTracks = new List<AlbumTrack>()
            {
                new AlbumTrack() { Album = album, AlbumTrackNumber = 1, Song = song1 },
                new AlbumTrack() { Album = album, AlbumTrackNumber = 2, Song = song2 }
            };
            album.AlbumTracks = albumTracks;
            _repository.Save(album);
            UnitOfWork.FlushAndClear();

            var fromDb = _repository.Get(album.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(album));
            Assert.That(fromDb.Title, Is.EqualTo(album.Title));
            Assert.That(fromDb.Label, Is.EqualTo(album.Label));
            Assert.That(fromDb.Year, Is.EqualTo(album.Year));
            Assert.That(fromDb.ArtistId, Is.EqualTo(artist.Id));
            Assert.That(fromDb.GenreId, Is.EqualTo(genre.Id));
            Assert.That(fromDb.AlbumTracks.Count, Is.EqualTo(2));
            Assert.That(fromDb.AlbumTracks.ToList()[0].Song.Title, Is.EqualTo(song1.Title));
            Assert.That(fromDb.AlbumTracks.ToList()[1].Song.Title, Is.EqualTo(song2.Title));
        }

        [Test]
        public void ShouldGetAllAlbumsFromDatabase()
        {
            var artist = new Artist() { Name = "Public Enemy" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Hip-Hop" };
            _genreRepository.Save(genre);
            UnitOfWork.FlushAndClear();

            _repository.Save(new Album() { Artist = artist, Genre = genre, Title = "Yo! Bum Rush The Show", Label = "Def Jam/Columbia", Year = 1987 });
            _repository.Save(new Album() { Artist = artist, Genre = genre, Title = "It Takes A Nation Of Millions To Hold Us Back", Label = "Def Jam/Columbia", Year = 1988 });
            _repository.Save(new Album() { Artist = artist, Genre = genre, Title = "Fear Of A Black Planet", Label = "Def Jam/Columbia", Year = 1990 });
            _repository.Save(new Album() { Artist = artist, Genre = genre, Title = "Apocalypse '91...The Enemy Strikes Black", Label = "Def Jam", Year = 1991 });

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.GetAll();

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb.Count, Is.EqualTo(4));
        }

        [Test]
        public void ShouldDeleteAlbumFromDatabaseButNotDeleteItsArtistOrGenre()
        {
            var artist = new Artist() { Name = "Eddie Money" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Rock" };
            _genreRepository.Save(genre);
            var song1 = new Song() { Artist = artist, Genre = genre, Title = "Take Me Home Tonight" };
            _songRepository.Save(song1);
            var song2 = new Song() { Artist = artist, Genre = genre, Title = "One Love" };
            _songRepository.Save(song2);
            UnitOfWork.FlushAndClear();

            var album = new Album() { Artist = artist, Genre = genre, Label = "Columbia", Title = "Can't Hold Back", Year = 1986 };
            var albumTracks = new List<AlbumTrack>()
            {
                new AlbumTrack() { Album = album, AlbumTrackNumber = 1, Song = song1 },
                new AlbumTrack() { Album = album, AlbumTrackNumber = 2, Song = song2 }
            };
            album.AlbumTracks = albumTracks;
            _repository.Save(album);
            UnitOfWork.FlushAndClear();

            var fromDb = _repository.Get(album.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(album));
            Assert.That(fromDb.Title, Is.EqualTo(album.Title));
            Assert.That(fromDb.Label, Is.EqualTo(album.Label));
            Assert.That(fromDb.Year, Is.EqualTo(album.Year));
            Assert.That(fromDb.ArtistId, Is.EqualTo(artist.Id));
            Assert.That(fromDb.GenreId, Is.EqualTo(genre.Id));
            Assert.That(fromDb.AlbumTracks.ToList()[0].Song.Title, Is.EqualTo(song1.Title));
            Assert.That(fromDb.AlbumTracks.ToList()[1].Song.Title, Is.EqualTo(song2.Title));

            UnitOfWork.FlushAndClear();
            _repository.Delete(album);
            var albumFromDbAfterDelete = _repository.Get(album.Id);

            Assert.That(albumFromDbAfterDelete, Is.Null);

            UnitOfWork.FlushAndClear();
            var artistFromDb = _artistRepository.Get(artist.Id);

            Assert.That(artistFromDb, Is.Not.Null);
            Assert.That(artistFromDb.Name, Is.EqualTo(artist.Name));

            var genreFromDb = _genreRepository.Get(genre.Id);

            Assert.That(genreFromDb, Is.Not.Null);
            Assert.That(genreFromDb.Name, Is.EqualTo(genre.Name));
        }

        [Test]
        public void ShouldDeleteAlbumFromDatabaseAndAlsoDeleteItsAlbumTracks()
        {
            var artist = new Artist() { Name = "Eddie Money" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Rock" };
            _genreRepository.Save(genre);
            var song1 = new Song() { Artist = artist, Genre = genre, Title = "Take Me Home Tonight" };
            _songRepository.Save(song1);
            var song2 = new Song() { Artist = artist, Genre = genre, Title = "One Love" };
            _songRepository.Save(song2);
            UnitOfWork.FlushAndClear();

            var album = new Album() { Artist = artist, Genre = genre, Label = "Columbia", Title = "Can't Hold Back", Year = 1986 };
            var albumTracks = new List<AlbumTrack>()
            {
                new AlbumTrack() { Album = album, AlbumTrackNumber = 1, Song = song1 },
                new AlbumTrack() { Album = album, AlbumTrackNumber = 2, Song = song2 }
            };
            album.AlbumTracks = albumTracks;
            _repository.Save(album);
            UnitOfWork.FlushAndClear();

            var fromDb = _repository.Get(album.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(album));
            Assert.That(fromDb.Title, Is.EqualTo(album.Title));
            Assert.That(fromDb.Label, Is.EqualTo(album.Label));
            Assert.That(fromDb.Year, Is.EqualTo(album.Year));
            Assert.That(fromDb.ArtistId, Is.EqualTo(artist.Id));
            Assert.That(fromDb.GenreId, Is.EqualTo(genre.Id));
            Assert.That(fromDb.AlbumTracks.ToList()[0].Song.Title, Is.EqualTo(song1.Title));
            Assert.That(fromDb.AlbumTracks.ToList()[1].Song.Title, Is.EqualTo(song2.Title));

            UnitOfWork.FlushAndClear();
            _repository.Delete(album);
            var albumFromDbAfterDelete = _repository.Get(album.Id);

            Assert.That(albumFromDbAfterDelete, Is.Null);

            UnitOfWork.FlushAndClear();
            var albumTracksFromDb = _albumTrackRepository.GetAll();

            Assert.That(albumTracksFromDb.Count, Is.Zero);
        }
    }
}
