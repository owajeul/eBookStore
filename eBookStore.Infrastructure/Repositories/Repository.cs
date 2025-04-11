using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eBookStore.Infrastructure.Repositories;
public class Repository<T> :IRepository<T> where T : class
{
    private readonly AppDbContext _db;
    internal DbSet<T> _dbSet;

    public Repository(AppDbContext db)
    {
        _db = db;
        _dbSet = db.Set<T>();
    }
    public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return query.ToListAsync();
    }

    public Task<T> Get(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = _dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return query.FirstOrDefaultAsync();
    }

    public async Task Add(T entity)
    {
        await _db.AddAsync(entity);
    }
    public void Update(T entity)
    {
        _db.Update(entity);
    }
    public void Remove(T entity)
    {
        _db.Remove(entity);
    }
    public async Task Save()
    {
        await _db.SaveChangesAsync();
    }
}

