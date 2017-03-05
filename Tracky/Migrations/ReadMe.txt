This file discusses customizations done to migrations to facilitate avoidance of Cascade Delete errors.

The code-first data model of this project began throwing the following error from Package Manager Console
when running a new migration after the Album domain object was modified to change its AlbumTracks collection
from IEnumerable toICollection, which is what's needed to make it a navigation property, along with adding an
Album navigation property to AlbumTracks:

"Introducing FOREIGN KEY constraint 'FK_dbo.AlbumTracks_dbo.Albums_AlbumId' on table 'AlbumTracks' may cause cycles or multiple cascade paths. Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
Could not create constraint or index. See previous errors."

Apparently this message is happening now because I've added a second cascade path to the same data, but I can't for the life of me understand what
that would be, given that there are no other objects with a required dependency on the Album object.**

Nevertheless, I attempted to try adding the following line to OnModelCreating on the context, just like docs discuss
(https://msdn.microsoft.com/en-us/library/jj591620(v=vs.113).aspx || 
http://www.britishdeveloper.co.uk/2013/05/how-to-solve-introducing-foreign-key.html ||
http://stackoverflow.com/questions/14489676/entity-framework-how-to-solve-foreign-key-constraint-may-cause-cycles-or-multi),
but it is not doing the trick.

			modelBuilder.Entity<Album>().HasMany(a => a.AlbumTracks).WithRequired(at => at.Album).WillCascadeOnDelete(false);

So, as a fall back, I have disabled this problematic cascadeDelete setting manually on a freshly-created "Initial" migration
by locating the exact FK relationship the error is pointing to and changing the cascadeDelete value to false:

            CreateTable(
                "dbo.AlbumTracks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AlbumId = c.Int(nullable: false),
                        AlbumTrackNumber = c.Int(nullable: false),
                        SongId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Songs", t => t.SongId, cascadeDelete: true)
                .ForeignKey("dbo.Albums", t => t.AlbumId, cascadeDelete: false) //<---------- was true, so changed to false (same as removing the cascadeDelete attribute)
                .Index(t => t.AlbumId)
                .Index(t => t.SongId);

It remains a to-do to try testing this model in isolation to determine the exact issue, and how to properly handle it
in the context, which is preferable to doing it in a migration file.

UPDATE! OK, realized that I wasn't re-creating a migration after adding the line to the context. Now that I have this line in there, and recreate
a fresh "Initial" migration, the cascadeDelete attribute pointed to above is missing, and Create-Database is doing its thing. This is good!
However...now I'm worried that when I delete an Album, the AlbumTracks will not get automatically deleted, which is what I want. If that's the case,
I suppose I'll just have to add that manually to the DELETE controller method, since EF won't allow multiple cascadeDelete paths to the same data.
That test will be next.

UPDATE 2! So you now get a runtime error when you try to delete an album that still has tracks. Bullshit. This must be the price of turning off
cascade delete, meaning the relational deletion will have to be handled manually I suppose. Maybe that's best anyway.


** could be a deeper...AlbumTrack has a FK depenency on Song, as does PlaylistTrack. Is this the real issue, obscured by the error?