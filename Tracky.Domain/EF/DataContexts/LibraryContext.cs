using System.Data.Entity;
using Tracky.Domain.EF.Books;
using Tracky.Domain.EF.Music;

namespace Tracky.Domain.EF.DataContexts
{
    public class LibraryContext : DbContext//, ILibraryContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public LibraryContext() : base("name=LibraryContext")
        {
            // Adding this line can help in debugging by exposing EF's generated SQL
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public System.Data.Entity.DbSet<Author> Authors { get; set; }

        public System.Data.Entity.DbSet<Book> Books { get; set; }
        public virtual System.Data.Entity.DbSet<Artist> Artists { get; set; }
        public virtual System.Data.Entity.DbSet<Genre> Genres { get; set; }
        public virtual System.Data.Entity.DbSet<Song> Songs { get; set; }
        public virtual System.Data.Entity.DbSet<Album> Albums { get; set; }
        public virtual System.Data.Entity.DbSet<AlbumTrack> AlbumTracks { get; set; }
        public virtual System.Data.Entity.DbSet<Playlist> Playlists { get; set; }
        public virtual System.Data.Entity.DbSet<PlaylistTrack> PlaylistTracks { get; set; }

        /// <summary>
        /// This added level of indirection allows for edit behavior to be mocked on the context so that it can be called in unit testing
        /// for MVC controllers. Originally sourced from:
        /// http://stackoverflow.com/questions/5035323/mocking-or-faking-dbentityentry-or-creating-a-new-dbentityentry
        /// </summary>
        /// <param name="entity"></param>
        public virtual void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // This line addresses a multiple cascadeDelete path issue that arose when AlbumTrack was foreign-keyed to album
            modelBuilder.Entity<Album>().HasMany(a => a.AlbumTracks).WithRequired(at => at.Album).WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
