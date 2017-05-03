using System.Data.Entity.Migrations;
using Tracky.Domain.Entities.Books;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.EF.DataContexts;

namespace Tracky.Migrations
{
    public static class DataSeeder
    {
        public static void SeedData(LibraryContext context)
        {
            context.Authors.AddOrUpdate(x => x.Id,
                new Author() {Id = 1, Name = "Jane Austen"},
                new Author() {Id = 2, Name = "Charles Dickens"},
                new Author() {Id = 3, Name = "Miguel de Cervantes"},
                new Author() {Id = 4, Name = "Jack Kerouac"}
            );

            context.Books.AddOrUpdate(x => x.Id,
                new Book() {Id = 1, Title = "Pride and Prejudice", Year = 1813, AuthorId = 1, Price = 9.99M, Genre = "Comedy of manners"},
                new Book() {Id = 2, Title = "Northanger Abbey", Year = 1817, AuthorId = 1, Price = 12.95M, Genre = "Gothic parody"},
                new Book() {Id = 3, Title = "David Copperfield", Year = 1850, AuthorId = 2, Price = 15, Genre = "Bildungsroman"},
                new Book() {Id = 4, Title = "Don Quixote", Year = 1617, AuthorId = 3, Price = 8.95M, Genre = "Picaresque"},
                new Book() {Id = 5, Title = "On The Road", Year = 1957, AuthorId = 4, Price = 179.97M, Genre = "Beat novel"}
            );

            context.Artists.AddOrUpdate(x => x.Id,
                new Artist() {Id = 1, Name = "Incredible Bongo Band"},
                new Artist() {Id = 2, Name = "Public Enemy"},
                new Artist() {Id = 3, Name = "Bad Religion"},
                new Artist() {Id = 4, Name = "Funkadelic"},
                new Artist() {Id = 5, Name = "Ministry"},
                new Artist() {Id = 6, Name = "New Edition"},
                new Artist() {Id = 7, Name = "Eric B. & Rakim"},
                new Artist() {Id = 8, Name = "Talking Heads"},
                new Artist() {Id = 9, Name = "Against Me!"},
                new Artist() {Id = 10, Name = "Rodney Dangerfield"},
                new Artist() {Id = 11, Name = "De La Soul"},
                new Artist() {Id = 12, Name = "Stetsasonic"},
                new Artist() {Id = 13, Name = "The Dead Milkmen"},
                new Artist() {Id = 14, Name = "M|A|R|R|S"},
                new Artist() {Id = 15, Name = "Beastie Boys"},
                new Artist() {Id = 16, Name = "Sesame Street"},
                new Artist() {Id = 17, Name = "Gregory Abbott"}
            );

            context.Genres.AddOrUpdate(x => x.Id,
                new Genre() {Id = 1, Name = "Funk"},
                new Genre() {Id = 2, Name = "Hip-Hop"},
                new Genre() {Id = 3, Name = "Punk"},
                new Genre() {Id = 4, Name = "Rock"},
                new Genre() {Id = 5, Name = "Electronic"},
                new Genre() {Id = 6, Name = "Comedy"},
                new Genre() {Id = 7, Name = "Pop"},
                new Genre() {Id = 8, Name = "Children"},
                new Genre() {Id = 9, Name = "R&B"},
                new Genre() {Id = 10, Name = "Country"}
            );

            context.Songs.AddOrUpdate(x => x.Id,
                new Song() {Id = 1, Title = "Apache", ArtistId = 1, GenreId = 1},
                new Song() {Id = 2, Title = "Lost At Birth", ArtistId = 2, GenreId = 2},
                new Song() {Id = 3, Title = "Hooray For Me...", ArtistId = 3, GenreId = 3},
                new Song() {Id = 4, Title = "Get Off Your Ass And Jam", ArtistId = 4, GenreId = 1},
                new Song() {Id = 5, Title = "N.W.O.", ArtistId = 5, GenreId = 5},
                new Song() {Id = 6, Title = "Mr. Telephone Man", ArtistId = 6, GenreId = 7},
                new Song() {Id = 7, Title = "Eric B. Is President", ArtistId = 7, GenreId = 2},
                new Song() {Id = 8, Title = "Crosseyed & Painless", ArtistId = 8, GenreId = 4},
                new Song() {Id = 9, Title = "Dead Rats", ArtistId = 9, GenreId = 3},
                new Song() {Id = 10, Title = "Rappin' Rodney", ArtistId = 10, GenreId = 6},
                new Song() {Id = 11, Title = "The Magic Number", ArtistId = 11, GenreId = 2},
                new Song() {Id = 12, Title = "DBC Let The Music Play", ArtistId = 12, GenreId = 2},
                new Song() {Id = 13, Title = "Takin' Retards To The Zoo", ArtistId = 13, GenreId = 3},
                new Song() {Id = 14, Title = "Pump Up The Volume", ArtistId = 14, GenreId = 5},
                new Song() {Id = 15, Title = "Shake Your Rump", ArtistId = 15, GenreId = 2},
                new Song() {Id = 16, Title = "I Refuse To Sing Along", ArtistId = 16, GenreId = 8},
                new Song() {Id = 17, Title = "Shake You Down", ArtistId = 17, GenreId = 9},
                new Song() {Id = 18, Title = "333", ArtistId = 9, GenreId = 3},
                new Song() {Id = 19, Title = "Ghetto Thang", ArtistId = 11, GenreId = 2}
            );

            context.Albums.AddOrUpdate(x => x.Id,
                new Album() {Id = 1, Title = "Bongo Rock", ArtistId = 1, GenreId = 1, Year = 1973, Label = "Pride"},
                new Album() {Id = 2, Title = "Apocalypse 91...The Enemy Strikes Black", ArtistId = 2, GenreId = 2, Year = 1991, Label = "Def Jam"},
                new Album() {Id = 3, Title = "Stranger Than Fiction", ArtistId = 3, GenreId = 3, Year = 1994, Label = "Epitaph"},
                new Album() {Id = 4, Title = "Let's Take It To The Stage", ArtistId = 4, GenreId = 1, Year = 1975, Label = "Westbound"},
                new Album() {Id = 5, Title = "Psalm 69: The Way To Succeed And The Way To Suck Eggs", ArtistId = 5, GenreId = 5, Year = 1992, Label = "Sire/Warner Bros."},
                new Album() {Id = 6, Title = "New Edition", ArtistId = 6, GenreId = 9, Year = 1984, Label = "MCA"},
                new Album() {Id = 7, Title = "Paid In Full", ArtistId = 7, GenreId = 2, Year = 1987, Label = "4th & Broadway"},
                new Album() {Id = 8, Title = "Remain In Light", ArtistId = 8, GenreId = 4, Year = 1980, Label = "Sire"},
                new Album() {Id = 9, Title = "Shape Shift With Me", ArtistId = 9, GenreId = 3, Year = 2016, Label = "Total Treble"},
                new Album() {Id = 10, Title = "Rappin' Rodney", ArtistId = 10, GenreId = 2, Year = 1983, Label = "RCA"},
                new Album() {Id = 11, Title = "3 Feet High And Rising", ArtistId = 11, GenreId = 2, Year = 1989, Label = "Tommy Boy"},
                new Album() {Id = 12, Title = "In Full Gear", ArtistId = 12, GenreId = 2, Year = 1988, Label = "Tommy Boy"},
                new Album() {Id = 13, Title = "Big Lizard In My Back Yard", ArtistId = 13, GenreId = 3, Year = 1985, Label = "Fever/Restless"},
                new Album() {Id = 14, Title = "Pump Up The Volume [12\"]", ArtistId = 14, GenreId = 5, Year = 1987, Label = "4AD"},
                new Album() {Id = 15, Title = "Paul's Boutique", ArtistId = 15, GenreId = 2, Year = 1989, Label = "Capitol"},
                new Album() {Id = 16, Title = "Bert & Ernie Sing-Along", ArtistId = 16, GenreId = 8, Year = 1975, Label = "Children's Television Workshop"},
                new Album() {Id = 17, Title = "Shake You Down", ArtistId = 17, GenreId = 9, Year = 1986, Label = "Columbia"}
            );

            context.AlbumTracks.AddOrUpdate(x => x.Id, // context.AlbumTracks.AddOrUpdate(x => new { x.AlbumId, x.AlbumTrackNumber },
                new AlbumTrack() {Id = 1, AlbumId = 1, AlbumTrackNumber = 2, SongId = 1},
                new AlbumTrack() {Id = 2, AlbumId = 2, AlbumTrackNumber = 1, SongId = 2},
                new AlbumTrack() {Id = 3, AlbumId = 3, AlbumTrackNumber = 10, SongId = 3},
                new AlbumTrack() {Id = 4, AlbumId = 4, AlbumTrackNumber = 6, SongId = 4},
                new AlbumTrack() {Id = 5, AlbumId = 5, AlbumTrackNumber = 1, SongId = 5},
                new AlbumTrack() {Id = 6, AlbumId = 6, AlbumTrackNumber = 2, SongId = 6},
                new AlbumTrack() {Id = 7, AlbumId = 7, AlbumTrackNumber = 9, SongId = 7},
                new AlbumTrack() {Id = 8, AlbumId = 8, AlbumTrackNumber = 2, SongId = 8},
                new AlbumTrack() {Id = 9, AlbumId = 9, AlbumTrackNumber = 8, SongId = 18},
                new AlbumTrack() {Id = 10, AlbumId = 10, AlbumTrackNumber = 2, SongId = 10},
                new AlbumTrack() {Id = 11, AlbumId = 11, AlbumTrackNumber = 2, SongId = 11},
                new AlbumTrack() {Id = 12, AlbumId = 12, AlbumTrackNumber = 2, SongId = 12},
                new AlbumTrack() {Id = 13, AlbumId = 13, AlbumTrackNumber = 15, SongId = 13},
                new AlbumTrack() {Id = 14, AlbumId = 14, AlbumTrackNumber = 1, SongId = 14},
                new AlbumTrack() {Id = 15, AlbumId = 15, AlbumTrackNumber = 2, SongId = 15},
                new AlbumTrack() {Id = 16, AlbumId = 16, AlbumTrackNumber = 1, SongId = 16},
                new AlbumTrack() {Id = 17, AlbumId = 17, AlbumTrackNumber = 3, SongId = 17},
                new AlbumTrack() {Id = 18, AlbumId = 9, AlbumTrackNumber = 6, SongId = 9 },
                new AlbumTrack() {Id = 19, AlbumId = 11, AlbumTrackNumber = 7, SongId = 19 }
            );

            context.Playlists.AddOrUpdate(x => x.Id,
                new Playlist() {Id = 1, Name = "Happy Happy Tracks"},
                new Playlist() {Id = 2, Name = "Mellow Pimpin' Tracks"},
                new Playlist() {Id = 3, Name = "Zelda Jams"},
                new Playlist() {Id = 4, Name = "Luncheonette Soul Jazz" }
            );

            context.PlaylistTracks.AddOrUpdate(x => x.Id, // context.PlaylistTracks.AddOrUpdate(x => new { x.PlaylistId, x.PlaylistTrackNumber },
                new PlaylistTrack() {Id = 1, PlaylistId = 1, PlaylistTrackNumber = 1, SongId = 1},
                new PlaylistTrack() {Id = 2, PlaylistId = 1, PlaylistTrackNumber = 2, SongId = 2},
                new PlaylistTrack() {Id = 3, PlaylistId = 1, PlaylistTrackNumber = 3, SongId = 3},
                new PlaylistTrack() {Id = 4, PlaylistId = 1, PlaylistTrackNumber = 4, SongId = 4},
                new PlaylistTrack() {Id = 5, PlaylistId = 1, PlaylistTrackNumber = 5, SongId = 5},
                new PlaylistTrack() {Id = 6, PlaylistId = 1, PlaylistTrackNumber = 6, SongId = 7},
                new PlaylistTrack() {Id = 7, PlaylistId = 1, PlaylistTrackNumber = 7, SongId = 9},
                new PlaylistTrack() {Id = 8, PlaylistId = 1, PlaylistTrackNumber = 8, SongId = 8},
                new PlaylistTrack() {Id = 9, PlaylistId = 1, PlaylistTrackNumber = 9, SongId = 12},
                new PlaylistTrack() {Id = 10, PlaylistId = 1, PlaylistTrackNumber = 10, SongId = 10},
                new PlaylistTrack() {Id = 11, PlaylistId = 1, PlaylistTrackNumber = 11, SongId = 14},
                new PlaylistTrack() {Id = 12, PlaylistId = 1, PlaylistTrackNumber = 12, SongId = 11},
                new PlaylistTrack() {Id = 13, PlaylistId = 1, PlaylistTrackNumber = 13, SongId = 13},
                new PlaylistTrack() {Id = 14, PlaylistId = 1, PlaylistTrackNumber = 14, SongId = 15},
                new PlaylistTrack() {Id = 15, PlaylistId = 1, PlaylistTrackNumber = 15, SongId = 16},
                new PlaylistTrack() {Id = 16, PlaylistId = 2, PlaylistTrackNumber = 1, SongId = 6},
                new PlaylistTrack() {Id = 17, PlaylistId = 2, PlaylistTrackNumber = 2, SongId = 17}
            );
        }
    }
}