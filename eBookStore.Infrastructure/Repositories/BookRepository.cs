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
    private readonly AppDbContext _db;
    public BookRepository(AppDbContext db): base(db)
    {
        _db = db;
    }
}
