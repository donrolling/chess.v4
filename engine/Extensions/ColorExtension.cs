namespace engine.Extensions;

using engine.Models.Enums;

public static class ColorExtensions
{
	public static Color Reverse(this Color color)
	{
		return color == Color.White ? Color.Black : Color.White;
	}
}