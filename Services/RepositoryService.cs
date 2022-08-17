using AutoMapper;
using Helpdesk_Backend_API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helpdesk_Backend_API.Services
{
    public interface IRepositoryService
    {
        /// <summary>
        /// Adds <see cref="{T}"/> record to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="bool"/></returns>
        ValueTask<bool> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Adds <see cref="List{T}"/> record to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="bool"/></returns>
        ValueTask<bool> AddRangeAsync<T>(List<T> entity, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Gets all <see cref="{T}"/> records 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><see cref="IQueryable{T}"/></returns>
        IQueryable<T> ListAll<T>() where T : class;

        /// <summary>
        /// Finds a record <see cref="{T}"/> from Dbset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id">Id of type <see cref="string"/></param>
        /// <returns><see cref="{T}"/></returns>
        ValueTask<T> FindAsync<T>(string Id) where T : class;

        /// <summary>
        /// Returns true if one or more records exists from DbSet <see cref="{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><see cref="bool"/></returns>
        ValueTask<bool> AnyAsync<T>() where T : class;

        /// <summary>
        /// Updates a record with new values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="bool"/></returns>
        ValueTask<bool> ModifyAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Deletes a record from a DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="bool"/></returns>
        ValueTask<bool> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Deletes multiple records from a DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="bool"/></returns>
        ValueTask<bool> DeleteRangeAsync<T>(List<T> entity, CancellationToken cancellationToken = default) where T : class;

        ValueTask<IDbContextTransaction> BeginTransactionAsync();

    }

    public class RepositoryService : IRepositoryService
    {
        public HttpContext HttpContext { get; }
        public IMapper Mapper { get; }
        public AppDbContext DbContext { get; }

        public RepositoryService(IHttpContextAccessor contextAccessor, IMapper mapper, AppDbContext dbContext)
        {
            HttpContext = contextAccessor.HttpContext;
            Mapper = mapper;
            DbContext = dbContext;
        }

        public async ValueTask<bool> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            await DbContext.AddAsync(entity, cancellationToken);
            return await DbContext.SaveChangesAsync(cancellationToken) > 0;
        }


        //controller <=> Service (on modeltype(s)) <=> Repository <=> Database
        //Repository Pattern


        //Factory Pattern
        //Builder Pattern
        //Controller <=> Repository
        //Controller <=> DbContext
        //Controller <=> Service(DbContext)
        //most of your code in the controller
        public async ValueTask<bool> AddRangeAsync<T>(List<T> entity, CancellationToken cancellationToken = default) where T : class
        {
            await DbContext.AddRangeAsync(entity, cancellationToken);
            return await DbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async ValueTask<bool> AnyAsync<T>() where T : class
        {
            var result = await ListAll<T>().AnyAsync<T>();
            return result;
        }

        public async ValueTask<bool> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            DbContext.Remove<T>(entity);
            return await DbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async ValueTask<T> FindAsync<T>(string Id) where T : class
        {
            if (string.IsNullOrWhiteSpace(Id))
                return null;

            return await DbContext.FindAsync<T>(Id);
        }

        public IQueryable<T> ListAll<T>() where T : class
        {
            return DbContext.Set<T>();
        }

        public async ValueTask<bool> ModifyAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            DbContext.Update<T>(entity);
            return await DbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async ValueTask<bool> DeleteRangeAsync<T>(List<T> entity, CancellationToken cancellationToken = default) where T : class
        {
            DbContext.RemoveRange(entity, cancellationToken);
            return await DbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async ValueTask<IDbContextTransaction> BeginTransactionAsync()
        {
            return await DbContext.Database.BeginTransactionAsync();
        }
    }
}
