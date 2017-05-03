using FluentNHibernate.Mapping;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Mappings.NH
{
    public class AlbumMap : ClassMap<Album>
    {
        public AlbumMap()
        {
            Table("Albums");

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
                //.ForeignKey("GenreId");
            Map(a => a.GenreId).ReadOnly();
            Map(a => a.Year);
            Map(a => a.Label);
            HasMany(x => x.AlbumTracks)
                .Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }
}
