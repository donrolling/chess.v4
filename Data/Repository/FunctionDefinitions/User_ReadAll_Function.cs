using Data.Repository.Dapper.Base;
using System;

namespace Data.Repository.FunctionDefinitions {
	public class User_ReadAll_Function : BasePageableFunction {	
		public User_ReadAll_Function() {
			this.DatabaseSchema = "Membership";
			this.UserDefinedFunctionName = "User_ReadAll";
			this.Signature = "@readActive, @readInactive";
		}
	}
}