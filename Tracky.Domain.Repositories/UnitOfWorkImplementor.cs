using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Tracky.Domain.Repositories
{
    public class UnitOfWorkImplementor : IUnitOfWork
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ISession _session;

        public UnitOfWorkImplementor(IUnitOfWorkFactory factory, ISession session)
        {
            _unitOfWorkFactory = factory;
            _session = session;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
