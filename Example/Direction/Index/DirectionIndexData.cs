using DataBase.Models.Direction;

using HelpDeskCore.Controllers;
using HelpDeskCore.Models.Base;
using HelpDeskCore.Models.Base.Admin.Index;
using HelpDeskCore.Properties;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpDeskCore.Models.Direction.Index;
public class DirectionIndexData : IAdminIndexData
{
	public DirectionIndexData(DirectionEntity source)
	{
		Name = source.Name!;
		Id = source.IdString!;
		IsActive = source.IsActive;
	}
	public string Id { get; init; }
	public string Name { get; init; }
	public bool IsActive { get; init; }
	public DefaultAdminIndexGridControl GetGridControl() => new(
		IsActive,
		DirectionController.GetHref(nameof(DirectionController.Edit), Id),
		DirectionController.GetHref(nameof(DirectionController.Deactivate), Id),
		DirectionController.GetHref(nameof(DirectionController.Activate), Id),
		DirectionController.GetHref(nameof(DirectionController.Delete), Id),
		$"<a href=\"{DirectionController.GetHref(nameof(DirectionController.Setting), Id)}\">Настройки</a>");

	public Task<IHtmlContent> GetGridControl(IHtmlHelper htmlHelper)
		=> htmlHelper.PartialAsync(Resources.PathAdminIndexGridControl, GetGridControl());
}