using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Repositories.NH
{
    public interface IPlaylistRepository
    {
        Playlist Get(int id);
        void Save(Playlist playlist);
        void Delete(Playlist playlist);
        IList<Playlist> GetAll();
    }
}
