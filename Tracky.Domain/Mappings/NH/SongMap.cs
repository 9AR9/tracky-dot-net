using FluentNHibernate.Mapping;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Mappings.NH
{
    public class SongMap : ClassMap<Song>
    {
        public SongMap()
        {
            Table("Songs");

            Id(a => a.Id);
            Map(a => a.Title);
            References(a => a.Artist)
                .Not.Nullable()
                .Column("ArtistId");
                //.ForeignKey("ArtistId");
            Map(a => a.ArtistId).ReadOnly();
            References(a => a.Genre)
                .Not.Nullable()
                .Column("GenreId");
                //.ForeignKey("SongId");
            Map(a => a.GenreId).ReadOnly();
        }
    }
}
