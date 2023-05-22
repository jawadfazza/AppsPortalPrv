using System;
using System.Linq;
using System.Linq.Expressions;

namespace AppsPortal.Extensions
{
    public static class CommonWhere
    {
        public static IQueryable<TSource> WherePK<TSource>(this IQueryable<TSource> source, Guid PK)
        {
            var pkFieldName = typeof(TSource).GetProperties().FirstOrDefault().Name;
            var param = Expression.Parameter(typeof(TSource), "e");
            var predicate = Expression.Lambda<Func<TSource, bool>>(Expression.Equal(Expression.Property(param, pkFieldName), Expression.Constant(PK)), param);
            return source.Where(predicate);
        }
    }
}