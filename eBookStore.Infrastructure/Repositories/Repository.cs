using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Application.Interfaces;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eBookStore.Infrastructure.Repositories;

public class Repository<T> :IRepository<T> where T : class
{
    private readonly AppDbContext _dbContext;
    internal DbSet<T> _dbSet;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
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
            query = query.AsNoTracking();
        }
        return query.FirstOrDefaultAsync();
    }

    public async Task Add(T entity)
    {
        await _dbContext.AddAsync(entity);
    }
    public void Update(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
    }
    public void Remove(T entity)
    {
        _dbContext.Remove(entity);
    }
    public async Task Save()
    {
        await _dbContext.SaveChangesAsync();
    }
}

