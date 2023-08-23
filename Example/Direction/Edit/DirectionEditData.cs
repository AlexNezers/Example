using DataBase.Handlers;
using DataBase.Models.Direction;

using HelpDeskCore.Controllers;
using HelpDeskCore.Service.DataBaseHandlerProvider;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models.Direction.Edit;
public class DirectionEditData : DirectionData
{
	public DirectionEditData() : base()
	{
	}
	public DirectionEditData(DirectionEntity source) : base(source)
	{
		Id = source.IdString;
	}
	[Required]
	public string? Id { get; set; }
	public static async Task<DirectionEditData> CreateAsync(DirectionHandler handler, string id)
		=> await handler.GetOneAsync(id, m => new DirectionEditData(m));
	public async Task<bool> SaveAsync(DataBaseHandlerProviderService handlerProvider, DefaultController controller)
		=> await base.SaveAsync(controller, async () => await handlerProvider
			.Direction
			.EditAsync(await ToDataBaseAsync(handlerProvider)));
	public async Task FillAsync(DataBaseHandlerProviderService handlerProvider)
	{
	}
	public override async Task<DirectionEntity> ToDataBaseAsync(DataBaseHandlerProviderService handlerProvider)
	{
		var result = await base.ToDataBaseAsync(handlerProvider);
		result.IdString = Id;
		return result;
	}
	public override async Task<IHtmlContent> GetHiddenIdHtmlAsync(IHtmlHelper htmpHelper)
	{
		return htmpHelper.Hidden(nameof(Id), Id);
	}
}