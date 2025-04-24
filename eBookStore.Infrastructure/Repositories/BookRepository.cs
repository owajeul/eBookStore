using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data;
using eBookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace eBookStore.Infrastructure.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    private readonly AppDbContext _dbContext;
    public BookRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<decimal> GetBookPriceAsync(int bookId)
    {
        var book = await _dbContext.Books.FindAsync(bookId);
        return book.Price;
    }

}
