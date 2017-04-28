using System;

namespace Tracky.Orm.NH.GabrielSchenker
{
    public interface IGenericTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
