﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgrammersBlog.Data.Abstract;
using ProgrammersBlog.Data.Concrete.EntityFramework.Contexts;
using ProgrammersBlog.Data.Concrete.EntityFramework.Repositories;

namespace ProgrammersBlog.Data.Concrete
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ProgrammersBlogContext _context;
        private EfArticleRepository _articleRepository;
        private EfCategoryRepository _categoryRepository;
        private EfCommentRepository _commentRepository;
        private EfCityRepository _cityRepository;
        private EfPlaceRepository _placeRepository;

        public UnitOfWork(ProgrammersBlogContext context)
        {
            _context = context;
        }

        public IArticleRepository Articles => _articleRepository ??= new EfArticleRepository(_context);
        public ICategoryRepository Categories => _categoryRepository ??= new EfCategoryRepository(_context);
        public ICommentRepository Comments => _commentRepository ??= new EfCommentRepository(_context);
        public ICityRepository Cities => _cityRepository ?? new EfCityRepository(_context);
        public IPlaceRepository Places => _placeRepository ?? new EfPlaceRepository(_context);
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
