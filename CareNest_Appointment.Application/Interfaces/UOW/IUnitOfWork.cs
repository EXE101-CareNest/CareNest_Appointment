using CareNest_Appointment.Domain.Repositories;

namespace CareNest_Appointment.Application.Interfaces.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        void Save();
        Task SaveAsync();
        void BeginTransaction();
        void CommitTransaction();
        void RollBack();
    }
}
