using Newtonsoft.Json;

namespace Common.Extensions
{
	public static class DeepCopier
	{
		public static T DeepCopy<T>(this T source)
		{
			var serialized = JsonConvert.SerializeObject(source);
			return JsonConvert.DeserializeObject<T>(serialized);
		}
	}
}