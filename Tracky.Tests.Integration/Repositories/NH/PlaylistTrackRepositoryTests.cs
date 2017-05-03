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
    class PlaylistTrackRepositoryTests : NHDatabaseIntegrationTestBase
    {
        private IPlaylistTrackRepository _repository;
        private IArtistRepository _artistRepository;
        private IGenreRepository _genreRepository;
        private ISongRepository _songRepository;
        private IPlaylistRepository _playlistRepository;

        [SetUp]
        public void SetUp()
        {
            _repository = new PlaylistTrackRepository(UnitOfWork);
            _artistRepository = new ArtistRepository(UnitOfWork);
            _genreRepository = new GenreRepository(UnitOfWork);
            _songRepository = new SongRepository(UnitOfWork);
            _playlistRepository = new PlaylistRepository(UnitOfWork);
        }

        [Test]
        public void ShouldAddNewPlaylistTrackAndRetrieveItFromDatabase()
        {
            var artist = new Artist() { Name = "Eddie Money" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Rock" };
            _genreRepository.Save(genre);
            var playlist = new Playlist() { Name = "Eddie Tunes" };
            _playlistRepository.Save(playlist);
            var song = new Song() { Artist = artist, Genre = genre, Title = "Take Me Home Tonight" };
            _songRepository.Save(song);
            UnitOfWork.FlushAndClear();

            var playlistTrack = new PlaylistTrack() { Playlist = playlist, PlaylistTrackNumber = 1, Song = song };
            _repository.Save(playlistTrack);
            UnitOfWork.FlushAndClear();

            var fromDb = _repository.Get(playlistTrack.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(playlistTrack));
            Assert.That(fromDb.PlaylistId, Is.EqualTo(playlist.Id));
            Assert.That(fromDb.SongId, Is.EqualTo(song.Id));
            Assert.That(fromDb.PlaylistTrackNumber, Is.EqualTo(playlistTrack.PlaylistTrackNumber));
        }

        [Test]
        public void ShouldGetAllAlbumTracksFromDatabase()
        {
            var artist = new Artist() { Name = "Public Enemy" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Hip-Hop" };
            _genreRepository.Save(genre);
            var playlist = new Playlist() { Name = "PE In Effect" };
            _playlistRepository.Save(playlist);
            var song1 = new Song() { Artist = artist, Genre = genre, Title = "Lost At Birth" };
            _songRepository.Save(song1);
            var song2 = new Song() { Artist = artist, Genre = genre, Title = "Rebirth" };
            _songRepository.Save(song2);
            var song3 = new Song() { Artist = artist, Genre = genre, Title = "Nighttrain" };
            _songRepository.Save(song3);
            UnitOfWork.FlushAndClear();

            _repository.Save(new PlaylistTrack() { Playlist = playlist, PlaylistTrackNumber = 1, Song = song1 });
            _repository.Save(new PlaylistTrack() { Playlist = playlist, PlaylistTrackNumber = 2, Song = song2 });
            _repository.Save(new PlaylistTrack() { Playlist = playlist, PlaylistTrackNumber = 3, Song = song3 });

            UnitOfWork.FlushAndClear();
            var fromDb = _repository.GetAll();

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb.Count, Is.EqualTo(3));
        }

        [Test]
        public void ShouldDeletePlaylistTrackFromDatabaseButNotDeleteItsPlaylistOrSong()
        {
            var artist = new Artist() { Name = "Eddie Money" };
            _artistRepository.Save(artist);
            var genre = new Genre() { Name = "Rock" };
            _genreRepository.Save(genre);
            var playlist = new Playlist() { Name = "Eddie Tunes" };
            _playlistRepository.Save(playlist);
            var song = new Song() { Artist = artist, Genre = genre, Title = "Take Me Home Tonight" };
            _songRepository.Save(song);
            UnitOfWork.FlushAndClear();

            var playlistTrack = new PlaylistTrack() { Playlist = playlist, PlaylistTrackNumber = 1, Song = song };
            _repository.Save(playlistTrack);
            UnitOfWork.FlushAndClear();

            var fromDb = _repository.Get(playlistTrack.Id);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(fromDb, Is.Not.SameAs(playlistTrack));
            Assert.That(fromDb.PlaylistId, Is.EqualTo(playlist.Id));
            Assert.That(fromDb.SongId, Is.EqualTo(song.Id));
            Assert.That(fromDb.PlaylistTrackNumber, Is.EqualTo(playlistTrack.PlaylistTrackNumber));

            UnitOfWork.FlushAndClear();
            _repository.Delete(playlistTrack);
            var albumTrackFromDbAfterDelete = _repository.Get(playlistTrack.Id);

            Assert.That(albumTrackFromDbAfterDelete, Is.Null);

            UnitOfWork.FlushAndClear();
            var playlistFromDb = _playlistRepository.Get(playlist.Id);

            Assert.That(playlistFromDb, Is.Not.Null);
            Assert.That(playlistFromDb.Name, Is.EqualTo(playlist.Name));

            var songFromDb = _songRepository.Get(song.Id);

            Assert.That(songFromDb, Is.Not.Null);
            Assert.That(songFromDb.Title, Is.EqualTo(song.Title));
        }
    }
}
