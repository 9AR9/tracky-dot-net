using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Repositories.NH
{
    public interface IAlbumTrackRepository
    {
        AlbumTrack Get(int id);
        void Save(AlbumTrack albumTrack);
        void Delete(AlbumTrack albumTrack);
        IList<AlbumTrack> GetAll();
    }
}
