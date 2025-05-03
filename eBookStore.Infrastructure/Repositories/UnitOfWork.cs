using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Application.Interfaces;
using eBookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace eBookStore.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private IDbContextTransaction _transaction;
    public IAdminRepository Admin { get; private set; }
    public IBookRepository Book { get; private set; }
    public ICartRepository Cart { get; private set; }
    public IOrderRepository Order { get; private set; }

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        Admin = new AdminRepository(_dbContext);
        Book = new BookRepository(_dbContext);
        Cart = new CartRepository(_dbContext);
        Order = new OrderRepository(_dbContext);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _transaction.CommitAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _transaction.RollbackAsync();
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
