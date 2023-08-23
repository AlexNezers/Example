namespace HelpDeskCore.Models.Direction.Setting;
public class DirectionSettingView : DirectionView
{
	public DirectionSettingView(string directionId)
	{
		TitleCollection.Add("Настройки");
		DirectionId = directionId;
	}
	public string DirectionId { get; init; }
}