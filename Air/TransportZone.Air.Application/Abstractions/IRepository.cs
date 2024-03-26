using System.Linq.Expressions;
using ErrorOr;
using TransportZone.Air.Domain.Abstractions;

namespace TransportZone.Air.Application.Abstractions;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
	public Task<ErrorOr<Success>> CreateAsync(TEntity entity, CancellationToken token);
	public IAsyncEnumerable<TResult> GetAsync<TResult>(Expression<Func<TEntity,bool>> filter, Expression<Func<TEntity, TResult>> selector);
	public IAsyncEnumerable<TResult> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector);
	public Task<TResult?> FirstOrDefaultAsync<TResult>(
		Expression<Func<TEntity, bool>> filter,
		Expression<Func<TEntity, TResult>> selector,
		CancellationToken cancellationToken,
		bool noTracking = true);
	public Task<TEntity?> FindAsync(object id);
	public Task<ErrorOr<Success>> UpdateAsync(TEntity entity);
}
