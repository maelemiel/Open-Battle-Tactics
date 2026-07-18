using System.Linq.Expressions;

namespace System.Linq
{
	public interface IQueryProvider
	{
		IQueryable CreateQuery(Expression expression);

		object Execute(Expression expression);

		IQueryable<TElement> CreateQuery<TElement>(Expression expression);

		TResult Execute<TResult>(Expression expression);
	}
}
