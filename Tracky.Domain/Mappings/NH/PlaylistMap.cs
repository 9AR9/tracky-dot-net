using FluentNHibernate.Mapping;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Mappings.NH
{
    public class PlaylistMap : ClassMap<Playlist>
    {
        public PlaylistMap()
        {
            Table("Playlists");

            Id(p => p.Id);
            Map(p => p.Name);
            HasMany(p => p.PlaylistTracks)
                .Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }
}
