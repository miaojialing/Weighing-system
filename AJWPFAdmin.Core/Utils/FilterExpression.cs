using Masuit.Tools.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AJWPFAdmin.Core.Utils
{
    public static class FilterExpression
    {

        internal class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return base.VisitParameter(_parameter);
            }

            internal ParameterReplacer(ParameterExpression parameter)
            {
                _parameter = parameter;
            }
        }

        /// <summary>
        /// 根据动态的字段和排序方式动态排序
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="field">要排序的字段</param>
        /// <param name="order">排序顺序: ascend descend</param>
        /// <returns></returns>
        public static IQueryable<T> DynamicOrderBy<T, K>(this IQueryable<T> sources,
            string field, Expression<Func<T, K>> defaultField, string order = "descend")
        {
            if (string.IsNullOrWhiteSpace(order))
            {
                order = "descend";
            }

            if (string.IsNullOrEmpty(field))
            {
                field = (defaultField.Body as MemberExpression).Member.Name;
            }
            var propInfo = typeof(T).GetRuntimeProperties().FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));
            if (propInfo == null)
            {
                throw new ArgumentException("字段参数错误或不存在于实体中", nameof(field));
            }

            var paramExp = Expression.Parameter(typeof(T), "p");

            var columnExpr = Expression.Property(paramExp, propInfo);

            var command = "OrderBy";

            if (order == "descend")
            {
                command = "OrderByDescending";
            }

            Expression resultExpression = null;

            var orderByExpression = Expression.Lambda(columnExpr, paramExp);

            // finally, call the "OrderBy" / "OrderByDescending" method with the order by lamba expression
            resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { typeof(T), propInfo.PropertyType },
               sources.Expression, Expression.Quote(orderByExpression));

            return sources.Provider.CreateQuery<T>(resultExpression);

        }

        /// <summary>
        /// 是否根据 condition 判断, 是正序(condition == true)还是倒序(condition == false)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="sources"></param>
        /// <param name="condition"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> IfOrderByASCOrDesc<T, TKey>(this IQueryable<T> sources,
            Func<bool> condition,
            Expression<Func<T, TKey>> keySelector)
        {
            if (condition())
            {
                return sources.OrderBy(keySelector);
            }
            return sources.OrderByDescending(keySelector);
        }

        /// <summary>
        /// 是否根据 condition 判断按 哪个字段倒序排序
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <typeparam name="TureKey">condition 为 true 时,排序的字段</typeparam>
        /// <typeparam name="FalseKey">condition 为 false 时,排序的字段</typeparam>
        /// <param name="sources"></param>
        /// <param name="condition"></param>
        /// <param name="keySelectorWhenTrue"></param>
        /// <param name="keySelectorWhenFalse"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> IfOrderByDescending<T, TureKey, FalseKey>(this IQueryable<T> sources,
            Func<bool> condition,
            Expression<Func<T, TureKey>> keySelectorWhenTrue,
            Expression<Func<T, FalseKey>> keySelectorWhenFalse)
        {
            if (condition())
            {
                return sources.OrderByDescending(keySelectorWhenTrue);
            }
            return sources.OrderByDescending(keySelectorWhenFalse);
        }

        /// <summary>
        /// 给定一组值, 筛选各个字段按顺序都等于values中的数据,如 p.Field == values[0] && p.Field == values[1]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="sources"></param>
        /// <param name="values"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static IQueryable<T> EqualAndEqual<T, K>(this IQueryable<T> sources, K[] values,
            params Expression<Func<T, K>>[] props)
        {
            if (values != null && values.Length > 0)
            {
                Expression andExp = null;

                for (int i = 0; i < values.Length; i++)
                {
                    var pname = props.ElementAtOrDefault(i);

                    if (pname == null)
                    {
                        continue;
                    }

                    var valExp = Expression.Constant(values[i]);

                    andExp = i > 0
                        ? Expression.AndAlso(andExp, Expression.Equal(pname.Body, valExp))
                        : Expression.Equal(pname.Body, valExp);

                    var nextPName = props.ElementAtOrDefault(i + 1);

                    if (nextPName != null)
                    {
                        var nextVal = values.ElementAtOrDefault(i + 1);
                        if (nextVal == null)
                        {
                            continue;
                        }
                        var nextValExp = Expression.Constant(nextVal);
                        var right = Expression.Equal(nextPName.Body, nextValExp);
                        andExp = Expression.AndAlso(andExp, right);
                        i += 1;
                    }
                }

                var param = props.LastOrDefault().Parameters.FirstOrDefault();

                andExp = new ParameterReplacer(param).Visit(andExp);

                sources = sources.Where(Expression.Lambda<Func<T, bool>>(andExp, param));
            }
            return sources;
        }
        public static IQueryable<T> IfWhere<T>(this IQueryable<T> sources, Func<bool> condition, Expression<Func<T, bool>> where)
        {
            if (condition())
            {
                sources = sources.Where(where);
            }
            return sources;
        }

        /// <summary>
        /// 扩展 IQueryable, 类似 SQL的 in 操作符
        /// 如果 selected 只有一个元素时, 会优化成 propName = x0 
        /// </summary>
        /// <typeparam name="T">数据源</typeparam>
        /// <typeparam name="K">列表类型</typeparam>
        /// <param name="sources"></param>
        /// <param name="selected"></param>
        /// <param name="propName"></param>
        /// <param name="splitCount">当 selected 超过 1000 时,分页的条数 </param>
        /// <returns></returns>
        public static IQueryable<T> In<T, K>(this IQueryable<T> sources, K[] selected,
            Expression<Func<T, K>> propName, int splitCount = 1000)
        {

            if (selected != null && selected.Length > 0)
            {
                Expression<Func<T, bool>> lambda = null;

                if (selected.Length == 1)
                {
                    lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(propName.Body, Expression.Constant(selected[0])), propName.Parameters);

                }
                else if (selected.Length <= 1000)
                {
                    var method = typeof(Enumerable).GetMethods()
                        .FirstOrDefault(info => info.GetParameters().Length == 2 && info.Name == "Contains");

                    var callExp = Expression.Call(null, method.MakeGenericMethod(typeof(K)),
                            Expression.Constant(selected), propName.Body);
                    lambda = Expression.Lambda<Func<T, bool>>(callExp, propName.Parameters);
                }
                else
                {
                    var page = Convert.ToInt32(Math.Ceiling(selected.Length / (double)splitCount));

                    var method = typeof(Enumerable).GetMethods().FirstOrDefault(info => info.GetParameters().Length == 2 && info.Name == "Contains");

                    Expression exp = null;

                    for (int i = 1; i <= page; i++)
                    {
                        var pagedIds = selected.ToPagedList(i, splitCount);

                        if (i == 1)
                        {
                            exp = Expression.Call(null, method.MakeGenericMethod(typeof(K)),
                            Expression.Constant(pagedIds.Data), propName.Body);
                        }
                        else
                        {
                            exp = Expression.OrElse(exp, Expression.Call(null,
                                method.MakeGenericMethod(typeof(K)),
                                Expression.Constant(pagedIds.Data), propName.Body));
                        }
                    }

                    lambda = Expression.Lambda<Func<T, bool>>(exp, propName.Parameters);
                }

                sources = sources.Where(lambda);

            }



            return sources;
        }

        /// <summary>
        /// 筛选布尔类型的表达式扩展, p.Field == (x == 1)
        /// </summary>
        /// <typeparam name="T">数据源</typeparam>
        /// <typeparam name="K">字段类型</typeparam>
        /// <param name="sources"></param>
        /// <param name="val"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static IQueryable<T> FilterBoolean<T, K>(this IQueryable<T> sources, int? val,
            Expression<Func<T, K>> propName) where K : struct
        {
            if (val.HasValue && new int[] { 0, 1 }.Contains(val.GetValueOrDefault()))
            {
                var expbody = Expression
                    .Equal(propName.Body,
                    Expression.Constant(val == 1));
                var exp = Expression.Lambda<Func<T, bool>>(expbody, propName.Parameters);

                sources = sources.Where(exp);
            }

            return sources;
        }

        /// <summary>
        /// 筛选枚举值, p.Field == x
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="J">实体字段类型</typeparam>
        /// <param name="sources"></param>
        /// <param name="val"></param>
        /// <param name="propName"></param>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static IQueryable<T> FilterEnum<T, J>(this IQueryable<T> sources,
            int? val,
            Expression<Func<T, J>> propName,
            Type enumType) where J : struct
        {
            if (val.HasValue && Enum.IsDefined(enumType, val))
            {
                var expbody = Expression
                    .Equal(propName.Body,
                    Expression.Constant(val)); // 构建 p=>p.field >= val
                var exp = Expression.Lambda<Func<T, bool>>(expbody, propName.Parameters);

                sources = sources.Where(exp);
            }

            return sources;
        }

        public static IQueryable<T> FilterEnum<T, J>(this IQueryable<T> sources,
            int? val,
            Expression<Func<T, J>> propName) where J : Enum
        {
            var emumType = typeof(J);
            if (val.HasValue && Enum.IsDefined(emumType, val))
            {
                var emumVal = Enum.Parse(emumType, val.ToString());
                var expbody = Expression
                    .Equal(propName.Body,
                    Expression.Constant(emumVal)); // 构建 p=>p.field >= val
                var exp = Expression.Lambda<Func<T, bool>>(expbody, propName.Parameters);

                sources = sources.Where(exp);
            }

            return sources;
        }

        /// <summary>
        /// 筛选多个字符串字段的或条件, 比如  name like '%test%' or phone like '%test%'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="sources"></param>
        /// <param name="val"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static IQueryable<T> LikeOrLike<T, K>(this IQueryable<T> sources, object val,
            params Expression<Func<T, K>>[] props)
        {
            if (val == null || props == null || props.Length == 0)
            {
                return sources;
            }

            if (val.GetType() == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(val.ToString()))
                {
                    return sources;
                }
            }

            Expression orExp = null;

            var containMethod = typeof(string).GetMethod(nameof(string.Contains),
                new Type[1] { typeof(string) });

            var valExp = Expression.Constant(val, val.GetType());

            for (int i = 0; i < props.Length; i++)
            {
                var pname = props[i];

                if (pname.ReturnType == typeof(string))
                {

                    orExp = i > 0
                        ? Expression.OrElse(orExp, Expression.Call(pname.Body, containMethod, valExp))
                        : Expression.Call(pname.Body, containMethod, valExp);
                }
                else
                {
                    orExp = i > 0
                        ? Expression.OrElse(orExp, Expression.Equal(pname.Body, valExp))
                        : Expression.Equal(pname.Body, valExp);
                }

                var nextPName = props.ElementAtOrDefault(i + 1);

                if (nextPName != null)
                {
                    Expression right = null;
                    if (nextPName.ReturnType == typeof(string))
                    {
                        right = Expression.Call(nextPName.Body, containMethod, valExp);
                    }
                    else
                    {
                        right = Expression.Equal(nextPName.Body, valExp);
                    }
                    orExp = Expression.OrElse(orExp, right);
                    i += 1;
                }
            }

            var param = props.LastOrDefault().Parameters.FirstOrDefault();

            orExp = new ParameterReplacer(param).Visit(orExp);

            sources = sources.Where(Expression.Lambda<Func<T, bool>>(orExp, param));

            return sources;
        }

        /// <summary>
        /// 筛选日期范围, p.Field 大于等于 start and p.Field 小于等于 end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sources"></param>
        /// <param name="dates"></param>
        /// <param name="datePropName">实体类的日期属性名称</param>
        /// <returns></returns>
        public static IQueryable<T> BetweenInDates<T>(this IQueryable<T> sources, DateTime?[] dates,
            Expression<Func<T, DateTime>> datePropName)
        {
            if (dates == null || dates.Length == 0)
            {
                return sources;
            }

            Expression condition = null;

            var start = dates.ElementAtOrDefault(0);

            var invalidDates = new DateTime?[] { DateTime.MinValue, DateTime.MaxValue };

            if (start.HasValue && !invalidDates.Contains(start))
            {
                condition = Expression
                    .GreaterThanOrEqual(datePropName.Body, Expression.Constant(start.GetValueOrDefault(), typeof(DateTime)));
            }

            var end = dates.ElementAtOrDefault(1);

            if (end.HasValue && !invalidDates.Contains(end))
            {
                if (condition != null)
                {
                    condition = Expression.AndAlso(condition,
                        Expression.LessThanOrEqual(datePropName.Body,
                        Expression.Constant(end.GetValueOrDefault(), typeof(DateTime))));
                }
                else
                {
                    condition = Expression.LessThanOrEqual(datePropName.Body,
                        Expression.Constant(end.GetValueOrDefault(), typeof(DateTime)));
                }
            }

            if (condition != null)
            {
                sources = sources
                    .Where(Expression.Lambda<Func<T, bool>>(condition, datePropName.Parameters));
            }

            return sources;
        }

        /// <summary>
        /// 筛选日期范围, p.Field 大于等于 start and p.Field 小于等于 end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sources"></param>
        /// <param name="dates"></param>
        /// <param name="datePropName">实体类的日期属性名称</param>
        /// <returns></returns>
        public static IQueryable<T> BetweenInDates<T>(this IQueryable<T> sources, string[] dates,
            Expression<Func<T, DateTime>> datePropName)
        {
            if (dates == null || dates.Length == 0)
            {
                return sources;
            }

            Expression condition = null;

            var start = dates.ElementAtOrDefault(0);

            if (DateTime.TryParse(start, out var startDate))
            {
                condition = Expression
                    .GreaterThanOrEqual(datePropName.Body, Expression.Constant(startDate, typeof(DateTime)));
            }

            var end = dates.ElementAtOrDefault(1);

            if (DateTime.TryParse(end, out var endDate))
            {
                if (condition != null)
                {
                    condition = Expression.AndAlso(condition,
                        Expression.LessThanOrEqual(datePropName.Body,
                        Expression.Constant(endDate, typeof(DateTime))));
                }
                else
                {
                    condition = Expression.LessThanOrEqual(datePropName.Body,
                        Expression.Constant(endDate, typeof(DateTime)));
                }
            }

            if (condition != null)
            {
                sources = sources
                    .Where(Expression.Lambda<Func<T, bool>>(condition, datePropName.Parameters));
            }

            return sources;
        }

        /// <summary>
        /// 筛选日期范围, p.Field 大于等于 start and p.Field 小于等于 end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sources"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="datePropName">实体类的日期属性名称</param>
        /// <returns></returns>
        public static IQueryable<T> BetweenInDates<T>(this IQueryable<T> sources, string start,
            string end,
            Expression<Func<T, DateTime>> datePropName)
        {

            Expression condition = null;

            if (DateTime.TryParse(start, out var startDate))
            {
                condition = Expression
                    .GreaterThanOrEqual(datePropName.Body, Expression.Constant(startDate, typeof(DateTime)));
            }

            if (DateTime.TryParse(end, out var endDate))
            {
                if (condition != null)
                {
                    condition = Expression.AndAlso(condition,
                        Expression.LessThanOrEqual(datePropName.Body,
                        Expression.Constant(endDate, typeof(DateTime))));
                }
                else
                {
                    condition = Expression.LessThanOrEqual(datePropName.Body,
                        Expression.Constant(endDate, typeof(DateTime)));
                }
            }

            if (condition != null)
            {
                sources = sources
                    .Where(Expression.Lambda<Func<T, bool>>(condition, datePropName.Parameters));
            }

            return sources;
        }

        /// <summary>
        /// 筛选字段范围, p.Field 大于等于 x and p.Field 小于等于 y
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="sources"></param>
        /// <param name="rangeStr">如, 10,20</param>
        /// <param name="propName">实体类的属性名称</param>
        /// <param name="splitChar">rangeStr分割字符</param>
        /// <returns></returns>
        public static IQueryable<T> BetweenIn<T, K>(this IQueryable<T> sources, string rangeStr,
            Expression<Func<T, K>> propName, char splitChar = ',')
        {


            if (string.IsNullOrWhiteSpace(rangeStr))
            {
                return sources;
            }

            var array = rangeStr.Split(splitChar);

            if (array == null || array.Length == 0)
            {
                return sources;
            }

            array = array.OrderBy(p => p).ToArray();

            var start = array.FirstOrDefault();
            var end = array.LastOrDefault();

            Expression condition = null;

            if (CommonUtil.TryGetJSONObject<K>(start, out var sVal))
            {
                condition = Expression
                    .GreaterThanOrEqual(propName.Body, Expression.Constant(sVal, typeof(K)));
            }

            if (CommonUtil.TryGetJSONObject<K>(end, out var eVal))
            {
                if (condition != null)
                {
                    condition = Expression.AndAlso(condition,
                        Expression.LessThanOrEqual(propName.Body,
                        Expression.Constant(eVal, typeof(K))));
                }
                else
                {
                    condition = Expression.LessThanOrEqual(propName.Body,
                        Expression.Constant(eVal, typeof(K)));
                }
            }

            if (condition != null)
            {
                sources = sources
                    .Where(Expression.Lambda<Func<T, bool>>(condition, propName.Parameters));
            }

            return sources;
        }
    }
}
