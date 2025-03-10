namespace api.Models;

public class GoToMoveResource
{
	public GameStateResource GameState { get; set; }

	public int HistoryIndex { get; set; }
}