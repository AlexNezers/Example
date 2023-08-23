using DataBase.Handlers;
using DataBase.Models.Default;

using HelpDeskCore.Filter.AuthorizationFilter;
using HelpDeskCore.Models.Authorizatino;
using HelpDeskCore.Models.Base.Admin.Create;
using HelpDeskCore.Models.Base.Admin.Index;
using HelpDeskCore.Models.Direction.Action;
using HelpDeskCore.Models.Direction.Create;
using HelpDeskCore.Models.Direction.Edit;
using HelpDeskCore.Models.Direction.Index;
using HelpDeskCore.Models.Direction.Setting;
using HelpDeskCore.Models.Project.Setting;
using HelpDeskCore.Service.Authorization;
using HelpDeskCore.Service.DataBaseHandlerProvider;

using Microsoft.AspNetCore.Mvc;

namespace HelpDeskCore.Controllers;
[ApiController, Route("[controller]/[action]")]
public class DirectionController : DefaultController
{
	private DirectionHandler _handler => HandlerProvider.Direction;
	public DirectionController(
		DataBaseHandlerProviderService handlerProvider,
		IAuthorizationService serviceAuthorization,
		IWebHostEnvironment hostingEnvironmet)
			: base(handlerProvider, serviceAuthorization, hostingEnvironmet)
	{
	}
	public static string GetHref(params string[] node) => GetHref(typeof(DirectionController), node);
	public enum ActionAuthorization : byte
	{
		Show,
		Create,
		Edit,
		ShowAdmin
	}
	public static new Func<AuthorizationUser, bool>? GetAuthorizationPredicate(byte Action) => (ActionAuthorization)Action switch
	{
		_ => null
	};

	[TypeAuthorizationFilter(typeof(DirectionController), (byte)ActionAuthorization.ShowAdmin)]
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		return await Index(new DirectionIndexFilter());
	}
	[TypeAuthorizationFilter(typeof(DirectionController), (byte)ActionAuthorization.ShowAdmin)]
	[HttpPost]
	public async Task<IActionResult> Index(DirectionIndexFilter filter)
	{
		IAdminIndexView view = new DirectionIndexView(filter);
		await view.FillAsync(HandlerProvider, this);
		return CreateAdminIndexView(view);
	}
	[TypeAuthorizationFilter(typeof(ProjectController), (byte)ActionAuthorization.ShowAdmin)]
	[HttpGet("{id}")]
	public IActionResult Setting(string id)
	{
		return CreateView(new DirectionSettingView(id));
	}
	[TypeAuthorizationFilter(typeof(DirectionController), (byte)ActionAuthorization.Create)]
	[HttpGet]
	public async Task<IActionResult> Create()
	{
		var data = new DirectionCreateData();
		IAdminUpsertView view = new DirectionCreateView(data);
		await view.FillAsync(this);
		return CreateUpsertView(view);
	}
	[TypeAuthorizationFilter(typeof(DirectionController), (byte)ActionAuthorization.Create)]
	[HttpPost]
	public async Task<IActionResult> Create([FromForm] DirectionCreateData data)
	{
		if (await data.SaveAsync(HandlerProvider, this))
		{
			return RedirectToAction(nameof(Index).ToLower());
		}
		else
		{
			IAdminUpsertView view = new DirectionCreateView(data);
			await view.FillAsync(this);
			return CreateUpsertView(view);
		}
	}
	[TypeAuthorizationFilter(typeof(DirectionController), (byte)ActionAuthorization.Edit)]
	[HttpGet("{id}")]
	public async Task<IActionResult> Edit(string id)
	{
		try
		{
			IAdminUpsertView view = await DirectionEditView.CreateAsync(id, this);
			return CreateUpsertView(view);
		}
		catch (DataBaseException ex)
		{
			TempDataError = ex.Message;
			return RedirectToAction(nameof(Index).ToLower());
		}
	}
	[TypeAuthorizationFilter(typeof(DirectionController), (byte)ActionAuthorization.Edit)]
	[HttpPost("{id}")]
	public async Task<IActionResult> Edit(string id, [FromForm] DirectionEditData data)
	{
		if (await data.SaveAsync(HandlerProvider, this))
		{
			return RedirectToAction(nameof(Index).ToLower());
		}
		else
		{
			IAdminUpsertView view = new DirectionEditView(data);
			await view.FillAsync(this);
			return CreateUpsertView(view);
		}
	}
	[TypeAuthorizationFilter(typeof(DirectionController), (byte)ActionAuthorization.Edit)]
	[HttpGet("{id}")]
	public async Task<IActionResult> Activate(string id) => await DirectionAction.Activate(id, _handler, this);
	[TypeAuthorizationFilter(typeof(DirectionController), (byte)ActionAuthorization.Edit)]
	[HttpGet("{id}")]
	public async Task<IActionResult> Deactivate(string id) => await DirectionAction.Deactivate(id, _handler, this);
	[TypeAuthorizationFilter(typeof(DirectionController), (byte)ActionAuthorization.Edit)]
	[HttpGet("{id}")]
	public async Task<IActionResult> Delete(string id) => await DirectionAction.Delete(id, _handler, this);
}