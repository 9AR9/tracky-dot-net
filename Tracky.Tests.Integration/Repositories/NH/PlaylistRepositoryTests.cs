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
    class PlaylistRepositoryTests : NhDatabaseIntegrationTestBase
    {
        private IPlaylistRepository _repository;
        private ISongRepository _songRepository;
        private IArtistRepository _artistRepository;
        private IGenreRepository _genreRepository;
        private IPlaylistTrackRepository _playlistTrackRepository;

        [SetUp]
        public void SetUp()
        {
            _repository = new PlaylistRepository(UnitOfWork);
            _songRepository = new SongRepository(UnitOfWork);
            _artistRepository = new ArtistRepository(UnitOfWork);
            _genreRepository = new GenreRepository(UnitOfWork);
            _playlistTrackRepository = new PlaylistTrackRepository(UnitOfWork);
        }

        [Test]
        public void ShouldAddNewPlaylistAndRetrieveItFromDatabase()
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

            var playlist = new Playlist() { Name = "Eddie Tunes" };
            var playlistTracks = new List<PlaylistTrack>()
            {
                new PlaylistTrack() { Playlist = playlist, PlaylistTrackNumber = 1, Song = song1 },
                new PlaylistTrack() { Playlist = playlist, PlaylistTrackNumber = 2, Song = song2 }
            };
            playlist.PlaylistTracks = playlistTracks;
            _repository.Save(playlist);
            UnitOfWork.FlushAndClear();

            var fromDb = _repository.Get(playlist.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(playlist));
            Assert.That(fromDb.Name, Is.EqualTo(playlist.Name));
            Assert.That(fromDb.PlaylistTracks.Count, Is.EqualTo(2));
            Assert.That(fromDb.PlaylistTracks.ToList()[0].Song.Title, Is.EqualTo(song1.Title));
            Assert.That(fromDb.PlaylistTracks.ToList()[1].Song.Title, Is.EqualTo(song2.Title));
        }

        [Test]
        public void ShouldGetAllPlaylistsFromDatabase()
        {
            _repository.Save(new Playlist() { Name = "My tracks" });
            _repository.Save(new Playlist() { Name = "Your tracks" });
            _repository.Save(new Playlist() { Name = "His tracks" });
            _repository.Save(new Playlist() { Name = "Her tracks" });

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.GetAll();

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb.Count, Is.EqualTo(4));
        }

        [Test]
        public void ShouldDeletePlaylistFromDatabaseAndAlsoDeleteItsPlaylistTracks()
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

            var playlist = new Playlist() { Name = "Eddie Tunes" };
            var playlistTracks = new List<PlaylistTrack>()
            {
                new PlaylistTrack() { Playlist = playlist, PlaylistTrackNumber = 1, Song = song1 },
                new PlaylistTrack() { Playlist = playlist, PlaylistTrackNumber = 2, Song = song2 }
            };
            playlist.PlaylistTracks = playlistTracks;
            _repository.Save(playlist);
            UnitOfWork.FlushAndClear();

            var fromDb = _repository.Get(playlist.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(playlist));
            Assert.That(fromDb.PlaylistTracks.ToList()[0].Song.Title, Is.EqualTo(song1.Title));
            Assert.That(fromDb.PlaylistTracks.ToList()[1].Song.Title, Is.EqualTo(song2.Title));

            UnitOfWork.FlushAndClear();
            _repository.Delete(playlist);
            var albumFromDbAfterDelete = _repository.Get(playlist.Id);

            Assert.That(albumFromDbAfterDelete, Is.Null);

            UnitOfWork.FlushAndClear();
            var playlistTracksFromDb = _playlistTrackRepository.GetAll();

            Assert.That(playlistTracksFromDb.Count, Is.Zero);
        }
    }
}
