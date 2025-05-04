
namespace eBookStore.Application.Interfaces;

public interface IUnitOfWork
{
    IAdminRepository Admin { get; }
    IBookRepository Book { get; }
    ICartRepository Cart { get; }
    IOrderRepository Order { get; }
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task SaveAsync();
    void DetachAllEntities();
}