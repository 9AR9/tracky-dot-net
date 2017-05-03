using FluentNHibernate.Mapping;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Mappings.NH
{
    public class AlbumTrackMap : ClassMap<AlbumTrack>
    {
        public AlbumTrackMap()
        {
            Table("AlbumTracks");

            Id(at => at.Id);
            References(at => at.Album)
                .Not.Nullable()
                .Column("AlbumId");
            //.ForeignKey("AlbumId");
            Map(at => at.AlbumId).ReadOnly();
            Map(at => at.AlbumTrackNumber);
            References(at => at.Song)
                .Not.Nullable()
                .Column("SongId");
            //.ForeignKey("SongId");
            Map(at => at.SongId).ReadOnly();
        }
    }
}
