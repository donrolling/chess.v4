using Chess.v4.Models.Enums;

namespace Chess.v4.Engine.Extensions
{
    public static class ColorExtensions
    {
        public static Color Reverse(this Color color)
        {
            return color == Color.White ? Color.Black : Color.White;
        }
    }
}