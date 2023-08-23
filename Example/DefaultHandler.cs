using DataBase.Context;
using DataBase.Models.Default;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

using Utilits;

namespace DataBase.Handlers;
public class DefaultHandler
{
	protected const int _lengthIdString = 32;
	public HelpDeskDataBase? Context { get; init; }
	protected async Task<bool> SaveChangesAsync() => await Context!.SaveChangesAsync() > 0;
	protected virtual async Task<TResult> CreateAsync<TResult>(DbSet<TResult> tableContext)
		where TResult : DefaultEntity, new()
	{
		var source = new TResult
		{
			DateCreate = DateTime.Now,
			IsActive = true
		};
		do
		{
			source.IdString = GenerationRandomString.GetString(_lengthIdString);
		}
		while (await tableContext.AnyAsync(m => m.IdString == source.IdString));
		return source;
	}
	protected async Task<bool> AddAsync<TSource>(DbSet<TSource> tableContext, TSource source)
		where TSource : DefaultEntity
	{
		source.DateCreate = DateTime.Now;
		source.IsActive = true;
		do
		{
			source.IdString = GenerationRandomString.GetString(_lengthIdString);
		}
		while (await tableContext.AnyAsync(m => m.IdString == source.IdString));
		await tableContext.AddAsync(source);
		return await SaveChangesAsync();
	}
	protected async Task<bool> EditAsync<TException, TSource>(
		DbSet<TSource> tableContext, TSource source, Action<TSource, TSource> action)
			where TSource : DefaultEntity
			where TException : DataBaseException, new ()
	{
		var destination = await GetOneAsync<TException, TSource, TSource>(
			tableContext, m => m.IdString == source.IdString, m => m, m => m, false);
		action(destination, source);
		return await SaveChangesAsync();
	}

	protected static async Task<TResult> GetOneAsync<TException, TSource, TResult>(
		DbSet<TSource> tableContext,
		Expression<Func<TSource, bool>> predicate,
		Expression<Func<TSource, TResult>> selector,
		Func<IQueryable<TSource>, IQueryable<TSource>> path,
		bool IsNoTracking = true)
			where TSource : DefaultEntity
			where TException : DataBaseException, new ()
	{
		var query = path(tableContext).AsQueryable();
		if (IsNoTracking)
		{
			query = query.AsNoTracking();
		}
		var result = await query
			.Where(predicate)
			.Select(selector)
			.FirstOrDefaultAsync();
		return result ?? throw DataBaseException.NotFound;
	}
	protected static (int Amount, IEnumerable<TResult> Collection) GetCollection<TSource, TResult>(
		DbSet<TSource> tableContext,
		DefaultFilter<TSource> filter,
		Expression<Func<TSource, TResult>> selector,
		Func<IQueryable<TSource>, IQueryable<TSource>> path,
		Func<IQueryable<TSource>, IQueryable<TSource>> filtration)
			where TSource : DefaultEntity
	{
		var query = path(tableContext.AsQueryable().AsTracking());
		if (filter.IsActive.HasValue)
		{
			query = query.Where(m => m.IsActive == filter.IsActive.Value);
		}
		if (filter.IsDelete.HasValue)
		{
			query = query.Where(m => m.IsDelete == filter.IsDelete.Value);
		}
		query = filtration(query);
		query = filter.OrderPredicate(query);
		int amount = query.Count();
		if (filter.Page.HasValue && filter.Size.HasValue)
		{
			query = query
				.Skip(filter.Page.Value * filter.Size.Value - filter.Size.Value)
				.Take(filter.Size.Value);
		}
		return (amount, query.Select(selector));
	}
	protected async Task<bool> ChangeAsync<TSource>(
		DbSet<TSource> tableContext, string id, Action<TSource> action)
			where TSource : DefaultEntity
	{
		var source = await GetOneAsync<DataBaseException, TSource, TSource>(
			tableContext, m => m.IdString == id, m => m, m => m, false);
		action(source);
		return await SaveChangesAsync();
	}
	protected async Task<bool> Activate<TSource>(DbSet<TSource> tableContext, string id)
		where TSource : DefaultEntity
	=> await ChangeAsync<TSource>(tableContext, id, (m) =>
	{
		m.IsActive = true;
	});
	protected async Task<bool> Deactivate<TSource>(DbSet<TSource> tableContext, string id)
		where TSource : DefaultEntity
	=> await ChangeAsync<TSource>(tableContext, id, (m) =>
	{
		m.IsActive = false;
	});
	protected async Task<bool> Delete<TSource>(DbSet<TSource> tableContext, string id)
		where TSource : DefaultEntity
	=> await ChangeAsync(tableContext, id, (m) =>
	{
		m.IsDelete = true;
	});
	protected async Task<bool> Remove<TSource>(DbSet<TSource> tableContext, string id)
		where TSource : DefaultEntity
	{
		var source = await GetOneAsync<DataBaseException, TSource, TSource>(
			tableContext, m => m.IdString == id, m => m, m => m, false);
		tableContext.Remove(source);
		return await SaveChangesAsync();
	}
	protected static async Task<TSource> GetTraceableOneAsync<TSource>(
		DbSet<TSource> tableContext, string id, Func<IQueryable<TSource>, IQueryable<TSource>> path)
			where TSource : DefaultEntity
		=> await GetOneAsync<DataBaseException, TSource, TSource>(tableContext, m => m.IdString == id, m => m, path, false);
}