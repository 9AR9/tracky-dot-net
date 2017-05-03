using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.NH.Tracky;

namespace Tracky.Domain.Repositories.NH
{
    public class AlbumTrackRepository : IAlbumTrackRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public AlbumTrackRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AlbumTrack Get(int id)
        {
            return _unitOfWork.Session.Get<AlbumTrack>(id);
        }

        public void Save(AlbumTrack albumTrack)
        {
            _unitOfWork.Session.SaveOrUpdate(albumTrack);
        }

        public void Delete(AlbumTrack albumTrack)
        {
            _unitOfWork.Session.Delete(albumTrack);
        }

        public IList<AlbumTrack> GetAll()
        {
            return _unitOfWork.Session.QueryOver<AlbumTrack>().List();
        }
    }
}
