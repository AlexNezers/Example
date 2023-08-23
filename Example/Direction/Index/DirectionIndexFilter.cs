using DataBase.Models.Direction;

using HelpDeskCore.Models.Base;

namespace HelpDeskCore.Models.Direction.Index;
public class DirectionIndexFilter : BaseFilter
{
	public DirectionFilter ToDataBase() => new()
	{
		Page = null,
		Size = Size,
		IsActive = null
	};
}