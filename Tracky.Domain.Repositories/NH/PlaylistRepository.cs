using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.NH.Tracky;

namespace Tracky.Domain.Repositories.NH
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public PlaylistRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Playlist Get(int id)
        {
            return _unitOfWork.Session.Get<Playlist>(id);
        }

        public void Save(Playlist playlist)
        {
            _unitOfWork.Session.SaveOrUpdate(playlist);
        }

        public void Delete(Playlist playlist)
        {
            _unitOfWork.Session.Delete(playlist);
        }

        public IList<Playlist> GetAll()
        {
            return _unitOfWork.Session.QueryOver<Playlist>().List();
        }
    }
}
