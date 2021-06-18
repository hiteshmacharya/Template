using Employee.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UpScript.Services.Employee.API.Infrastructure;

namespace Employee.API.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly EmployeeContext _context;

        /// <summary>
        /// Contructor of GenericRepository
        /// </summary>
        public GenericRepository(EmployeeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This method is to add a record.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            _context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// This method is to add a range of records.
        /// </summary>
        /// <param name="entities"></param>
        public async Task AddRange(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        /// <summary>
        /// This method is to get the records using an expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        /// <summary>
        /// This method is to get all the records.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// This method is to get a single record using ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}
