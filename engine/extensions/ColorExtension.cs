using chess.v4.models.enumeration;

namespace chess.v4.engine.extensions {

	public static class ColorExtensions {

		public static Color Reverse(this Color color) {
			return color == Color.White ? Color.Black : Color.White;
		}
	}
}