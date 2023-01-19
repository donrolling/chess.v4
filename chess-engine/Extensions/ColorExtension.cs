using chess_engine.Models.Enums;

namespace chess_engine.Engine.Extensions
{
	public static class ColorExtensions
	{
		public static Color Reverse(this Color color)
		{
			return color == Color.White ? Color.Black : Color.White;
		}
	}
}