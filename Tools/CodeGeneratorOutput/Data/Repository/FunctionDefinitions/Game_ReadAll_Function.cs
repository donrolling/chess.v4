using Data.Repository.Dapper.Base;
using System;

namespace Data.Repository.FunctionDefinitions {
	public class Game_ReadAll_Function : BasePageableFunction {	
		public Game_ReadAll_Function() {
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "Game_ReadAll";
			this.Signature = "@readActive, @readInactive";
		}
	}
}