using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;

namespace Tracky.Domain.Repositories
{
    public interface IUnitOfWorkFactory
    {
        ISession CurrentSession { get; set; }
        Configuration Configuration { get; }
        ISessionFactory SessionFactory { get; }
        IUnitOfWork Create();
    }
}
