namespace common.Extensions;

using System.Text.Json;

public static class DeepCopier
{
	public static T? DeepCopy<T>(this T source)
	{
		var serialized = JsonSerializer.Serialize(source);
		return JsonSerializer.Deserialize<T>(serialized);
	}
}