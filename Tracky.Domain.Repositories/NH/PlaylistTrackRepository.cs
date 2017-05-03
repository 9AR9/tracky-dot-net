using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.NH.Tracky;

namespace Tracky.Domain.Repositories.NH
{
    public class PlaylistTrackRepository : IPlaylistTrackRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public PlaylistTrackRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PlaylistTrack Get(int id)
        {
            return _unitOfWork.Session.Get<PlaylistTrack>(id);
        }

        public void Save(PlaylistTrack playlistTrack)
        {
            _unitOfWork.Session.SaveOrUpdate(playlistTrack);
        }

        public void Delete(PlaylistTrack playlistTrack)
        {
            _unitOfWork.Session.Delete(playlistTrack);
        }

        public IList<PlaylistTrack> GetAll()
        {
            return _unitOfWork.Session.QueryOver<PlaylistTrack>().List();
        }
    }
}
