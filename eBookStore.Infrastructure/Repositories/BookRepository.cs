﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Application.Interfaces;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data;
using eBookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

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
        var book = await Get(b => b.Id  == bookId);
        return book.Price;
    }

    public async Task AddBookReviewAsync(BookReview bookReview)
    {
        await _dbContext.BookReviews.AddAsync(bookReview);
    }
    public async Task<BookReview?> GetBookReviewAsync(int bookId, string userId)
    {
        return await _dbContext.BookReviews
            .FirstOrDefaultAsync(br => br.BookId == bookId && br.UserId == userId);
    }
    public async Task<bool> HasUserPurchasedBookAsync(int bookId, string userId)
    {
        return await _dbContext.Orders
            .Include(o => o.OrderItems)
                      .AnyAsync(o => o.UserId == userId && o.OrderItems.Any(oi => oi.BookId == bookId));
    }

    public async Task<Book?> GetBookWithReviewsAsync(int bookId)
    {
        return await _dbContext.Books
            .Include(b => b.Reviews)
            .FirstOrDefaultAsync(b => b.Id == bookId);
    }

    public async Task<List<string>> GetDistinctGenresAsync()
    {
        return await _dbContext.Books
            .Where(b => b.Genre != null)
            .Select(b => b.Genre)
            .Distinct()
            .ToListAsync();
    }
}
