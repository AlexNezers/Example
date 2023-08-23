using HelpDeskCore.Controllers;
using HelpDeskCore.Models.Base.Admin.Create;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpDeskCore.Models.Direction.Create;
public class DirectionCreateView : DirectionView, IAdminUpsertView
{
	public DirectionCreateView(DirectionCreateData data)
	{
		Data = data;
		TitleCollection.Add("Добавить");
		HrefBack = DirectionController.GetHref(nameof(DirectionController.Index));
	}
	public new string Title
	{
		get => base.Title!;
		init => TitleCollection.Add(value);
	}
	public DirectionCreateData Data { get; init; }
	public string HrefBack { get; init; }
	public async Task FillAsync(DefaultController controller)
	{
		await Data.FillAsync(controller.HandlerProvider);
		base.Fill(controller);
	}
	public async Task<IHtmlContent> GetViewFormAsync(IHtmlHelper htmlHelper)
		=> await htmlHelper.PartialAsync(PathPartialViewForm, Data);
}