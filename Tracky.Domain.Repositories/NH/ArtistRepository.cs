using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.NH.Tracky;

namespace Tracky.Domain.Repositories.NH
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public ArtistRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Artist Get(int id)
        {
            return _unitOfWork.Session.Get<Artist>(id);
        }

        public void Save(Artist artist)
        {
            _unitOfWork.Session.SaveOrUpdate(artist);
        }

        public void Delete(Artist artist)
        {
            _unitOfWork.Session.Delete(artist);
        }

        public IList<Artist> GetAll()
        {
            return _unitOfWork.Session.QueryOver<Artist>().List();
        }
    }
}
