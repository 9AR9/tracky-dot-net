using FluentNHibernate.Mapping;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Mappings.NH
{
    public class ArtistMap : ClassMap<Artist>
    {
        public ArtistMap()
        {
            Table("Artists");

            Id(a => a.Id);
            Map(a => a.Name);
        }
    }
}
