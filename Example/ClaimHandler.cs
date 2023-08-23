using DataBase.Models.Claim;
using DataBase.Models.Claim.Link;
using DataBase.Models.Claim.Value;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

using Utilits.Extension;

namespace DataBase.Handlers;
public class ClaimHandler : DefaultHandler
{
	public DbSet<ClaimEntity> TableContext => base.Context!.Claims!;
	public async Task<bool> AddAsync(ClaimEntity source) => await base.AddAsync(TableContext, source);
	public async Task<bool> EditAsync(ClaimEntity source) => await base.EditAsync<ClaimException, ClaimEntity>(TableContext, source, (d, s) =>
	{
		d.DateLastEdit = DateTime.Now;
		d.Title = s.Title;
		d.PriorityId = s.PriorityId;
		d.PerformerId = s.PerformerId;
		d.StatusId = s.StatusId;
		d.CategoryId = s.CategoryId;
		d.ViewId = s.ViewId;
		d.PreliminaryEstimate = s.PreliminaryEstimate;
		d.Deadline = s.Deadline;
		d.RouteNodeId = s.RouteNodeId;
	});
	public async Task<ClaimEntity> GetOneAsync(string id)
		=> await GetOneAsync(m => m.IdString == id, m => m);
	public async Task<TResult> GetOneAsync<TResult>(
		string id,
		Expression<Func<ClaimEntity, TResult>> selector)
		=> await GetOneAsync(m => m.IdString == id, selector, m => m);

	public async Task<TResult> GetOneAsync<TResult>(
		string id,
		Expression<Func<ClaimEntity, TResult>> selector,
		Func<IQueryable<ClaimEntity>, IQueryable<ClaimEntity>> path)
		=> await GetOneAsync(m => m.IdString == id, selector, path);
	public async Task<TResult> GetOneAsync<TResult>(
		Expression<Func<ClaimEntity, bool>> predicate,
		Expression<Func<ClaimEntity, TResult>> selector)
		=> await GetOneAsync(predicate, selector, m => m);
	public async Task<TResult> GetOneAsync<TResult>(
		Expression<Func<ClaimEntity, bool>> predicate,
		Expression<Func<ClaimEntity, TResult>> selector,
		Func<IQueryable<ClaimEntity>, IQueryable<ClaimEntity>> path)
			=> await GetOneAsync<ClaimException, ClaimEntity, TResult>(TableContext, predicate, selector, path);
	public (int Amount, IEnumerable<TResult> Collection) GetCollection<TResult>(ClaimFilter filter, Func<ClaimEntity, TResult> selector)
		=> GetCollection(filter, selector, m => m);
	public (int Amount, IEnumerable<TResult> Collection) GetCollection<TResult>(
		ClaimFilter filter,
		Func<ClaimEntity, TResult> selector,
		Func<IQueryable<ClaimEntity>, IQueryable<ClaimEntity>> path)
	{
		var page = filter.Page;
		filter.Page = null;
		var (amount, sourceCollection) = GetCollection(
			TableContext,
			filter,
			m => m,
			p => path(p).Include(m => m.Comments!)
				.ThenInclude(m => m.Source),
			query =>
			{

				if (filter.TreeView)
				{
					query = query.Where(m => m.Parent!.IdString == filter.ParentId);
				}
				if (filter.DateBegin.HasValue)
				{
					query = query.Where(m => m.DateCreate >= filter.DateBegin.Value);
				}
				if (filter.DateEnd.HasValue)
				{
					query = query.Where(m => m.DateCreate <= filter.DateEnd.Value);
				}
				if (filter.DateCloseBegin.HasValue)
				{
					query = query.Where(m => m.DateClose >= filter.DateCloseBegin.Value);
				}
				if (filter.DateCloseEnd.HasValue)
				{
					query = query.Where(m => m.DateClose <= filter.DateCloseEnd.Value);
				}
				if (filter.InitiatorIdCollection?.Length > 0)
				{
					query = query.Where(m => filter.InitiatorIdCollection.Contains(m.Initiator!.IdString));
				}
				if (filter.PerformersIdCollection?.Length > 0)
				{
					query = query.Where(m => filter.PerformersIdCollection.Contains(m.Performer!.IdString));
				}
				if (filter.OrganizationsIdCollection?.Length > 0)
				{
					query = query.Where(m => filter.OrganizationsIdCollection.Contains(m.Organization!.IdString));
				}
				if (filter.StatusIdCollection?.Length > 0)
				{
					query = query.Where(m => filter.StatusIdCollection.Contains(m.RouteNode!.IdString));
				}
				if (filter.PrioritiesIdCollection?.Length > 0)
				{
					query = query.Where(m => filter.PrioritiesIdCollection.Contains(m.Priority!.IdString));
				}
				if (filter.TypesIdCollection?.Length > 0)
				{
					query = query.Where(m => filter.TypesIdCollection.Contains(m.Type!.IdString));
				}
				if (filter.CategoriesIdCollection?.Length > 0)
				{
					query = query.Where(m => filter.CategoriesIdCollection.Contains(m.Category!.IdString));
				}
				if (filter.ProjectIdArray != null)
				{
					query = query.Where(m => filter.ProjectIdArray.Contains(m.Project!.IdString));
				}
				if (string.IsNullOrEmpty(filter.Search) == false)
				{
					query = query.Where(m =>
						m.Id.ToString() == filter.Search
						||m.Number!.Contains(filter.Search)
						|| m.Title!.Contains(filter.Search)
						|| m.Description!.Contains(filter.Search)
						|| m.Comments!.Where(mm => mm.Text!.Contains(filter.Search!)).Any()
					);
				}
				if (filter.HasDirectionIdArray)
				{
					query = query.Where(claim => filter.DirectionIdArray!.Contains(claim.Project!.DirectionId));
				}
				return query;
			});
		IEnumerable<ClaimEntity> collection = sourceCollection.ToList();
		if (filter.State?.Length > 0)
		{
			collection = collection.Where(m => filter.State.Contains(m.State)).AsEnumerable();
		}
		amount = collection.Count();
		if (page.HasValue && filter.Size.HasValue)
		{
			collection = collection
				.Skip(page.Value * filter.Size.Value - filter.Size.Value)
				.Take(filter.Size.Value);
		}
		return (amount, collection.Select(selector));
	}
	public async Task<bool> Activate(string id) => await base.Activate(TableContext, id);
	public async Task<bool> Deactivate(string id) => await base.Deactivate(TableContext, id);
	public async Task<bool> Delete(string id) => await base.Delete(TableContext, id);
	public async Task<bool> Cancel(string id)
	{
		var source = await GetOneAsync<ClaimException, ClaimEntity, ClaimEntity>(
			TableContext,
			m => m.IdString == id,
			m => m,
			m => m,
			false);
		source.DateClose = DateTime.Now;
		source.DateLastEdit = DateTime.Now;
		return await SaveChangesAsync();
	}
	public async Task<bool> SetSolution(string claimId, int solutionId)
	{
		if (TableContext.Where(m => m.Parent!.IdString == claimId && m.DateClose.HasValue == false).Any())
		{
			throw ClaimException.NotAllChilderAreClosed;
		}
		var destination = await GetOneAsync<ClaimException, ClaimEntity, ClaimEntity>(
			TableContext,
			m => m.IdString == claimId,
			selector => selector,
			path => path.Include(m => m.Comments),
			false);
		destination.SolutionId = solutionId;
		destination.DateClose = DateTime.Now;
		return await SaveChangesAsync();
	}
	public async Task<bool> AddLink(string claimId, string linkId)
	{
		var claim = await GetTraceableOneAsync(TableContext, claimId, p => p.Include(m => m.LinkClaimCollection));
		var link = await GetTraceableOneAsync(TableContext, linkId, p => p);
		claim.LinkClaimCollection!.Add(new LinkClaimClaimEntity
		{
			ClaimId = claim.Id,
			RightId = link.Id
		});
		return await SaveChangesAsync();
	}
	public async Task<bool> RemoveLink(string claimId, string linkId)
	{
		var claim = await GetTraceableOneAsync(TableContext, claimId, p => p.Include(m => m.LinkClaimCollection!).ThenInclude(m => m.Right));
		var link = claim.LinkClaimCollection!.FirstOrDefault(m => m.Right!.IdString == linkId);
		claim.LinkClaimCollection!.Remove(link!);
		return await SaveChangesAsync();
	}
	public async Task<bool> SetDynamicFieldValue(string claimId, string fieldId, string value)
	{
		bool result;
		var source = await GetTraceableOneAsync(TableContext, claimId, p => p
			.Include(m => m.ValueCollection)
			.Include(m => m.Project)
				.ThenInclude(m => m!.Direction)
					.ThenInclude(m => m.Fields));
		var field = source.Project!.Direction!.Fields!.First(m => m.IdString == fieldId);
		var valueEntity = source.ValueCollection!.FirstOrDefault(m => m.FieldId == field.Id);
		if (valueEntity == null)
		{
			result = await AddAsync(Context!.ClaimValue!, new ClaimValueEntity
			{
				Value = value,
				FieldId = field.Id,
				ClaimId = source.Id
			});
		}
		else
		{
			valueEntity.Value = value;
			result = await base.EditAsync<ClaimException, ClaimValueEntity>(Context!.ClaimValue!, valueEntity, (d, s) =>
			{
				d.Value = s.Value;
			});
		}
		return result;
	}
}