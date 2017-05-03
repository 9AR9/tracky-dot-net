using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Repositories.NH
{
    public interface IAlbumRepository
    {
        Album Get(int id);
        void Save(Album album);
        void Delete(Album album);
        IList<Album> GetAll();
    }
}
