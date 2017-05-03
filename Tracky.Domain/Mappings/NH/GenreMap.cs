using FluentNHibernate.Mapping;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Mappings.NH
{
    public class GenreMap : ClassMap<Genre>
    {
        public GenreMap()
        {
            Table("Genres");

            Id(a => a.Id);
            Map(a => a.Name);
        }
    }
}
