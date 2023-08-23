using HelpDeskCore.Controllers;
using HelpDeskCore.Models.Base.Admin.Create;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpDeskCore.Models.Direction.Edit;
public class DirectionEditView : DirectionView, IAdminUpsertView
{
	public DirectionEditView(DirectionEditData data)
	{
		Data = data;
		TitleCollection.Add("Изменить");
	}
	public DirectionEditData Data { get; init; }
	public string HrefBack => DirectionController.GetHref(nameof(DirectionController.Index));
	public new string Title
	{
		get => base.Title!;
		init => TitleCollection.Add(value);
	}
	public async Task FillAsync(DefaultController controller)
	{
		await Data.FillAsync(controller.HandlerProvider);
		base.Fill(controller);
	}
	public static async Task<IAdminUpsertView> CreateAsync(string id, DefaultController controller)
	{
		var handler = controller.HandlerProvider.Direction;
		var source = await DirectionEditData.CreateAsync(handler, id);
		var view = new DirectionEditView(source);
		await view.FillAsync(controller);
		return view;
	}
	public async Task<IHtmlContent> GetViewFormAsync(IHtmlHelper htmlHelper)
		=> await htmlHelper.PartialAsync(PathPartialViewForm, Data);
}