using HelpDeskCore.Models.Base;

namespace HelpDeskCore.Models.Direction;
public class DirectionView : BaseView
{
	protected const string PathPartialViewForm = "/Views/Direction/Form.cshtml";
	public DirectionView()
	{
		TitleCollection.Add("Направление");
	}
}