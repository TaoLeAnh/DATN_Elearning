using Elearning.Shared.Commons.Extensions;
using Elearning.Shared.Commons.Querys.Grid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Elearning.Shared.Commons.Querys.SQL
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Lấy danh sách bản ghi theo trang và sắp xếp
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <param name="strColumnName"></param>
        /// <param name="strOrder"></param>
        /// <param name="rowsCount"></param>
        /// <returns></returns> 
        public static IQueryable<T> GetPaged<T>(this IQueryable<T> query,
                           int pageNum, int pageSize,
                          string strColumnName,
                           string strOrder, ref int rowsCount)
        {
            string strOrderBy = "";
            if (string.IsNullOrEmpty(strColumnName))
            {
                strOrderBy = "CreatedDate DESC";
            }
            else
            {
                strOrderBy = strColumnName + " ";
                if (!string.IsNullOrEmpty(strOrder) && strOrder != "0")
                {
                    strOrderBy += "DESC";
                }
                else
                    strOrderBy += "ASC";
            }

            if (pageSize <= 0) pageSize = 20;

            rowsCount = query.Count();

            if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            int excludedRows = (pageNum - 1) * pageSize;
            return query.OrderBy(strOrderBy).Skip(excludedRows).Take(pageSize);
        }

        public static IQueryable<T> GetPaged<T>(this IQueryable<T> query, IQueryable<int> queryCount,
                               int pageNum, int pageSize,
                              string strColumnName,
                               string strOrder, ref int rowsCount)
        {
            string strOrderBy = "";
            if (string.IsNullOrEmpty(strColumnName))
            {
                strOrderBy = "CreatedDate DESC";
            }
            else
            {
                strOrderBy = strColumnName + " ";
                if (!string.IsNullOrEmpty(strOrder) && strOrder != "0")
                {
                    strOrderBy += "DESC";
                }
                else
                    strOrderBy += "ASC";
            }

            if (pageSize <= 0) pageSize = 20;

            rowsCount = queryCount.Count();

            if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            int excludedRows = (pageNum - 1) * pageSize;

            return query.OrderBy(strOrderBy).Skip(excludedRows).Take(pageSize);
        }
        /// <summary>
        /// Lấy danh sách bản ghi theo trang và sắp xếp
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
        /// <param name="query">Quy vấn</param>
        /// <param name="pageNum">Trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi trên trang</param>
        /// <param name="rowsCount">Tổng số bản ghi</param>
        /// <returns></returns>       
        public static List<T> GetPaged<T>(this IQueryable<T> query,
                           int pageNum, int pageSize, ref int rowsCount)
        {


            if (pageSize <= 0) pageSize = 20;

            rowsCount = query.Count();

            if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            int excludedRows = (pageNum - 1) * pageSize;

            return query.Skip(excludedRows).Take(pageSize).ToList();
        }


        /// <summary>
        /// Lấy danh sách bản ghi theo trang và sắp xếp
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
        /// <param name="query">Quy vấn</param>
        /// <param name="pageNum">Trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi trên trang</param>
        /// <param name="rowsCount">Tổng số bản ghi</param>
        /// <returns></returns>       
        public static IQueryable<T> GetQuery<T>(this IQueryable<T> query,
                           int pageNum, int pageSize, ref int rowsCount)
        {


            if (pageSize <= 0) pageSize = 20;

            rowsCount = query.Count();

            if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            int excludedRows = (pageNum - 1) * pageSize;

            return query.Skip(excludedRows).Take(pageSize);
        }

        public static IQueryable<T> GetPaged<T, TResult>(this IQueryable<T> query,
                            int pageNum, int pageSize,
                            Func<T, TResult> orderByProperty,
                            bool isAscendingOrder, ref int rowsCount)
        {
            if (pageSize <= 0) pageSize = 20;

            rowsCount = query.Count();

            if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            int excludedRows = (pageNum - 1) * pageSize;

            //if (isAscendingOrder)
            //    query = query.OrderBy(orderByProperty);
            //else
            //    query = query.OrderByDescending(orderByProperty);

            return query.Skip(excludedRows).Take(pageSize);
        }

        /// <summary> 
        /// Truy vấn Order by
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string propertyName)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            // DataSource control passes the sort parameter with a direction
            // if the direction is descending          
            int descIndex = propertyName.IndexOf(" DESC");
            if (descIndex >= 0)
            {
                propertyName = propertyName.Substring(0, descIndex).Trim();
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                return source;
            }

            ParameterExpression parameter = Expression.Parameter(source.ElementType, string.Empty);
            MemberExpression property = Expression.Property(parameter, propertyName);
            LambdaExpression lambda = Expression.Lambda(property, parameter);

            string methodName = descIndex < 0 ? "OrderBy" : "OrderByDescending";

            Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                                new Type[] { source.ElementType, property.Type },
                                                source.Expression, Expression.Quote(lambda));
            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

        /// <summary>
        /// Kiểm tra trùng dữ liệu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="arrFieldName"></param>
        /// <param name="arrFieldValue"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static IQueryable<T> CheckExitsEx<T>(this IQueryable<T> source, string[] arrFieldName, string[] arrFieldValue, string ID = "")
        {
            // Kiểm tra các mảng đầu vào có null hoặc rỗng không
            if (arrFieldName == null || arrFieldValue == null || arrFieldName.Length != arrFieldValue.Length)
            {
                return source; // Trả về source nếu không có dữ liệu để xử lý
            }

            Expression methodCallExpression = source.Expression;
            ParameterExpression parameter = Expression.Parameter(typeof(T), "obj");
            MemberExpression property;
            LambdaExpression lambda;
            Expression? left = null, right = null, exField = null, predicateBody = null;

            // Duyệt qua các trường trong arrFieldName và arrFieldValue
            for (int i = 0; i < arrFieldName.Length; i++)
            {
                if (string.IsNullOrEmpty(arrFieldName[i]) || arrFieldValue[i] == null)
                    continue; // Bỏ qua nếu tên trường hoặc giá trị là null

                property = Expression.Property(parameter, arrFieldName[i]);

                // Kiểm tra nếu thuộc tính có phương thức ToLower
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                if (toLowerMethod == null) return source; // Tránh lỗi nếu không có phương thức ToLower.

                left = Expression.Call(property, toLowerMethod);
                right = Expression.Constant(arrFieldValue[i], typeof(string)); // = "value"                            
                exField = Expression.Equal(left, right);

                // Kiểm tra nếu predicateBody đã có giá trị, nếu chưa thì gán exField vào
                predicateBody = predicateBody == null ? exField : Expression.And(predicateBody, exField);
            }

            // Kiểm tra ID có null hoặc rỗng không
            if (!string.IsNullOrEmpty(ID))
            {
                property = Expression.Property(parameter, "ID");

                // Kiểm tra nếu thuộc tính có phương thức ToLower
                var toLowerMethodID = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                if (toLowerMethodID == null) return source; // Tránh lỗi nếu không có phương thức ToLower.

                left = Expression.Call(property, toLowerMethodID);
                right = Expression.Constant(ID, typeof(string)); // = "value"     
                Expression exID = Expression.NotEqual(left, right);

                // Kiểm tra nếu predicateBody đã có giá trị, nếu chưa thì gán exID vào
                predicateBody = predicateBody == null ? exID : Expression.And(predicateBody, exID);
            }

            // Tạo LambdaExpression và Expression.Where
            if (predicateBody == null)
            {
                return source; // Trả về source nếu không có biểu thức nào để áp dụng
            }

            lambda = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);
            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { source.ElementType },
                methodCallExpression,
                Expression.Quote(lambda)
            );

            // Trả về kết quả sau khi áp dụng phương thức Where
            IQueryable<T> results = source.Provider.CreateQuery<T>(whereCallExpression);
            return results;
        }



        public static IQueryable<T> GetByGridRequest<T, C, O, N, G>(IQueryable<T> source, GridRequest request, ref int totalRecords)
        {
            Expression methodCallExpression = source.Expression;
            LambdaExpression lambda;
            List<ParameterExpression> lstParameter = new List<ParameterExpression>();
            lstParameter.Add(Expression.Parameter(typeof(T), "objT"));
            lstParameter.Add(Expression.Parameter(typeof(C), "objC"));
            lstParameter.Add(Expression.Parameter(typeof(O), "objU"));
            lstParameter.Add(Expression.Parameter(typeof(N), "objV"));
            lstParameter.Add(Expression.Parameter(typeof(G), "objW"));
            List<Type> lstTypeObject = new List<Type>();
            lstTypeObject.Add(typeof(T));
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
                    Expression temp = GetExpressionDeQuy<T, C, O, N, G>(currentfilter, lstParameter, lstTypeObject);
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

        public static Expression GetExpressionDeQuy<T, C, O, N, G>(Filter currentfilter, List<ParameterExpression> lstParameter, List<Type> lstTypeObject)
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
                    if (string.IsNullOrEmpty(currentfilter?.Field))
                        throw new InvalidInputException("Có 1 field trống. Tìm kiếm không được");



                    MemberExpression express = Expression.Property(lstParameter[0], currentfilter.Field);
                    Type typeField = express.Type;
                    Expression? temp = GetExpresionByType(typeField, express, strValue, strPhuongThuc);
                    if (temp != null)
                        result = temp;
                }
                else if (lstField?.Count > 1)
                {
                    #region Trường hợp any
                    ParameterExpression parameterFieldCompare = lstParameter[lstField.Count - 1];
                    MemberExpression express = Expression.Property(parameterFieldCompare, lstField[lstField.Count - 1]);
                    Type typeField = express.Type;
                    Expression? temp = GetExpresionByType(typeField, express, strValue, strPhuongThuc);
                    result = AddQueryAny<T, C, O, N, G>(temp!, lstParameter, lstTypeObject, lstField);
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
                    Expression temp = GetExpressionDeQuy<T, C, O, N, G>(childFilter, lstParameter, lstTypeObject);
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
            return result!;
        }

        public static Expression? AddQueryAny<T, C, O, N, G>(Expression expresionLast, List<ParameterExpression> lstParameter, List<Type> lstTypeObject, List<string> lstField)
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
                result = Expression.Call(method, express, lambda!);
            }
            return result;
        }

        public static Expression? GetExpresionByType(Type typeField, MemberExpression express, string? strValue, string? strPhuongThuc = "contains")
        {

            //// Kiểm tra fulltext search trước (áp dụng cho mọi kiểu string)
            //if (strPhuongThuc?.ToLower() == "fulltext" && typeField == typeof(string))
            //{
            //    return AddQueryFullText(express, strValue);
            //}

            if (string.IsNullOrWhiteSpace(strValue))
                return null;
            if (string.IsNullOrWhiteSpace(strPhuongThuc))
                return null;

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

        public static Expression AddQueryString(MemberExpression propertyField, string strValue, string strPhuongThuc)
        {



            var termConstant = Expression.Constant(strValue, typeof(string)); // = "value"
            var ToLower = Expression.Call(propertyField, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);
            var StartWith = Expression.Call(ToLower, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, termConstant); // obj => obj.ToLower().StartsWith();
            var Contains = Expression.Call(ToLower, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, termConstant); // obj => obj.ToLower().Contains();
            var Equals = Expression.Call(ToLower, typeof(string).GetMethod("Equals", new[] { typeof(string) })!, termConstant); // obj => obj.ToLower().Equals();
            var EndsWith = Expression.Call(ToLower, typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!, termConstant); // obj => obj.ToLower().EndWith();

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
        public static Expression AddQueryDateTime(MemberExpression propertyField, string strValue, string strPhuongThuc)
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

        public static Expression AddQueryNumeric(MemberExpression propertyField, string strValue, string strPhuongThuc)
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



        private static Expression? AddQueryFullText(MemberExpression propertyField, string? strValue)
        {
            if (string.IsNullOrWhiteSpace(strValue))
                return Expression.Constant(true);

            // Trả về expression cho Raw SQL thông qua SqlCommand
            // Tạo một marker để sau này replace bằng CONTAINS
            var markerConstant = Expression.Constant($"__FULLTEXT_MARKER__{strValue}__", typeof(string));
            return Expression.Equal(propertyField, markerConstant);
        }

        // Extension method để xử lý Full-Text riêng
        public static string ConvertToFullTextQuery(this string originalQuery, Dictionary<string, string> fullTextFields)
        {
            foreach (var field in fullTextFields)
            {
                var marker = $"__FULLTEXT_MARKER__{field.Value}__";
                var fullTextReplace = $"CONTAINS({field.Key}, '{field.Value}')";
                originalQuery = originalQuery.Replace($"[{field.Key}] = N'{marker}'", fullTextReplace);
            }
            return originalQuery;
        }


        private static readonly MethodInfo ContainsMethod =
            typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        private static readonly MethodInfo ToLowerMethod =
    typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!; // Đổi từ ToLowerInvariant thành ToLower

        private static Expression? BuildContainsExpression(
            MemberExpression propertyField,
            string strValue,
            bool checkAnyWord = false)
        {
            // field.ToLower() - thay vì ToLowerInvariant()
            var toLowerField = Expression.Call(propertyField, ToLowerMethod);

            if (!checkAnyWord)
            {
                var keywordConstant = Expression.Constant(strValue.ToLower(), typeof(string)); // Đổi ToLowerInvariant thành ToLower
                return Expression.Call(toLowerField, ContainsMethod, keywordConstant);
            }

            // Tách từ khóa
            var keywords = strValue
                .ToLower() // Đổi ToLowerInvariant thành ToLower
                .Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            if (keywords.Length == 0)
                return Expression.Constant(true);

            // Build chuỗi AndAlso
            return keywords
                .Select(k => Expression.Call(toLowerField, ContainsMethod, Expression.Constant(k)))
                .Aggregate<Expression>((acc, next) => Expression.AndAlso(acc, next));
        }


    }
}
