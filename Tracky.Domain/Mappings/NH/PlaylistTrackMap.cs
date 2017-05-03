using FluentNHibernate.Mapping;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Mappings.NH
{
    public class PlaylistTrackMap : ClassMap<PlaylistTrack>
    {
        public PlaylistTrackMap()
        {
            Table("PlaylistTracks");

            Id(pt => pt.Id);
            References(pt => pt.Playlist)
                .Not.Nullable()
                .Column("PlaylistId");
            //.ForeignKey("PlaylistId");
            Map(pt => pt.PlaylistId).ReadOnly();
            Map(pt => pt.PlaylistTrackNumber);
            References(pt => pt.Song)
                .Not.Nullable()
                .Column("SongId");
            //.ForeignKey("SongId");
            Map(at => at.SongId).ReadOnly();
        }
    }
}
