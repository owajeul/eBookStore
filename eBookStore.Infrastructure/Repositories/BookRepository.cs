using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data;

namespace eBookStore.Infrastructure.Repositories;

public class BookRepository : Repository<Book>
{
    private readonly AppDbContext _dbContext;
    public BookRepository(AppDbContext dbContext): base(dbContext)
    {
        _dbContext = dbContext;
    }
}
