using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Repositories.NH
{
    public interface IArtistRepository
    {
        Artist Get(int id);
        void Save(Artist artist);
        void Delete(Artist artist);
        IList<Artist> GetAll();
    }
}
