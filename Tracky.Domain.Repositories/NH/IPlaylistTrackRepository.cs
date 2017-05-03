using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Repositories.NH
{
    public interface IPlaylistTrackRepository
    {
        PlaylistTrack Get(int id);
        void Save(PlaylistTrack playlistTrack);
        void Delete(PlaylistTrack playlistTrack);
        IList<PlaylistTrack> GetAll();
    }
}
