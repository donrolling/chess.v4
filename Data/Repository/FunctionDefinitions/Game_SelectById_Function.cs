using Dapper;
using Data.Dapper.Interfaces;
using Data.Repository.Dapper.Base;
using System;

namespace Data.Repository.FunctionDefinitions {
	public class Game_SelectById_Function : BaseFunction, ISelectByIdFunction {
		public long Id { get; protected set; }
		public override string Signature { get; protected set; } = "@id";

		public Game_SelectById_Function(long id) {
			this.Id = id;
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "Game_SelectById";
		}

		public override DynamicParameters DynamicParameters() {
			var parameters = new DynamicParameters();
			parameters.Add("Id", this.Id);
			return parameters;
		}
	}	
}