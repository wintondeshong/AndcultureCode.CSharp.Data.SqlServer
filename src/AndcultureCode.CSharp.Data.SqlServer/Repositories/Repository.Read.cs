using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AndcultureCode.CSharp.Core;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces;
using AndcultureCode.CSharp.Core.Interfaces.Data;
using AndcultureCode.CSharp.Core.Interfaces.Entity;
using AndcultureCode.CSharp.Core.Models;
using AndcultureCode.CSharp.Core.Models.Entities;
using AndcultureCode.CSharp.Core.Models.Errors;
using AndcultureCode.CSharp.Data.SqlServer.Extensions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace AndcultureCode.CSharp.Data.SqlServer.Repositories
{
    /// <summary>
    /// SqlServer implementation for READ operations for <typeparamref name="T"/>
    /// </summary>
    public partial class Repository<T> : IRepository<T> where T : Entity
    {
        #region Public Methods

        public virtual IResult<IQueryable<T>> FindAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool? ignoreQueryFilters = false,
            bool asNoTracking = false
        ) => Do<IQueryable<T>>.Try((r) =>
            GetQueryable(filter, orderBy, includeProperties, skip, take, ignoreQueryFilters, asNoTracking)
        ).Result;

        public IResult<IQueryable<IGrouping<TKey, T>>> FindAll<TKey>(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Expression<Func<T, TKey>> groupBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool? ignoreQueryFilters = false,
            bool asNoTracking = false
        ) => throw new NotImplementedException($"FindAll overload with groupBy params from AndcultureCode.CSharp.Core@0.7.0 not yet implemented.");

        public IResult<IQueryable<TResult>> FindAll<TKey, TResult>(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Expression<Func<T, TKey>> groupBy = null,
            Expression<Func<TKey, IEnumerable<T>, TResult>> groupBySelector = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool? ignoreQueryFilters = false,
            bool asNoTracking = false
        ) => throw new NotImplementedException($"FindAll overload with groupBy params from AndcultureCode.CSharp.Core@0.7.0 not yet implemented.");

        public virtual IResult<IList<T>> FindAllCommitted(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool? ignoreQueryFilters = false
        ) => Do<IList<T>>.Try((r) =>
            GetQueryable(filter, orderBy, includeProperties, skip, take, ignoreQueryFilters).ToList()
        ).Result;

        public async virtual Task<IResult<IList<T>>> FindAllCommittedAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool? ignoreQueryFilters = false)
        {
            var result = new Result<IList<T>>();

            try
            {
                result.ResultObject = await GetQueryable(filter, orderBy, includeProperties, skip, take, ignoreQueryFilters).ToListAsync();
            }
            catch (Exception ex)
            {
                result.AddError(ex.GetType().ToString(), ex.Message);
            }

            return result;
        }

        public IResult<T> FindById(long id, Expression<Func<T, bool>> filter)
            => throw new NotImplementedException($"FindById overload with filter params from AndcultureCode.CSharp.Core@0.8.0 not yet implemented.");

        public virtual IResult<T> FindById(long id, bool? ignoreQueryFilters = false) => Do<T>.Try((r) =>
        {
            if (ignoreQueryFilters.HasValue && ignoreQueryFilters.Value)
            {
                Query = Query.IgnoreQueryFilters();
            }

            return Query.FirstOrDefault(e => e.Id == id);
        }).Result;

        public virtual IResult<T> FindById(
            long id,
            bool? ignoreQueryFilters = false,
            params Expression<Func<T, object>>[] includeProperties
        ) => Do<T>.Try((r) =>
        {
            var query = Query;

            if (ignoreQueryFilters.HasValue && ignoreQueryFilters.Value)
            {
                query = query.IgnoreQueryFilters();
            }

            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }

            return query.FirstOrDefault(e => e.Id == id);
        }).Result;

        public virtual IResult<T> FindById(
            long id,
            params Expression<Func<T, object>>[] includeProperties
        ) => Do<T>.Try((r) =>
        {
            var query = Query;

            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }

            return query.FirstOrDefault(e => e.Id == id);
        }).Result;

        public virtual IResult<T> FindById(long id, params string[] includeProperties) => Do<T>.Try((r) =>
        {
            var query = Query;

            foreach (var property in includeProperties)
            {
                if (!string.IsNullOrEmpty(property))
                {
                    query = query.Include(property);
                }
            }

            return query.FirstOrDefault(e => e.Id == id);
        }).Result;

        public Task<IResult<T>> FindByIdAsync(long id, Expression<Func<T, bool>> filter)
            => throw new NotImplementedException($"FindByIdAsync overload with filter params from AndcultureCode.CSharp.Core@0.8.0 not yet implemented.");

        public async virtual Task<IResult<T>> FindByIdAsync(long id, bool? ignoreQueryFilters = false)
        {
            var result = new Result<T>();

            try
            {
                if (ignoreQueryFilters.HasValue && ignoreQueryFilters.Value)
                {
                    Query = Query.IgnoreQueryFilters();
                }

                result.ResultObject = await Query.FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                result.AddError(ex.GetType().ToString(), ex.Message);
            }

            return result;
        }

        public async virtual Task<IResult<T>> FindByIdAsync(
            long id,
            bool? ignoreQueryFilters = false,
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            var result = new Result<T>();

            try
            {
                var query = Query;

                if (ignoreQueryFilters.HasValue && ignoreQueryFilters.Value)
                {
                    query = query.IgnoreQueryFilters();
                }

                foreach (var property in includeProperties)
                {
                    query = query.Include(property);
                }

                result.ResultObject = await query.FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                result.AddError(ex.GetType().ToString(), ex.Message);
            }

            return result;
        }

        public async virtual Task<IResult<T>> FindByIdAsync(
            long id,
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            var result = new Result<T>();

            try
            {
                var query = Query;

                foreach (var property in includeProperties)
                {
                    query = query.Include(property);
                }

                result.ResultObject = await query.FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                result.AddError(ex.GetType().ToString(), ex.Message);
            }

            return result;
        }

        public async virtual Task<IResult<T>> FindByIdAsync(long id, params string[] includeProperties)
        {
            var result = new Result<T>();

            try
            {
                var query = Query;

                foreach (var property in includeProperties)
                {
                    if (!string.IsNullOrEmpty(property))
                    {
                        query = query.Include(property);
                    }
                }

                result.ResultObject = await query.FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                result.AddError(ex.GetType().ToString(), ex.Message);
            }

            return result;
        }

        #endregion Public Methods


        #region Protected Methods

        protected virtual IQueryable<T> GetQueryable(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool? ignoreQueryFilters = false,
            bool asNoTracking = false
        )
        {
            includeProperties = includeProperties ?? string.Empty;
            var query = Query;

            if (ignoreQueryFilters.HasValue && ignoreQueryFilters.Value)
            {
                query = query.IgnoreQueryFilters();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        #endregion Protected Methods
    }
}
