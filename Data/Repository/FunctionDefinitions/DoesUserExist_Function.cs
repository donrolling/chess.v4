using Dapper;
using Data.Dapper.Interfaces;
using Data.Repository.Dapper;
using Data.Repository.Dapper.Base;

namespace Data.Repository.FunctionDefinitions {
	public class DoesUserExist_Function : BaseScalarFunction, IScalarFunction {
		public string EmailAddress { get; set; }

		public DoesUserExist_Function(string emailAddress) {
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "DoesUserExist";
			this.Signature = "@emailAddress";
			this.EmailAddress = emailAddress;
		}

		public bool CallFunction(IDapperRepository repository) {
			var sql = SQL_Helper.GetScalarFunctionCallSQL(Signature, DatabaseSchema, UserDefinedFunctionName);
			var parameters = DynamicParameters();
			return repository.QuerySingle<bool>(sql, parameters);
		}

		public override DynamicParameters DynamicParameters() {
			var parameters = new DynamicParameters();
			parameters.Add("emailAddress", this.EmailAddress);
			return parameters;
		}
	}
}