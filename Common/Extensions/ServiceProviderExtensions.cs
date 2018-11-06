using System;

namespace Common.Extensions {

	public static class ServiceProviderExtensions {

		public static T GetService<T>(this IServiceProvider serviceProvider) {
			return (T)serviceProvider.GetService(typeof(T));
		}
	}
}