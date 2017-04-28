using NHibernate;
using NHibernate.Cfg;

namespace Tracky.Orm.NH.GabrielSchenker
{
    public interface IUnitOfWorkFactory
    {
        ISession CurrentSession { get; set; }
        Configuration Configuration { get; }
        ISessionFactory SessionFactory { get; }
        IUnitOfWorkImplementor Create();
        void DisposeUnitOfWork(IUnitOfWorkImplementor adapter);
    }
}
