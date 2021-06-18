using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Employee.API.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// This method is to get a single record using ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetById(int id);

        /// <summary>
        /// This method is to get all the records.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAll();

        /// <summary>
        /// This method is to get the records using an expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression);

        /// <summary>
        /// This method is to add a record.
        /// </summary>
        /// <param name="entity"></param>
        Task<T> Add(T entity);

        /// <summary>
        /// This method is to add a range of records.
        /// </summary>
        /// <param name="entities"></param>
        Task AddRange(IEnumerable<T> entities);
    }
}
