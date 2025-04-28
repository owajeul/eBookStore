using eBookStore.Domain.Entities;

namespace eBookStore.Application.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<decimal> GetBookPriceAsync(int bookId);
}