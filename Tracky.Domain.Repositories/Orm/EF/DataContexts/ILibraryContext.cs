﻿using System;
using Tracky.Domain.Entities.Books;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Repositories.Orm.EF.DataContexts
{
    public interface ILibraryContext : IDisposable
    {
        System.Data.Entity.DbSet<Author> Authors { get; set; }
        System.Data.Entity.DbSet<Book> Books { get; set; }
        System.Data.Entity.DbSet<Artist> Artists { get; set; }
        System.Data.Entity.DbSet<Genre> Genres { get; set; }
        System.Data.Entity.DbSet<Song> Songs { get; set; }
        System.Data.Entity.DbSet<Album> Albums { get; set; }
        System.Data.Entity.DbSet<AlbumTrack> AlbumTracks { get; set; }
        System.Data.Entity.DbSet<Playlist> Playlists { get; set; }
        System.Data.Entity.DbSet<PlaylistTrack> PlaylistTracks { get; set; }
        void SetModified(object entity);
    }
}