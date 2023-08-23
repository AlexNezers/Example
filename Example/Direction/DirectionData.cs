using DataBase.Models.Direction;
using DataBase.Models.Project;
using HelpDeskCore.Models.Base;
using HelpDeskCore.Service.DataBaseHandlerProvider;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models.Direction;
public class DirectionData : BaseData
{
	public DirectionData()
	{
	}
	public DirectionData(DirectionEntity source) : this()
	{
		Name = source.Name;
	}
	[Display(Name = "Наименование"), Required, MinLength(5), MaxLength(100)]
	public string? Name { get; set; }
	public async Task FillAsync(DataBaseHandlerProviderService handlerProvider)
	{
	}
	public virtual async Task<DirectionEntity> ToDataBaseAsync(DataBaseHandlerProviderService handlerProvider) => new()
	{
		Name = Name!
	};
	public virtual async Task<IHtmlContent> GetHiddenIdHtmlAsync(IHtmlHelper htmpHelper)
	{
		return null;
	}
}