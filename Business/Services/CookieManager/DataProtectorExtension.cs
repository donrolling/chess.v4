﻿using Microsoft.AspNetCore.DataProtection;
using System;

namespace Business.Services.Cookies {
	public static class DataProtectorExtension {

		public static bool TryUnprotect(this IDataProtector dataProtector, string protectedData, out string unProtectedData) {
			unProtectedData = string.Empty;
			try {
				unProtectedData = dataProtector.Unprotect(protectedData);
				return true;
			} catch (Exception) {
			}
			return false;
		}
	}
}