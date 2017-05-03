using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;
using Tracky.Domain.Repositories.Orm.NH.Tracky;

namespace Tracky.Domain.Repositories.NH
{
    public class GenreRepository : IGenreRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public GenreRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Genre Get(int id)
        {
            return _unitOfWork.Session.Get<Genre>(id);
        }

        public void Save(Genre genre)
        {
            _unitOfWork.Session.SaveOrUpdate(genre);
        }

        public void Delete(Genre genre)
        {
            _unitOfWork.Session.Delete(genre);
        }

        public IList<Genre> GetAll()
        {
            return _unitOfWork.Session.QueryOver<Genre>().List();
        }
    }
}
