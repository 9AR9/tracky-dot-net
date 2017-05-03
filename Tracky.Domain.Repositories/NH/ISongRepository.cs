using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Repositories.NH
{
    public interface ISongRepository
    {
        Song Get(int id);
        void Save(Song song);
        void Delete(Song song);
        IList<Song> GetAll();
    }
}
