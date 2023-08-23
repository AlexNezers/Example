using HelpDeskCore.Controllers;
using HelpDeskCore.Service.DataBaseHandlerProvider;

namespace HelpDeskCore.Models.Direction.Create;
public class DirectionCreateData : DirectionData
{
	public async Task<bool> SaveAsync(DataBaseHandlerProviderService handlerProvider, DefaultController controller)
		=> await base.SaveAsync(
			controller, async ()
				=> await handlerProvider.Direction
					.AddAsync(await ToDataBaseAsync(handlerProvider)));
	public async Task FillAsync(DataBaseHandlerProviderService handlerProvider)
	{
		
	}
}