using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Elearning.Publising.Infrastructure.Persistence
{
    public static class OrderByHelper
    {
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string orderBy)
        {
            return enumerable.AsQueryable().OrderBy(orderBy).AsEnumerable();
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> collection, string orderBy)
        {
            foreach (OrderByInfo orderByInfo in ParseOrderBy(orderBy))
                collection = ApplyOrderBy(collection, orderByInfo);

            return collection;
        }

        private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> collection, OrderByInfo orderByInfo)
        {

            // Kiểm tra các tham số null
            if (collection == null || orderByInfo == null || string.IsNullOrEmpty(orderByInfo.PropertyName))
            {
                return collection ?? Enumerable.Empty<T>().AsQueryable();
            }

            string[] props = orderByInfo.PropertyName.Split('.');
            Type type = typeof(T);
            if (type is null)
                return collection;


            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;


            foreach (string prop in props)
            {
                PropertyInfo? pi = type.GetProperty(prop);
                if (pi == null)
                {
                    return collection;
                }

                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            string methodName = string.Empty;

            // Kiểm tra và xác định phương thức OrderBy hoặc ThenBy
            if (!orderByInfo.Initial && collection is IOrderedQueryable<T>)
            {
                if (orderByInfo.Direction == SortDirection.Ascending)
                    methodName = "ThenBy";
                else
                    methodName = "ThenByDescending";
            }
            else
            {
                if (orderByInfo.Direction == SortDirection.Ascending)
                    methodName = "OrderBy";
                else
                    methodName = "OrderByDescending";
            }

            // Áp dụng phương thức OrderBy hoặc ThenBy
            var method = typeof(Queryable).GetMethods()
                .SingleOrDefault(m => m.Name == methodName
                                    && m.IsGenericMethodDefinition
                                    && m.GetGenericArguments().Length == 2
                                    && m.GetParameters().Length == 2);

            if (method == null)
            {
                return collection; // Trả về collection nếu không tìm thấy phương thức hợp lệ
            }

            // Tạo và gọi phương thức OrderBy/ThenBy
            var result = method
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { collection, lambda });

            return result as IOrderedQueryable<T> ?? collection ?? Enumerable.Empty<T>().AsQueryable();
        }


        private static IEnumerable<OrderByInfo> ParseOrderBy(string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
                yield break;

            string[] items = orderBy.Split(',');
            bool initial = true;
            foreach (string item in items)
            {
                string[] pair = item.Trim().Split(' ');

                if (pair.Length > 2)
                    throw new ArgumentException(string.Format("Invalid OrderBy string '{0}'. Order By Format: Property, Property2 ASC, Property2 DESC", item));

                string prop = pair[0].Trim();

                if (string.IsNullOrEmpty(prop))
                    throw new ArgumentException("Invalid Property. Order By Format: Property, Property2 ASC, Property2 DESC");

                SortDirection dir = SortDirection.Ascending;

                if (pair.Length == 2)
                    dir = "desc".Equals(pair[1].Trim(), StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending;

                yield return new OrderByInfo() { PropertyName = prop, Direction = dir, Initial = initial };

                initial = false;
            }

        }

        private class OrderByInfo
        {
            public string? PropertyName { get; set; }
            public SortDirection Direction { get; set; }
            public bool Initial { get; set; }
        }

        private enum SortDirection
        {
            Ascending = 0,
            Descending = 1
        }
    }
}
