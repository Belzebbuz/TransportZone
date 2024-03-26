using System.Linq.Expressions;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransportZone.Air.Application.Abstractions;
using TransportZone.Air.Domain.Abstractions;

namespace TransportZone.Air.Infrastructure.Persistence;

internal class DefaultRepository<TEntity>(AppDbContext context, ILogger<DefaultRepository<TEntity>> logger)
	: IRepository<TEntity> where TEntity : class, IEntity
{
	public async Task<ErrorOr<Success>> CreateAsync(TEntity entity, CancellationToken token)
	{
		try
		{
			var set = context.Set<TEntity>();
			await set.AddAsync(entity, token);
			await context.SaveChangesAsync(token);
			return Result.Success;
		}
		catch (Exception e)
		{
			logger.LogError(e.ToString());
			return Error.Conflict(description: e.Message);
		}
	}

	public async Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> filter,
		Expression<Func<TEntity, TResult>> selector,
		CancellationToken cancellationToken,
		bool noTracking = true)
	{
		var set = context.Set<TEntity>();

		if (noTracking)
			return await set.AsNoTracking()
				.Where(filter)
				.Select(selector)
				.FirstOrDefaultAsync(cancellationToken);
		
		return await set
			.Where(filter)
			.Select(selector)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<TEntity?> FindAsync(object key)
	{
		var set = context.Set<TEntity>();
		return await set.FindAsync(key);
	}

	public async Task<ErrorOr<Success>> UpdateAsync(TEntity entity)
	{
		var set = context.Set<TEntity>();
		set.Update(entity);
		await context.SaveChangesAsync();
		return Result.Success;
	}

	public IAsyncEnumerable<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> selector)
	{
		var set = context.Set<TEntity>();
		return set
			.AsNoTracking()
			.Where(filter)
			.Select(selector)
			.AsAsyncEnumerable();
	}

	public IAsyncEnumerable<TResult> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
	{
		var set = context.Set<TEntity>();
		return set
			.AsNoTracking()
			.Select(selector)
			.AsAsyncEnumerable();
	}
}