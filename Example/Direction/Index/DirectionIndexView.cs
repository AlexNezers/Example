using HelpDeskCore.Controllers;
using HelpDeskCore.Models.Base.Admin.Index;
using HelpDeskCore.Service.DataBaseHandlerProvider;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpDeskCore.Models.Direction.Index;
public class DirectionIndexView : DirectionView, IAdminIndexView
{
	public DirectionIndexView(DirectionIndexFilter filter)
	{
		Filter = filter;
		TitleCollection.Add("Все");
		HrefCreate = DirectionController.GetHref(nameof(DirectionController.Create));
		HrefBack = SettingController.GetHref(nameof(SettingController.Index));
	}
	public DirectionIndexFilter Filter { get; init; }
	public IEnumerable<IAdminIndexData>? Data { get; private set; }
	public IEnumerable<string>? StylePathsCollection => null;
	public IEnumerable<string>? ScriptPathCollection => null;
	public string HrefCreate { get; init; }
	public string HrefBack { get; init; }
	public new string Title
	{
		get => base.Title ?? "";
		init => TitleCollection.Add(value);
	}
	public async Task FillAsync(DataBaseHandlerProviderService handlerProvider, DefaultController controller)
	{
		(Filter.Amount, Data) = handlerProvider.Direction.GetCollection(
			Filter.ToDataBase(), m => new DirectionIndexData(m));
		base.Fill(controller);
	}
	public Task<IHtmlContent> GetHtmlFormFilter(IHtmlHelper htmlHelpder)
		=> htmlHelpder.PartialAsync("/Views/Direction/IndexFilterForm.cshtml", Filter);
	public Task<IHtmlContent> GetHtmlGrid(IHtmlHelper htmlHelpder)
		=> htmlHelpder.PartialAsync(PathViewIndexGrid, Data);
}