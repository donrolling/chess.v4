namespace engine.Models;

using engine.Models.Enums;

public class DirectionInfo
{
	public bool IsOrthogonal { get; set; } = false;
	public bool IsDiagonal { get; set; } = false;
	public Direction Direction { get; set; } = Direction.Invalid;
	public DiagonalDirection DiagonalDirection { get; set; } = DiagonalDirection.Invalid;
}