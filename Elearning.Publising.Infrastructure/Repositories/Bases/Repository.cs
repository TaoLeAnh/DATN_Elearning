using Elearning.Publising.Infrastructure.Persistence.Context;
using Elearning.Shared.Commons.Interfaces.SQL;
using Elearning.Shared.Commons.Model.SQL;
using Elearning.Shared.Commons.Querys.Grid;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Elearning.Publising.Infrastructure.Repositories.Bases
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        //Add Virtual >> OverRide
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public T GetById(int id) => _context.Set<T>().Find(id) ?? throw new Exception("bản ghi không tồn tại"); // Arrow Function
        public T GetById(Guid id) => _context.Set<T>().Find(id) ?? throw new Exception("bản ghi không tồn tại"); // Arrow Function

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id) ?? throw new Exception("bản ghi không tồn tại");
        }
        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id) ?? throw new Exception("bản ghi không tồn tại");
        }
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
        public T Find(Expression<Func<T, bool>> criteria) => _context.Set<T>().SingleOrDefault(criteria) ?? throw new Exception("bản ghi không tồn tại");
        public T FindInclude(Expression<Func<T, bool>> criteria, Expression<Func<T, object>>? orderBy = null, string orderByDirection = OrderBy.Ascending, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }


            return query.FirstOrDefault();

        }
        public T FindNoTraking(Expression<Func<T, bool>> criteria, string[]? includes = null) => _context.Set<T>().AsNoTracking().SingleOrDefault(criteria);
        public T FindNoTraking(Expression<Func<T, bool>> criteria, Expression<Func<T, bool>> criteria2, string[]? includes = null) => _context.Set<T>().Where(criteria).Where(criteria2).AsNoTracking().FirstOrDefault();

        public T FindNoTraking(Expression<Func<T, bool>> criteria, Expression<Func<T, bool>> criteria2, Expression<Func<T, bool>> criteria3, string[]? includes = null) => _context.Set<T>().Where(criteria).Where(criteria2).Where(criteria3).AsNoTracking().FirstOrDefault();

        public T FindNoTraking(Expression<Func<T, bool>> criteria, Expression<Func<T, object>>? orderBy = null) => _context.Set<T>().Where(criteria).OrderBy(orderBy).AsNoTracking().FirstOrDefault();


        public T FindNoTraking(Expression<Func<T, bool>> criteria, Expression<Func<T, bool>> criteria2,
            Expression<Func<T, object>>? orderBy = null, string orderByDirection = OrderBy.Ascending, string[]? includes = null) => _context.Set<T>().Where(criteria).Where(criteria2).OrderBy(orderBy).AsNoTracking().FirstOrDefault();

        public T Find(Expression<Func<T, bool>> criteria, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.SingleOrDefault(criteria);
        }
        public async Task<T> FindAsyncWithThenInclude(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>().AsQueryable();

            if (includes != null && includes.Any())
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.SingleOrDefaultAsync(criteria);
        }
        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria) => await _context.Set<T>().SingleOrDefaultAsync(criteria);
        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return await query.SingleOrDefaultAsync(criteria);
        }
        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria) => _context.Set<T>().Where(criteria).ToList();

        public IEnumerable<T> FindAlike(Expression<Func<T, bool>> criteria, string[]? includes = null)
        {
            return _context.Set<T>().Where(criteria).AsEnumerable();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.Where(criteria).ToList();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int skip, int take)
        {
            return _context.Set<T>().Where(criteria).Skip(skip).Take(take).ToList();
        }
        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? skip, int? take,
            Expression<Func<T, object>>? orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return query.ToList();
        }
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria) => await _context.Set<T>().Where(criteria).ToListAsync();
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.Where(criteria).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int take, int skip)
        {
            return await _context.Set<T>().Where(criteria).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? take, int? skip,
            Expression<Func<T, object>>? orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return await query.ToListAsync();
        }

        public IEnumerable<T> FindAllInclude(string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.ToList();
        }
        public IEnumerable<T> FindAllInclude(Expression<Func<T, object>>? orderBy, int skip, int take, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.OrderByDescending(orderBy).Skip(skip).Take(take).ToList();
        }
        public IEnumerable<T> FindAllInclude(int skip, int take, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.Skip(skip).Take(take).ToList();
        }
        public virtual IQueryable<T> GetAllNolist()
        {
            return _context.Set<T>().AsNoTracking();
        }
        public T Add(T entity)
        {
            //entity
            _context.Set<T>().Add(entity);
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {

            await _context.Set<T>().AddAsync(entity);
            //entity.Property(propertyName).CurrentValue = someValue;
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            return entities;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public T Update(T entity)
        {
            //_context.Set<T>().AsNoTracking().ExecuteUpdate(entity);
            _context.Update(entity);
            return entity;
        }
        public IEnumerable<T> UpdateRange(IEnumerable<T> entities)
        {
            _context.UpdateRange(entities);
            return entities;
        }
        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void Attach(T entity)
        {
            _context.Set<T>().Attach(entity);
        }

        public void AttachRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AttachRange(entities);
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().Count(criteria);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>().CountAsync(criteria);
        }

        public async Task<IEnumerable<T>> GetChildsAsync(Expression<Func<T, bool>> include, Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>()
                      .Include(include)
                      .Where(criteria).ToListAsync();
        }
        public bool IsExist(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().AsNoTracking().Any(criteria);
        }
        public void Commit()
        {
            _context.Database.CommitTransaction();

        }

        public void RollBack()
        {
            _context.Database.RollbackTransaction();

        }

        public IQueryable<T> GetTableAsTracking()
        {
            return _context.Set<T>().AsQueryable();

        }
        public IQueryable<T> GetTableNoTracking()
        {
            return _context.Set<T>().AsNoTracking().AsQueryable();
        }
        #region Truy vấn theo điều kiện
        /*
         * Tác giả: Công VM
        Sử dụng phiên bản cơ bản:
        csharpCopyvar result = FilterData(q => q.Where(e => e.IsActive), gridRequest, ref totalRecords);

        Sử dụng phiên bản với hai tham số generic:
        csharpCopyvar result = FilterData<RelatedEntity>(q => q.Include(e => e.RelatedEntities), gridRequest, ref totalRecords);

        Sử dụng phiên bản với ba tham số generic:
        csharpCopyvar result = FilterData<RelatedEntity1, RelatedEntity2>(
           q => q.Include(e => e.RelatedEntities1).Include(e => e.RelatedEntities2),
           gridRequest,
           ref totalRecords
        );

        Sử dụng phiên bản với bốn tham số generic:
        csharpCopyvar result = FilterData<RelatedEntity1, RelatedEntity2, RelatedEntity3>(
           q => q.Include(e => e.RelatedEntities1)
                 .Include(e => e.RelatedEntities2)
                 .Include(e => e.RelatedEntities3),
           gridRequest,
           ref totalRecords
        */
        public IQueryable<T> FilterData(Func<IQueryable<T>, IQueryable<T>> filterFunc, GridRequest gridRequest, ref int TotalRecords)
        {

            var query = _context.Set<T>().AsQueryable();
            if (filterFunc != null)
            {
                query = filterFunc(query);
            }

            // Áp dụng bộ lọc và sắp xếp phân trang
            query = GetByGridRequest<T, DefaultClass, DefaultClass, DefaultClass>(query, gridRequest, ref TotalRecords);

            return query;
        }
        public IQueryable<T> FilterData<O>(Func<IQueryable<T>, IQueryable<T>> filterFunc, GridRequest gridRequest, ref int totalRecords)
        {
            var query = _context.Set<T>().AsQueryable();
            query = GetByGridRequest<T, O, DefaultClass, DefaultClass>(query, gridRequest, ref totalRecords);
            return query;
        }
        public IQueryable<T> FilterData<U, V>(Func<IQueryable<T>, IQueryable<T>> filterFunc, GridRequest gridRequest, ref int totalRecords)
        {
            var query = _context.Set<T>().AsQueryable();
            query = GetByGridRequest<T, U, V, DefaultClass>(query, gridRequest, ref totalRecords);
            return query;
        }

        public IQueryable<T> FilterData<U, V, W>(Func<IQueryable<T>, IQueryable<T>> filterFunc, GridRequest gridRequest, ref int totalRecords)
        {
            var query = _context.Set<T>().AsQueryable();
            query = GetByGridRequest<T, U, V, W>(query, gridRequest, ref totalRecords);
            return query;
        }
        #endregion
        private IQueryable<T> GetByGridRequest<C, O, N, G>(IQueryable<T> source, GridRequest request, ref int totalRecords)
        {
            Expression methodCallExpression = source.Expression;
            LambdaExpression lambda;
            List<ParameterExpression> lstParameter = new List<ParameterExpression>();
            lstParameter.Add(Expression.Parameter(typeof(C), "objT"));
            lstParameter.Add(Expression.Parameter(typeof(O), "objU"));
            lstParameter.Add(Expression.Parameter(typeof(N), "objV"));
            lstParameter.Add(Expression.Parameter(typeof(G), "objW"));
            List<Type> lstTypeObject = new List<Type>();
            lstTypeObject.Add(typeof(C));
            lstTypeObject.Add(typeof(O));
            lstTypeObject.Add(typeof(N));
            lstTypeObject.Add(typeof(G));

            List<MethodInfo> lstMethodAny = new List<MethodInfo>();
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            #region dùng trong tìm kiếm

            if (request.filter != null && request.filter.Filters != null)
            {
                List<Expression> lstExpression = new List<Expression>();
                foreach (Filter currentfilter in request.filter.Filters)
                {
                    Expression temp = GetExpressionDeQuy<C, O, N, G>(currentfilter, lstParameter, lstTypeObject);
                    if (temp != null)
                        lstExpression.Add(temp);
                }
                if (lstExpression.Count > 0)
                {
                    Expression combinedExpression = lstExpression[0];

                    for (int i = 1; i < lstExpression.Count; i++)
                    {
                        if (request.filter.Logic == "or")
                        {
                            combinedExpression = Expression.OrElse(combinedExpression, lstExpression[i]);
                        }
                        else if (request.filter.Logic == "and")
                        {
                            combinedExpression = Expression.AndAlso(combinedExpression, lstExpression[i]);
                        }
                    }

                    var predicate = Expression.Lambda<Func<T, bool>>(combinedExpression, lstParameter[0]);
                    methodCallExpression = Expression.Call(
                        typeof(Queryable), "Where",
                        new Type[] { source.ElementType },
                        methodCallExpression, Expression.Quote(predicate));
                }
            }
            source = source.Provider.CreateQuery<T>(methodCallExpression);
            #endregion

            #region dùng trong sắp xếp
            if (!(request.sort != null && request.sort.Count > 0))
            {
                List<Sort> lstSort = new List<Sort>() { new Sort()
                {
                    dir = "asc",
                    field = "Id"
                }};
                request.sort = lstSort;
            }
            string propertyName = request.sort[0].field;
            string methodName = request.sort[0].dir == "asc" ? "OrderBy" : "OrderByDescending";
            string[] tmpArraySortField = propertyName.Split('.');
            ParameterExpression parameter;
            MemberExpression tmpProperty;
            if (tmpArraySortField.Length == 1)
            {
                parameter = Expression.Parameter(source.ElementType, string.Empty);
                tmpProperty = Expression.Property(parameter, propertyName);
                lambda = Expression.Lambda(tmpProperty, parameter);
                methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                                new Type[] { source.ElementType, tmpProperty.Type },
                                                source.Expression, Expression.Quote(lambda));
                source = source.Provider.CreateQuery<T>(methodCallExpression);
            }
            else
            {
                parameter = Expression.Parameter(source.ElementType, string.Empty);
                tmpProperty = Expression.Property(parameter, tmpArraySortField[0]);
                lambda = Expression.Lambda(tmpProperty, parameter);
                methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                                new Type[] { source.ElementType, tmpProperty.Type },
                                                source.Expression, Expression.Quote(lambda));
                source = source.Provider.CreateQuery<T>(methodCallExpression);
            }
            for (int iii = 1; iii < request.sort.Count; iii++)
            {
                propertyName = request.sort[iii].field;
                methodName = request.sort[iii].dir == "asc" ? "ThenBy" : "ThenByDescending";

                parameter = Expression.Parameter(source.ElementType, string.Empty);
                tmpProperty = Expression.Property(parameter, propertyName);
                lambda = Expression.Lambda(tmpProperty, parameter);
                methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                                new Type[] { source.ElementType, tmpProperty.Type },
                                                source.Expression, Expression.Quote(lambda));
                source = source.Provider.CreateQuery<T>(methodCallExpression);
            }
            #endregion


            #region dùng trong phân trang

            totalRecords = source.Count();

            if (request.page > 0 && request.pageSize > 0)
            {

                methodCallExpression = Expression.Call(
                    typeof(Queryable), "Skip",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Constant((request.page - 1) * request.pageSize));
                source = source.Provider.CreateQuery<T>(methodCallExpression);

                methodCallExpression = Expression.Call(
                    typeof(Queryable), "Take",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Constant(request.pageSize));
                source = source.Provider.CreateQuery<T>(methodCallExpression);
            }

            #endregion


            return source;
        }

        private Expression GetExpressionDeQuy<C, O, N, G>(Filter currentfilter, List<ParameterExpression> lstParameter, List<Type> lstTypeObject)
        {
            Expression? result = null;
            if (string.IsNullOrEmpty(currentfilter.Logic))
            {
                #region So sánh trực tiếp
                List<string>? lstField = currentfilter?.Field?.Split('.').ToList();
                string? strValue = currentfilter?.Value?.Trim();
                string? strPhuongThuc = currentfilter?.Method;
                if (lstField?.Count == 1)
                {
                    MemberExpression express = Expression.Property(lstParameter[0], currentfilter?.Field);
                    Type typeField = express.Type;
                    Expression temp = GetExpresionByType(typeField, express, strValue, strPhuongThuc);
                    if (temp != null)
                        result = temp;
                }
                else if (lstField?.Count > 1)
                {
                    #region Trường hợp any
                    ParameterExpression parameterFieldCompare = lstParameter[lstField.Count - 1];
                    MemberExpression express = Expression.Property(parameterFieldCompare, lstField[lstField.Count - 1]);
                    Type typeField = express.Type;
                    Expression temp = GetExpresionByType(typeField, express, strValue, strPhuongThuc);
                    result = AddQueryAny<C, O, N, G>(temp, lstParameter, lstTypeObject, lstField);
                    #endregion
                }
                #endregion
            }
            else
            {
                #region Điều kiện lồng
                List<Expression> lstExpression = new List<Expression>();
                foreach (Filter childFilter in currentfilter.Filters)
                {
                    Expression temp = GetExpressionDeQuy<C, O, N, G>(childFilter, lstParameter, lstTypeObject);
                    if (temp != null)
                        lstExpression.Add(temp);
                }

                if (lstExpression.Count > 0)
                {
                    BinaryExpression epSum;
                    if (currentfilter.Logic == "or")
                    {
                        if (lstExpression.Count == 1)
                            epSum = Expression.OrElse(lstExpression[0], lstExpression[0]);
                        else
                        {
                            int countExpression = 2;
                            epSum = Expression.OrElse(lstExpression[0], lstExpression[1]);
                            while (countExpression < lstExpression.Count)
                            {
                                epSum = Expression.OrElse(epSum, lstExpression[countExpression]);
                                countExpression++;
                            }
                        }
                        result = epSum;
                    }
                    else
                    {
                        if (lstExpression.Count == 1)
                            epSum = Expression.And(lstExpression[0], lstExpression[0]);
                        else
                        {
                            int countExpression = 2;
                            epSum = Expression.And(lstExpression[0], lstExpression[1]);
                            while (countExpression < lstExpression.Count)
                            {
                                epSum = Expression.And(epSum, lstExpression[countExpression]);
                                countExpression++;
                            }
                        }
                        result = epSum;
                    }
                }
                #endregion
            }
            return result;
        }

        private Expression? AddQueryAny<C, O, N, G>(Expression expresionLast, List<ParameterExpression> lstParameter, List<Type> lstTypeObject, List<string> lstField)
        {
            MethodCallExpression? result = null;
            for (int i = lstField.Count - 2; i >= 0; i--)
            {
                ParameterExpression parameterFieldCompare = lstParameter[i];
                MemberExpression express = Expression.Property(parameterFieldCompare, lstField[i]);
                MethodInfo method = typeof(Enumerable).
                                    GetMethods().
                                    Where(x => x.Name == "Any").
                                    Single(x => x.GetParameters().Length == 2).
                                    MakeGenericMethod(lstTypeObject[lstField.Count - i - 1]);
                LambdaExpression? lambda = null;

                if (result == null)
                {
                    if (i == 0)
                        lambda = Expression.Lambda<Func<O, bool>>(expresionLast, lstParameter[lstField.Count - i - 1]);
                    else if (i == 1)
                        lambda = Expression.Lambda<Func<N, bool>>(expresionLast, lstParameter[lstField.Count - i - 1]);
                    if (i == 2)
                        lambda = Expression.Lambda<Func<G, bool>>(expresionLast, lstParameter[lstField.Count - i - 1]);
                }
                else
                {
                    if (i == 0)
                        lambda = Expression.Lambda<Func<O, bool>>(result, lstParameter[lstField.Count - i - 1]);
                    else if (i == 1)
                        lambda = Expression.Lambda<Func<N, bool>>(result, lstParameter[lstField.Count - i - 1]);
                    if (i == 2)
                        lambda = Expression.Lambda<Func<G, bool>>(result, lstParameter[lstField.Count - i - 1]);
                }
                result = Expression.Call(method, express, lambda);
            }
            return result;
        }

        private Expression GetExpresionByType(Type typeField, MemberExpression express, string strValue, string strPhuongThuc)
        {
            if (typeField == typeof(long)
                                || typeField == typeof(int)
                                || typeField == typeof(long?)
                                || typeField == typeof(int?)
                                || typeField == typeof(double)
                                || typeField == typeof(double?)
                                || typeField == typeof(decimal)
                                || typeField == typeof(decimal?))
            {
                return AddQueryNumeric(express, strValue, strPhuongThuc);
            }
            else if (typeField == typeof(DateTime) || typeField == typeof(DateTime?))
            {
                return AddQueryDateTime(express, strValue, strPhuongThuc);
            }
            else if (typeField == typeof(string))
            {
                return AddQueryString(express, strValue, strPhuongThuc);
            }
            return null;
        }

        private Expression AddQueryString(MemberExpression propertyField, string strValue, string strPhuongThuc)
        {
            var termConstant = Expression.Constant(strValue, typeof(string)); // = "value"
            var ToLower = Expression.Call(propertyField, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
            var StartWith = Expression.Call(ToLower, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), termConstant); // obj => obj.ToLower().StartsWith();
            var Contains = Expression.Call(ToLower, typeof(string).GetMethod("Contains", new[] { typeof(string) }), termConstant); // obj => obj.ToLower().Contains();
            var Equals = Expression.Call(ToLower, typeof(string).GetMethod("Equals", new[] { typeof(string) }), termConstant); // obj => obj.ToLower().Equals();
            var EndsWith = Expression.Call(ToLower, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), termConstant); // obj => obj.ToLower().EndWith();

            if (strPhuongThuc == "contains")
                return Contains;
            else if (strPhuongThuc == "startswith")
                return StartWith;
            else if (strPhuongThuc == "endswith")
                return EndsWith;
            else if (strPhuongThuc == "lt" || strPhuongThuc == "gt")
                return null;
            else
                return Equals;
        }

#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8604 // Possible null reference argument.
        private Expression AddQueryDateTime(MemberExpression propertyField, string strValue, string strPhuongThuc)
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";
            dtfi.DateSeparator = "/";
            Type typeField = propertyField.Type;
            DateTime? Date_value_date = Convert.ToDateTime(strValue, dtfi);
            ConstantExpression Date_values;
            if (typeField == typeof(DateTime?))
                Date_values = Expression.Constant(Date_value_date, typeof(DateTime?));
            else
                Date_values = Expression.Constant(Date_value_date, typeof(DateTime));

            var Date_eq = Expression.Equal(propertyField, Date_values);
            var Date_neq = Expression.NotEqual(propertyField, Date_values);
            var Date_gte = Expression.GreaterThanOrEqual(propertyField, Date_values);
            var Date_gt = Expression.GreaterThan(propertyField, Date_values);
            var Date_lte = Expression.LessThanOrEqual(propertyField, Date_values);
            var Date_lt = Expression.LessThan(propertyField, Date_values);
            if (strPhuongThuc == "eq")
                return Date_eq;
            else if (strPhuongThuc == "neq")
                return Date_neq;
            else if (strPhuongThuc == "gte")
                return Date_gte;
            else if (strPhuongThuc == "gt")
                return Date_gt;
            else if (strPhuongThuc == "lte")
                return Date_lte;
            else
                return Date_lt;
        }

        private Expression AddQueryNumeric(MemberExpression propertyField, string strValue, string strPhuongThuc)
        {
            ConstantExpression Int_values;
            Type typeField = propertyField.Type;
            if (typeField == typeof(double) || typeField == typeof(double?))
                Int_values = Expression.Constant(Convert.ToDouble(strValue));
            else if (typeField == typeof(decimal) || typeField == typeof(decimal?))
                Int_values = Expression.Constant(Convert.ToDecimal(strValue));
            else if (typeField == typeof(long))
                Int_values = Expression.Constant(Convert.ToInt64(strValue)); // = "value" + Kiểu giá trị
            else
                Int_values = Expression.Constant(Convert.ToInt32(strValue)); // = "value" + Kiểu giá trị
            var Int_eq = Expression.Equal(propertyField, Expression.Convert(Int_values, propertyField.Type));
            var Int_neq = Expression.NotEqual(propertyField, Expression.Convert(Int_values, propertyField.Type));
            var Int_gte = Expression.GreaterThanOrEqual(propertyField, Expression.Convert(Int_values, propertyField.Type));
            var Int_gt = Expression.GreaterThan(propertyField, Expression.Convert(Int_values, propertyField.Type));
            var Int_lte = Expression.LessThanOrEqual(propertyField, Expression.Convert(Int_values, propertyField.Type));
            var Int_lt = Expression.LessThan(propertyField, Expression.Convert(Int_values, propertyField.Type));
            if (strPhuongThuc == "eq")
                return Int_eq;
            else if (strPhuongThuc == "neq")
                return Int_neq;
            else if (strPhuongThuc == "gte")
                return Int_gte;
            else if (strPhuongThuc == "gt")
                return Int_gt;
            else if (strPhuongThuc == "lte")
                return Int_lte;
            else
                return Int_lt;
        }

        public async Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().AnyAsync(predicate);
        }

        public async Task SaveChangesAsync(Guid UserId = default, Guid departmentId = default)
        {
            await _context.SaveChangesAsync(UserId, departmentId);
        }

        public void SaveChanges(Guid UserId = default, Guid departmentId = default)
        {
            _context.SaveChanges(UserId, departmentId);
        }

        public string GetQueryString(IQueryable<T> query)
        {
            return query.ToQueryString();
        }
        public async Task<List<T>> ToListAsync(IQueryable<T> query)
        {
            return await query.ToListAsync();
        }
    }
}
