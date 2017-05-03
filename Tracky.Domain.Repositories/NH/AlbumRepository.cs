using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.NH.Tracky;

namespace Tracky.Domain.Repositories.NH
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public AlbumRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Album Get(int id)
        {
            return _unitOfWork.Session.Get<Album>(id);
        }

        public void Save(Album album)
        {
            _unitOfWork.Session.SaveOrUpdate(album);
        }

        public void Delete(Album album)
        {
            _unitOfWork.Session.Delete(album);
        }

        public IList<Album> GetAll()
        {
            return _unitOfWork.Session.QueryOver<Album>().List();
        }
    }
}
