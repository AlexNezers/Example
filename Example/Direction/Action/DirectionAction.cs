using DataBase.Handlers;

using HelpDeskCore.Controllers;
using HelpDeskCore.Models.Base;

using Microsoft.AspNetCore.Mvc;

namespace HelpDeskCore.Models.Direction.Action;
public class DirectionAction : BaseAction
{
	public static async Task<RedirectToActionResult> Activate(
		string id, DirectionHandler handler, DefaultController controller)
	{
		await Action(controller, () => handler.Activate(id));
		return controller.RedirectToAction(nameof(DirectionController.Index).ToLower());
	}
	public static async Task<RedirectToActionResult> Deactivate(
		string id, DirectionHandler handler, DefaultController controller)
	{
		await Action(controller, () => handler.Deactivate(id));
		return controller.RedirectToAction(nameof(DirectionController.Index).ToLower());
	}
	public static async Task<RedirectToActionResult> Delete(
		string id, DirectionHandler handler, DefaultController controller)
	{
		await Action(controller, () => handler.Delete(id));
		return controller.RedirectToAction(nameof(DirectionController.Index).ToLower());
	}
}