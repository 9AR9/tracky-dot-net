using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracky.Domain.Entities.Music;

namespace Tracky.Domain.Repositories.NH
{
    public interface IGenreRepository
    {
        Genre Get(int id);
        void Save(Genre genre);
        void Delete(Genre genre);
        IList<Genre> GetAll();
    }
}
