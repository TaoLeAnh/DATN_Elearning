using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Elearning.Shared.Commons.Extensions
{
    public static class DynamicExpressionHelper
    {
        public static Expression<Func<T, string?>> BuildStringPropertySelector<T>(string propertyName)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var prop = Expression.PropertyOrField(param, propertyName);

            // nếu property không phải string thì ép về string?
            if (prop.Type != typeof(string))
                throw new InvalidOperationException($"{propertyName} is not a string property");

            return Expression.Lambda<Func<T, string?>>(prop, param);
        }
    }
}
