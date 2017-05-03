using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.NH.Tracky;

namespace Tracky.Domain.Repositories.NH
{
    public class SongRepository : ISongRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public SongRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Song Get(int id)
        {
            return _unitOfWork.Session.Get<Song>(id);
        }

        public void Save(Song song)
        {
            _unitOfWork.Session.SaveOrUpdate(song);
        }

        public void Delete(Song song)
        {
            _unitOfWork.Session.Delete(song);
        }

        public IList<Song> GetAll()
        {
            return _unitOfWork.Session.QueryOver<Song>().List();
        }
    }
}
